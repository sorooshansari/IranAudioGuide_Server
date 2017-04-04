using Elmah;
using IranAudioGuide_MainServer.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace IranAudioGuide_MainServer.Controllers
{
    public class AppManagerController : ApiController
    {
        dbTools dbTools = new dbTools();
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
        public GetAudioUrlRes GetAudioUrl(GetAudioUrlVM model)
        {
            try
            {
            //    if (model.email == null || model.email == "")
            //        model.email = "_";
                //var url = dbTools.GetAudioUrl(model);
                string path = (model.isAudio) ? Services.GlobalPath.FullPathAudios : Services.GlobalPath.FullPathStory;
                string url = string.Format("{0}{1}.mp3", path, model.trackId);
                return new GetAudioUrlRes(url);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new GetAudioUrlRes(GetAudioUrlStatus.unknownError, ex.Message);
            }
        }
        [HttpPost]
        public AutorizedCitiesVM GetAutorizedCities(GetAutorizedCitiesVM model)
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


                if (res.status == getUserStatus.confirmed)
                    res.cities = dbTools.GetAutorizedCities(user.Id);
            }
            catch (Exception ex)
            {
                res.status = getUserStatus.unknownError;
                res.errorMessage = ex.Message;
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return res;
        }
        [HttpPost]
        // POST: api/AppManager/GetPackages/5
        public GetPackagesVM GetPackages(int cityId)
        {
            return dbTools.GetPackagesByCity(cityId);
        }
        [HttpPost]
        // POST: api/AppManager/GetUpdates/5
        public GetUpdateVM GetUpdates(int LastUpdateNumber, string uuid)
        {
            GetUpdateVM res;
            try
            {
                res = dbTools.GetUpdate(LastUpdateNumber, uuid);
            }
            catch (Exception ex)
            {
                res = new GetUpdateVM(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return res;
        }
        // POST: api/AppManager/GetAll
        [HttpPost]
        public GetAllVM GetAll(string uuid)
        {
            GetAllVM res;
            try
            {
                res = dbTools.GetAllEntries(uuid);
            }
            catch (Exception ex)
            {
                res = new GetAllVM(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return res;
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
