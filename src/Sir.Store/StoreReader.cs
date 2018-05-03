using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sir.Store
{
    public class StoreReader : IReader
    {
        public string ContentType => string.Empty;
        public int Ordinal => 0;

        public Stream Read(string id, IModelBinder modelBinder, Query query = null)
        {
            throw new NotImplementedException();
        }
    }
}
