using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_Server.Models;
using System.IO;
using System.Data.SqlClient;

namespace IranAudioGuide_Server.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const int pagingLen = 5;
        private Object ChangeImgLock = new Object();
        private Object DelExtraImg = new Object();
        private Object DelAdo = new Object();
        private Object DelPlc = new Object();
        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.View = Views.AdminIndex;

            return View(GetCurrentUserInfo());
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult Audios(string PlaceId)
        {
            if (PlaceId.Length > 0)
            {
                string imgUrl = PlaceImg(PlaceId);
                if (imgUrl != null)
                {
                    return Json(new AudioViewVM()
                    {
                        audios = GetAudios(PlaceId),
                        PlaceImage = imgUrl
                    });
                }
                return Json(new AudioViewVM()
                {
                    audios = GetAudios(PlaceId),
                    respond = new Respond(content: "Error. Couldn't find any image.", status: 1)
                });
            }
            return Json(new AudioViewVM()
            {
                respond = new Respond(content: "Fatal error. Invalid Place Id.", status: 2)
            });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddAudio(NewAudioVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", 1));
            }
            if (model.AudioFile.ContentLength > 0 && IsAudioFile(model.AudioFile.FileName))
            {
                try
                {
                    using (var dbTran = db.Database.BeginTransaction())
                    {
                        var audio = new Audio()
                        {
                            Aud_Name = model.AudioName,
                            Pla_Id = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault()
                        };
                        db.Audios.Add(audio);
                        db.SaveChanges(); //Save audio and generate Aud_Id
                        string id = Convert.ToString(audio.Aud_Id);
                        string extention = Path.GetExtension(model.AudioFile.FileName);
                        string path = string.Format("~/Audios/{0}{1}", id, extention);
                        model.AudioFile.SaveAs(Server.MapPath(path));
                        audio.Aud_Url = string.Format("{0}{1}", id, extention);
                        db.SaveChanges();
                        dbTran.Commit();
                    }
                    return Json(new Respond(""));
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return Json(new Respond("Only WAV, MID, MIDI, WMA, MP3, OGG, and RMA are allowed.", 3));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelAudio(Guid Id)
        {
            var audio = db.Audios.Where(x => x.Aud_Id == Id).FirstOrDefault();
            if (audio != default(Audio))
            {
                try
                {
                    string path = Server.MapPath(string.Format("~/Audios/{0}", audio.Aud_Url));
                    lock (DelAdo)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    using (var dbTran = db.Database.BeginTransaction())
                    {
                        db.Audios.Remove(audio);
                        db.SaveChanges();
                        dbTran.Commit();
                    }
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    return Json(new Respond(ex.Message, 1));
                }
            }
            return Json(new Respond("Invalid Audio Id", 1));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddPlace(NewPlace model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", 1));
            }
            if (model.Image.ContentLength > 0 && IsImage(model.Image))
            {
                var place = new Place()
                {
                    Pla_Name = model.PlaceName,
                    Pla_Discription = model.PlaceDesc,
                    Pla_Address = model.PlaceAddress
                };
                if (model.PlaceCordinates != null)
                {
                    if (!model.PlaceCordinates.Contains(','))
                        return Json(new Respond("Enter X and Y cordinates and seprate them with \",\".", 2));
                    try
                    {
                        List<double> cordinates = (from c in model.PlaceCordinates.Split(',')
                                                   select Convert.ToDouble(c)).ToList();
                        place.Pla_cordinate_X = cordinates[0];
                        place.Pla_cordinate_Y = cordinates[1];
                    }
                    catch (Exception)
                    {
                        return Json(new Respond("Enter percise cordinates.", 2));
                    }
                }

                try
                {
                    using (var dbTran = db.Database.BeginTransaction())
                    {
                        place.Pla_city = db.Cities.Where(c => c.Cit_Id == model.PlaceCityId).FirstOrDefault();
                        db.Places.Add(place);
                        db.SaveChanges(); //Save place and generate Pla_Id
                        string id = Convert.ToString(place.Pla_Id);
                        string extention = Path.GetExtension(model.Image.FileName);
                        string path = string.Format("~/images/Places/{0}{1}", id, extention);
                        model.Image.SaveAs(Server.MapPath(path));
                        place.Pla_ImgUrl = string.Format("{0}{1}", id, extention);
                        db.SaveChanges();
                        dbTran.Commit();
                    }
                    return Json(new Respond(""));
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return Json(new Respond("Only jpg, png, gif, and jpeg are allowed.", 3));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelPlace(Guid Id)
        {
            var place = db.Places.Where(x => x.Pla_Id == Id).FirstOrDefault();
            if (place != default(Place))
            {
                try
                {
                    string path = Server.MapPath(string.Format("~/images/Places/{0}", place.Pla_ImgUrl));
                    lock (DelPlc)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    int result = db.Database.SqlQuery<int>("DeletePlace @Id", new SqlParameter("@Id", Id)).Single();
                    return Json(new Respond("", result));
                }
                catch (Exception ex)
                {
                    return Json(new Respond(ex.Message, 3));
                }
            }
            return Json(new Respond("Invalid Place Id.", 3));
        }
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public JsonResult DeactiveOnlinePlace(Guid Id)
        //{
        //    try
        //    {
        //        var place = db.OnlinePlaces.Where(x => x.OnP_Id == Id).FirstOrDefault();
        //        if (place == default(OnlinePlace))
        //        {
        //            return Json(new Respond("Invalid Place Id.", 2));
        //        }
        //        place.OnP_Deactive = true;
        //        db.SaveChanges();
        //        return Json(new Respond());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new Respond(ex.Message, 3));
        //    }

        //}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult EditPlace(EditPlaceVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", 1));
            }
            var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
            if (place == default(Place))
            {
                return Json(new Respond("Invalid Place Id", 2));
            }
            place.Pla_Name = model.PlaceName;
            place.Pla_Discription = model.PlaceDesc;
            place.Pla_city = db.Cities.Where(x => x.Cit_Id == model.PlaceCityId).FirstOrDefault();
            place.Pla_Address = model.PlaceAddress;
            if (model.PlaceCordinates != null)
            {
                if (!model.PlaceCordinates.Contains(','))
                    return Json(new Respond("Enter X and Y cordinates and seprate them with \",\".", 1));
                try
                {
                    List<double> cordinates = (from c in model.PlaceCordinates.Split(',')
                                               select Convert.ToDouble(c)).ToList();
                    place.Pla_cordinate_X = cordinates[0];
                    place.Pla_cordinate_Y = cordinates[1];
                }
                catch (Exception)
                {
                    return Json(new Respond("Enter percise cordinates.", 1));
                }
            }
            try
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    db.SaveChanges();
                    dbTran.Commit();
                }
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, 3));
            }
            return Json(new Respond());
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetPlaces(int PageNum, bool isOnline = false)
        {
            var places = GetPlaces(isOnline);
            int pagesLen = (places.Count() % pagingLen == 0) ? places.Count() / pagingLen : (places.Count() / pagingLen) + 1;
            int remain = places.Count - (PageNum * pagingLen);
            //Gernerate Indexes
            int counter = 0;
            var Places = new GetPlacesVM(
                (remain > pagingLen)
                ?
                    places.GetRange(PageNum * pagingLen, pagingLen)
                :
                    places.GetRange(PageNum * pagingLen, remain)
                , pagesLen);
            foreach (var place in Places.Places)
            {
                place.Index = ++counter;
            }
            return Json(Places);
        }
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public JsonResult GoOnline(Guid PlaceId)
        //{
        //    try
        //    {
        //        var place = db.Places.Where(x => x.Pla_Id == PlaceId).FirstOrDefault();
        //        if (place == default(Place))
        //        {
        //            return Json(new Respond("Invalid Place Id.", 2));
        //        }
        //        var onlinePlace = new OnlinePlace()
        //        {
        //            OnP_Address = place.Pla_Address,
        //            OnP_Audios = place.Pla_Audios,
        //            OnP_city = place.Pla_city,
        //            OnP_cordinate_X = place.Pla_cordinate_X,
        //            OnP_cordinate_Y = place.Pla_cordinate_Y,
        //            OnP_Discription = place.Pla_Discription,
        //            OnP_ExtraImages = place.Pla_ExtraImages,
        //            OnP_ImgUrl = place.Pla_ImgUrl,
        //            OnP_Name = place.Pla_Name
        //        };
        //        using (var dbTrans = db.Database.BeginTransaction())
        //        {
        //            place.Pla_Deactive = true;
        //            db.OnlinePlaces.Add(onlinePlace);
        //            db.SaveChanges();
        //            dbTrans.Commit();
        //        }
        //        return Json(new Respond());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new Respond(ex.Message, 3));
        //    }
        //}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult ChangePlaceImage(ChangeImageVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", 1));
            }
            try
            {
                string path = Server.MapPath(string.Format("~/images/Places/{0}", model.ImageName));
                lock (ChangeImgLock)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    model.NewImage.SaveAs(path);
                }
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, 3));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddPlaceExtraImage(NewImageVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", 1));
            }
            if (!(model.NewImage != null && model.NewImage.ContentLength > 0 && IsImage(model.NewImage)))
            {
                return Json(new Respond("Only jpg, png, gif, and jpeg are allowed.", 3));
            };
            try
            {
                var img = new Image() { Pla_Id = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault() };
                using (var dbTran = db.Database.BeginTransaction())
                {
                    db.Images.Add(img);
                    db.SaveChanges();
                    string id = Convert.ToString(img.Img_Id);
                    string extention = Path.GetExtension(model.NewImage.FileName);
                    string path = string.Format("~/images/Places/Extras/{0}{1}", id, extention);
                    model.NewImage.SaveAs(Server.MapPath(path));
                    img.Img_Name = string.Format("{0}{1}", id, extention);
                    db.SaveChanges();
                    dbTran.Commit();

                }
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, 3));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelPlaceExtraImage(Guid imgId)
        {
            try
            {
                lock (DelExtraImg)
                {
                    var img = db.Images.Where(x => x.Img_Id == imgId).FirstOrDefault();
                    if (img == default(Image))
                    {
                        return Json(new Respond("Invalid Image Id", 2));
                    }
                    string path = Server.MapPath(string.Format("~/images/Places/Extras/{0}", img.Img_Name));
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    db.Images.Remove(img);
                    db.SaveChanges();
                }
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, 3));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult EditPlaceExtraImageDesc(EditEIDescVM model)
        {
            try
            {
                var img = db.Images.Where(x => x.Img_Id == model.ImageId).FirstOrDefault();
                if (img == default(Image))
                {
                    return Json(new Respond("Invalid Image Id", 2));
                }
                img.Img_Description = model.ImageDesc;
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, 3));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetExtraImages(Guid placeId)
        {
            return Json(GetImages(placeId));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddCity(NewCity model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new OK(false));
            }
            var city = new city() { Cit_Name = model.CityName, Cit_Description = model.CityDesc };
            try
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    db.Cities.Add(city);
                    db.SaveChanges();
                    dbTran.Commit();
                }
                return Json(new OK());
            }
            catch (Exception)
            {
                return Json(new OK(false));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelCity(int Id)
        {
            int result = db.Database.SqlQuery<int>("DeleteCity @Id", new SqlParameter("@Id", Id)).Single();
            return Json(new Respond("", result));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetCities(int PageNum)
        {
            if (PageNum == -1)
                return Json(GetAllCitiesName());
            var Cities = GetCities();
            int pagesLen = (Cities.Count() % pagingLen == 0) ? Cities.Count() / pagingLen : (Cities.Count() / pagingLen) + 1;
            int remain = Cities.Count - (PageNum * pagingLen);
            return Json(new GetCitiesVM(
                (remain > pagingLen)
                ?
                    Cities.GetRange(PageNum * pagingLen, pagingLen)
                :
                    Cities.GetRange(PageNum * pagingLen, remain)
                , pagesLen));
        }
        private List<PlaceVM> GetPlaces(bool isOnline)
        {
            List<PlaceVM> Places;
            try
            {
                if (isOnline)
                {
                    Places =
                    (from place in db.Places
                     where place.Pla_isOnline && !place.Pla_Deactive
                     orderby place.Pla_Id descending
                     select new PlaceVM()
                     {
                         PlaceId = place.Pla_Id,
                         ImgUrl = place.Pla_ImgUrl,
                         PlaceDesc = place.Pla_Discription,
                         PlaceName = place.Pla_Name,
                         CityName = place.Pla_city.Cit_Name,
                         PlaceAddress = place.Pla_Address,
                         PlaceCordinates = place.Pla_cordinate_X.ToString() + "," + place.Pla_cordinate_Y.ToString(),
                         PlaceCityId = place.Pla_city.Cit_Id
                     }).ToList();
                }
                else
                {

                Places =
                    (from place in db.Places
                     where !place.Pla_Deactive
                     orderby place.Pla_Id descending
                     select new PlaceVM()
                     {
                         PlaceId = place.Pla_Id,
                         ImgUrl = place.Pla_ImgUrl,
                         PlaceDesc = place.Pla_Discription,
                         PlaceName = place.Pla_Name,
                         CityName = place.Pla_city.Cit_Name,
                         PlaceAddress = place.Pla_Address,
                         PlaceCordinates = place.Pla_cordinate_X.ToString() + "," + place.Pla_cordinate_Y.ToString(),
                         PlaceCityId = place.Pla_city.Cit_Id
                     }).ToList();
                }
                return Places;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private string PlaceImg(string placeId)
        {
            try
            {
                Place place = db.Places.Where(x => x.Pla_Id.ToString() == placeId).FirstOrDefault();
                if (place != default(Place))
                {
                    return place.Pla_ImgUrl;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private UserInfo GetCurrentUserInfo()
        {
            try
            {
                string userName = User.Identity.Name;
                UserInfo Info = (from user in db.Users
                                 where user.UserName == userName
                                 select new UserInfo()
                                 {
                                     Email = user.Email,
                                     FullName = user.FullName,
                                     imgUrl = user.ImgUrl
                                 }).FirstOrDefault();
                return Info;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private List<AudioVM> GetAudios(string PlaceId)
        {
            List<AudioVM> audios = (from a in db.Audios
                                    where a.Pla_Id == db.Places.Where(x => x.Pla_Id.ToString() == PlaceId).FirstOrDefault()
                                    select new AudioVM()
                                    {
                                        Aud_Discription = a.Aud_Discription,
                                        Aud_Name = a.Aud_Name,
                                        Aud_Url = a.Aud_Url,
                                        Aud_Id = a.Aud_Id
                                    }).ToList();
            int counter = 0;
            foreach (var item in audios)
            {
                item.Index = ++counter;
            }
            return audios;
        }
        private List<JNOCitiesVM> GetAllCitiesName()
        {
            List<JNOCitiesVM> JNOCities = (from c in db.Cities
                                           orderby c.Cit_Id descending
                                           select new JNOCitiesVM()
                                           {
                                               CityID = c.Cit_Id,
                                               CityName = c.Cit_Name
                                           }).ToList();
            return JNOCities;
        }
        private List<CityVM> GetCities()
        {
            List<CityVM> cities = (from c in db.Cities
                                   orderby c.Cit_Id descending
                                   select new CityVM()
                                   {
                                       CityDesc = c.Cit_Description,
                                       CityID = c.Cit_Id,
                                       CityName = c.Cit_Name
                                   }).ToList();
            int counter = 0;
            foreach (var item in cities)
            {
                item.Index = ++counter;
            }
            return cities;
        }
        private List<ImageVM> GetImages(Guid PlaceId)
        {
            var img = (from i in db.Images
                       where i.Pla_Id == db.Places.Where(x => x.Pla_Id == PlaceId).FirstOrDefault()
                       select new ImageVM()
                       {
                           ImageId = i.Img_Id,
                           ImageName = i.Img_Name,
                           ImageDesc = i.Img_Description
                       }).ToList();
            int counter = 0;
            foreach (var i in img)
                i.Index = counter++;
            return img;
        }
        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private bool IsAudioFile(string path)
        {
            string[] mediaExtensions = {
                ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA"
            };
            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant());
        }
    }
}