﻿using System.Collections.Generic;

namespace Sir.Index
{
    public class IndexWriter : IWriter
    {
        public int Ordinal => 1;

        public string ContentType => string.Empty;

        public void Write(string id, IEnumerable<IModel> data)
        {
        }
    }
}
