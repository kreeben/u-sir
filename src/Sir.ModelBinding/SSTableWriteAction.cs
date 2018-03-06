using System;
using System.IO;

namespace Sir.SSTable
{
    public class SSTableWriteAction : IWriteAction
    {
        public int Ordinal => 0;
        public string ContentType => "application/octet-stream";

        public void Execute(string table, Stream stream)
        {
            

        }
    }
}
