using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    //[Route("write")]
    public class IOController : Controller
    {
        private PluginCollection _plugins;

        public IOController(PluginCollection plugins)
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

            var binder = _plugins.Get<IModelBinder>(Request.ContentType);
            var writers = _plugins.All<IWriter>(Request.ContentType).ToList();

            if (binder == null || writers == null || writers.Count == 0)
            {
                return StatusCode(415); // Media type not supported
            }

            IList<IModel> data;

            try
            {
                data = binder.Parse(Request.Body);
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
                        writer.Append(collectionId, data);
                    });
                }
                catch (Exception ew)
                {
                    throw ew;
                }
            }
            //Response.Headers.Add(
            //    "Location", new Microsoft.Extensions.Primitives.StringValues(collectionId));

            return StatusCode(201); // Created
        }

        [HttpGet("{*collectionId}")]
        [HttpPut("{*collectionId}")]
        public HttpResponseMessage Get(string collectionId, string query)
        {
            if (string.IsNullOrWhiteSpace(collectionId) || string.IsNullOrWhiteSpace(query))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }

            var contentType = Request.ContentType;
            var accepts = Request.Headers["Accept"];

            if (query != null)
            {
                contentType = "text/plain";
            }
            else
            {
                using (var r = new StreamReader(Request.Body))
                {
                    query = r.ReadToEnd();
                }
            }

            var queryParser = _plugins.Get<IQueryParser>(contentType);
            var modelBinder = _plugins.Get<IModelBinder>(accepts);
            var reader = _plugins.Get<IReader>(accepts);

            if (queryParser == null || modelBinder == null || reader == null)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.UnsupportedMediaType);
            }

            var parsedQuery = string.IsNullOrWhiteSpace(query) ? null
                : queryParser.Parse(collectionId.ToHash(), query);

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var outputStream = reader.Read(modelBinder, parsedQuery);
            response.Content = new StreamContent(outputStream);
            return response;
        }
    }
}
