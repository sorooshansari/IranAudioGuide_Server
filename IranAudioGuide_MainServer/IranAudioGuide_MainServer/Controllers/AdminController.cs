using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using System.IO;
using System.Data.SqlClient;

namespace IranAudioGuide_MainServer.Controllers
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
        private const string storagePrefix = "http://iranaudioguide.com/";
        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.View = Views.AdminIndex;
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");
            return View(currentUser);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddTip(AddTipVM model)
        {
            using (var dbTran = db.Database.BeginTransaction())
            {

                try
                {
                    Tip newTip = new Tip()
                    {
                        Tip_Category = db.TipCategories.Where(x => x.TiC_Id == model.TipCategoryId).First(),
                        Tip_Content = model.content,
                        Pla_Id = db.Places.Where(x => x.Pla_Id == model.PlaceId).First()
                    };
                    db.Tips.Add(newTip);
                    db.SaveChanges();
                    UpdateLog(updatedTable.Tip, newTip.Tip_Id);
                    db.SaveChanges();
                    dbTran.Commit();
                    return Json(true);
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    return Json(false);
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult RemoveTip(Guid Id)
        {
            try
            {
                var tip = db.Tips.Where(x => x.Tip_Id == Id).FirstOrDefault();
                UpdateLog(updatedTable.Tip, tip.Tip_Id, true);
                db.Tips.Remove(tip);
                db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetPlaceTips(Guid placeId)
        {
            try
            {
                List<TipVM> res = (from t in db.Tips
                                   where t.Pla_Id.Pla_Id == placeId
                                   select new TipVM()
                                   {
                                       Content = t.Tip_Content,
                                       id = t.Tip_Id,
                                       TipcategoryID = t.Tip_Category.TiC_Id
                                   }).ToList();
                return Json(res);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetTipCategories()
        {
            try
            {
                List<TipCategoriesVM> res = (from c in db.TipCategories
                                             orderby c.TiC_Priority
                                             select new TipCategoriesVM()
                                             {
                                                 Class = c.TiC_Class,
                                                 id = c.TiC_Id,
                                                 unicode = c.TiC_Unicode,
                                                 name = c.TiC_Name,
                                                 iconicName = c.TiC_Unicode + " " + c.TiC_Name
                                             }).ToList();
                return Json(res);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult Storys(string PlaceId)
        {
            if (PlaceId.Length > 0)
            {
                string imgUrl = PlaceImg(PlaceId);
                if (imgUrl != null)
                {
                    return Json(new StoryViewVM()
                    {
                        Storys = GetStorys(PlaceId),
                        PlaceImage = imgUrl
                    });
                }
                return Json(new StoryViewVM()
                {
                    Storys = GetStorys(PlaceId),
                    respond = new Respond(content: "Error. Couldn't find any image.", status: status.invalidInput)
                });
            }
            return Json(new StoryViewVM()
            {
                respond = new Respond(content: "Fatal error. Invalid Place Id.", status: status.invalidInput)
            });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddStory(NewStoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            if (model.StoryFile.ContentLength > 0 && IsAudioFile(model.StoryFile.FileName))
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                        var Story = new Story()
                        {
                            Sto_Name = model.StoryName,
                            Pla_Id = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault()
                        };
                        db.Storys.Add(Story);
                        db.SaveChanges(); //Save Story and generate Sto_Id
                        string id = Convert.ToString(Story.Sto_Id);
                        string extention = Path.GetExtension(model.StoryFile.FileName);
                        string path = string.Format("~/Stories/{0}{1}", id, extention);
                        model.StoryFile.SaveAs(Server.MapPath(path));
                        Story.Sto_Url = string.Format("{0}{1}", id, extention);
                        UpdateLog(updatedTable.Story, Story.Sto_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                        return Json(new Respond());
                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                }
            }
            return Json(new Respond("Only WAV, MID, MIDI, WMA, MP3, OGG, and RMA are allowed.", status.invalidFileFormat));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelStory(Guid Id)
        {
            var Story = db.Storys.Where(x => x.Sto_Id == Id).FirstOrDefault();
            if (Story != default(Story))
            {
                try
                {
                    string path = Server.MapPath(string.Format("~/Stories/{0}", Story.Sto_Url));
                    UpdateLog(updatedTable.Story, Story.Sto_Id, true);
                    db.Storys.Remove(Story);
                    db.SaveChanges();
                    lock (DelAdo)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }
            return Json(new Respond("Invalid Story Id", status.invalidInput));
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
                    respond = new Respond(content: "Error. Couldn't find any image.", status: status.invalidInput)
                });
            }
            return Json(new AudioViewVM()
            {
                respond = new Respond(content: "Fatal error. Invalid Place Id.", status: status.invalidInput)
            });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddAudio(NewAudioVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            if (model.AudioFile.ContentLength > 0 && IsAudioFile(model.AudioFile.FileName))
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
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
                        UpdateLog(updatedTable.Audio, audio.Aud_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                        return Json(new Respond());
                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                }
            }
            return Json(new Respond("Only WAV, MID, MIDI, WMA, MP3, OGG, and RMA are allowed.", status.invalidFileFormat));
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
                    UpdateLog(updatedTable.Audio, audio.Aud_Id, true);
                    db.Audios.Remove(audio);
                    db.SaveChanges();
                    lock (DelAdo)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }
            return Json(new Respond("Invalid Audio Id", status.invalidInput));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddPlace(NewPlace model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
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
                        return Json(new Respond("Enter X and Y cordinates and seprate them with \",\".", status.ivalidCordinates));
                    try
                    {
                        List<double> cordinates = (from c in model.PlaceCordinates.Split(',')
                                                   select Convert.ToDouble(c)).ToList();
                        place.Pla_cordinate_X = cordinates[0];
                        place.Pla_cordinate_Y = cordinates[1];
                    }
                    catch (Exception)
                    {
                        return Json(new Respond("Enter percise cordinates.", status.ivalidCordinates));
                    }
                }

                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        place.Pla_city = db.Cities.Where(c => c.Cit_Id == model.PlaceCityId).FirstOrDefault();
                        db.Places.Add(place);
                        db.SaveChanges(); //Save place and generate Pla_Id
                        string id = Convert.ToString(place.Pla_Id);
                        string extention = Path.GetExtension(model.Image.FileName);
                        string path = string.Format("~/images/Places/{0}{1}", id, extention);
                        string tumbnailPath = string.Format("~/images/Places/TumbnailImages/{0}{1}", id, extention);
                        model.Image.SaveAs(Server.MapPath(path));
                        model.Image.SaveAs(Server.MapPath(tumbnailPath));
                        place.Pla_ImgUrl = string.Format("{0}{1}", id, extention);
                        place.Pla_TumbImgUrl = string.Format("{0}{1}", id, extention);
                        UpdateLog(updatedTable.Place, place.Pla_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                        return Json(new Respond());
                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                }
            }
            return Json(new Respond("Only jpg, png, gif, and jpeg are allowed.", status.invalidFileFormat));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelPlace(Guid Id)
        {
            var place = db.Places.Where(x => x.Pla_Id == Id).FirstOrDefault();
            if (place.Pla_isOnline)
            {
                return Json(new Respond("We can't remove Online places. First make it offline.", status.removeOnlinePlace));
            }
            if (place != default(Place))
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        int result = db.Database.SqlQuery<int>("DeletePlace @Id", new SqlParameter("@Id", Id)).Single();
                        switch (result)
                        {
                            case 0:
                                string path = Server.MapPath(string.Format("~/images/Places/{0}", place.Pla_ImgUrl));
                                string tumbPath = Server.MapPath(string.Format("~/images/Places/TumbnailImages/{0}", place.Pla_ImgUrl));
                                lock (DelPlc)
                                {
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                    if (System.IO.File.Exists(tumbPath))
                                    {
                                        System.IO.File.Delete(tumbPath);
                                    }
                                }
                                dbTran.Commit();
                                return Json(new Respond());
                            case 1:
                                return Json(new Respond("This place has some dependencies.", status.forignKeyError));
                            case 2:
                                return Json(new Respond("Something went wrong. Contact devloper team.", status.dbError));
                            default:
                                return Json(new Respond("Something went wrong. Contact devloper team.", status.unknownError));
                        }
                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                }
            }
            return Json(new Respond("Invalid Place Id.", status.invalidId));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult EditPlace(EditPlaceVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
            if (place == default(Place))
            {
                return Json(new Respond("Invalid Place Id", status.invalidId));
            }
            place.Pla_Name = model.PlaceName;
            place.Pla_Discription = model.PlaceDesc;
            place.Pla_city = db.Cities.Where(x => x.Cit_Id == model.PlaceCityId).FirstOrDefault();
            place.Pla_Address = model.PlaceAddress;
            if (model.PlaceCordinates != null)
            {
                if (!model.PlaceCordinates.Contains(','))
                    return Json(new Respond("Enter X and Y cordinates and seprate them with \",\".", status.ivalidCordinates));
                try
                {
                    List<double> cordinates = (from c in model.PlaceCordinates.Split(',')
                                               select Convert.ToDouble(c)).ToList();
                    place.Pla_cordinate_X = cordinates[0];
                    place.Pla_cordinate_Y = cordinates[1];
                }
                catch (Exception)
                {
                    return Json(new Respond("Enter percise cordinates.", status.ivalidCordinates));
                }
            }
            try
            {
                UpdateLog(updatedTable.Place, place.Pla_Id);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
            return Json(new Respond());
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetPlaces(int PageNum)
        {
            var places = GetPlaces();
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetAllPlaces()
        {
            var places = GetPlaces();
            int counter = 0;
            foreach (var place in places)
            {
                place.Index = ++counter;
            }
            return Json(places);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult SwichPrimaryStatus(Guid PlaceId)
        {
            try
            {
                var place = db.Places.Where(x => x.Pla_Id == PlaceId).FirstOrDefault();
                if (place == default(Place))
                {
                    return Json(new Respond("Invalid Place Id.", status.invalidId));
                }
                place.Pla_isPrimary = !place.Pla_isPrimary;
                UpdateLog(updatedTable.Place, PlaceId);
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GoOnline(Guid PlaceId)
        {
            try
            {
                var place = db.Places.Where(x => x.Pla_Id == PlaceId).FirstOrDefault();
                if (place == default(Place))
                {
                    return Json(new Respond("Invalid Place Id.", status.invalidId));
                }
                place.Pla_isOnline = true;
                UpdateLog(updatedTable.Place, place.Pla_Id);
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GoOffline(Guid PlaceId)
        {
            try
            {
                var place = db.Places.Where(x => x.Pla_Id == PlaceId).FirstOrDefault();
                if (place == default(Place))
                {
                    return Json(new Respond("Invalid Place Id.", status.invalidId));
                }
                place.Pla_isOnline = false;
                UpdateLog(updatedTable.Place, place.Pla_Id, true);
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult ChangePlaceTumbImage(ChangeImageVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            try
            {
                var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                if (place == default(Place))
                {
                    return Json(new Respond("Invalid PlaceId", status.invalidId));
                }
                string oldPath = Server.MapPath(string.Format("~/images/Places/TumbnailImages/{0}", place.Pla_TumbImgUrl));
                string imgName = Path.GetFileNameWithoutExtension(place.Pla_TumbImgUrl);
                string extention = Path.GetExtension(model.NewImage.FileName);
                string newPath = Server.MapPath(string.Format("~/images/Places/TumbnailImages/{0}{1}", imgName, extention));
                lock (ChangeImgLock)
                {
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    model.NewImage.SaveAs(newPath);
                    place.Pla_TumbImgUrl = string.Format("{0}{1}", imgName, extention);
                }
                UpdateLog(updatedTable.Place, place.Pla_Id);
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult ChangePlaceImage(ChangeImageVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            try
            {
                var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                if (place == default(Place))
                {
                    return Json(new Respond("Invalid PlaceId", status.invalidId));
                }
                string oldPath = Server.MapPath(string.Format("~/images/Places/{0}", place.Pla_ImgUrl));
                string imgName = Path.GetFileNameWithoutExtension(place.Pla_ImgUrl);
                string extention = Path.GetExtension(model.NewImage.FileName);
                string newPath = Server.MapPath(string.Format("~/images/Places/{0}{1}", imgName, extention));
                lock (ChangeImgLock)
                {
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    model.NewImage.SaveAs(newPath);
                    place.Pla_ImgUrl = string.Format("{0}{1}", imgName, extention);
                }
                UpdateLog(updatedTable.Place, place.Pla_Id);
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddPlaceExtraImage(NewImageVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            if (!(model.NewImage != null && model.NewImage.ContentLength > 0 && IsImage(model.NewImage)))
            {
                return Json(new Respond("Only jpg, png, gif, and jpeg are allowed.", status.invalidFileFormat));
            };
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    var img = new Image() { Pla_Id = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault() };
                    var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                    db.Images.Add(img);
                    db.SaveChanges();
                    string id = Convert.ToString(img.Img_Id);
                    string extention = Path.GetExtension(model.NewImage.FileName);
                    string path = string.Format("~/images/Places/Extras/{0}{1}", id, extention);
                    model.NewImage.SaveAs(Server.MapPath(path));
                    img.Img_Name = string.Format("{0}{1}", id, extention);
                    UpdateLog(updatedTable.ExtraImage, img.Img_Id);
                    db.SaveChanges();
                    dbTran.Commit();

                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
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
                        return Json(new Respond("Invalid Image Id", status.invalidId));
                    }
                    string path = Server.MapPath(string.Format("~/images/Places/Extras/{0}", img.Img_Name));
                    UpdateLog(updatedTable.ExtraImage, img.Img_Id, true);
                    db.Images.Remove(img);
                    db.SaveChanges();
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
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
                    return Json(new Respond("Invalid Image Id", status.invalidId));
                }
                img.Img_Description = model.ImageDesc;
                UpdateLog(updatedTable.ExtraImage, img.Img_Id);
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
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
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            var city = new city() { Cit_Name = model.CityName, Cit_Description = model.CityDesc };
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    db.Cities.Add(city);
                    UpdateLog(updatedTable.City, Guid.Empty, false, city.Cit_Id);
                    db.SaveChanges();
                    dbTran.Commit();
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelCity(int Id)
        {
            int result = db.Database.SqlQuery<int>("DeleteCity @Id", new SqlParameter("@Id", Id)).Single();
            switch (result)
            {
                case 0:
                    UpdateLog(updatedTable.City, Guid.Empty, true, Id);
                    return Json(new Respond());
                case 1:
                    return Json(new Respond("This city has some places.", status.forignKeyError));
                case 2:
                    return Json(new Respond("Something went wrong. Contact devloper team.", status.dbError));
                default:
                    return Json(new Respond("Something went wrong. Contact devloper team.", status.unknownError));
            }
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
        private List<PlaceVM> GetPlaces()
        {
            try
            {
                List<PlaceVM> Places =
                    (from place in db.Places
                     where !place.Pla_Deactive
                     orderby place.Pla_Id descending
                     select new PlaceVM()
                     {
                         PlaceId = place.Pla_Id,
                         ImgUrl = place.Pla_ImgUrl,
                         TumbImgUrl = place.Pla_TumbImgUrl,
                         PlaceDesc = place.Pla_Discription,
                         PlaceName = place.Pla_Name,
                         CityName = place.Pla_city.Cit_Name,
                         PlaceAddress = place.Pla_Address,
                         PlaceCordinates = place.Pla_cordinate_X.ToString() + "," + place.Pla_cordinate_Y.ToString(),
                         PlaceCityId = place.Pla_city.Cit_Id,
                         isOnline = place.Pla_isOnline,
                         isPrimary = place.Pla_isPrimary
                     }).ToList();
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
                return null;
            }
        }

        private List<StoryVM> GetStorys(string PlaceId)
        {
            List<StoryVM> Storys = (from a in db.Storys
                                    where a.Pla_Id == db.Places.Where(x => x.Pla_Id.ToString() == PlaceId).FirstOrDefault()
                                    select new StoryVM()
                                    {
                                        Discription = a.Sto_Discription,
                                        Name = a.Sto_Name,
                                        Url = a.Sto_Url,
                                        Id = a.Sto_Id
                                    }).ToList();
            int counter = 0;
            foreach (var item in Storys)
            {
                item.Index = ++counter;
            }
            return Storys;
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
        private bool IsAudioFile(string path)
        {
            string[] mediaExtensions = {
                ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA"
            };
            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant());
        }
        private void UpdateLog(updatedTable updatedTable, Guid id, bool removed = false, int cityId = 0)
        {
            switch (updatedTable)
            {
                case updatedTable.Place:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Pla_ID == id));
                    db.UpdateLogs.Add(new UpdateLog() { Pla_ID = id, isRemoved = removed });
                    break;
                case updatedTable.Audio:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Aud_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Aud_Id = id, isRemoved = removed });
                    break;
                case updatedTable.Story:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Sto_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Sto_Id = id, isRemoved = removed });
                    break;
                case updatedTable.Tip:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Tip_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Tip_Id = id, isRemoved = removed });
                    break;
                case updatedTable.ExtraImage:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Ima_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Ima_Id = id, isRemoved = removed });
                    break;
                case updatedTable.City:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Cit_ID == cityId));
                    db.UpdateLogs.Add(new UpdateLog() { Cit_ID = cityId, isRemoved = removed });
                    break;
                default:
                    break;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}