using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using System.IO;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using IranAudioGuide_MainServer.Services;
using System.Data.Entity;
using Elmah;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private const int pagingLen = 10;
        private Object ChangeImgLock = new Object();
        private Object DelExtraImg = new Object();
        private Object DelAdo = new Object();
        private Object DelPlc = new Object();
        private Object DelCit = new Object();
        public int lang
        {
            get
            {
                var lang = HttpContext.Request.RequestContext.RouteData.Values["lang"];
                return lang != null ? ServiceCulture.GetIntLang(lang.ToString()) : (int)EnumLang.en;
            }
        }
        //private const string storagePrefix = "http://iranaudioguide.com/";
        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ServiceCulture.SetCulture();
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
                        Place = db.Places.FirstOrDefault(x => x.Pla_Id == model.PlaceId),
                        langId = lang,
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
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return Json(false);
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult RemoveTip(Guid Id)
        {
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    var tip = db.Tips.Where(x => x.Tip_Id == Id).FirstOrDefault();
                    UpdateLog(updatedTable.Tip, tip.Tip_Id, true);
                    db.Tips.Remove(tip);
                    db.SaveChanges();
                    dbTran.Commit();
                    return Json(true);
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return Json(false);
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetPlaceTips(Guid placeId)
        {
            try
            {
                List<TipVM> res = db.Tips.Include(x => x.Place)
                    .Where(x => x.Place.Pla_Id == placeId && x.langId == lang)
                    .Select(t => new TipVM()
                    {
                        Content = t.Tip_Content,
                        id = t.Tip_Id,
                        TipcategoryID = t.Tip_Category.TiC_Id
                    }).ToList();
                return Json(res);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                            Place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault(),
                            langId = lang
                        };
                        db.Storys.Add(Story);
                        db.SaveChanges();
                        //uplaod file
                        var request = new ServiceFtp();
                        var fileName = Convert.ToString(Story.Sto_Id) + Path.GetExtension(model.StoryFile.FileName);
                        var fullPath = GlobalPath.FtpPrimaryPathStory + fileName;
                        var isSuccess = request.Upload(model.StoryFile, fullPath);

                        //end upload file
                        Story.Sto_Url = fileName;

                        UpdateLog(updatedTable.Story, Story.Sto_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                        return Json(new Respond());
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                }
            }
            return Json(new Respond("Only WAV, MID, MIDI, WMA, MP3, OGG, and RMA are allowed.", status.invalidFileFormat));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        //todo test
        public JsonResult DelStory(Guid Id)
        {
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    var Story = db.Storys.Where(x => x.Sto_Id == Id).FirstOrDefault();
                    var fileName = Story.Sto_Url;
                    if (Story == default(Story))
                        return Json(new Respond("Invalid Story Id", status.invalidInput));


                    UpdateLog(updatedTable.Story, Story.Sto_Id, true);
                    db.Storys.Remove(Story);
                    db.SaveChanges();
                    dbTran.Commit();
                    var request = new ServiceFtp();
                    lock (DelAdo)
                    {
                        if (request.IsDirectoryExist(fileName, GlobalPath.FtpPrimaryPathStory))
                        {
                            request.delete(fileName, GlobalPath.FtpPrimaryPathStory);
                        }
                    }
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }


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
        //todo test
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
                        var place = db.Places.Include(x => x.Pla_Audios).FirstOrDefault(x => x.Pla_Id == model.PlaceId);
                        if (place == null)
                            return Json(new Respond());

                        if (place.Pla_Audios == null || place.Pla_Audios.Count == 0)
                            place.Pla_Audios = new List<Audio>();
                        var audio = new Audio()
                        {
                            langId = lang,
                            Aud_Name = model.AudioName
                            //Place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault()
                        };
                        place.Pla_Audios.Add(audio);

                        db.SaveChanges();

                        //uplaod file
                        var request = new ServiceFtp();
                        var fileName = Convert.ToString(audio.Aud_Id) + Path.GetExtension(model.AudioFile.FileName);
                        var fullpath = GlobalPath.FtpPrimaryPathAudios + fileName;
                        var isSuccess = request.Upload(model.AudioFile, fullpath);
                        if (!isSuccess)
                        {
                            throw new Exception("admin cant Upload Audio file");
                            //request.createDirectory(GlobalPath.FullPathAudios);
                        }
                        //end upload file

                        audio.Aud_Url = fileName;
                        UpdateLog(updatedTable.Audio, audio.Aud_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                        return Json(new Respond());
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
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
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    var audio = db.Audios.Where(x => x.Aud_Id == Id).FirstOrDefault();
                    if (audio == default(Audio))
                        return Json(new Respond("Invalid Audio Id", status.invalidInput));

                    db.Audios.Remove(audio);
                    UpdateLog(updatedTable.Audio, audio.Aud_Id, true);
                    db.SaveChanges();


                    var request = new ServiceFtp();
                    var fileName = audio.Aud_Url;
                    lock (DelAdo)
                    {
                        if (request.IsDirectoryExist(fileName, GlobalPath.FtpPrimaryPathAudios))
                        {
                            request.delete(fileName, GlobalPath.FtpPrimaryPathAudios);
                        }
                    }

                    dbTran.Commit();
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }

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
                var place = new Place();

             
                place.Pla_Name = model.PlaceName;
                place.Pla_Discription = model.PlaceDesc;
                place.Pla_Address = model.PlaceAddress;
                place.TranslatePlaces = new List<TranslatePlace>();


                TranslatePlace tp = new TranslatePlace()
                {
                    langId = lang,
                    TrP_Name = model.PlaceName,
                    TrP_Address = model.PlaceAddress,
                    TrP_Description = model.PlaceDesc
                };
                place.TranslatePlaces.Add(tp);
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
                    catch (Exception ex)
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
                        db.SaveChanges();

                        //uplaod file
                        var request = new ServiceFtp();
                        var fileName = Convert.ToString(place.Pla_Id) + Path.GetExtension(model.Image.FileName);
                        //end upload file

                        place.Pla_ImgUrl = fileName;
                        place.Pla_TumbImgUrl = fileName;

                        UpdateLog(updatedTable.Place, place.Pla_Id);
                        UpdateLog(updatedTable.TPlace, tp.TrP_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                        var fullpath = GlobalPath.FtpPathImagePlace + fileName;
                        var isSuccess = request.Upload(model.Image, fullpath);
                        if (!isSuccess)
                            throw new ArgumentException("Dont save image in Server", "original");


                        var PathSource = GlobalPath.FtpPathImagePlace + fileName;
                        var Destination = GlobalPath.FtpPathImageTumbnail + fileName;
                        isSuccess = request.Copy(PathSource, Destination);
                        if (!isSuccess)
                            throw new ArgumentException("Dont save Tumbnail image in Server", "original");

                        return Json(new Respond());
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                }
            }
            return Json(new Respond("Only jpg, png, gif, and jpeg are allowed.", status.invalidFileFormat));
        }


        //public int MyProperty()
        //{

        //    var request = new ServiceFtp();
        //    var fileName = img.Img_Name;
        //    if (request.IsDirectoryExist(fileName, GlobalPath.FullPathImageExtras))
        //    {
        //        request.delete(fileName, GlobalPath.FullPathImageExtras); ;
        //    }
        //}
        //public void DelPlaceExtraImage(Guid imgId)
        //{
        //    using (var dbTran = db.Database.BeginTransaction())
        //    {
        //        lock (DelExtraImg)
        //        {
        //            var img = db.Images.Include(x => x.Place).Where(x => x.Img_Id == imgId).FirstOrDefault();
        //            if (img == default(Image))
        //                return;
        //            UpdateLog(updatedTable.ExtraImage, img.Img_Id, true);
        //            db.Images.Remove(img);
        //        }

        //    }
        //}

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelPlace(Guid Id)
        {
            var place = db.Places
                .Include(x => x.Pla_Audios)
                .Include(x => x.Pla_Stories)
               .Include(x => x.Pla_ExtraImages)
                .Include(x => x.TranslatePlaces)
                .Include(x => x.Pla_Tips)
                .Where(x => x.Pla_Id == Id).FirstOrDefault(x => x.Pla_isOnline == false);

            if (place == default(Place))
            {
                return Json(new Respond("We can't remove places", status.removeOnlinePlace));
            }
            if (place != default(Place))
            {
                if (place.Pla_Audios.Count != 0 || place.Pla_Stories.Count != 0)
                    return Json(new Respond("This place has some dependencies.", status.forignKeyError));


                List<ImagRemoveVm> extraImages = new List<ImagRemoveVm>();

                extraImages = place.Pla_ExtraImages.Select(x => new ImagRemoveVm
                {
                    Id = x.Img_Id,
                    Name = x.Img_Name
                }).ToList();

                using (var dbTran = db.Database.BeginTransaction())
                {

                    try
                    {
                        //db.Database.SqlQuery<int>("DeletePlace @Id", new SqlParameter("@Id", Id)).Single();

                        UpdateLog(updatedTable.Place, place.Pla_Id, true);

                        lock (DelPlc)
                        {

                            foreach (var itemImg in place.Pla_ExtraImages)
                            {
                                UpdateLog(updatedTable.ExtraImage, itemImg.Img_Id, true);
                            }
                            foreach (var item in place.Pla_Tips)
                            {
                                UpdateLog(updatedTable.Tip, item.Tip_Id, true);
                            }

                            foreach (var item in place.TranslatePlaces)
                            {
                                UpdateLog(updatedTable.TPlace, item.TrP_Id, true);
                            }
                        }
                        db.Places.Remove(place);
                        var result = db.SaveChanges() > 0;
                        dbTran.Commit();
                        if (result)
                            lock (DelPlc)
                            {
                                removeImage(place.Pla_ImgUrl, GlobalPath.FullPathImagePlace);
                                removeImage(place.Pla_TumbImgUrl, GlobalPath.FullPathImagePlace);
                                foreach (var itemImg in extraImages)
                                {
                                    removeImage(itemImg.Name, GlobalPath.FullPathImageExtras);
                                }
                            }
                        if (result)
                            return Json(new Respond("remove place.", status.success));
                        else
                            return Json(new Respond("Something went wrong. Contact devloper team.", status.dbError));
                    }

                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                }
            }
            return Json(new Respond("Invalid Place Id.", status.invalidId));
        }

        private void removeImage(string fileName, string fullPathImagePlace)
        {
            var request = new ServiceFtp();
            if (request.IsDirectoryExist(fileName, fullPathImagePlace))
            {
                request.delete(fileName, fullPathImagePlace);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult EditPlace(EditPlaceVM model)
        {

            //Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            var place = db.Places.Include(c => c.Pla_city).Include(d => d.TranslatePlaces)
                .Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
            if (place == default(Place))
            {
                return Json(new Respond("Invalid Place Id", status.invalidId));
            }
            if (lang == (int)EnumLang.en)
            {

                place.Pla_Name = model.PlaceName;
                place.Pla_Discription = model.PlaceDesc;
                place.Pla_Address = model.PlaceAddress;
            }

            var newLang = false;

            var t = place.TranslatePlaces.FirstOrDefault(x => x.langId == lang);
            if (t == null)
            {
                t = new TranslatePlace() { langId = lang };
                newLang = true;
            }


            t.TrP_Name = model.PlaceName;
            t.TrP_Description = model.PlaceDesc;
            t.TrP_Address = model.PlaceAddress;

            if (newLang)
                place.TranslatePlaces.Add(t);
            else
            {
                db.Entry(t).State = EntityState.Modified;
            }

            if (place.Pla_city.Cit_Id != model.PlaceCityId)
                place.Pla_city = db.Cities.Where(x => x.Cit_Id == model.PlaceCityId).FirstOrDefault();

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
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return Json(new Respond("Enter percise cordinates.", status.ivalidCordinates));
                }
            }
            try
            {
                UpdateLog(updatedTable.TPlace, t.TrP_Id);
                UpdateLog(updatedTable.Place, place.Pla_Id);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new Respond(ex.Message, status.unknownError));
            }
            return Json(new Respond());
            //Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            //return Json(place);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                using (var dbTran = db.Database.BeginTransaction())
                {
                    var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                    if (place == default(Place))
                    {
                        return Json(new Respond("Invalid PlaceId", status.invalidId));
                    }

                    //uplaod file
                    var request = new ServiceFtp();
                    var fileName = Convert.ToString(model.PlaceId) + Path.GetExtension(model.NewImage.FileName);

                    try
                    {
                        if (Path.GetFileNameWithoutExtension(place.Pla_ImgUrl) != Path.GetFileNameWithoutExtension(model.NewImage.FileName))
                        {
                            request.delete(place.Pla_ImgUrl, GlobalPath.FullPathImageTumbnail);
                            place.Pla_ImgUrl = fileName;
                        }
                        UpdateLog(updatedTable.Place, place.Pla_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                    var fullpaht = GlobalPath.FullPathImageTumbnail + fileName;
                    var isSuccess = request.Upload(model.NewImage, fullpaht);
                    if (!isSuccess)
                        throw new ArgumentException("Dont save image in Server", "original");

                }
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
            if (!(model.NewImage != null && model.NewImage.ContentLength > 0 && IsImage(model.NewImage)))
            {
                return Json(new Respond("Only jpg, png, gif, and jpeg are allowed.", status.invalidFileFormat));
            };
            try
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                    if (place == default(Place))
                    {
                        return Json(new Respond("Invalid PlaceId", status.invalidId));
                    }

                    //uplaod file
                    var request = new ServiceFtp();
                    var fileName = Convert.ToString(model.PlaceId) + Path.GetExtension(model.NewImage.FileName);

                    try
                    {
                        if (Path.GetFileNameWithoutExtension(place.Pla_TumbImgUrl) != Path.GetFileNameWithoutExtension(model.NewImage.FileName))
                        {
                            request.delete(place.Pla_TumbImgUrl, GlobalPath.FtpPathImagePlace);
                            place.Pla_TumbImgUrl = fileName;
                        }
                        UpdateLog(updatedTable.Place, place.Pla_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                    var fullpath = GlobalPath.FtpPathImagePlace + fileName;
                    var isSuccess = request.Upload(model.NewImage, fullpath);
                    if (isSuccess)
                        throw new ArgumentException("Dont save image in Server", "original");

                }

                return Json(new Respond());
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                    var getPlace = db.Places.Include(p => p.Pla_ExtraImages).Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                    int o = 1;
                    if (getPlace.Pla_ExtraImages.Count != 0)
                        o = getPlace.Pla_ExtraImages.Max(x => x.Tmg_Order) + 1;
                    var img = new Image()
                    {
                        Place = getPlace,
                        Tmg_Order = o
                        //todo do it
                        //TranslateImages = new List<TranslateImage>()
                    };
                    //todo do it
                    //var tImage = new TranslateImage() {
                    //    langId = lang,
                    //    TrI_Description = "",
                    //    TrI_Name ="",
                    //};

                    db.Images.Add(img);
                    db.SaveChanges();

                    //uplaod file
                    var request = new ServiceFtp();
                    var fileName = Convert.ToString(img.Img_Id) + Path.GetExtension(model.NewImage.FileName);
                    var fullpaht = GlobalPath.FtpPathImageExtras + fileName;
                    var isSuccess = request.Upload(model.NewImage, fullpaht);
                    //end upload file

                    img.Img_Name = fileName;
                    UpdateLog(updatedTable.ExtraImage, img.Img_Id);
                    db.SaveChanges();
                    dbTran.Commit();

                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelPlaceExtraImage(Guid imgId)
        {
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    lock (DelExtraImg)
                    {
                        var img = db.Images.Include(x => x.Place).Where(x => x.Img_Id == imgId).FirstOrDefault();
                        if (img == default(Image))
                        {
                            return Json(new Respond("Invalid Image Id", status.invalidId));
                        }
                        UpdateLog(updatedTable.ExtraImage, img.Img_Id, true);
                        db.Images.Remove(img);
                        db.SaveChanges();
                        var request = new ServiceFtp();
                        var fileName = img.Img_Name;
                        if (request.IsDirectoryExist(fileName, GlobalPath.FullPathImageExtras))
                        {
                            request.delete(fileName, GlobalPath.FullPathImageExtras); ;
                        }
                        dbTran.Commit();
                    }
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }



        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult EditPlaceExtraImageDesc(EditEIDescVM model)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new Respond("invalid img id", status.invalidInput));
                }
                var trImg = db.TranslateImages.FirstOrDefault(ti => ti.Img_Id == model.ImageId && ti.langId == lang);
                if (trImg == default(TranslateImage))
                {
                    var newTrImg = new TranslateImage()
                    {
                        langId = lang,
                        Img_Id = model.ImageId,
                        TrI_Description = model.ImageDesc
                    };
                    db.TranslateImages.Add(newTrImg);
                    db.SaveChanges();
                    UpdateLog(updatedTable.TImage, newTrImg.TrI_Id, false);
                }
                else
                {
                    trImg.TrI_Description = model.ImageDesc;
                    UpdateLog(updatedTable.TImage, trImg.TrI_Id, false);
                }
                db.SaveChanges();
                return Json(new Respond());
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetExtraImages(Guid placeId)
        {
            var img = db.Images.Include(x => x.TranslateImages).Include(x => x.Place)
                .Where(x => x.Place.Pla_Id == placeId)
                .Select(i => new ImageVM()
                {
                    ImageId = i.Img_Id,
                    ImageName = i.Img_Name,
                    ImageDesc = i.TranslateImages.FirstOrDefault(tr => tr.langId == lang).TrI_Description,
                    Index = i.Tmg_Order
                }).ToList();

            return Json(img);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddCity(NewCity model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            var city = new city() { Cit_Name = model.CityName, Cit_Description = model.CityDesc, TranslateCities = new List<TranslateCity>() };
            var tcity = new TranslateCity()
            {
                langId = lang, // LangService.GetId(model.lang),
                TrC_Name = model.CityName,
                TrC_Description = model.CityDesc
            };
            city.TranslateCities.Add(tcity);
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    db.Cities.Add(city);
                    db.SaveChanges();

                    //uplaod file
                    var request = new ServiceFtp();
                    var fileName = Convert.ToString(city.Cit_Id) + Path.GetExtension(model.CityImage.FileName);
                    var fullpath = GlobalPath.FtpPathImageCity + fileName;
                    var isSuccess = request.Upload(model.CityImage, fullpath);
                    //end upload file

                    city.Cit_ImageUrl = fileName;

                    UpdateLog(updatedTable.TCity, tcity.TrC_Id, false);
                    UpdateLog(updatedTable.City, Guid.Empty, false, city.Cit_Id);
                    db.SaveChanges();
                    dbTran.Commit();
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult EditCity(EditCityVM model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }

            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            var cityT = db.TranslateCities.Include(i => i.city)
                .FirstOrDefault(x => x.Cit_Id == model.CityID && x.langId == lang);

            if (cityT == default(TranslateCity))
            {
                cityT = new TranslateCity();
                //todo remove this for v2  
                // cityT.cityid = model.CityID;
                cityT.city = db.Cities.FirstOrDefault(x => x.Cit_Id == model.CityID);
                db.TranslateCities.Add(cityT);
            }
            //todo remove this for v2
            if (lang == (int)EnumLang.en)
            {
                cityT.city.Cit_Name = model.CityName;
                cityT.city.Cit_Description = model.CityDesc;
            }
            cityT.TrC_Name = model.CityName;
            cityT.TrC_Description = model.CityDesc;
            cityT.langId = lang;


            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    UpdateLog(updatedTable.TCity, cityT.TrC_Id, false);
                    //todo remove this for v2
                    UpdateLog(updatedTable.City, Guid.Empty, false, model.CityID);
                    db.SaveChanges();
                    dbTran.Commit();
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult ChangeCityImage(ChangeCityImageVM model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            if (!(model.NewImage != null && model.NewImage.ContentLength > 0 && IsImage(model.NewImage)))
            {
                return Json(new Respond("Only jpg, png, gif, and jpeg are allowed.", status.invalidFileFormat));
            };

            try
            {

                using (var dbTran = db.Database.BeginTransaction())
                {
                    var city = db.Cities.Where(x => x.Cit_Id == model.CityId).FirstOrDefault();
                    if (city == default(city))
                    {
                        return Json(new Respond("Invalid PlaceId", status.invalidId));
                    }
                    //uplaod file
                    var request = new ServiceFtp();
                    var fileName = Convert.ToString(model.CityId) + Path.GetExtension(model.NewImage.FileName);

                    if (Path.GetFileNameWithoutExtension(city.Cit_ImageUrl) != Path.GetFileNameWithoutExtension(model.NewImage.FileName))
                        request.delete(city.Cit_ImageUrl, GlobalPath.FtpPathImageCity);

                    try
                    {
                        city.Cit_ImageUrl = fileName;
                        UpdateLog(updatedTable.City, Guid.Empty, false, city.Cit_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        dbTran.Rollback();
                        return Json(new Respond(ex.Message, status.unknownError));
                    }
                    var fullpath = GlobalPath.FtpPathImageCity + fileName;
                    var isSuccess = request.Upload(model.NewImage, fullpath);
                    if (isSuccess)
                        throw new ArgumentException("Dont save image in Server", "original");
                }

                //end upload file

                return Json(new Respond());
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelCity(int Id)
        {

            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    var city = db.Cities.Where(x => x.Cit_Id == Id).First();
                    int result = db.Database.SqlQuery<int>("DeleteCity @Id", new SqlParameter("@Id", Id)).Single();
                    UpdateLog(updatedTable.City, Guid.Empty, true, Id);
                    db.SaveChanges();
                    dbTran.Commit();
                    switch (result)
                    {
                        case 0:
                            var request = new ServiceFtp();
                            var fileName = city.Cit_ImageUrl;
                            lock (DelCit)
                            {
                                if (request.IsDirectoryExist(fileName, GlobalPath.FullPathImageCity))
                                {
                                    request.delete(fileName, GlobalPath.FullPathImageCity); ;
                                }
                            }
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
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbTran.Rollback();
                    return Json(new Respond("Something went wrong. Contact devloper team.", status.unknownError));
                }
            }
        }
        public virtual JsonResult HandleException(Exception exception)
        {
            DbUpdateConcurrencyException concurrencyEx = exception as DbUpdateConcurrencyException;
            if (concurrencyEx != null)
            {
                //so something
            }

            DbUpdateException dbUpdateEx = exception as DbUpdateException;
            if (dbUpdateEx != null)
            {
                if (dbUpdateEx != null
                        && dbUpdateEx.InnerException != null
                        && dbUpdateEx.InnerException.InnerException != null)
                {
                    SqlException sqlException = dbUpdateEx.InnerException.InnerException as SqlException;
                    if (sqlException != null)
                    {
                        switch (sqlException.Number)
                        {
                            case 2627:  // Unique constraint error
                                return Json(new Respond(exception.Message, status.unknownError));
                            case 547:   // Constraint check violation
                                return Json(new Respond("Forign key Error.", status.forignKeyError));
                            case 2601:  // Duplicated key row error
                                        // Constraint violation exception
                                return Json(new Respond(exception.Message, status.unknownError));

                            default:
                                // A custom exception of yours for other DB issues
                                return Json(new Respond(exception.Message, status.unknownError));
                        }
                    }

                    //so something
                }
            }
            return Json(new Respond(exception.Message, status.unknownError));

            // If we're here then no exception has been thrown
            // So add another piece of code below for other exceptions not yet handled...
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetCities(int PageNum)
        {
            if (PageNum == -1)
                return Json(GetAllCitiesName(lang));
            var Cities = GetCities2(lang);
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
                List<PlaceVM> Places = db.Places
                    .Include(i => i.TranslatePlaces)
                    .Include(i => i.Pla_city.TranslateCities)
                    .OrderBy(X => X.Pla_city.Cit_Order)
                    .Select(place => new PlaceVM()
                    {
                        PlaceId = place.Pla_Id,
                        ImgUrl = place.Pla_ImgUrl,
                        TumbImgUrl = place.Pla_TumbImgUrl,
                        PlaceDesc = place.TranslatePlaces.FirstOrDefault(x => x.langId == lang).TrP_Description,
                        PlaceName = place.TranslatePlaces.FirstOrDefault(x => x.langId == lang).TrP_Name,
                        CityName = place.Pla_city.TranslateCities.FirstOrDefault(x => x.langId == lang).TrC_Name,
                        PlaceAddress = place.TranslatePlaces.FirstOrDefault(x => x.langId == lang).TrP_Address,
                        PlaceCordinates = place.Pla_cordinate_X.ToString() + "," + place.Pla_cordinate_Y.ToString(),
                        PlaceCityId = place.Pla_city.Cit_Id,
                        isOnline = place.Pla_isOnline,
                        isPrimary = place.Pla_isPrimary
                    }).ToList();

                //List<PlaceVM> Places =
                //    (from place in db.Places
                //     where !place.Pla_Deactive
                //     orderby place.Pla_Id descending
                //     select new PlaceVM()
                //     {
                //         PlaceId = place.Pla_Id,
                //         ImgUrl = place.Pla_ImgUrl,
                //         TumbImgUrl = place.Pla_TumbImgUrl,
                //         PlaceDesc = place.Pla_Discription,
                //         PlaceName = place.Pla_Name,
                //         CityName = place.Pla_city.Cit_Name,
                //         PlaceAddress = place.Pla_Address,
                //         PlaceCordinates = place.Pla_cordinate_X.ToString() + "," + place.Pla_cordinate_Y.ToString(),
                //         PlaceCityId = place.Pla_city.Cit_Id,
                //         isOnline = place.Pla_isOnline,
                //         isPrimary = place.Pla_isPrimary
                //     }).ToList();
                return Places.ToList();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }

        private List<StoryVM> GetStorys(string PlaceId)
        {
            //where a.Place == db.Places.Where(x => x.Pla_Id.ToString() == PlaceId).FirstOrDefault()

            List<StoryVM> Storys = db.Storys.Where(x => x.langId == lang).Include(s => s.Place)
                .Where(x => x.Place.Pla_Id.ToString() == PlaceId)
                .Select(a => new StoryVM()
                {
                    Discription = a.Sto_Discription,
                    Name = a.Sto_Name,
                    Url = a.Sto_Url,
                    Id = a.Sto_Id
                }).ToList();
            var isAdmin = User.IsInRole("Admin");
            int counter = 0;
            foreach (var s in Storys)
            {
                var model = new GetAudioUrlVM()
                {
                    email = "",
                    uuid = "",
                    trackId = s.Id,
                    isAudio = false
                };
                s.Url = ServiceDownload.GetUrl(model, isAdmin);
                s.Index = ++counter;
            }

            return Storys;
        }
        private List<AudioVM> GetAudios(string PlaceId)
        {
            List<AudioVM> list = db.Audios.Where(x => x.langId == lang).Include(s => s.Place)
               .Where(x => x.Place.Pla_Id.ToString() == PlaceId)
               .Select(a => new AudioVM()
               {
                   Aud_Discription = a.Aud_Discription,
                   Aud_Name = a.Aud_Name,
                   Aud_Url = a.Aud_Url,
                   Aud_Id = a.Aud_Id,
               }).ToList();
            var isAdmin = User.IsInRole("Admin");
            int counter = 0;
            foreach (var s in list)
            {
                var model = new GetAudioUrlVM()
                {
                    email = "",
                    uuid = "",
                    trackId = s.Aud_Id,
                    isAudio = true
                };
                s.Aud_Url = ServiceDownload.GetUrl(model, isAdmin);
                s.Index = ++counter;
            }

            return list;
        }
        private List<JNOCitiesVM> GetAllCitiesName(int langId)
        {
            List<JNOCitiesVM> JNOCities = db.Cities.Include(i => i.TranslateCities).OrderByDescending(c => c.Cit_Id)
                .Select(c => new JNOCitiesVM()
                {
                    CityID = c.Cit_Id,
                    CityName = c.TranslateCities.FirstOrDefault(x => x.langId == langId).TrC_Name
                }).ToList();
            return JNOCities;
        }
        private List<CityVM> GetCities2(int langId)
        {
            List<CityVM> cities = db.Cities.Include(i => i.TranslateCities).OrderByDescending(c => c.Cit_Id)
                .Select(c => new CityVM()
                {
                    CityDesc = c.TranslateCities.FirstOrDefault(x => x.langId == langId).TrC_Description,
                    CityID = c.Cit_Id,
                    CityName = c.TranslateCities.FirstOrDefault(x => x.langId == langId).TrC_Name,
                    CityImageUrl = c.Cit_ImageUrl
                }).ToList();
            //List<CityVM> cities = (from c in db.Cities
            //                       orderby c.Cit_Id descending
            //                       select new CityVM()
            //                       {
            //                           CityDesc = c.Cit_Description,
            //                           CityID = c.Cit_Id,
            //                           CityName = c.Cit_Name,
            //                           CityImageUrl = c.Cit_ImageUrl
            //                       }).ToList();
            int counter = 0;
            foreach (var item in cities)
            {
                item.Index = ++counter;
            }
            return cities;
        }
        //private List<ImageVM> GetImages(Guid PlaceId)
        //{
        //    var img = (from i in db.Images
        //               where i.Pla_Id == db.Places.Where(x => x.Pla_Id == PlaceId).FirstOrDefault()
        //               select new ImageVM()
        //               {
        //                   ImageId = i.Img_Id,
        //                   ImageName = i.Img_Name,
        //                   ImageDesc = i.Img_Description
        //               }).ToList();
        //    int counter = 0;
        //    foreach (var i in img)
        //        i.Index = counter++;
        //    return img;
        //}
        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
                return true;

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
        private void UpdateLog(updatedTable updatedTable, Guid id, bool isRemoved = false, int cityId = 0)
        {
            switch (updatedTable)
            {
                case updatedTable.TPlace:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.TrP_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { TrP_Id = id, isRemoved = isRemoved });
                    break;

                case updatedTable.TCity:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.TrC_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { TrC_Id = id, isRemoved = isRemoved });
                    break;
                case updatedTable.TImage:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.TrI_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { TrI_Id = id, isRemoved = isRemoved });
                    break;

                case updatedTable.Place:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Pla_ID == id));
                    db.UpdateLogs.Add(new UpdateLog() { Pla_ID = id, isRemoved = isRemoved });
                    break;
                case updatedTable.Audio:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Aud_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Aud_Id = id, isRemoved = isRemoved });
                    break;
                case updatedTable.Story:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Sto_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Sto_Id = id, isRemoved = isRemoved });
                    break;
                case updatedTable.Tip:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Tip_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Tip_Id = id, isRemoved = isRemoved });
                    break;
                case updatedTable.ExtraImage:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Img_Id == id));
                    db.UpdateLogs.Add(new UpdateLog() { Img_Id = id, isRemoved = isRemoved });
                    break;
                case updatedTable.City:
                    db.UpdateLogs.RemoveRange(db.UpdateLogs.Where(x => x.Cit_ID == cityId));
                    db.UpdateLogs.Add(new UpdateLog() { Cit_ID = cityId, isRemoved = isRemoved });
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


        //private List<Guid> GetPackageCities(Guid pac_Id)
        //{
        //    List<Guid> packageCities = (from c in db.PackageCities
        //                                where c.Package_Pac_Id = pac_Id
        //                                select c.city_Cit_Id).ToList();
        //    return packageCities;
        //}


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetPackages()
        {


            List<PackageVM> packages = db.Packages
                .Include(x => x.Pac_Cities.Select(c => c.TranslateCities))//.Include("Pac_Cities.TranslateCities")
              .Where(x => x.langId == lang)
                .Select(p => new PackageVM()
                {
                    PackageName = p.Pac_Name,
                    PackageId = p.Pac_Id,
                    PackagePrice = p.Pac_Price,
                    PackagePriceDollar = p.Pac_Price_Dollar,
                    PackageCities = p.Pac_Cities.Select(c => new CityVM()
                    {
                        CityDesc = c.Cit_Description,
                        CityID = c.Cit_Id,
                        CityName = c.TranslateCities.FirstOrDefault(f => f.langId == lang).TrC_Name
                    }).ToList()
                }).OrderByDescending(o => o.PackageId).ToList();

            //(from p in db.Packages
            // orderby p.Pac_Id descending
            // select new PackageVM()
            // {
            //     PackageName = p.Pac_Name,
            //     PackageId = p.Pac_Id,
            //     PackagePrice = p.Pac_Price,
            //     PackageCities =
            //     (from c in db.Cities
            //      where
            //      (from pc in p.Pac_Cities select pc.Cit_Id).Contains(c.Cit_Id)
            //      select new CityVM()
            //      {
            //          CityDesc = c.Cit_Description,
            //          CityID = c.Cit_Id,
            //          CityName = c.Cit_Name
            //      }).ToList()
            // }).ToList();
            int counter = 0;
            foreach (var item in packages)
            {
                item.Index = ++counter;
            }
            return Json(packages);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddPackage(NewPackage model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            using (var dbTran = db.Database.BeginTransaction())
            {

                try
                {
                    var package = new Package()
                    {
                        Pac_Name = model.Name,
                        Pac_Price = model.PackagePrice,
                        Pac_Price_Dollar = model.PackagePrice_Dollar,
                        langId = lang
                    };
                    var cities = db.Cities.Where(x => model.Cities.Any(y => y == x.Cit_Id)).ToList();
                    package.Pac_Cities = cities;

                    db.Packages.Add(package);
                    db.SaveChanges();
                    dbTran.Commit();
                    return Json(new Respond());
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    dbTran.Rollback();
                    return Json(new Respond(ex.Message, status.unknownError));
                }
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DelPackage(int Id)
        {
            int result = db.Database.SqlQuery<int>("DeletePackage @Id", new SqlParameter("@Id", Id)).Single();
            switch (result)
            {
                case 0:
                    //UpdateLog(updatedTable.Package, Guid.Empty, true, Id);
                    return Json(new Respond());
                case 1:
                    return Json(new Respond("This package has some cities.", status.forignKeyError));
                case 2:
                    return Json(new Respond("Something went wrong. Contact devloper team.", status.dbError));
                default:
                    return Json(new Respond("Something went wrong. Contact devloper team.", status.unknownError));
            }
        }

    }
}