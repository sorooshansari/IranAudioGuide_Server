using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IAG_API_Server.Models;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IAG_API_Server.Controllers
{
    public class PlaceController : ApiController
    {
        dbManager dbManager = new dbManager();
        // GET: api/Place
        public IEnumerable<PlaceBasicInfo> Get()
        {
            var dt = dbManager.TableResultSP("AllPlaces");
            List<PlaceBasicInfo> res = new List<PlaceBasicInfo>();
            foreach (DataRow dr in dt.Rows)
                res.Add(new PlaceBasicInfo() { PlaceId = (Guid)dr["Id"], UpdateNum = (int)dr["UpdateNum"] });
            return res;
        }
        //POST: api/Place
        public IEnumerable<PlaceFullInfo> Post(params Guid[] IDs)
        {
            DataTable dt;
            if (IDs == null || IDs.Length == 0)
                dt = dbManager.TableResultSP("GetPlaceWithDetails");
            else
            {
                var table = new DataTable();
                table.Columns.Add("Id", typeof(Guid));
                foreach (var item in IDs)
                    table.Rows.Add(item);
                var pList = new SqlParameter("@list", SqlDbType.Structured);
                pList.TypeName = "dbo.IdList";
                pList.Value = table;
                dt = dbManager.TableResultSP("GetPlaceWithDetails", pList);
            }
            List<PlaceFullInfo> res = new List<PlaceFullInfo>();
            foreach (DataRow dr in dt.Rows)
                res.Add(new PlaceFullInfo()
                {
                    Id = (Guid)dr["Id"],
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString(),
                    ImgUrl = (dr["Url"] == DBNull.Value) ? string.Empty : dr["Url"].ToString(),
                    Desc = (dr["Descript"] == DBNull.Value) ? string.Empty : dr["Descript"].ToString(),
                    CX = (double)dr["X"],
                    CY = (double)dr["Y"],
                    Address = (dr["Adr"] == DBNull.Value) ? string.Empty : dr["Adr"].ToString(),
                    CityId = (int)dr["CityId"],
                    UpdateNumber = (int)dr["UN"]
                });
            return res;
        }

        // PUT: api/Place/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Place/5
        public void Delete(int id)
        {
        }
    }
}
