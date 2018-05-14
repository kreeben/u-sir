using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public class JsonFormatter : IModelFormatter
    {
        public string ContentType => "application/json";

        public void Dispose()
        {
        }

        public void Format(IEnumerable<IModel> data, Stream output)
        {
            foreach (var item in data)
            {
                var bytes = Serialize(item);
                output.Write(bytes, 0, bytes.Length);
            }
        }

        private byte[] Serialize(IModel data)
        {
            var dict = new Dictionary<string, IComparable>();
            for (int i = 0; i < data.Keys.Length; i++)
            {
                var key = (string)data.Keys[i];
                if (key != "_id")
                    dict.Add(key, data.Values[i]);
            }
            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            return System.Text.Encoding.Unicode.GetBytes(json);
        }
    }
}
