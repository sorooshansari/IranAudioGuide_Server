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
        // POST: api/Place
        public IEnumerable<PlaceFullInfo> Post(params Guid[] IDs)
        {
            if (IDs == null || IDs.Length > 0)
                return null;
            var table = new DataTable();
            table.Columns.Add("Id", typeof(Guid));
            foreach (var item in IDs)
                table.Rows.Add(item);
            var pList = new SqlParameter("@list", SqlDbType.Structured);
            pList.TypeName = "dbo.IdList";
            pList.Value = table;
            var dt = dbManager.TableResultSP("GetPlaceWithDetails", pList);
            List<PlaceFullInfo> res = new List<PlaceFullInfo>();
            foreach (DataRow dr in dt.Rows)
                res.Add(new PlaceFullInfo()
                {
                    Id = (Guid)dr["[OnP_Id]"],
                    Name = (string)dr["[OnP_Name]"],
                    imgUrl = (string)dr["[OnP_ImgUrl]"],
                    desc = (string)dr["[OnP_Discription]"],
                    c_x = (float)dr["[OnP_cordinate_X]"],
                    C_y = (float)dr["[OnP_cordinate_Y]"],
                    address = (string)dr["[OnP_Address]"],
                    CityName = (string)dr["[Cit_Name]"],
                    UpdateNumber = (int)dr["[OnP_UpdateNumber]"]
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
