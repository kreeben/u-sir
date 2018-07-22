using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public interface IModelBinder : IPlugin
    {   
        IList<IModel> Parse(Stream data);
    }
}
