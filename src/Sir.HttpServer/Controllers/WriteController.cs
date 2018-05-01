using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Route("write")]
    public class WriteController : Controller
    {
        private PluginCollection _plugins;

        public WriteController(PluginCollection plugins)
        {
            _plugins = plugins;
        }

        [HttpPost("{*id}")]
        public IActionResult Post(string id)
        {
            var modelBinder = _plugins.Get<IModelBinder>(Request.ContentType);
            var writers = _plugins.All<IWriter>(Request.ContentType);

            if (modelBinder == null || writers == null)
            {
                // Media type not supported
                return StatusCode(415);
            }

            IList<IModel> data;

            try
            {
                data = modelBinder.Bind(Request).ToList();
            }
            catch (Exception wtf)
            {
                //todo: log
                throw wtf;
            }

            foreach (var writer in writers)
            {
                try
                {
                    writer.Write(id, data);
                }
                catch (Exception ew)
                {
                    //todo: log
                    throw ew;
                }
            }
            return StatusCode(200);
        }
    }
}
