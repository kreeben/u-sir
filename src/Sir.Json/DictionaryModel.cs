using System;
using System.Collections.Generic;
using System.Linq;

namespace Sir.Json
{
    public class DictionaryModel : IModel
    {
        private readonly IDictionary<string, IComparable> _dict;

        public DictionaryModel(IDictionary<string, IComparable> dict)
        {
            _dict = dict;
        }

        public IComparable Id { get { return _dict["_id"]; } }

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
