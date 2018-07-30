using System;
using System.Collections;

namespace Sir
{
    public class Model : IModel, IDictionary
    {
        private readonly IDictionaryEnumerator _enumerator;

        public Model(IComparable[] keys, IComparable[] values)
        {
            Keys = keys;
            Values = values;
            _enumerator = new ModelDictionaryEnumerator(keys, values);
        }

        public object this[object key] { get => Values[Array.IndexOf(Keys, (IComparable)key)]; set => throw new NotImplementedException(); }

        public IComparable[] Keys { get; }

        public IComparable[] Values { get; }

        public bool IsFixedSize => true;

        public bool IsReadOnly => true;

        public int Count => Keys.Length;

        public bool IsSynchronized => false;

        public object SyncRoot => new object();

        ICollection IDictionary.Keys => Keys;

        ICollection IDictionary.Values => Values;

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            return Array.IndexOf(Keys, (IComparable)key) > -1;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _enumerator;
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }

    public class ModelDictionaryEnumerator : IDictionaryEnumerator
    {
        private readonly IEnumerator _keysEnumerator;
        private readonly IEnumerator _valuesEnumerator;

        public DictionaryEntry Entry => new DictionaryEntry(_keysEnumerator.Current, _valuesEnumerator.Current);

        public object Key => throw new NotImplementedException();

        public object Value => throw new NotImplementedException();

        public object Current => throw new NotImplementedException();

        public ModelDictionaryEnumerator(IComparable[] keys, IComparable[] values)
        {
            _keysEnumerator = keys.GetEnumerator();
            _valuesEnumerator = values.GetEnumerator();
        }

        public bool MoveNext()
        {
            return _keysEnumerator.MoveNext() && _valuesEnumerator.MoveNext();
        }

        public void Reset()
        {
            _keysEnumerator.Reset();
            _valuesEnumerator.Reset();
        }
    }
}
