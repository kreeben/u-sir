using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Route("write")]
    public class WriteController : Controller
    {
        private ModelBinderCollection _modelBinders;

        public WriteController(ModelBinderCollection modelBinders)
        {
            _modelBinders = modelBinders;
        }

        [HttpPost("{*path}")]
        public IActionResult Post(string path)
        {
            var modelBinder = _modelBinders.Get(Request.ContentType);

            if (modelBinder == null)
            {
                // HTTP error code 415: media type not supported
                return StatusCode(415);
            }
            
            var payload = Request.Body;
            var fileId = string.Format("{0}.{1}", Guid.NewGuid().ToString(), "sir");
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", fileId);

            using (var fileStream = System.IO.File.Create(fileName))
            {
                payload.CopyTo(fileStream);
            }

            return StatusCode(200);
        }
    }
}
