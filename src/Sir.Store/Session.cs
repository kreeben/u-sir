using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Sir.Store
{
    public class Session : IDisposable
    {
        public BPlusTree<uint, byte[]> Index { get; set; }
        public Stream ValueStream { get; set; }
        public Stream KeyStream { get; set; }
        public Stream DocStream { get; set; }
        public Stream ValueIndexStream { get; set; }
        public Stream KeyIndexStream { get; set; }
        public Stream DocIndexStream { get; set; }
        public Stream PostingsStream { get; set; }

        public (uint keyId, uint valId, long posOffset) Get(uint term)
        {
            uint keyId, valId;
            long posOffset;

            if (!TryGet(term, out keyId, out valId, out posOffset))
            {
                throw new InvalidOperationException();
            }

            return (keyId, valId, posOffset);
        }

        public bool TryGet(uint term, out uint keyId, out uint valId, out long posOffset)
        {
            byte[] buf;
            if (!Index.TryGetValue(term, out buf))
            {
                keyId = 0;
                valId = 0;
                posOffset = 0;

                return false;
            }

            keyId = BitConverter.ToUInt32(buf, 0);
            valId = BitConverter.ToUInt32(buf, sizeof(uint));
            posOffset = BitConverter.ToInt64(buf, sizeof(uint) + sizeof(uint));

            return true;
        }

        public void Add(uint term, uint keyId, uint valId, long posOffset)
        {
            var keyIdBuf = BitConverter.GetBytes(keyId);
            var valIdBuf = BitConverter.GetBytes(valId);
            var posOffsetBuf = BitConverter.GetBytes(posOffset);

            var buf = new byte[sizeof(uint) + sizeof(uint) + sizeof(long)];
            Buffer.BlockCopy(keyIdBuf, 0, buf, 0, keyIdBuf.Length);
            Buffer.BlockCopy(valIdBuf, 0, buf, sizeof(uint), valIdBuf.Length);
            Buffer.BlockCopy(posOffsetBuf, 0, buf, sizeof(uint) + sizeof(uint), posOffsetBuf.Length);

            Index.Add(term, buf);
        }

        public void Dispose()
        {
            Index.Dispose();
        }
    }

}
