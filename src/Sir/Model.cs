using System;

namespace Sir.Json
{
    public class Model : IModel
    {
        public Model(string[] keys, IComparable[] values)
        {
            Keys = keys;
            Values = values;
        }

        public string[] Keys { get; }

        public IComparable[] Values { get; }
    }


}
