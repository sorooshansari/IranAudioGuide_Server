﻿using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

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
                        //the first time for change dective device
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
                throw ex;
            }
        }
        [HttpPost]
        // GET: User
        [Authorize(Roles = "AppUser")]
        public IHttpActionResult GetPackagesPurchased()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    string userName = User.Identity.Name;
                    var list = db.Payments.Include("User").Include("Package").Include("Package.Pac_Cities")
                           .Where(x => x.User.UserName == userName)
                           .Where(x => x.PaymentFinished == true)
                           .Select(p => p.Package).Select(p => new PackageVM()
                           {
                               PackagePrice = p.Pac_Price,
                               PackagePriceDollar = p.Pac_Price_Dollar,
                               PackageId = p.Pac_Id,
                               PackageName = p.Pac_Name,
                               PackageCities = p.Pac_Cities.Select(c => new CityVM()
                               {
                                   CityName = c.Cit_Name,
                                   CityDesc = c.Cit_Description,
                                   CityID = c.Cit_Id,
                                   CityImageUrl = c.Cit_ImageUrl,
                                   Places = (from pl in db.Places
                                             where pl.Pla_city.Cit_Id == c.Cit_Id
                                             select new PlaceVM()
                                             {
                                                 PlaceName = pl.Pla_Name,
                                                 PlaceId = pl.Pla_Id,
                                                 CityName = pl.Pla_Name,
                                                 PlaceAddress = pl.Pla_Address,
                                                 PlaceDesc = pl.Pla_Discription,
                                                 ImgUrl = pl.Pla_ImgUrl,

                                             }).ToList()
                               }).ToList()
                           }).ToList();

                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        [Authorize(Roles = "AppUser")]
        public IHttpActionResult GetPackages()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    string userName = User.Identity.Name;
                    var list = db.Packages.Include("Package.Pac_Cities")
                        .Select(p => new PackageVM()
                        {
                            PackagePrice = p.Pac_Price,
                            PackagePriceDollar = p.Pac_Price_Dollar,
                            PackageId = p.Pac_Id,
                            PackageName = p.Pac_Name,
                            PackageCities = p.Pac_Cities.Select(c => new CityVM()
                            {
                                CityName = c.Cit_Name,
                                CityDesc = c.Cit_Description,
                                CityID = c.Cit_Id,
                                CityImageUrl = c.Cit_ImageUrl,
                                Places = (from pl in db.Places
                                          where (pl.Pla_city.Cit_Id == c.Cit_Id & !pl.Pla_Deactive)
                                          select new PlaceVM()
                                          {
                                              PlaceName = pl.Pla_Name,
                                              PlaceId = pl.Pla_Id,
                                              CityName = pl.Pla_Name,
                                              PlaceAddress = pl.Pla_Address,
                                              PlaceDesc = pl.Pla_Discription,
                                              ImgUrl = pl.Pla_ImgUrl,

                                          }).ToList()
                            }).ToList()
                        }).ToList();

                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                        return BadRequest("You should expect to wait up to " + (30 * 6) + daywating + " months");
                }
                db.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadFile()
        {

            //  System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;


            //HttpResponseMessage result = null;          
            //    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            //}

            try
            {
                //var ftpurl = @"ftp://iranaudioguide.net/";

                //var ftpusername = "test@iranaudioguide.net";
                //var ftppassword = "{S+Iao&)H&SH";

                //var ftpurl = "ftp://iranaudioguide.com/test.iranaudioguide.com/images/Files";

                //var ftpusername = "pourmand";
                //var ftppassword = "QQwwee11@@";

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count <= 0)
                    return BadRequest();// Request.CreateResponse(HttpStatusCode.BadRequest)


                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    //var filePath = HttpContext.Current.Server.MapPath(@"~/images/Files/" + postedFile.FileName);
                    //postedFile.SaveAs(filePath);
                    //var ftpClinet = new ftp(ftpurl, ftpusername, ftppassword);
                    var ftpClinet = new ServiceFtp();
                    //ftpClinet.Upload(postedFile);

                }//end ForEach
                return Ok();
            }
            catch (WebException ex)
            {
                String status = ((FtpWebResponse)ex.Response).StatusDescription;
                return BadRequest(ex.Message);
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
                        getimg.Order = img.Index;
                        db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Ima_Id == getimg.Img_Id));
                        db.UpdateLogs.Add(new UpdateLog() { Ima_Id = getimg.Img_Id, isRemoved = false });
                    }
                    //var Imgs = db.Images.Where(x => listImg.Any(i => i.ImageId == x.Img_Id)).ToList();
                    db.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }




    }
}
