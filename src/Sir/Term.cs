using System;

namespace Sir
{
    public class Term
    {
        public IComparable Key { get; private set; }
        public IComparable Value { get; set; }

        public Term(IComparable key, IComparable value)
        {
            Key = key;
            Value = value;
        }
    }
}