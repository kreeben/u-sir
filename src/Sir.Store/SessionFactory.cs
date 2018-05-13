using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sir.Store
{
    public class SessionFactory : IDisposable
    {
        private readonly IDictionary<string, Stream> _streams;

        public Session CreateWriteSession(ulong collectionId)
        {
            return new Session
            {
                ValueStream = GetWriteStream(string.Format("{0}.val", collectionId)),
                KeyStream = GetWriteStream(string.Format("{0}.key", collectionId)),
                DocStream = GetWriteStream(string.Format("{0}.doc", collectionId)),
                ValueIndexStream = GetWriteStream(string.Format("{0}.vix", collectionId)),
                KeyIndexStream = GetWriteStream(string.Format("{0}.kix", collectionId)),
                DocIndexStream = GetWriteStream(string.Format("{0}.dix", collectionId)),
                PostingsStream = GetWriteStream(string.Format("{0}.pos", collectionId))
            };
        }

        public Session CreateReadSession(ulong collectionId)
        {
            return new Session
            {
                ValueStream = GetReadStream(string.Format("{0}.val", collectionId)),
                KeyStream = GetReadStream(string.Format("{0}.key", collectionId)),
                DocStream = GetReadStream(string.Format("{0}.doc", collectionId)),
                ValueIndexStream = GetWriteStream(string.Format("{0}.vix", collectionId)),
                KeyIndexStream = GetWriteStream(string.Format("{0}.kix", collectionId)),
                DocIndexStream = GetWriteStream(string.Format("{0}.dix", collectionId)),
                PostingsStream = GetReadStream(string.Format("{0}.pos", collectionId))
            };
        }

        private Stream GetWriteStream(string fileName)
        {
            // https://stackoverflow.com/questions/122362/how-to-empty-flush-windows-read-disk-cache-in-c
            //const FileOptions FileFlagNoBuffering = (FileOptions)0x20000000;
            //FileStream file = new FileStream(fileName, fileMode, fileAccess, fileShare, blockSize,
            //    FileFlagNoBuffering | FileOptions.WriteThrough | fileOptions);

            Stream stream;
            if (!_streams.TryGetValue(fileName, out stream))
            {
                stream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
                _streams.Add(fileName, stream);
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
