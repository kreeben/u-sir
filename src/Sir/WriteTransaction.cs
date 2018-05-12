using System;
using System.Collections.Generic;
using System.Threading;

namespace Sir
{
    public class WriteTransaction : IDisposable
    {
        public ulong CollectionId { get; }
        public IEnumerable<IModel> Data { get; private set; }
        public bool Committed { get { return _committed; } set { _committed = value; } }
        private volatile bool _committed;

        public WriteTransaction(ulong collectionId, IEnumerable<IModel> data)
        {
            CollectionId = collectionId;
            Data = data;
        }

        public void Dispose()
        {
            while (!_committed)
            {
                Thread.Sleep(10);
            }
        }
    }
}
