using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Produces("application/json")]
    [Route("plugins")]
    public class PluginsController : Controller
    {
        private WriteOperationCollection _writeActions;

        public PluginsController(WriteOperationCollection writeActions)
        {
            _writeActions = writeActions;
        }

        public class PluginModel
        {
            public string ContentType { get; set; }
            public IEnumerable<string> Actions { get; set; }
        }
    }

}