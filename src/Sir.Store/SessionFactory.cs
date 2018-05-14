using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sir.Store
{
    public class SessionFactory : IDisposable
    {
        private readonly IDictionary<string, Stream> _streams;
        private readonly IDictionary<string, BPlusTree<uint, byte[]>> _ixs;
        private readonly object Sync = new object();

        public SessionFactory()
        {
            _streams = new Dictionary<string, Stream>();
            _ixs = new Dictionary<string, BPlusTree<uint, byte[]>>();
        }

        public Session CreateWriteSession(ulong collectionId)
        {
            return new Session
            {
                Index = GetIndex(string.Format("{0}.ix", collectionId)),
                ValueStream = GetAppendStream(string.Format("{0}.val", collectionId)),
                KeyStream = GetAppendStream(string.Format("{0}.key", collectionId)),
                DocStream = GetAppendStream(string.Format("{0}.doc", collectionId)),
                ValueIndexStream = GetAppendStream(string.Format("{0}.vix", collectionId)),
                KeyIndexStream = GetAppendStream(string.Format("{0}.kix", collectionId)),
                DocIndexStream = GetAppendStream(string.Format("{0}.dix", collectionId)),
                PostingsStream = GetReadWriteStream(string.Format("{0}.pos", collectionId)),
            };
        }

        public Session CreateReadSession(ulong collectionId)
        {
            return new Session
            {
                Index = GetIndex(string.Format("{0}.ix", collectionId)),
                ValueStream = GetReadStream(string.Format("{0}.val", collectionId)),
                KeyStream = GetReadStream(string.Format("{0}.key", collectionId)),
                DocStream = GetReadStream(string.Format("{0}.doc", collectionId)),
                ValueIndexStream = GetAppendStream(string.Format("{0}.vix", collectionId)),
                KeyIndexStream = GetAppendStream(string.Format("{0}.kix", collectionId)),
                DocIndexStream = GetAppendStream(string.Format("{0}.dix", collectionId)),
                PostingsStream = GetReadStream(string.Format("{0}.pos", collectionId))
            };
        }

        private BPlusTree<uint, byte[]> GetIndex(string fileName)
        {
            BPlusTree<uint, byte[]> tree;
            if (!_ixs.TryGetValue(fileName, out tree))
            {
                lock (Sync)
                {
                    if (!_ixs.TryGetValue(fileName, out tree))
                    {
                        tree = CreateTree(Path.Combine(Directory.GetCurrentDirectory(), fileName));
                        _ixs.Add(fileName, tree);
                    }
                }
            }
            return tree;
        }

        private static BPlusTree<uint, byte[]> CreateTree(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "_.ix"), fileName);
            }

            var options = new BPlusTree<uint, byte[]>.OptionsV2(PrimitiveSerializer.UInt32, new BytesSerializer());

            options.CalcBTreeOrder(32, 128);
            options.FileName = fileName;
            options.CreateFile = CreatePolicy.Never;

            return new BPlusTree<uint, byte[]>(options);
        }

        private Stream GetReadWriteStream(string fileName)
        {
            Stream stream;
            if (!_streams.TryGetValue(fileName, out stream))
            {
                lock (Sync)
                {
                    if (!_streams.TryGetValue(fileName, out stream))
                    {
                        stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        _streams.Add(fileName, stream);
                    }
                }
            }
            return stream;
        }

        private Stream GetAppendStream(string fileName)
        {
            // https://stackoverflow.com/questions/122362/how-to-empty-flush-windows-read-disk-cache-in-c
            //const FileOptions FileFlagNoBuffering = (FileOptions)0x20000000;
            //FileStream file = new FileStream(fileName, fileMode, fileAccess, fileShare, blockSize,
            //    FileFlagNoBuffering | FileOptions.WriteThrough | fileOptions);

            Stream stream;
            if (!_streams.TryGetValue(fileName, out stream))
            {
                lock (Sync)
                {
                    if (!_streams.TryGetValue(fileName, out stream))
                    {
                        stream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
                        _streams.Add(fileName, stream);
                    }
                }
            }
            return stream;
        }

        private Stream GetReadStream(string fileName)
        {
            if (File.Exists(fileName))
            {
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var s in _streams.ToList())
            {
                s.Value.Dispose();
                _streams.Remove(s.Key);
            }
        }

        ~SessionFactory()
        {
            if (_streams.Count > 0)
                Dispose();
        }
    }
}
