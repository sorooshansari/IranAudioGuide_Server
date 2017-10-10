using Elmah;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Models_v2;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using IranAudioGuide_MainServer.Services;
using System.Linq;

namespace IranAudioGuide_MainServer.Controllers
{
    [RoutePrefix("api/AppManagerV2")]
    public class AppManagerV2Controller : ApiController
    {
        dbToolsV2 dbTools = new dbToolsV2();
        private AccountTools _acTools;
        public AccountTools acTools
        {
            get
            {
                return _acTools ?? new AccountTools();
            }
            private set
            {
                _acTools = value;
            }
        }


        [HttpPost]
        public string getBaseUrl(GetVersoinVm version)
        {
            return GlobalPath.host;
        }

        [HttpPost]
        public IHttpActionResult GetAudioUrl(GetAudioUrlVM model)
        {

            string FullSource;
            if (model.isAudio)
            {

                FullSource = GlobalPath.PrimaryPathAudios;
                //pathDestination = GlobalPath.DownloadPathAudios;
            }
            else
            {
                FullSource = GlobalPath.PrimaryPathStory;
                //pathDestination = GlobalPath.DownloadPathStory;

            }
            var url = string.Format("{0}{1}{2}.mp3", GlobalPath.host, FullSource, model.trackId);
            return Ok(url);
        }
        //[HttpPost]
        //        public IHttpActionResult GetAudioUrl(GetAudioUrlVM model)
        //        {
        //            try
        //            {
        //                if (string.IsNullOrEmpty(model.uuid))
        //                {
        //                    return BadRequest(((int)GetAudioUrlStatus.unauthorizedUser).ToString());
        //                }

        //                if (string.IsNullOrEmpty(model.email))
        //                    model.email = string.Empty;

        //                var isAdmin = User.IsInRole("Admin");
        //                var url = ServiceDownload.GetUrl(model, isAdmin);
        //                //var result= new GetAudioUrlRes(url);

        //                return Ok(url);
        //            }
        //            catch (Exception ex)
        //            {
        //                ErrorSignal.FromCurrentContext().Raise(ex);
        //                return BadRequest(((int)GetAudioUrlStatus.unknownError).ToString());
        //            }
        //        }
        [HttpPost]
        public IHttpActionResult GetAutorizedCities(InfoUser model)

        {
            var res = new AutorizedCitiesVM();
            try
            {
                if (!ModelState.IsValid)
                {
                    res.status = getUserStatus.notUser;
                    ModelState.AddModelError("error", ((int)res.status).ToString());
                    return BadRequest(ModelState);
                }

                var user = acTools.getUser(model.username);
                res.status =
                    (user == null) ? getUserStatus.notUser :
                    (user.uuid != model.uuid) ? getUserStatus.uuidMissMatch :
                    (!user.EmailConfirmed) ? getUserStatus.notConfirmed :
                    getUserStatus.confirmed;


                if (res.status != getUserStatus.confirmed)
                {
                    ModelState.AddModelError("error", ((int)res.status).ToString());
                    return BadRequest(ModelState);
                }
                res.cities = dbTools.GetAutorizedCities(user.Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ((int)getUserStatus.unknownError).ToString());
                ModelState.AddModelError("ex", ex);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        public IHttpActionResult GetAutorizedPlaces(InfoUser model)
        {
            var res = new AutorizedPlacesVM();
            try
            {
                if (!ModelState.IsValid)
                {
                    res.status = getUserStatus.notUser;
                    ModelState.AddModelError("error", ((int)res.status).ToString());
                    res.errorMessage = res.status.ToString();
                    return BadRequest(ModelState);
                }

                var user = acTools.getUser(model.username);
                res.status =
                    (user == null) ? getUserStatus.notUser :
                    (user.uuid != model.uuid) ? getUserStatus.uuidMissMatch :
                    (!user.EmailConfirmed) ? getUserStatus.notConfirmed :
                    getUserStatus.confirmed;
                res.errorMessage = res.status.ToString();


                if (res.status != getUserStatus.confirmed)
                {
                    ModelState.AddModelError("error", ((int)res.status).ToString());
                    return BadRequest(ModelState);
                }
                res.places = dbTools.GetAutorizedPlaces(user.Id);
                if (res.places == null)
                {
                    res.errorMessage = getUserStatus.unknownError.ToString();

                    ModelState.AddModelError("error", ((int)getUserStatus.unknownError).ToString());
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ((int)getUserStatus.unknownError).ToString());
                ModelState.AddModelError("errorMsg", (getUserStatus.unknownError.ToString()).ToString());
                ModelState.AddModelError("ex", ex);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest(ModelState);
            }
        }
        [HttpGet]
        public IHttpActionResult IsForign()
        {
            var res = ExtensionMethods.IsForeign;
            return Ok(res);
        }

        [HttpPost]
        // POST: api/AppManager/GetPackages/5
        public IHttpActionResult GetPackages(GetPackagesByLangVM model)
        {
            try
            {
                var s = dbTools.GetPackagesByCity(model.CityId, model.LangId);
                if (!string.IsNullOrEmpty(s.errorMessage))
                    return BadRequest(s.errorMessage);
                return Ok(s);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);
                return BadRequest(ModelState);
            }
        }


        [HttpPost]
        // POST: api/AppManagerV2/GetAllPackages
        public IHttpActionResult GetAllPackages()
        {
            try
            {
                var s = dbTools.GetAllPackages();
                if (!string.IsNullOrEmpty(s.errorMessage))
                    return BadRequest(s.errorMessage);
                return Ok(s);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);
                return BadRequest(ModelState);
            }
        }

