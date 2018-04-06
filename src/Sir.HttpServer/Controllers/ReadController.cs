using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Sir.HttpServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Read")]
    public class ReadController : Controller
    {
        // GET: api/Read
        [HttpGet]
        public IEnumerable<string> Get()

        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Read/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Read
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Read/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
