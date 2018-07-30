using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public interface IReader : IPlugin
    {
        IEnumerable<IModel> Read(Query query);
        void Render(IModelFormatter modelBinder, Query query, Stream output);
    }
}
