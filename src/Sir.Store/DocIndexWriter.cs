using System;
using System.IO;

namespace Sir.Store
{
    public class DocIndexWriter
    {
        private readonly Stream _stream;
        private static int _blockSize = sizeof(long)+sizeof(int);

        public DocIndexWriter(Stream stream)
        {
            _stream = stream;

            stream.Seek(0, SeekOrigin.Begin);

            var buf = new byte[_blockSize];
            var read = stream.Read(buf, 0, _blockSize);

            if (read == 0)
            {
                stream.SetLength(_blockSize);
                stream.Seek(0, SeekOrigin.End);
            }
            else
            {
                stream.Seek(0, SeekOrigin.End);
            }
        }

        public ulong GetNextDocId()
        {
            return (ulong)_stream.Position / (ulong)_blockSize;
        }

        public ulong Append(long offset, int len)
        {
            var id = GetNextDocId();

            _stream.Write(BitConverter.GetBytes(offset), 0, sizeof(long));
            _stream.Write(BitConverter.GetBytes(len), 0, sizeof(int));

            return id;
        }
    }
}
