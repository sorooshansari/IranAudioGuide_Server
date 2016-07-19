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
        dbManager dbManager = new dbManager();
        // GET: api/Update
        public GetAllVM Get()
        {
            var dt = dbManager.MultiTableResultSP("GetAll");
            var res = new GetAllVM()
            {
                Places = FillPlaceVM(dt[0]),
                Audios = FillAudioVM(dt[1]),
                Cities = FillCityVM(dt[2]),
                UpdateNumber = GetNumFromdataTable(dt[3])
            };
            return res;
        }
        // GET: api/Update/5
        public GetAllVM Get(int LastUpdateNumber)
        {
            var SP = new SqlParameter("@UpdateNumber", LastUpdateNumber);
            var dt = dbManager.MultiTableResultSP("GetUpdates", SP);
            var res = new GetAllVM()
            {
                Places = FillPlaceVM(dt[0]),
                Audios = FillAudioVM(dt[1]),
                Cities = FillCityVM(dt[2]),
                UpdateNumber = GetNumFromdataTable(dt[3])
            };
            return res;
        }

        // POST: api/Update
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Update/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Update/5
        public void Delete(int id)
        {
        }
        private int GetNumFromdataTable(DataTable dataTable)
        {
            int res = 0;
            if (dataTable.Rows.Count > 0)
                res = (dataTable.Rows[0]["LastUpdate"] == DBNull.Value) ? 0 : (int)dataTable.Rows[0]["LastUpdate"];
            return res;
        }

        private List<CitiesFullInfno> FillCityVM(DataTable dataTable)
        {
            List<CitiesFullInfno> res = new List<CitiesFullInfno>();
            foreach (DataRow dr in dataTable.Rows)
                res.Add(new CitiesFullInfno()
                {
                    Id = (int)dr["Id"],
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString()
                });
            return res;
        }

        private List<AudioFullInfo> FillAudioVM(DataTable dataTable)
        {
            List<AudioFullInfo> res = new List<AudioFullInfo>();
            foreach (DataRow dr in dataTable.Rows)
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

        private List<PlaceFullInfo> FillPlaceVM(DataTable dataTable)
        {
            List<PlaceFullInfo> res = new List<PlaceFullInfo>();
            foreach (DataRow dr in dataTable.Rows)
                res.Add(new PlaceFullInfo()
                {
                    Id = (Guid)dr["Id"],
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString(),
                    ImgUrl = (dr["Url"] == DBNull.Value) ? string.Empty : dr["Url"].ToString(),
                    Desc = (dr["Descript"] == DBNull.Value) ? string.Empty : dr["Descript"].ToString(),
                    CX = (double)dr["X"],
                    CY = (double)dr["Y"],
                    Address = (dr["Adr"] == DBNull.Value) ? string.Empty : dr["Adr"].ToString(),
                    CityId = (int)dr["CityId"]
                });
            return res;
        }

    }
}
