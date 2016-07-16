using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IAG_API_Server.Models;

namespace IAG_API_Server.Controllers
{
    public class testController : ApiController
    {
        dbManager dbManager = new dbManager();
        // GET: api/test
        public IEnumerable<Guid> Get()
        {
            var table = new DataTable();
            table.Columns.Add("Item", typeof(string));

            for (int i = 0; i < 10; i++)
                table.Rows.Add(new Guid());

            var pList = new SqlParameter("@list", SqlDbType.Structured);
            pList.TypeName = "dbo.IdList";
            pList.Value = table;
            
            var items = dbManager.TableResultSP("AllPlacesId", pList);
            List<Guid> res = items.AsEnumerable()
                           .Select(r => r.Field<Guid>("Item"))
                           .ToList();
            return res;
        }

        // GET: api/test/5
        public string Get(int id)
        {
            return dbManager.IntegerResultSP("Test", new SqlParameter("@id",id)).ToString();
        }

        // POST: api/test
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/test/5
        public void Delete(int id)
        {
        }
    }
}
