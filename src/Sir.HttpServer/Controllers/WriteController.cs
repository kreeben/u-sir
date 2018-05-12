using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [HttpPost("{*collectionId}")]
        public async Task<IActionResult> Post(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId))
            {
                throw new ArgumentException("message", nameof(collectionId));
            }

            var modelParser = _plugins.Get<IModelParser>(Request.ContentType);
            var writers = _plugins.All<IWriter>(Request.ContentType);

            if (modelParser == null || writers == null)
            {
                return StatusCode(415); // Media type not supported
            }

            IList<IModel> data;

            try
            {
                data = modelParser.Parse(Request.Body);
            }
            catch (Exception wtf)
            {
                throw wtf;
            }

            foreach (var writer in writers)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        writer.Write(collectionId, data);
                    });
                }
                catch (Exception ew)
                {
                    throw ew;
                }
            }
            Response.Headers.Add(
                "Location", new Microsoft.Extensions.Primitives.StringValues("/read/" + id));

            return StatusCode(201); // Created
        }
    }
}
