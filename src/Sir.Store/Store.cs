using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sir.Store
{
    public class Store : IWriter
    {
        public string ContentType => string.Empty;

        private readonly BlockingCollection<WriteTransaction> _writeQueue;
        private readonly SessionFactory _sessionFactory;

        public bool IsDisposed { get; private set; }

        public Store(SessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _writeQueue = new BlockingCollection<WriteTransaction>();

            Task.Run(() =>
            {
                while (!_writeQueue.IsCompleted)
                {
                    WriteTransaction tx = null;
                    try
                    {
                        tx = _writeQueue.Take();
                    }
                    catch (InvalidOperationException) { }

                    if (tx != null)
                    {
                        Commit(tx);
                    }
                }
            });
        }

        public void Append(string collectionId, IEnumerable<IModel> data)
        {
            using (var tx = new WriteTransaction(collectionId.ToHash(), data))
            {
                _writeQueue.Add(tx);
            }
        }

        private void Commit(WriteTransaction tx)
        {
            using (var session = _sessionFactory.CreateWriteSession(tx.CollectionId))
            {
                Write(tx.Data, session);                
            }
            tx.Committed = true;
        }

        private void Write(IEnumerable<IModel> data, Session session)
        {
            var vals = new ValueWriter(session.ValueStream);
            var keys = new ValueWriter(session.KeyStream);
            var docs = new DocWriter(session.DocStream);
            var valIx = new ValueIndexWriter(session.ValueIndexStream);
            var keyIx = new ValueIndexWriter(session.KeyIndexStream);
            var docIx = new DocIndexWriter(session.DocIndexStream);
            var postings = new PostingsWriter(session.PostingsStream);
            var docMapping = new List<(uint keyId, uint valId)>();
            var docTerms = new List<uint>();

            foreach (var model in data)
            {
                for (int i = 0; i < model.Keys.Length; i++)
                {
                    ulong term = BinaryHelper.To64BitTerm(model.Keys[i], model.Values[i]);
                    uint keyId, valId;
                    long posOffset;

                    if (!session.TryGet(term, out keyId, out valId, out posOffset))
                    {
                        // We have found a new unique term!

                        // store value
                        var valMeta = vals.Append(model.Values[i]);
                        valId = valIx.Append(valMeta.offset, valMeta.len, valMeta.dataType);

                        // store key
                        var keyMeta = keys.Append(model.Values[i]);
                        keyId = keyIx.Append(keyMeta.offset, keyMeta.len, keyMeta.dataType);

                        // allocate space on disk for postings
                        posOffset = postings.AllocatePage();

                        // add term to index
                        session.Add(term, keyId, valId, posOffset);
                    }
                    docMapping.Add((keyId, valId));
                    docTerms.Add(term);
                }

                var docMeta = docs.Append(docMapping);
                var docId = docIx.Append(docMeta.offset, docMeta.length);

                foreach (var term in docTerms)
                {
                    var postingMeta = session.Get(term);
                    postings.Append(postingMeta.posOffset, docId);
                }
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                // cleanup
                _writeQueue.CompleteAdding();
                while (!_writeQueue.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                _writeQueue.Dispose();

                IsDisposed = true;
            }
        }

        ~Store()
        {
            Dispose();
        }
    }
}