        //[HttpPost]
        // POST: api/AppManagerV2/GetAllPlaces
        //public IHttpActionResult GetAllPlaces()
        //{
        //    try
        //    {
        //        var list = db.TranslatePlaces.Select(x => new
        //        {
        //            LangId = x.langId,
        //            PlaceId = x.Pla_Id,
        //            Price = x.TrP_Price,
        //            PriceDollar = x.TrP_PriceDollar
        //        }).ToList();

        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("ex", ex);
        //        return BadRequest(ModelState);
        //    }
        //}
        [HttpPost]
        // POST: api/AppManagerV2/GetAllPricePlaces
        public IHttpActionResult GetAllPricePlaces()
        {
            var list = dbTools.GetAllPricePlaces();
            if (list == null)
                return BadRequest(App_GlobalResources.Global.ErrorUnknown);
            return Ok(list);
        }
        [HttpPost]
        // [Route("GetUpdates")]
        // POST: api/AppManager/GetUpdates/5
        public IHttpActionResult GetUpdates(GetUpdateInfoVm model)
        {
            try
            {
                if (ModelState.IsValid)
                    return Ok(dbTools.GetUpdate(model));
                ModelState.AddModelError("error", "Information received is not valid ");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("ex", ex);
                return BadRequest(ModelState);
            }
        }
        public IHttpActionResult GetAllEntitiesRemoved()
        {
            try
            {

                return Ok(dbTools.GetAllEntitiesRemoved());
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("ex", ex);
                return BadRequest(ModelState);
            }
        }
        // POST: api/AppManager/GetAll

