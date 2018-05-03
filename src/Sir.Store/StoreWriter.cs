using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Sir.Store
{
    public class StoreWriter : IWriter
    {
        public int Ordinal => 0;
        public string ContentType => string.Empty;

        public void Write(string id, IEnumerable<IModel> data)
        {

        }
    }

    public class FileSpan
    {
        public long Offset { get; set; }
        public int Length { get; set; }
    }

    public class FileIndexWriter
    {
        private readonly Stream _stream;
        
        public FileIndexWriter(Stream stream)
        {
            _stream = stream;
        }

        public void Write(ulong id, FileSpan span)
        {
            var location = Convert.ToInt64(id) * sizeof(long) + sizeof(int);

            _stream.Seek(location, SeekOrigin.Begin);
            _stream.Write(BitConverter.GetBytes(span.Offset), 0, sizeof(long));
            _stream.Write(BitConverter.GetBytes(span.Length), 0, sizeof(int));
        }
    }

    public class FileIndexReader
    {
        private readonly Stream _stream;

        public FileIndexReader(Stream stream)
        {
            _stream = stream;
        }

        public FileSpan Read(ulong id)
        {
            var location = Convert.ToInt64(id) * sizeof(long) + sizeof(int);
            var buffer = new byte[sizeof(long) + sizeof(int)];

            _stream.Seek(location, SeekOrigin.Begin);
            _stream.Read(buffer, 0, sizeof(long) + sizeof(int));

            var span = new FileSpan();
            span.Offset = BitConverter.ToInt64(buffer, 0);
            span.Length = BitConverter.ToInt32(buffer, sizeof(long));
            return span;
        }
    }

    public class DocumentStore
    {
        private readonly Stream _stream;

        public DocumentStore(Stream stream)
        {
            _stream = stream;
        }

    }
}
