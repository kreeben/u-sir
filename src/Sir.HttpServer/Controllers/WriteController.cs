using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Route("write")]
    public class WriteController : Controller
    {
        private WriteOperationCollection _writeOperations;
        private ModelBinderCollection _modelBinders;

        public WriteController(ModelBinderCollection modelBinders, WriteOperationCollection writeOperations)
        {
            _modelBinders = modelBinders;
            _writeOperations = writeOperations;
        }

        [HttpPost("{*id}")]
        public IActionResult Post(string id)
        {
            var modelBinder = _modelBinders.Get(Request.ContentType);
            var writers = _writeOperations.GetMany(Request.ContentType);

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
