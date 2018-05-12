using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sir.Json
{
    public class JsonParser : IModelParser
    {
        public string ContentType => "application/json";

        public IList<IModel> Parse(Stream data)
        {
            return ParseIterator(data).ToList();
        }

        private IEnumerable<IModel> ParseIterator(Stream data)
        {
            foreach (var dict in Deserialize(data))
            {
                var keys = dict.Keys.ToArray();
                var values = new IComparable[keys.Length];

                for (int i = 0; i < keys.Length; i++)
                {
                    values[i] = dict[keys[i]];
                }

                yield return new Model(keys, values);
            }
        }

        private static IList<IDictionary<string, IComparable>> Deserialize(Stream stream)
        {
            var dicts = new List<IDictionary<string, IComparable>>();
            var tokens = DeserializeIntoTokens(stream);

            foreach (var token in tokens)
            {
                var dict = new SortedDictionary<string, IComparable>();
                Flatten(dict, token, string.Empty);
                dicts.Add(dict);
            }

            return dicts;
        }

        /// <summary>
        /// https://stackoverflow.com/a/32800161/46645
        /// </summary>
        private static void Flatten(IDictionary<string, IComparable> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        Flatten(dict, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        Flatten(dict, value, Join(prefix, index.ToString()));
                        index++;
                    }
                    break;

                default:
                    dict.Add(prefix, (IComparable)((JValue)token).Value);
                    break;
            }
        }

        private static string Join(string prefix, string name)
        {
            return (string.IsNullOrEmpty(prefix) ? name : prefix + "." + name);
        }

        private static IList<JToken> DeserializeIntoTokens(Stream stream)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var obj = serializer.Deserialize(jsonTextReader);
                return JArray.FromObject(obj);
            }
        }

        public void Dispose()
        {
        }
    }
}
