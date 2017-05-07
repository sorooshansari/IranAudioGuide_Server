using Elmah;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Models_v2;
using System;
using System.Threading.Tasks;
using System.Web.Http;

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
        public async Task<int> SendEmailConfirmedAgain(ConfirmEmailVM model)
        {
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var res = acTools.SendEmailConfirmedAgain(model.email, baseUrl);
            return await res;
        }

        [HttpPost]
        public string getBaseUrl()
        {
            return Services.GlobalPath.host;
        }

        [HttpPost]
        public IHttpActionResult GetUrl(GetAudioUrlVM model)
        {
            try
            {
                throw new Exception();
                if (string.IsNullOrEmpty(model.email) || (string.IsNullOrEmpty(model.uuid)))
                {
                    //var result = new GetAudioUrlRes("", true);
                    return BadRequest(((int)GetAudioUrlStatus.unauthorizedUser).ToString());
                }
                var isAdmin = User.IsInRole("Admin");
                var url = Services.ServiceDownload.GetUrl(model, isAdmin);
                //var result= new GetAudioUrlRes(url);
                return Ok(url);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest(((int)GetAudioUrlStatus.unknownError).ToString());
            }
        }

        [HttpPost]
        public IHttpActionResult GetAutorizedCities(GetAutorizedCitiesVM model)

        {
            var res = new AutorizedCitiesVM();
            try
            {

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
        // [Route("GetUpdates")]
        // POST: api/AppManager/GetUpdates/5
        public IHttpActionResult GetUpdates(GetUpdateInfoVm model)
        {
            try
            {

                return Ok( dbTools.GetUpdate(model.LastUpdateNumber,model.uuid));
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
                var d = dbTools.GetAllEntries(model.uuid);
               return Ok(d);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("ex", ex);
                //return BadRequest(ModelState);
              return  Content(System.Net.HttpStatusCode.BadRequest, "Any object");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPassUser user)
        {
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var res = await acTools.ForgotPassword(user.email, user.uuid, baseUrl);
            return Json(res);
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
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var res = await acTools.CreateAppUser(user.fullName, user.email, user.password, user.uuid, baseUrl);
            return Json(res);
        }
        [HttpPost]
        public async Task<AuthorizedUser> AuthorizeAppUser(AppUser user)
        {
            var res = await acTools.AutorizeAppUser(user.email, user.password, user.uuid);
            return res;
        }


        public IHttpActionResult GetCurrentUserInfo()
        {
            string userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return BadRequest();
            var user = acTools.GetUserByName(userName);
            if (user == null)
                return null;
            var userProfile = new UserProfile()
            {
                Email = user.UserName,
                FullName = user.FullName,
                imgUrl = user.ImgUrl,

            };
            userProfile.RolesName = acTools.GetUserRoles(user)[0];
            return Ok(userProfile);
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
                return "";
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return ex.Message;
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
