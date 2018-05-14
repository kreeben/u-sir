using System.Collections.Generic;

namespace Sir.Index
{
    public class IndexWriter : IWriter
    {
        public string ContentType => string.Empty;

        public void Append(string collectionId, IEnumerable<IModel> data)
        {
        }

        public void Dispose()
        {
        }
    }
}
