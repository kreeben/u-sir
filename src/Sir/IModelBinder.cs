using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public interface IModelBinder : IPlugin
    {   
        IEnumerable<IModel> Deserialize(Stream data);
        void Serialize(IEnumerable<IModel> data, Stream outputStream);
    }
}
