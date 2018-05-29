using System;
using System.IO;

namespace Sir.Store
{
    public class PostingsWriter
    {
        private readonly Stream _stream;
        private static int PAGE_SIZE = 4096;
        private static int SLOTS_PER_PAGE = 64;

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

        public long Append(long offset, ulong documentId)
        {
            var page = new byte[4096];
            ulong slot = 0;
            int pos = 0;

            _stream.Seek(offset, SeekOrigin.Begin);
            _stream.Read(page, 0, PAGE_SIZE);

            while (pos < SLOTS_PER_PAGE)
            {
                slot = BitConverter.ToUInt64(page, (pos*sizeof(ulong)));

                if (slot == 0)
                {
                    break;
                }
                else
                {
                    pos++;
                }
            }

            if (pos == SLOTS_PER_PAGE)
            {
                // Page is full but luckily the last word 
                // is the offset for the next page.
                // Jump to that location and start over.

                long nextOffset = Convert.ToInt64(BitConverter.ToUInt64(page, PAGE_SIZE - sizeof(ulong)));
                Append(nextOffset, documentId);
            }
            else if (pos == SLOTS_PER_PAGE - 2)
            {
                // Only two slots left in the page.
                // Fill the penultimate with a docId, 
                // allocate a new page and
                // store the offset of the new page
                // in the last slot.

                _stream.Write(BitConverter.GetBytes(documentId), 0, sizeof(ulong));
                var nextOffset = AllocatePage();
                _stream.Write(BitConverter.GetBytes(nextOffset), 0, sizeof(long));
            }
            else
            {
                // There was a vacant slot in the page.

                _stream.Seek(offset + pos * sizeof(ulong), SeekOrigin.Begin);
                _stream.Write(BitConverter.GetBytes(documentId), 0, sizeof(ulong));
            }
        }
    }
}
