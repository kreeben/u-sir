using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public interface IModelBinder : IPlugin
    {   
        IEnumerable<IModel> Bind(HttpRequest request);
        void Serialize(IEnumerable<IModel> data, Stream outputStream);
    }
}
