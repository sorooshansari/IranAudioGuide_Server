using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IAG_API_Server.Controllers
{
    public class test2Controller : ApiController
    {
        // GET: api/test2
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/test2/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/test2
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/test2/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/test2/5
        public void Delete(int id)
        {
        }
    }
}
