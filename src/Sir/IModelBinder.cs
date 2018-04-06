using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace Sir
{
    public interface IModelBinder : IHasContentType
    {   
        IEnumerable<IModel> Bind(HttpRequest request);
    }
}
