using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IranAudioGuide_MainServer.Models
{
    public class dbTools
    {
        private dbManager _dbManager;
        public dbManager dbManager
        {
            get
            {
                return _dbManager ?? new dbManager();
            }
            private set
            {
                _dbManager = value;
            }
        }
        public GetUpdateVM GetUpdate(int LastUpdateNumber)
        {
            var SP = new SqlParameter("@UpdateNumber", LastUpdateNumber);
            var dt = dbManager.MultiTableResultSP("GetUpdates", SP);
            var res = new GetUpdateVM()
            {
                UpdateNumber = GetNumFromdataTable(dt[0], "LastUpdate"),
                Places = FillPlaceVM(dt[1]),
                Audios = FillAudioVM(dt[2]),
                Stories = FillAudioVM(dt[3]),
                Images = FillImageVM(dt[4]),
                Tips = FillTipVM(dt[5]),
                Cities = FillCityVM(dt[6]),
                RemovedEntries = new RemovedEntries()
                {
                    Places = GetTableIds(dt[7]),
                    Audios = GetTableIds(dt[8]),
                    Stories = GetTableIds(dt[9]),
                    Images = GetTableIds(dt[10]),
                    Tips = GetTableIds(dt[11]),
                    Cities = GetIntTableIds(dt[12])
                }
            };
            return res;
        }
        public GetAllVM GetAllEntries()
        {
            var dt = dbManager.MultiTableResultSP("GetAll");
            var res = new GetAllVM()
            {
                UpdateNumber = GetNumFromdataTable(dt[0], "LastUpdate"),
                Places = FillPlaceVM(dt[1]),
                Audios = FillAudioVM(dt[2]),
                Stories = FillAudioVM(dt[3]),
                Images = FillImageVM(dt[4]),
                Tips = FillTipVM(dt[5]),
                Cities = FillCityVM(dt[6]),
                TipCategories = FillTipCategoriesVM(dt[7])
            };
            return res;
        }
        private int GetNumFromdataTable(DataTable dataTable, string columnName)
        {
            int res = 0;
            if (dataTable.Rows.Count > 0)
                res = (dataTable.Rows[0][columnName] == DBNull.Value) ? 0 : (int)dataTable.Rows[0][columnName];
            return res;
        }
        private List<Guid> GetTableIds(DataTable dt)
        {
            List<Guid> res = new List<Guid>();
            foreach (DataRow dr in dt.Rows)
                res.Add((Guid)dr["Id"]);
            return res;
        }
        private List<int> GetIntTableIds(DataTable dt)
        {
            List<int> res = new List<int>();
            foreach (DataRow dr in dt.Rows)
                res.Add((int)dr["Id"]);
            return res;
        }
        private List<TipsFullInfo> FillTipVM(DataTable dataTable)
        {
            List<TipsFullInfo> res = new List<TipsFullInfo>();
            foreach (DataRow dr in dataTable.Rows)
                res.Add(new TipsFullInfo()
                {
                    PlaceId = (Guid)dr["PlaceId"],
                    ID = (Guid)dr["Id"],
                    CategoryId = (Guid)dr["CategoryId"],
                    Content = (dr["Content"] == DBNull.Value) ? string.Empty : dr["Content"].ToString()
                });
            return res;
        }
        private List<TipCategoriesFullInfo> FillTipCategoriesVM(DataTable dataTable)
        {
            List<TipCategoriesFullInfo> res = new List<TipCategoriesFullInfo>();
            foreach (DataRow dr in dataTable.Rows)
                res.Add(new TipCategoriesFullInfo()
                {
                    ID = (Guid)dr["Id"],
                    Class = (dr["Class"] == DBNull.Value) ? string.Empty : dr["Class"].ToString(),
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString(),
                    Unicode = (dr["TipUnicode"] == DBNull.Value) ? string.Empty : dr["TipUnicode"].ToString(),
                    Priority = (dr["TipPriority"] == DBNull.Value) ? 10 : (int)dr["TipPriority"]
                });
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
                    CityId = (int)dr["CityId"],
                    isPrimary = (bool)dr["isPrimary"]
                });
            return res;
        }
    }
}