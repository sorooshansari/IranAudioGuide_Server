using Elmah;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Services;
using IranAudioGuide_MainServer.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using IranAudioGuide_MainServer.Models_v2;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Ajax.Utilities;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class UserApiController : ApiController
    {

        [HttpPost]
        public IHttpActionResult GetCurrentUserInfo()
        {
            try
            {

                string userName = User.Identity.Name;
                using (var db = new ApplicationDbContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.UserName == userName);
                    var Info = new UserInfo()
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        imgUrl = user.ImgUrl,
                        IsEmailConfirmed = user.EmailConfirmed,
                        IsSetuuid = (user.uuid == null) ? false : true,
                        IsAccessChangeUuid = false,
                        IsForeign = ExtensionMethods.IsForeign
                    };
                    //Info.IsAccessChangeUuid = false;
                    if (!Info.IsSetuuid)
                        return Ok(Info);
                    else if (user.TimeSetUuid == null && Info.IsSetuuid == true)
                    {
                        //the first time for change deactivate device
                        Info.IsAccessChangeUuid = true;
                        return Ok(Info);

                    }
                    else if (Info.IsSetuuid && user.TimeSetUuid != null)
                    {

                        var startDay = user.TimeSetUuid.Value;
                        var endDay = DateTime.Now;
                        var day = endDay.Day - startDay.Day;
                        var month = endDay.Month - startDay.Month;
                        var year = endDay.Year - startDay.Year;
                        var monthwating = ((year * 365) + (month * 31) + day) / 30;
                        if (monthwating >= 6)
                            Info.IsAccessChangeUuid = true;

                    }
                    return Ok(Info);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public bool IsTheFirstLogin()
        {
            var serviceIpAdress = new ServiceIpAdress();
            return serviceIpAdress.IsTheFirstLogin();
        }
        [HttpPost]
        public IHttpActionResult GetPackages()
        {
            var pro_list = GetPackagesProcurements();
            //  var lang = ServiceCulture.GeLangFromCookie();          
            var resualt = GetPackagesByLangId(1);
            //  var resualt2 = GetPackagesByLangId(2);
            //   resualt.AddRange(resualt2);

            foreach (var item in resualt)
            {
                item.isPackagesPurchased = pro_list.Any(pr => pr == item.PackageId);
            }


            return Ok(resualt);
        }

        private List<GetPackageVM> GetPackagesByLangId(int lang)
        {
            var d = new dbManagerV2();
            var parameter = new SqlParameter("@langId", lang);
            var listdata = d.MultiTableResultSP("GetPackages_website", parameter);
            var getModel = new List<PackageUserVM>();

            foreach (DataRow c in listdata[1].Rows)
            {
                getModel.Add(new PackageUserVM
                {
                    PackageId = c["PackageId"].ConvertToGuid(),
                    PackageName = c["PackageName"].ConvertToString(),
                    PackagePrice = c["PackagePrice"].ConvertToString(),
                    PackagePriceDollar = c["PackagePriceDollar"].ConvertToString(),
                    PackageOrder = c["PackageOrder"].ConvertToInt(),
                    CityId = c["CityId"].ConvertToInt(),
                    CityName = c["CityName"].ConvertToString(),
                    CityOrder = c["CityOrder"].ConvertToString(),
                    CityImageUrl = c["CityImgUrl"].ConvertToString(),
                    CityDescription = c["CityDescription"].ConvertToString(),

                });
            };
            var listPlaces = new List<PlaceUserVM>();
            foreach (DataRow c in listdata[0].Rows)
            {

                listPlaces.Add(new PlaceUserVM
                {
                    PlaceId = c["Pla_Id"].ConvertToGuid(),
                    PlaceName = c["Name"].ConvertToString(),
                    PlaceDesc = c["Discription"].ConvertToString(),
                    PlaceAddress = c["Address"].ConvertToString(),
                    ImgUrl = c["ImgUrl"].ConvertToString(),
                    TumbImgUrl = c["TumbImgUrl"].ConvertToString(),
                    AudiosCount = c["AudiosCount"].ConvertToInt(),
                    StoriesCount = c["StoriesCount"].ConvertToInt(),
                    Cit_Id = c["Cit_Id"].ConvertToInt(),
                    OrderItem = c["OrderItem"].ConvertToInt()
                });
            }
            return getModel.OrderBy(x => x.PackageOrder).Select(p => new GetPackageVM()
            {
                // convert toman to rial
                PackagePrice = p.PackagePrice + "0",
                PackagePriceDollar = p.PackagePriceDollar,
                PackageId = p.PackageId,
                PackageName = p.PackageName,
                //isPackagesPurchased = pro_list.Any(pr => pr == p.PackageId),
                PackageCities = getModel.Where(pc => pc.PackageId == p.PackageId).OrderBy(x => x.CityOrder).Select(c => new CityUserVM()
                {
                    CityName = c.CityName,
                    CityID = c.CityId,
                    CityImageUrl = c.CityImageUrl,
                    CityDesc = c.CityDescription,
                    IsloadImage = true,
                    TotalTrackCount = listPlaces.Where(lp => lp.Cit_Id == c.CityId).Sum(pl => pl.AudiosCount + pl.StoriesCount),
                    Places = listPlaces.Where(lp => lp.Cit_Id == c.CityId).OrderBy(x => x.OrderItem)
                                   .Select(pl => new PlaceUserVM()
                                   {
                                       PlaceName = pl.PlaceName,
                                       PlaceId = pl.PlaceId,
                                       ImgUrl = GlobalPath.FullPathImageTumbnail + pl.TumbImgUrl
                                   }).ToList()
                }).ToList()
            }).DistinctBy(x => x.PackageId).ToList();

        }

        [HttpPost]
        public List<Guid> GetPackagesProcurements()
        {
            var dt1 = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(GlobalPath.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("GetPackagesProcurements_website", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Username", User.Identity.Name));
                sqlConnection.Open();
                var reader = cmd.ExecuteReader();
                dt1.Load(reader);
            }
            var result = dt1.AsEnumerable().Select(x => x["Pac_Id"].ConvertToGuid()).ToList();
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "AppUser")]

        public IHttpActionResult GetPackages1()
        {

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    string userName = User.Identity.Name;
                    var list = db.Packages.Include("procurements.Pro_User").Include("Package.Pac_Cities")
                        .Select(p => new PackageVM()
                        {
                            PackagePrice = p.Pac_Price,
                            PackagePriceDollar = p.Pac_Price_Dollar,
                            PackageId = p.Pac_Id,
                            PackageName = p.Pac_Name,
                            isPackagesPurchased = p.procurements.Any(t => t.Pro_User.UserName == User.Identity.Name && t.Pro_PaymentFinished),

                            PackageCities = p.Pac_Cities.Select(c => new CityVM()
                            {
                                CityName = c.Cit_Name,
                                CityDesc = c.Cit_Description,
                                CityID = c.Cit_Id,
                                CityImageUrl = c.Cit_ImageUrl,
                                Places = db.Places
                                .Where(pl => pl.Pla_city.Cit_Id == c.Cit_Id && pl.Pla_isOnline == true)
                                .Select(pl => new PlaceVM()
                                {
                                    PlaceName = pl.Pla_Name,
                                    PlaceId = pl.Pla_Id,
                                    CityName = pl.Pla_Name,
                                    PlaceAddress = pl.Pla_Address,
                                    PlaceDesc = pl.Pla_Discription,
                                    ImgUrl = pl.Pla_ImgUrl,
                                    isOnline = pl.Pla_isOnline,
                                }).ToList()
                            }).ToList()
                        }).ToList();

                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
        }

        [HttpGet]
        [Authorize(Roles = "AppUser")]

        public IList<Guid> GetPackagesPurchased()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    //var s = db.Procurements.Include(x => x.Pro_Package).Include(x => x.Pro_Payment).Include(x => x.Pro_WMPayment).Include(x => x.Pro_User).ToList();
                    //return s;
                    string userName = User.Identity.Name;
                    var list = db.Procurements
                        .Include(x => x.Pro_User)
                        .Where(x => x.Pro_User.UserName == userName && x.Pro_PaymentFinished)
                        .Select(p => p.Pac_Id).ToList();
                    return list;
                }

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult SaveRequest(RequestForAppVM requestForApp)
        {
            using (var db = new ApplicationDbContext())
            {
                var d = new RequestForApp()
                {
                    NameDevice = requestForApp.NameDevice,
                    Email = requestForApp.Email
                };
                db.RequestForApps.Add(d);
                db.SaveChanges();
                return Ok();
            }
        }


        [HttpGet]
        [Authorize(Roles = "AppUser")]

        public IHttpActionResult DeactivateMobile()
        {
            using (var db = new ApplicationDbContext())
            {
                string userName = User.Identity.Name;
                var user = db.Users.FirstOrDefault(x => x.UserName == userName);
                //user.TimeSetUuid = new DateTime(2016, 9, 9);
                if (user.TimeSetUuid == null)
                {
                    user.TimeSetUuid = DateTime.Now;
                    user.uuid = null;
                }
                else
                {
                    var startDay = user.TimeSetUuid.Value;
                    var endDay = DateTime.Now;
                    var day = endDay.Day - startDay.Day;
                    var month = endDay.Month - startDay.Month;
                    var year = endDay.Year - startDay.Year;
                    var daywating = ((year * 365) + (month * 31) + day) / 30;
                    if (daywating > 6)
                    {
                        user.TimeSetUuid = DateTime.Now;
                        user.uuid = null;
                    }
                    else
                        return BadRequest(string.Format(Global.ServerDeactivateMobileInavlid, (30 * 6) + daywating));
                }
                db.SaveChanges();
                return Ok(Global.ServerDeactivateMobile);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult SaveOrderExtraImg(List<ImageVM> listImg)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    foreach (var img in listImg)
                    {
                        var getimg = db.Images.FirstOrDefault(x => x.Img_Id == img.ImageId);
                        if (getimg == null)
                            continue;
                        getimg.Tmg_Order = img.Index;
                        db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Img_Id == getimg.Img_Id));
                        db.UpdateLogs.Add(new UpdateLog() { Img_Id = getimg.Img_Id, isRemoved = false });
                    }
                    //var Imgs = db.Images.Where(x => listImg.Any(i => i.ImageId == x.Img_Id)).ToList();
                    db.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }
    }
}
