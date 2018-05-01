using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    public class ReadController : Controller
    {
        [HttpGet("read/{*id}")]
        public HttpResponseMessage Get(string id, string query)
        {
            var contentType = query == null ? Response.ContentType : "text/plain";
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
        
        
    }
}
