using IAG_API_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;

namespace IAG_API_Server.Controllers
{
    public class UpdateController : ApiController
    {
        dbTools dbTools = new dbTools();
        // GET: api/Update/5
        public GetAllVM Get(int LastUpdateNumber)
        {
            return dbTools.GetUpdate(LastUpdateNumber);
        }
        
        // POST: api/Update/5
        public GetAllVM Post(int LastUpdateNumber)
        {
            return dbTools.GetUpdate(LastUpdateNumber);
        }

        // PUT: api/Update/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Update/5
        public void Delete(int id)
        {
        }

    }
}
