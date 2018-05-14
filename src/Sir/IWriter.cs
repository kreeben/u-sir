using System.Collections.Generic;

namespace Sir
{
    public interface IWriter : IPlugin
    {
        void Append(string collectionId, IEnumerable<IModel> data);
    }
}
