using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public interface IModelFormatter : IPlugin
    {
        void Format(IEnumerable<IModel> data, Stream output);
    }
}
