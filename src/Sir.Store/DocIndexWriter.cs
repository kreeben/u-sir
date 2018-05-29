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
                // Because the stream is empty we can assume it is new.
                // Write an empty block so that docId starts at 1 instead of 0.
                // Because 0 is interpreted as "uninitialized data" in the postings file.

                stream.SetLength(_blockSize);
                stream.Seek(0, SeekOrigin.End);
            }
            else
            {
                stream.Seek(0, SeekOrigin.End);
            }
        }

        public ulong Append(long offset, int len)
        {
            var position = _stream.Position;
            var i = (ulong)position / (ulong)_blockSize;

            _stream.Write(BitConverter.GetBytes(offset), 0, sizeof(long));
            _stream.Write(BitConverter.GetBytes(len), 0, sizeof(int));

            return i;
        }
    }
}