        [HttpPost]
        public IHttpActionResult GetAll(GetUpdateInfoVm model)
        {
            try
            {
                return Ok(dbTools.GetAllEntries(model.uuid));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("ex", ex);
                return Content(System.Net.HttpStatusCode.BadRequest, "Any object");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPassUser user)
        {
            var lang = ServiceCulture.FindGetSting(user.lang);
            ServiceCulture.SetCulture(lang);
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/" + lang + "/";
            var res = await acTools.ForgotPassword(user.email, user.uuid, baseUrl);
            return Json(res);
        }

        [HttpPost]
        public string CreateComment(CommentVm comment)
        {
            try
            {
                var newComment = new Comment()
                {
                    Message = comment.Message,
                    uuid = comment.uuid,
                    Subject = "",
                    Email = comment.email
                };
                dbTools.CreateComment(newComment);
                var email = new EmailService();

                var msg = new Microsoft.AspNet.Identity.IdentityMessage()
                {
                    Body = "Comment From :: " + comment.email + "<br />" + comment.Message,
                    Destination = "iranaudioguide@gmail.com",
                    Subject = "Comment From" + comment.email
                };
                email.SendWithoutTemplateAsync(msg);
                return "";
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return ex.Message;
            }
        }
        [HttpPost]
        public async Task<CreatingUserResult> AutenticateGoogleUser(GoogleUserInfo user)
        {
            try
            {
                var res = await acTools.CreateGoogleUser(new ApplicationUser()
                {
                    Email = user.email,
                    GoogleId = user.google_id,
                    UserName = user.email,
                    Picture = user.picture,
                    FullName = user.name,
                    EmailConfirmed = true,
                    uuid = user.uuid
                });
                return res;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return CreatingUserResult.fail;
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ResgisterAppUser(AppUser user)
        {
            var lang = ServiceCulture.FindGetSting(user.lang);
            ServiceCulture.SetCulture(lang);
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/" + lang + "/";
            var res = await acTools.CreateAppUser(user.fullName, user.email, user.password, user.uuid, baseUrl);
            return Json(res);
        }


        [HttpPost]
        public async Task<int> SendEmailConfirmedAgain(ConfirmEmailVM model)
        {
            var lang = ServiceCulture.FindGetSting(model.lang);
            ServiceCulture.SetCulture(lang);
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/" + lang + "/";
            var res = acTools.SendEmailConfirmedAgain(model.email, baseUrl);
            return await res;
        }

        [HttpPost]
        public async Task<AuthorizedUser> AuthorizeAppUser(AppUser user)
        {
            return await acTools.AutorizeAppUser(user.email, user.password, user.uuid);
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpPost]
        //public BuyWithBarcodeStatus BuyWithBarcode(Guid packId, string email, string uuid, string barcode)
        //{
        public BuyWithBarcodeStatus BuyWithBarcode(BuyWithBarcodeVM model)
        {
            try
            {
                var user = acTools.getUser(model.email);
                var status =
                    (user == null) ? BuyWithBarcodeStatus.notUser :
                    (user.uuid != model.uuid) ? BuyWithBarcodeStatus.uuidMissMatch :
                    (!user.EmailConfirmed) ? BuyWithBarcodeStatus.notConfirmed :
                    BuyWithBarcodeStatus.confirmed;
                if (status != BuyWithBarcodeStatus.confirmed)
                {
                    return status;
                }
                ConvertBarcodetoStringVM cbs = new ConvertBarcodetoStringVM();
                using (BarcodeServices brs = new BarcodeServices())
                {
                    try
                    {
                        cbs = brs.ConvertBarcodetoString(model.barcode);
                    }
                    catch (Exception)
                    {
                        var ex = new Exception(string.Format("invalid baicode-->{0}", model.barcode));
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        return BuyWithBarcodeStatus.invalidBarcode;
                    }
                    long packPrice;
                    BarcodeVM bav = new BarcodeVM();
                    packPrice = brs.Getpackage(model.packId);
                    bav = brs.GetBarcodes(cbs.CBS_id_bar);
                    if (cbs.CBS_price_pri != bav.price)
                    {
                        return BuyWithBarcodeStatus.invalidprice;
                    }
                    if (cbs.CBS_sellername != bav.sellerName)
                    {
                        return BuyWithBarcodeStatus.invalidSellerName;
                    }
                    try
                    {
                        if (bav.price != packPrice)
                        {
                            return BuyWithBarcodeStatus.invalidpackprice;
                        }
                        if (bav.isUsed == true)
                        {
                            return BuyWithBarcodeStatus.isused_true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        return BuyWithBarcodeStatus.unknownError;
                    }
                    brs.saved(cbs.CBS_id_bar, user.Id, model.packId);
                    return BuyWithBarcodeStatus.success;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BuyWithBarcodeStatus.unknownError;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_acTools != null)
                {
                    _acTools.Dispose();
                    _acTools = null;
                }
            }

            base.Dispose(disposing);
        }

    }
}
