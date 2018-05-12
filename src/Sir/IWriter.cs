using System.Collections.Generic;

namespace Sir
{
    public interface IWriter : IPlugin
    {
        void Write(string collectionId, IEnumerable<IModel> data);
    }
}
