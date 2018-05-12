using System;
using System.Collections.Generic;
using System.IO;

namespace Sir.Store
{
    public class DocumentIndex
    {
        private readonly Stream _writeStream;
        private readonly Stream _readStream;
       
        public DocumentIndex(IEnumerable<(long, int)> memoryBlocks, Stream readStream, Stream writeStream)
        {
            _writeStream = writeStream;
            _readStream = readStream;
        }

        public int Add(BlockInfo blockInfo)
        {
            int id;

        

            return id;
        }

        public string Get(int id)
        {
            if (id > _list.Count - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return _list[id];
        }


    }


    public struct BlockInfo : IComparable<BlockInfo>, IEquatable<BlockInfo>
    {
        public long Offset { get; private set; }
        public int Length { get; private set; }

        public BlockInfo(long offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public int CompareTo(BlockInfo other)
        {
            if (other.Offset == Offset && other.Length == Length)
                return 0;
            else if (other.Offset < Offset || other.Length < Length)
                return -1;
            else
                return 1;
        }

        public bool Equals(BlockInfo other)
        {
            if (other.Offset == Offset && other.Length == Length)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Offset.GetHashCode();
            hash = (hash * 7) + Length.GetHashCode();
            return hash;
        }
    }
}
