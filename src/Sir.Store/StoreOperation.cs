using System;
using System.Collections.Generic;

namespace Sir.Store
{
    public class StoreOperation : IWriteOperation
    {
        public int Ordinal => 0;

        public string ContentType => string.Empty;

        public void Write(string id, IEnumerable<IModel> data)
        {
        }
    }
}
