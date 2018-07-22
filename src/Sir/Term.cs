using System;

namespace Sir
{
    public class Term
    {
        public IComparable Key { get; private set; }
        public IComparable Value { get; set; }
        public ulong CollectionId { get; private set; }

        public Term(IComparable key, IComparable value, ulong collectionId)
        {
            Key = key;
            Value = value;
            CollectionId = collectionId;
        }
    }
}