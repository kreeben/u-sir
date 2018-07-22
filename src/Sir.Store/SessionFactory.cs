using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sir.Store
{
    public class SessionFactory : IDisposable
    {
        private readonly IDictionary<string, Stream> _streams;
        private readonly object Sync = new object();
        private readonly VectorTree _index;
        private readonly string _dir;

        public bool IsDisposed { get; private set; }

        public SessionFactory(string dir)
        {
            _streams = new SortedList<string, Stream>();
            _index = Deserialize(dir);
            _dir = dir;
        }

        public static VectorTree Deserialize(string dataDir)
        {
            var ix = new SortedList<ulong, SortedList<uint, VectorNode>>();

            foreach (var collectionDir in Directory.GetDirectories(dataDir))
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


        public WriteSession CreateWriteSession(ulong collectionId)
        {
            var session = new WriteSession(_dir, collectionId)
            {
                ValueStream = CreateAppendStream(string.Format("{0}.val", collectionId)),
                KeyStream = CreateAppendStream(string.Format("{0}.key", collectionId)),
                DocStream = CreateAppendStream(string.Format("{0}.doc", collectionId)),
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
            var session = new Session(_dir, collectionId)
            {
                ValueStream = CreateReadStream(string.Format("{0}.val", collectionId)),
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

        private SortedList<uint, VectorNode> GetIndex(ulong collectionId)
        {
            return _index.GetOrCreateIndex(collectionId);
        }

        private Stream CreateReadWriteStream(string fileName)
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

        private Stream CreateAppendStream(string fileName)
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

        private Stream CreateReadStream(string fileName)
        {
            if (File.Exists(fileName))
            {
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            return null;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                foreach (var s in _streams.ToList())
                {
                    if (s.Value != null) s.Value.Dispose();
                }

                IsDisposed = true;
            }
        }
    }
}
