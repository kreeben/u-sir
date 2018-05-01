﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sir.Json
{
    public class JsonModelBinder : IModelBinder
    {
        public string ContentType => "application/json";
        public int Ordinal => 0;

        public IEnumerable<IModel> Deserialize(Stream data)
        {
            foreach (var dict in Deserialize(data))
            {
                yield return new DictionaryModel(dict);
            }
        }

        private static IList<Dictionary<string, IComparable>> Deserialize(Stream stream)
        {
            var dicts = new List<Dictionary<string, IComparable>>();
            var tokens = DeserializeIntoTokens(stream);

            foreach (var token in tokens)
            {
                var dict = new Dictionary<string, IComparable>();
                FillDictionaryFromJToken(dict, token, string.Empty);
                dicts.Add(dict);
            }

            return dicts;
        }

        /// <summary>
        /// https://stackoverflow.com/a/32800161/46645
        /// </summary>
        private static void FillDictionaryFromJToken(Dictionary<string, IComparable> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
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

        public void Serialize(IEnumerable<IModel> data, Stream outputStream)
        {
            int offset = 0;
            foreach (var item in data)
            {
                var bytes = Serialize(item);
                outputStream.Write(bytes, offset, bytes.Length);
                offset += bytes.Length;
            }
        }

        private byte[] Serialize(IModel data)
        {
            var dict = new Dictionary<string, IComparable>();
            foreach(var key in data.Keys)
            {
                dict.Add(key, data.Get(key));
            }
            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            return System.Text.Encoding.Unicode.GetBytes(json);
        }
    }
}