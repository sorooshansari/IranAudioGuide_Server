using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IranAudioGuide_MainServer.Models
{
    public class dbTools
    {
        dbManager dbManager = new dbManager();
        public GetAllVM GetUpdate(int LastUpdateNumber)
        {
            List<DataTable> dt;
            if (LastUpdateNumber == 0)
                dt = dbManager.MultiTableResultSP("GetAll");
            else
            {
                var SP = new SqlParameter("@UpdateNumber", LastUpdateNumber);
                dt = dbManager.MultiTableResultSP("GetUpdates", SP);
            }
            var res = new GetAllVM()
            {
                Places = FillPlaceVM(dt[0]),
                Audios = FillAudioVM(dt[1]),
                Cities = FillCityVM(dt[2]),
                Images = FillImageVM(dt[3]),
                UpdateNumber = GetNumFromdataTable(dt[4])
            };
            return res;
        }
        private int GetNumFromdataTable(DataTable dataTable)
        {
            int res = 0;
            if (dataTable.Rows.Count > 0)
                res = (dataTable.Rows[0]["LastUpdate"] == DBNull.Value) ? 0 : (int)dataTable.Rows[0]["LastUpdate"];
            return res;
        }

        private List<ImagesFullInfno> FillImageVM(DataTable dataTable)
        {
            List<ImagesFullInfno> res = new List<ImagesFullInfno>();
            foreach (DataRow dr in dataTable.Rows)
                res.Add(new ImagesFullInfno()
                {
                    PlaceId = (Guid)dr["PlaceId"],
                    ID = (Guid)dr["Id"],
                    Desc = (dr["Descript"] == DBNull.Value) ? string.Empty : dr["Descript"].ToString(),
                    Url = (dr["Url"] == DBNull.Value) ? string.Empty : dr["Url"].ToString()
                });
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
                    TNImgUrl = (dr["Url"] == DBNull.Value) ? string.Empty : dr["TNUrl"].ToString(),
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