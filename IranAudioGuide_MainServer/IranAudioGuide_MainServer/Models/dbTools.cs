using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
        //public SkippedUserVM skipUser(string uuid)
        //{
        //    var res = new SkippedUserVM();
        //    try
        //    {
        //        using (var db = new ApplicationDbContext())
        //        {
        //            var existingTempUsers = db.TempUsers.Where(x => x.TeU_UUId == uuid).Count();
        //            if (existingTempUsers > 0)
        //                res.status = skippedUserStatus.uuidExist;
        //            else
        //            {
        //                var existingPrimaryUsers = db.Users.Where(x => x.uuid == uuid).Count();
        //                if (existingPrimaryUsers > 0)
        //                    res.status = skippedUserStatus.uuidExistInPraimaryUsers;
        //                else
        //                {
        //                    db.TempUsers.Add(new TempUsers() { TeU_UUId = uuid });
        //                    db.SaveChanges();
        //                    res.status = skippedUserStatus.uuidAdded;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.status = skippedUserStatus.unknownError;
        //        res.errorMessage = ex.Message;
        //    }
        //    return res;
        //}


        //public GetPackagesVM GetPackagesByCity(int cityId)
        //{
        //    //var SP = new SqlParameter("@cityId", cityId);
        //    //var dt = dbManager.MultiTableResultSP("GetPackagesByCity", SP);
        //    //throw new NotImplementedException();
        //    List<PackageVM> res = new List<PackageVM>();
        //    string error = "";
        //    try
        //    {
        //        using (var db = new ApplicationDbContext())
        //        {
        //            res = (from p in db.Packages
        //                   orderby p.Pac_Price ascending
        //                   where (from pc in p.Pac_Cities select pc.Cit_Id).Contains(cityId)
        //                   select new PackageVM()
        //                   {
        //                       PackageName = p.Pac_Name,
        //                       PackageId = p.Pac_Id,
        //                       PackagePrice = p.Pac_Price,
        //                       PackageCities =
        //                       (from c in db.Cities
        //                        where
        //                        (from pc in p.Pac_Cities select pc.Cit_Id).Contains(c.Cit_Id)
        //                        select new CityVM()
        //                        {
        //                            CityDesc = c.Cit_Description,
        //                            CityID = c.Cit_Id,
        //                            CityName = c.Cit_Name
        //                        }).ToList()
        //                   }).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;
        //    }
        //    return new GetPackagesVM { packages = res, errorMessage = error };
        //}

        public string GetAudioUrl(Guid trackId, bool isAudio)
        {
            using (var db = new ApplicationDbContext())
            {
                string url, trakName;
                if (isAudio)
                {
                    trakName = (from a in db.Audios
                                  where a.Aud_Id == trackId
                                  select a.Aud_Url).FirstOrDefault();
                    url = string.Format("{0}{1}", GlobalPath.FullPathAudios, trakName);

                }
                else
                {
                    trakName = (from s in db.Storys
                                where s.Sto_Id == trackId
                                select s.Sto_Url).FirstOrDefault();
                    url = string.Format("{0}{1}", GlobalPath.FullPathStory, trakName);
                }
                return url;
            }
        }
        public GetPackagesVM GetPackagesByCity(int cityId)
        {
            GetPackagesVM res;
            try
            {
                var SP = new SqlParameter("@CityId", cityId);
                SP.SqlDbType = SqlDbType.Int;
                var dt = dbManager.MultiTableResultSP("GetPackages", SP);
                res = new GetPackagesVM()
                {
                    packages = FillApiPackageVM(dt[0]),
                    cities = FillApiCitInfoVM(dt[1])
                };
            }
            catch (Exception ex)
            {
                res = new GetPackagesVM() { errorMessage = ex.Message };
            }
            return res;
        }

        public List<int> GetAutorizedCities(string userId)
        {
            var SP = new SqlParameter("@UserID", userId);
            var dt = dbManager.TableResultSP("GetAutorizedCities", SP);
            var res = new List<int>();
            foreach (DataRow dr in dt.Rows)
                res.Add((int)dr["cityID"]);
            return res;
        }

        public GetUpdateVM GetUpdate(int LastUpdateNumber, string uuid)
        {
            var SP1 = new SqlParameter("@UpdateNumber", LastUpdateNumber);
            SP1.SqlDbType = SqlDbType.Int;
            var SP2 = new SqlParameter("@uuid", uuid);
            SP1.SqlDbType = SqlDbType.NVarChar;
            var dt = dbManager.MultiTableResultSP("GetUpdates", SP1, SP2);
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


        public GetAllVM GetAllEntries(string uuid)
        {
            var SP = new SqlParameter("@uuid", uuid);
            SP.SqlDbType = SqlDbType.NVarChar;
            var dt = dbManager.MultiTableResultSP("GetAll", SP);
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
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString(),
                    Desc = (dr["Descript"] == DBNull.Value) ? string.Empty : dr["Descript"].ToString(),
                    ImageUrl = (dr["ImageUrl"] == DBNull.Value) ? string.Empty : dr["ImageUrl"].ToString()
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

        #region PackageInfoFiller

        private List<ApiPackageVM> FillApiPackageVM(DataTable dataTable)
        {
            List<ApiPackageVM> res = new List<ApiPackageVM>();
            foreach (DataRow dr in dataTable.Rows)
                res.Add(new ApiPackageVM()
                {
                    Id = (Guid)dr["Id"],
                    Name = (dr["Name"] == DBNull.Value) ? string.Empty : dr["Name"].ToString(),
                    Price = (long)dr["Price"],
                    PriceD = (float)dr["PriceD"]
                });
            return res;
        }
        private List<ApiCitInfoVM> FillApiCitInfoVM(DataTable dataTable)
        {
            List<ApiCitInfoVM> res = new List<ApiCitInfoVM>();
            foreach (DataRow dr in dataTable.Rows)
                res.Add(new ApiCitInfoVM()
                {
                    CityId = (int)dr["CityId"],
                    PackageId = (Guid)dr["PackageId"],
                    CityName = (dr["CityName"] == DBNull.Value) ? string.Empty : dr["CityName"].ToString(),
                    PlacesCount = (int)dr["PlacesCount"],
                    AudiosCount = (int)dr["AudiosCount"],
                    StoriesCount = (int)dr["StoriesCount"]
                });
            return res;
        }
        #endregion


        #region comment

        public void CreateComment(Comment newComment)
        {

            var Message = new SqlParameter("@Message", newComment.Message);
            Message.SqlDbType = SqlDbType.NVarChar;

            var uuid = new SqlParameter("@uuid", newComment.uuid);
            uuid.SqlDbType = SqlDbType.NVarChar;
            
            var Subject = new SqlParameter("@Subject", newComment.Subject);
            Subject.SqlDbType = SqlDbType.NVarChar;

            var email = new SqlParameter("@email", newComment.Email);
            uuid.SqlDbType = SqlDbType.NVarChar;

            var dt = dbManager.MultiTableResultSP("CreateComment", Message, uuid, Subject, email);
        }
        #endregion
    }
}