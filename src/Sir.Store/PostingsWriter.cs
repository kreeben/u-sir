using System;
using System.IO;

namespace Sir.Store
{
    public class PostingsWriter
    {
        private readonly Stream _stream;
        private static int PAGE_SIZE = 4096;

        public PostingsWriter(Stream stream)
        {
            _stream = stream;
            _stream.Seek(0, SeekOrigin.End);
        }

        public long AllocatePage()
        {
            _stream.Seek(0, SeekOrigin.End);

            var pos = _stream.Position;
            _stream.SetLength(pos + PAGE_SIZE);
            return pos;
        }

        public void Append(long offset, ulong documentId)
        {
            _stream.Seek(offset, SeekOrigin.Begin);

            var page = new byte[4096];
            var read = _stream.Read(page, 0, PAGE_SIZE);

            if (read == PAGE_SIZE)
            {
                // Page is full but luckily the last word 
                // is the offset for the next page.

                long nextOffset = BitConverter.ToInt64(page, PAGE_SIZE - sizeof(long));

                Append(nextOffset, documentId);
            }
            else if (read == PAGE_SIZE - 2 * sizeof(long))
            {
                // Only one slot left in the page.
                // Fill it, allocate a new page and
                // store the offset of the new page
                // as the last word of the current.

                _stream.Write(BitConverter.GetBytes(documentId), 0, sizeof(ulong));
                var nextOffset = AllocatePage();
                _stream.Write(BitConverter.GetBytes(nextOffset), 0, sizeof(long));
            }
            else
            {
                var data = BitConverter.GetBytes(documentId);
                Buffer.BlockCopy(data, 0, page, read, sizeof(ulong));
                _stream.Seek(offset, SeekOrigin.Begin);
                _stream.Write(page, 0, PAGE_SIZE);
            }
        }
    }
}
