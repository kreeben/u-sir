using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Sir
{
    public interface IModelBinder : IPlugin
    {   
        IEnumerable<IModel> Bind(HttpRequest request);
    }
}
