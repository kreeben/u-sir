using System.Collections.Generic;

namespace Sir
{
    public interface IWriter : IPlugin
    {
        void Write(string id, IEnumerable<IModel> data);
    }
}
