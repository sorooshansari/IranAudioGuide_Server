using IAG_API_Server.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IAG_API_Server.Controllers
{
    public class AudioController : ApiController
    {
        dbManager dbManager = new dbManager();
        // GET: api/Audio
        public IEnumerable<AudioFullInfo> Get()
        {
            var dt = dbManager.TableResultSP("GetAllAudios");
            List<AudioFullInfo> res = new List<AudioFullInfo>();
            foreach (DataRow dr in dt.Rows)
                res.Add(new AudioFullInfo()
                {
                    PlaceId = (Guid)dr["PlaceId"],
                    ID = (Guid)dr["Id"],
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString(),
                    Desc = (dr["Descript"] == DBNull.Value) ? string.Empty : dr["Descript"].ToString(),
                    Url = (dr["Url"] == DBNull.Value) ? string.Empty : dr["Url"].ToString()
                });
            return res;
        }

        // GET: api/Audio/5
        public AudioFullInfo Get(int id)
        {
            var sp = new SqlParameter("@PlaceId", id);
            var dt = dbManager.TableResultSP("GetAllAudios", sp);
            AudioFullInfo res = new AudioFullInfo();
            if (dt.Rows.Count>0)
            {
                var dr = dt.Rows[0];
                res = new AudioFullInfo()
                {
                    PlaceId = (Guid)dr["PlaceId"],
                    ID = (Guid)dr["Id"],
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString(),
                    Desc = (dr["Descript"] == DBNull.Value) ? string.Empty : dr["Descript"].ToString(),
                    Url = (dr["Url"] == DBNull.Value) ? string.Empty : dr["Url"].ToString()
                };
            }
            return res;
        }
        // POST: api/Audio
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Audio/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Audio/5
        public void Delete(int id)
        {
        }
    }
}
