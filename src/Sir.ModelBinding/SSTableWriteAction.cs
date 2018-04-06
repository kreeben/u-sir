using System;
using System.IO;

namespace Sir.SSTable
{
    public class SSTableWriter : IWriteAction
    {
        public int Ordinal => 0;
        public string ContentType => "application/octet-stream";

        public void Write(string id, Stream stream)
        {
            


        }
    }
}
