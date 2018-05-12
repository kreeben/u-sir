using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    public class ReadController : Controller
    {
        private PluginCollection _plugins;

        public ReadController(PluginCollection plugins)
        {
            _plugins = plugins;
        }

        [HttpGet("read/{*id}")]
        [HttpPut("read/{*id}")]
        public HttpResponseMessage Get(string id, string query)
        {
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
            var modelBinder = _plugins.Get<IModelParser>(accepts);
            var reader = _plugins.Get<IReader>(accepts);

            if (queryParser == null || modelBinder == null || reader == null)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.UnsupportedMediaType);
            }

            var parsedQuery = string.IsNullOrWhiteSpace(query) ? null : queryParser.Parse(query);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var outputStream = reader.Read(id, modelBinder, parsedQuery);
            response.Content = new StreamContent(outputStream);
            return response;
        }
        
        
    }
}
