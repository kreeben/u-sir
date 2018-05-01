using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Produces("application/json")]
    [Route("plugins")]
    public class PluginsController : Controller
    {
        private PluginCollection _writeActions;

        public PluginsController(PluginCollection writeActions)
        {
            _writeActions = writeActions;
        }

        [HttpGet]
        public IEnumerable<PluginModel> Get()
        {
            return _writeActions.Keys.Select(s => new PluginModel
            {
                ContentType = s,
                Actions = _writeActions.All<IPlugin>(s).Select(a=>a.ContentType)
            });
        }

        public class PluginModel
        {
            public string ContentType { get; set; }
            public IEnumerable<string> Actions { get; set; }
        }
    }

}