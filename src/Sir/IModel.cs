using System;

namespace Sir
{
    public interface IModel
    {
        IComparable[] Keys { get; }
        IComparable[] Values { get; }
    }
}
