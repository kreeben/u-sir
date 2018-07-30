using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Route("io")]
    public class IOController : Controller
    {
        private PluginsCollection _plugins;

        public IOController(PluginsCollection plugins)
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
            Response.Headers.Add(
                "Location", new Microsoft.Extensions.Primitives.StringValues(string.Format("/{0}", collectionId)));

            return StatusCode(201); // Created
        }

        [HttpGet("{*collectionId}")]
        [HttpPut("{*collectionId}")]
        public ObjectResult Get(string collectionId, string query)
        {
            //TODO: add pagination

            var contentType = Request.ContentType;
            var accepts = Request.Headers["Accept"];

            if (query == null)
            {
                using (var r = new StreamReader(Request.Body))
                {
                    query = r.ReadToEnd();
                }
            }

            var queryParser = _plugins.Get<IQueryParser>(contentType);
            var reader = _plugins.Get<IReader>();

            if (queryParser == null || reader == null)
            {
                throw new NotSupportedException();
            }

            var parsedQuery = queryParser.Parse(query);
            parsedQuery.CollectionId = collectionId.ToHash();

            var payload = reader.Read(parsedQuery);
            return new ObjectResult(payload);
        }
    }
}
