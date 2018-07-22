using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sir.Store
{
    public class SessionFactory : IDisposable
    {
        private readonly SortedList<ulong, uint> _keys;
        private readonly IDictionary<string, Stream> _streams;
        private readonly object Sync = new object();
        private readonly VectorTree _index;
        private readonly string _dir;
        private readonly Stream _writableValueStream;
        private readonly Stream _keyMapWriteStream;
        private readonly Stream _valueStream;

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                foreach (var s in _streams.ToList())
                {
                    s.Value.Dispose();
                }

                _writableValueStream.Dispose();
                _keyMapWriteStream.Dispose();
                _valueStream.Dispose();

                IsDisposed = true;
            }
        }

        public SessionFactory(string dir)
        {
            _keys = LoadKeyMap(dir);
            _streams = new SortedList<string, Stream>();
            _index = DeserializeTree(dir);
            _dir = dir;
            _valueStream = CreateReadWriteStream(Path.Combine(dir, "_.val"));
            _writableValueStream = CreateAppendStream(Path.Combine(dir, "_.val"));
            _keyMapWriteStream = new FileStream(Path.Combine(dir, "_.kmap"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        private static SortedList<ulong, uint> LoadKeyMap(string dir)
        {
            var keys = new SortedList<ulong, uint>();

            using (var stream = new FileStream(Path.Combine(dir, "_.kmap"), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            {
                uint i = 0;
                var buf = new byte[sizeof(uint)];
                var read = stream.Read(buf, 0, buf.Length);

                while (read > 0)
                {
                    keys.Add(BitConverter.ToUInt32(buf, 0), i++);

                    read = stream.Read(buf, 0, buf.Length);
                }
            }

            return keys;
        }

        private static VectorTree DeserializeTree(string dir)
        {
            var ix = new SortedList<ulong, SortedList<uint, VectorNode>>();

            foreach (var collectionDir in Directory.GetDirectories(dir))
            {
                var collectionHash = new DirectoryInfo(collectionDir).Name;
                var collectionId = ulong.Parse(collectionHash);
                var colIndex = new SortedList<uint, VectorNode>();

                foreach (var ixFileName in Directory.GetFiles(collectionDir, "*.ix"))
                {
                    var keyHash = Path.GetFileNameWithoutExtension(new FileInfo(ixFileName).Name);
                    var keyId = uint.Parse(keyHash);
                    var key = collectionHash + "_" + keyHash;

                    using (var treeStream = File.OpenRead(Path.Combine(collectionDir, keyHash + ".ix")))
                    using (var wordStream = File.OpenRead(Path.Combine(collectionDir, "_.vec")))
                    {
                        var root = VectorNode.Deserialize(treeStream, wordStream);
                        colIndex.Add(keyId, root);
                    }
                }

                ix.Add(collectionId, colIndex);
            }
            return new VectorTree(ix);
        }

        public void AddKey(ulong keyHash, uint keyId)
        {
            _keys.Add(keyHash, keyId);

            var buf = BitConverter.GetBytes(keyHash);

            _keyMapWriteStream.Write(buf, 0, sizeof(ulong));
        }

        public uint GetKey(ulong keyHash)
        {
            return _keys[keyHash];
        }

        public bool TryGetKeyId(ulong key, out uint keyId)
        {
            if (!_keys.TryGetValue(key, out keyId))
            {
                keyId = 0;
                return false;
            }
            return true;
        }

        public WriteSession CreateWriteSession(ulong collectionId)
        {
            var session = new WriteSession(_dir, collectionId, this)
            {
                ValueStream = _writableValueStream,
                KeyStream = CreateAppendStream(string.Format("{0}.key", collectionId)),
                DocStream = CreateAppendStream(string.Format("{0}.docs", collectionId)),
                ValueIndexStream = CreateAppendStream(string.Format("{0}.vix", collectionId)),
                KeyIndexStream = CreateAppendStream(string.Format("{0}.kix", collectionId)),
                DocIndexStream = CreateReadWriteStream(string.Format("{0}.dix", collectionId)),
                PostingsStream = CreateReadWriteStream(string.Format("{0}.pos", collectionId))
            };

            session.Index = GetIndex(collectionId);
            return session;
        }

        public Session CreateReadSession(ulong collectionId)
        {
            var session = new Session(_dir, collectionId, this)
            {
                ValueStream = _valueStream,
                KeyStream = CreateReadStream(string.Format("{0}.key", collectionId)),
                DocStream = CreateReadStream(string.Format("{0}.doc", collectionId)),
                ValueIndexStream = CreateAppendStream(string.Format("{0}.vix", collectionId)),
                KeyIndexStream = CreateAppendStream(string.Format("{0}.kix", collectionId)),
                DocIndexStream = CreateAppendStream(string.Format("{0}.dix", collectionId)),
                PostingsStream = CreateReadStream(string.Format("{0}.pos", collectionId))
            };

            session.Index = GetIndex(collectionId);
            return session;
        }

        protected SortedList<uint, VectorNode> GetIndex(ulong collectionId)
        {
            return _index.GetOrCreateIndex(collectionId);
        }

        protected Stream CreateReadWriteStream(string fileName)
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

        protected Stream CreateAppendStream(string fileName)
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

        protected Stream CreateReadStream(string fileName)
        {
            if (File.Exists(fileName))
            {
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            return null;
        }
    }
}
