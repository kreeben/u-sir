using System;
using System.IO;

namespace Sir.Store
{
    public class DocIndexWriter
    {
        private readonly Stream _stream;
        private static uint _blockSize = sizeof(long)+sizeof(int);

        public DocIndexWriter(Stream stream)
        {
            _stream = stream;
        }

        public ulong Append(long offset, int len)
        {
            var position = _stream.Position;
            var i = (ulong)position / _blockSize;

            _stream.Write(BitConverter.GetBytes(offset), 0, sizeof(long));
            _stream.Write(BitConverter.GetBytes(len), 0, sizeof(int));

            return i;
        }
    }
}
