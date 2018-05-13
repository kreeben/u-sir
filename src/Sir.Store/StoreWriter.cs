using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sir.Store
{
    public class StoreWriter : IWriter
    {
        public string ContentType => string.Empty;

        private readonly BlockingCollection<WriteTransaction> _writeQueue;
        private readonly SessionFactory _streamProviderFactory;

        public StoreWriter(SessionFactory streamProviderFactory)
        {
            _streamProviderFactory = streamProviderFactory;
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

        public void Write(string collectionId, IEnumerable<IModel> data)
        {
            using (var tx = new WriteTransaction(Hash.ToHash(collectionId), data))
            {
                _writeQueue.Add(tx);
            }
            DateTime.Now.Kind
            //todo: log request async in a timestamped collection
        }

        private void Commit(WriteTransaction tx)
        {
            var streamProvider = _streamProviderFactory.CreateWriteStreamProvider(tx.CollectionId);
            foreach (var model in tx.Data)
            {
                var ix = Write(model, streamProvider);
            }
            tx.Committed = true;
        }

        private (long keyOffset, int keyLength) Write(IModel model, Session streamProvider)
        {
            var ix = new byte[model.Keys.Length][];

            for (int i = 0; i < model.Keys.Length; i++)
            {
                var key = model.Keys[i];
                var keyId = _keyIndex.Add(key);
                int keyLen = key.Length * sizeof(char);
                var keyBin = System.Text.Encoding.Unicode.GetBytes(key);

                var meta = StoreSerializer.Serialize(model.Values[i], streamProvider.ValueStream);
                var offset = meta.offset;
                byte valType = meta.dataType;
                int valLen = meta.len;
            }
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] combined = new byte[arrays.Sum(x => x.Length)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, combined, offset, data.Length);
                offset += data.Length;
            }
            return combined;
        }

        public void Dispose()
        {
            _writeQueue.CompleteAdding();
            while (!_writeQueue.IsCompleted)
            {
                Thread.Sleep(10);
            }
            _writeQueue.Dispose();
        }
    }

    public class PostingsReader
    {
        private readonly Stream _stream;

        public PostingsReader(Stream stream)
        {
            _stream = stream;
        }

        public IEnumerable<ulong> GetDocumentIds(long offset, int length)
        {
            var buf = new byte[length];
            var parsed = 0;

            _stream.Seek(offset, SeekOrigin.Begin);
            _stream.Read(buf, 0, length);

            while (parsed < length)
            {
                yield return BitConverter.ToUInt64(buf, parsed);
                parsed += sizeof(ulong);
            }
        }
    }

    public class PostingsWriter
    {
        private readonly Stream _stream;

        public PostingsWriter(Stream stream)
        {
            _stream = stream;
        }

        public (long offset, int length) Append(IEnumerable<ulong> documentIds)
    }
}
