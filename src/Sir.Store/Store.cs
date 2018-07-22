using System;
using System.Collections.Generic;
using System.IO;

namespace Sir.Store
{
    public class Store : IReader, IWriter
    {
        public string ContentType => string.Empty;

        private readonly ProducerConsumerQueue<WriteTransaction> _writeQueue;
        private readonly SessionFactory _sessionFactory;
        private readonly ITokenizer _tokenizer;

        public bool IsDisposed { get; private set; }

        public Store(SessionFactory sessionFactory, ITokenizer analyzer)
        {
            _tokenizer = analyzer;
            _sessionFactory = sessionFactory;
            _writeQueue = new ProducerConsumerQueue<WriteTransaction>(Commit);
        }

        public void Append(string collectionId, IEnumerable<IModel> data)
        {
            using (var tx = new WriteTransaction(collectionId.ToHash(), data))
            {
                _writeQueue.Enqueue(tx);
            }
        }

        private void Commit(WriteTransaction tx)
        {
            using (var session = _sessionFactory.CreateWriteSession(tx.CollectionId))
            {
                session.Write(tx.Data, _tokenizer);                
            }
            tx.Committed = true;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                _writeQueue.Dispose();

                IsDisposed = true;
            }
        }

        public Stream Read(IModelBinder modelBinder, Query query)
        {
            throw new NotImplementedException();
        }
    }
}
