using System;

namespace Sir
{
    public interface IModel
    {
        string[] Keys { get; }
        IComparable[] Values { get; }
    }
}
