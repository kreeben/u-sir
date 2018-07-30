using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var list = data.ToList();

            foreach (var item in list)
            {
                var jsonDoc = Convert(item);
                var bytes = Serialize(jsonDoc);

                output.Write(bytes, 0, bytes.Length);
            }
        }

        private Dictionary<string, IComparable> Convert(IModel model)
        {
            var doc = new Dictionary<string, IComparable>();
            for (int i = 0; i < model.Keys.Length; i++)
            {
                var key = (string)model.Keys[i];
                doc.Add(key, model.Values[i]);

            }
            return doc;
        }

        private byte[] Serialize(Dictionary<string, IComparable> model)
        {
            var json = JsonConvert.SerializeObject(model, Formatting.None);
            return System.Text.Encoding.Unicode.GetBytes(json);
        }
    }
}
