using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IranAudioGuide_MainServer.Models_v2
{
    public class dbToolsV2
    {
        private dbManagerV2 _dbManager;


        public dbManagerV2 dbManager
        {
            get
            {
                return _dbManager ?? new dbManagerV2();
            }
            private set
            {
                _dbManager = value;
            }
        }

        public int CnvertToInt(string item)
        {
            int number;
            bool success = Int32.TryParse(item, out number);
            if (success)
            {
                return number;
            }
            else
            {
                return 0;
            }
        }
        // change package storeProced
        public GetPackagesVM GetPackagesByCity(int cityId, int langId)
        {
            GetPackagesVM res;
            try
            {

                var SP = new SqlParameter("@CityId", cityId);
                var SP2 = new SqlParameter("@langId", langId);

                SP.SqlDbType = SqlDbType.Int;
                SP2.SqlDbType = SqlDbType.Int;

                var dt = dbManager.MultiTableResultSP("GetPackages_v2", SP, SP2);
                var IsForeign = ExtensionMethods.IsForeign;
                res = new GetPackagesVM()
                {
                    packages = FillApiPackageVM(dt[0]),
                    cities = FillApiCitInfoVM(dt[1]),
                    IsForeign = IsForeign
                };
            }
            catch (Exception ex)
            {
                res = new GetPackagesVM() { errorMessage = ex.Message };
            }
            return res;
        }

        public List<citiesLng> GetAutorizedCities(string userId)
        {
            var SP = new SqlParameter("@UserID", userId);
            var dt1 = dbManager.TableResultSP("GetAutorizedCities_v2", SP);
            var list = dt1.AsEnumerable().Select(x => new citiesLng
            {
                cityID = CnvertToInt(x["cityID"].ToString()),
                LangId = CnvertToInt(x["langId"].ToString()),

            }).ToList();

            //foreach (var item in list)
            //{
            //    item.LangTitle = Enum.GetName(typeof(EnumLang), item.LangId).ToString();

            //}
            return list;

        }

        public GetUpdateVM GetUpdate(int LastUpdateNumber, string uuid)
        {
            var SP1 = new SqlParameter("@UpdateNumber", LastUpdateNumber);
            var SP2 = new SqlParameter("@uuid", uuid);
            SP1.SqlDbType = SqlDbType.Int;
            SP1.SqlDbType = SqlDbType.NVarChar;
            var dt = dbManager.MultiTableResultSP("GetUpdates_v2", SP1, SP2);
            var res = new GetUpdateVM()
            {
                UpdateNumber = GetNumFromdataTable(dt[0], "LastUpdate"),
                Places = FillPlaceVM(dt[1]),
                Audios = FillAudio(dt[2]),
                Stories = FillAudio(dt[3]),
                Images = FillImageVM(dt[4]),
                Tips = FillTipVM(dt[5]),
                Cities = FillCityVM(dt[6]),
                TranslateCities = FillTranslateCities(dt[7]),
                TranslateImages = FillTranslateImage(dt[8]),
                TranslatePlaces = FillTranslatePlaces(dt[9]),
                RemovedEntries = new RemovedEntries()
                {
                    Places = GetTableIds(dt[10]),
                    Audios = GetTableIds(dt[11]),
                    Stories = GetTableIds(dt[12]),
                    Images = GetTableIds(dt[13]),
                    Tips = GetTableIds(dt[14]),
                    Cities = GetIntTableIds(dt[15]),
                    TPlace = GetTableIds(dt[16]),
                    TCity = GetTableIds(dt[17]),
                    TImage = GetTableIds(dt[18]),
                }
            };
            return res;
        }

        public RemovedEntries GetAllEntitiesRemoved()
        {


            var dt = dbManager.MultiTableResultSP("GetAllEntitiesRemoved");
            var RemovedEntries = new RemovedEntries()
            {
                Places = GetTableIds(dt[0]),
                Audios = GetTableIds(dt[1]),
                Stories = GetTableIds(dt[2]),
                Images = GetTableIds(dt[3]),
                Tips = GetTableIds(dt[4]),
                Cities = GetIntTableIds(dt[5])
            };

            return RemovedEntries;
        }

        //public GetUpdateVM_v2 GetUpdateV2(int LastUpdateNumber, string uuid)
        //{
        //    var SP1 = new SqlParameter("@UpdateNumber", LastUpdateNumber);
        //    SP1.SqlDbType = SqlDbType.Int;
        //    var SP2 = new SqlParameter("@uuid", uuid);
        //    SP1.SqlDbType = SqlDbType.NVarChar;
        //    var dt = dbManager.MultiTableResultSP("GetUpdatesV2", SP1, SP2);
        //    var res = new GetUpdateVM_v2()
        //    {
        //        UpdateNumber = GetNumFromdataTable(dt[0], "LastUpdate"),
        //        Places = FillPlaceVM(dt[1]),
        //        Audios = FillAudio(dt[2]),
        //        Stories = FillAudio(dt[3]),
        //        Images = FillImageVM(dt[4]),
        //        Tips = FillTipVM(dt[5]),
        //        Cities = FillCityVM(dt[6]),
        //        RemovedEntries = new RemovedEntries()
        //        {
        //            Places = GetTableIds(dt[7]),
        //            Audios = GetTableIds(dt[8]),
        //            Stories = GetTableIds(dt[9]),
        //            Images = GetTableIds(dt[10]),
        //            Tips = GetTableIds(dt[11]),
        //            Cities = GetIntTableIds(dt[12])
        //        }
        //    };
        //    return res;
        //}

        public GetAllVm GetAllEntries(string uuid)
        {
            var SP = new SqlParameter("@uuid", uuid);
            SP.SqlDbType = SqlDbType.NVarChar;
            var dt = dbManager.MultiTableResultSP("GetAll_v2", SP);

            var res = new GetAllVm()
            {
                UpdateNumber = GetNumFromdataTable(dt[0], "LastUpdate"),
                Places = FillPlaceVM(dt[1]),
                Audios = FillAudio(dt[2]),
                Stories = FillAudio(dt[3]),
                Images = FillImageVM(dt[4]),
                Tips = FillTipVM(dt[5]),
                Cities = FillCityVM(dt[6]),
                TipCategories = FillTipCategoriesVM(dt[7]),
                TranslateCities = FillTranslateCities(dt[8]),
                TranslateImages = FillTranslateImage(dt[9]),
                TranslatePlaces = FillTranslatePlaces(dt[10]),

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
        private List<TipsVm> FillTipVM(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(dr => new TipsVm()
                {
                    PlaceId = dr["PlaceId"].convertToGuid(),
                    ID = dr["Id"].convertToGuid(),
                    CategoryId = dr["CategoryId"].convertToGuid(),
                    Content = dr["Content"].convertToString(),
                    OrderItem = dr["OrderItem"].convertToInt(),
                    LangId = dr["LangId"].convertToInt(),
                }).ToList();
            }
            catch
            {
                return null;
            }
        }
        private List<TipCategoriesVm> FillTipCategoriesVM(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(dr => new TipCategoriesVm()
                {
                    ID = dr["Id"].convertToGuid(),
                    Class = dr["Class"].convertToString(),
                    Name = dr["Name"].convertToString(),
                    Unicode = dr["TipUnicode"].convertToString(),
                    Priority = dr["TipPriority"].convertToInt(10)
                }).ToList();
            }
            catch
            {
                return null;
            }
        }
        private List<ImagesVm> FillImageVM(DataTable dataTable)
        {

            try
            {
                return dataTable.AsEnumerable().Select(dr => new ImagesVm()
                {
                    ID = dr["Id"].convertToGuid(),
                    Name = dr["Name"].convertToString(),
                    //Desc = dr["Descript"].convertToString(),
                    PlaceId = dr["PlaceId"].convertToGuid(),
                    OrderItem = dr["OrderItem"].convertToInt(),
                }).ToList();
            }
            catch
            {
                return null;
            }
        }

        private List<CitiesVm> FillCityVM(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(dr => new CitiesVm()
                {
                    Id = dr["Id"].convertToInt(),
                    //Name =dr["Name"].convertToString(),
                    // Desc = dr["Descript"].convertToString(),
                    ImageUrl = dr["ImageUrl"].convertToString(),
                    OrderItem = dr["OrderItem"].convertToInt(),
                    //   LangId = dr["LangId"].convertToInt(),
                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private List<AudioVm> FillAudio(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(dr => new AudioVm
                {
                    PlaceId = dr["PlaceId"].convertToGuid(),
                    ID = dr["Id"].convertToGuid(),
                    Name = dr["Name"].convertToString(),
                    Desc = dr["Descript"].convertToString(),
                    //Url = dr["Url"].convertToString(),
                    OrderItem = dr["OrderItem"].convertToInt(),
                    LangId = dr["LangId"].convertToInt(),
                }).ToList();
            }
            catch
            {
                return null;
            }
        }

        private List<PlaceVm> FillPlaceVM(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(dr => new PlaceVm
                {
                    Id = dr["Id"].convertToGuid(),
                    Name = dr["Name"].convertToString(),
                    ImgUrl = dr["ImgUrl"].convertToString(),
                    TNImgUrl = dr["TNImgUrl"].convertToString(),
                    Desc = dr["Descript"].convertToString(),
                    CX = (double)dr["CX"],
                    CY = (double)dr["CY"],
                    Address = dr["Adr"].convertToString(),
                    CityId = dr["CityId"].convertToInt(),
                    isPrimary = (bool)dr["isPrimary"],
                    OrderItem = dr["OrderItem"].convertToInt()
                }).ToList();
            }
            catch
            {
                return null;
            }
        }

        private List<TranslateCityVm> FillTranslateCities(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(x => new TranslateCityVm
                {
                    Description = x["Description"].convertToString(),
                    LangId = x["langId"].convertToInt(),
                    Id = x["Id"].convertToGuid(),
                    CityId = x["CityId"].convertToInt(),
                    Name = x["Name"].convertToString()
                }).ToList();
            }
            catch
            {
                return null;
            }
        }
        private List<TranslateImageVm> FillTranslateImage(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(x => new TranslateImageVm
                {
                    Description = x["Description"].convertToString(),
                    LangId = x["langId"].convertToInt(),
                    Id = x["Id"].convertToGuid(),
                    ImageId = x["ImageId"].convertToInt(),
                    //  Name = x["Name"].convertToString(),
                }).ToList();
            }
            catch
            {
                return null;
            }
        }
        private List<TranslatePlacesVm> FillTranslatePlaces(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(x => new TranslatePlacesVm
                {
                    Description = x["Description"].convertToString(),
                    LangId = x["langId"].convertToInt(),
                    Id = x["Id"].convertToGuid(),
                    PlaceId = x["PlaceId"].convertToInt(),
                    Name = x["Name"].convertToString(),
                    Adr = x["Adr"].convertToString(),
                }).ToList();
            }
            catch
            {
                return null;
            }
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
                    PriceD = float.Parse(dr["PriceD"].ToString())
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