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
        //private const string storagePrefix = "http://iranaudioguide.com/";
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
                        Place = db.Places.Where(x => x.Pla_Id == model.PlaceId).First()
                    };
                    db.Tips.Add(newTip);
                    //db.SaveChanges();
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
                List<TipVM> res = (from t in db.Tips
                                   where t.Place.Pla_Id == placeId
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
                            Place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault()
                        };
                        db.Storys.Add(Story);
                        db.SaveChanges();

                        //Save Story and generate Sto_Id
                        //string id = Convert.ToString(Story.Sto_Id);
                        //string extention = Path.GetExtension(model.StoryFile.FileName);
                        //string path = string.Format("~/Stories/{0}{1}", id, extention);
                        //model.StoryFile.SaveAs(Server.MapPath(path));
                        //Story.Sto_Url = string.Format("{0}{1}", id, extention);

                        //uplaod file
                        var request = new ServiceFtp();
                        var fileName = Convert.ToString(Story.Sto_Id) + Path.GetExtension(model.StoryFile.FileName);
                        var isSuccess = request.Upload(model.StoryFile, fileName, GlobalPath.PathStory);

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
                        if (request.IsDirectoryExist(fileName, GlobalPath.PathStory))
                        {
                            request.delete(fileName, GlobalPath.PathStory);
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
                            Place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault()
                        };
                        db.Audios.Add(audio);
                        db.SaveChanges();

                        //Save audio and generate Aud_Id
                        //string id = Convert.ToString(audio.Aud_Id);
                        //string extention = Path.GetExtension(model.AudioFile.FileName);
                        //string path = string.Format("~/Audios/{0}{1}", id, extention);
                        //model.AudioFile.SaveAs(Server.MapPath(path));
                        //audio.Aud_Url = string.Format("{0}{1}", id, extention);


                        //uplaod file
                        var request = new ServiceFtp();
                        var fileName = Convert.ToString(audio.Aud_Id) + Path.GetExtension(model.AudioFile.FileName);
                        var isSuccess = request.Upload(model.AudioFile, fileName, GlobalPath.PathAudios);
                        if (!isSuccess)
                        {
                            request.createDirectory(GlobalPath.PathAudios);
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
                        if (request.IsDirectoryExist(fileName, GlobalPath.PathAudios))
                        {
                            request.delete(fileName, GlobalPath.PathAudios);
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
                        db.SaveChanges();

                        //uplaod file
                        var request = new ServiceFtp();
                        var fileName = Convert.ToString(place.Pla_Id) + Path.GetExtension(model.Image.FileName);
                        //end upload file

                        place.Pla_ImgUrl = fileName;
                        place.Pla_TumbImgUrl = fileName;

                        UpdateLog(updatedTable.Place, place.Pla_Id);
                        db.SaveChanges();
                        dbTran.Commit();
                        var isSuccess = request.Upload(model.Image, fileName, GlobalPath.PathImagePlace);
                        if (!isSuccess)
                            throw new ArgumentException("Dont save image in Server", "original");


                        var PathSource = GlobalPath.PathImagePlace + "/" + fileName;
                        var Destination = GlobalPath.PathImageTumbnail + "/" + fileName;
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


                                var request = new ServiceFtp();
                                var fileName = place.Pla_ImgUrl;
                                lock (DelPlc)
                                {
                                    if (request.IsDirectoryExist(fileName, GlobalPath.PathImagePlace))
                                    {
                                        request.delete(fileName, GlobalPath.PathImagePlace); ;
                                    }
                                    fileName = place.Pla_TumbImgUrl;
                                    if (request.IsDirectoryExist(fileName, GlobalPath.PathImageTumbnail))
                                    {
                                        request.delete(fileName, GlobalPath.PathImageTumbnail);
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
                        ErrorSignal.FromCurrentContext().Raise(ex);
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

            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
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
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new Respond(ex.Message, status.unknownError));
            }
           // return Json(new Respond());
            Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            return Json(place);
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
                            request.delete(place.Pla_ImgUrl, GlobalPath.PathImageTumbnail);
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
                    var isSuccess = request.Upload(model.NewImage, fileName, GlobalPath.PathImageTumbnail);
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
                            request.delete(place.Pla_TumbImgUrl, GlobalPath.PathImagePlace);
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
                    var isSuccess = request.Upload(model.NewImage, fileName, GlobalPath.PathImagePlace);
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
                    var getPlace = db.Places.Include("Pla_ExtraImages").Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();

                    int o = getPlace.Pla_ExtraImages.Max(x => x.Order) + 1;
                    var img = new Image() { Place = getPlace , Order = o };
                    var place = db.Places.Where(x => x.Pla_Id == model.PlaceId).FirstOrDefault();
                    db.Images.Add(img);
                    db.SaveChanges();

                    //uplaod file
                    var request = new ServiceFtp();
                    var fileName = Convert.ToString(img.Img_Id) + Path.GetExtension(model.NewImage.FileName);
                    var isSuccess = request.Upload(model.NewImage, fileName, GlobalPath.PathImageExtras);
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
                        var img = db.Images.Include("Place").Where(x => x.Img_Id == imgId).FirstOrDefault();
                        if (img == default(Image))
                        {
                            return Json(new Respond("Invalid Image Id", status.invalidId));
                        }
                        UpdateLog(updatedTable.ExtraImage, img.Img_Id, true);


                        db.Images.Remove(img);

                        db.SaveChanges();
                        var request = new ServiceFtp();
                        var fileName = img.Img_Name;
                        if (request.IsDirectoryExist(fileName, GlobalPath.PathImageExtras))
                        {
                            request.delete(fileName, GlobalPath.PathImageExtras); ;
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
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetExtraImages(Guid placeId)
        {
            var img = (from i in db.Images
                       where i.Place == db.Places.Where(x => x.Pla_Id == placeId).FirstOrDefault()
                       select new ImageVM()
                       {
                           ImageId = i.Img_Id,
                           ImageName = i.Img_Name,
                           ImageDesc = i.Img_Description,
                           Index = i.Order
                       }).ToList();
         //   int counter = 0;
            //foreach (var i in img)

            //    i.Index = counter++;

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
            var city = new city() { Cit_Name = model.CityName, Cit_Description = model.CityDesc };
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {

                    db.Cities.Add(city);
                    db.SaveChanges();

                    //uplaod file
                    var request = new ServiceFtp();
                    var fileName = Convert.ToString(city.Cit_Id) + Path.GetExtension(model.CityImage.FileName);
                    var isSuccess = request.Upload(model.CityImage, fileName, GlobalPath.PathImageCity);
                    //end upload file

                    city.Cit_ImageUrl = fileName;

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
            var city = db.Cities.Where(x => x.Cit_Id == model.CityID).FirstOrDefault();
            if (city == default(city))
            {
                return Json(new Respond("Invalid Place Id", status.invalidId));
            }
            city.Cit_Name = model.CityName;
            city.Cit_Description = model.CityDesc;

            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
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
                        request.delete(city.Cit_ImageUrl, GlobalPath.PathImageCity);

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

                    var isSuccess = request.Upload(model.NewImage, fileName, GlobalPath.PathImageCity);
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
                                if (request.IsDirectoryExist(fileName, GlobalPath.PathImageCity))
                                {
                                    request.delete(fileName, GlobalPath.PathImageCity); ;
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
                return Places.AsEnumerable().Reverse().ToList();
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
            List<StoryVM> Storys = (from a in db.Storys
                                    where a.Place == db.Places.Where(x => x.Pla_Id.ToString() == PlaceId).FirstOrDefault()
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
                                    where a.Place == db.Places.Where(x => x.Pla_Id.ToString() == PlaceId).FirstOrDefault()
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
                                       CityName = c.Cit_Name,
                                       CityImageUrl = c.Cit_ImageUrl
                                   }).ToList();
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
            List<PackageVM> packages =
                (from p in db.Packages
                 orderby p.Pac_Id descending
                 select new PackageVM()
                 {
                     PackageName = p.Pac_Name,
                     PackageId = p.Pac_Id,
                     PackagePrice = p.Pac_Price,
                     PackageCities =
                     (from c in db.Cities
                      where
                      (from pc in p.Pac_Cities select pc.Cit_Id).Contains(c.Cit_Id)
                      select new CityVM()
                      {
                          CityDesc = c.Cit_Description,
                          CityID = c.Cit_Id,
                          CityName = c.Cit_Name
                      }).ToList()
                 }).ToList();
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
                        Pac_Name = model.PackageName,
                        Pac_Price = model.PackagePrice,
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