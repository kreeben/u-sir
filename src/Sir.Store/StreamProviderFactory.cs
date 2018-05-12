using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sir.Store
{
    public class StreamProviderFactory : IDisposable
    {
        private readonly IDictionary<string, Stream> _streams;

        public StreamProvider CreateWriteStreamProvider(ulong collectionId)
        {
            return new StreamProvider
            {
                IndexStream = GetWriteStream(string.Format("{0}.dix", collectionId)),
                ValueStream = GetWriteStream(string.Format("{0}.val", collectionId)),
                KeyStream = GetWriteStream(string.Format("{0}.key", collectionId)),
                KeyDictionaryStream = GetWriteStream(string.Format("{0}.kdic", collectionId))
            };
        }

        public StreamProvider CreateReadStreamProvider(ulong collectionId)
        {
            return new StreamProvider
            {
                IndexStream = GetReadStream(string.Format("{0}.dix", collectionId)),
                ValueStream = GetReadStream(string.Format("{0}.val", collectionId)),
                KeyStream = GetReadStream(string.Format("{0}.key", collectionId)),
                KeyDictionaryStream = GetReadStream(string.Format("{0}.kdic", collectionId))
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

        ~StreamProviderFactory()
        {
            if (_streams.Count > 0)
                Dispose();
        }
    }
}
