using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Route("write")]
    public class WriteController : Controller
    {
        private WriteActionCollection _writeActions;

        public WriteController(WriteActionCollection writeActions)
        {
            _writeActions = writeActions;
        }

        [HttpPost("{*table}")]
        public IActionResult Post(string table)
        {
            var postActions = _writeActions.Get(Request.ContentType);
            if (postActions.Count == 0)
            {
                // Media type not supported
                return StatusCode(415);
            }
            foreach(var action in postActions)
            {
                try
                {
                    action.Execute(table, Request.Body);
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
