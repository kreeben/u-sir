using System;
using System.Collections.Generic;
using System.Linq;

namespace Sir.Json
{
    public class JsonModel : IModel
    {
        private readonly IDictionary<string, IComparable> _dict;

        public JsonModel(IDictionary<string, IComparable> dict)
        {
            _dict = dict;
        }

        public string[] Keys { get { return _dict.Keys.ToArray(); } }

        public void Add(string key, IComparable value)
        {
            _dict.Add(key, value);
        }

        public IComparable Get(string key)
        {
            return _dict[key];
        } 
    }
}
