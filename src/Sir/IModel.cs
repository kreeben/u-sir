using System;

namespace Sir
{
    public interface IModel
    {
        string[] Keys { get; }
        void Add(string key, IComparable value);
        IComparable Get(string key);
    }
}
