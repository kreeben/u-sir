using System;

namespace Sir.Json
{
    public class Model : IModel
    {
        public Model(IComparable[] keys, IComparable[] values)
        {
            Keys = keys;
            Values = values;
        }

        public IComparable[] Keys { get; }

        public IComparable[] Values { get; }
    }


}
