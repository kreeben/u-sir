using System.Collections.Generic;

namespace Sir
{
    public interface IWriteOperation : IHasContentType
    {
        int Ordinal { get; }
        void Write(string id, IEnumerable<IModel> data);
    }
}
