using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public interface IModelParser : IPlugin
    {   
        IList<IModel> Parse(Stream data);
    }
}
