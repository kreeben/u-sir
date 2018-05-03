using System;

namespace Sir
{
    public interface IModel
    {
        IComparable Id { get; }
        string[] Keys { get; }
        void Add(string key, IComparable value);
        IComparable Get(string key);
    }
}
