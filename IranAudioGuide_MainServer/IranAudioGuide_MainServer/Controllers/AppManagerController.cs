using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

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
        // POST: api/AppManager/GetUpdates/5
        public GetUpdateVM GetUpdates(int LastUpdateNumber)
        {
            return dbTools.GetUpdate(LastUpdateNumber);
        }
        // POST: api/AppManager/GetAll
        [HttpPost]
        public GetAllVM GetAll()
        {
            return dbTools.GetAllEntries();
        }
        [HttpPost]
        public async Task<CreateingUserResult> AutenticateGoogleUser(GoogleUserInfo user)
        {
            var res = await acTools.CreateGoogleUser(new ApplicationUser()
            {
                Email = user.email,
                GoogleId = user.google_id,
                UserName = user.email,
                Picture = user.picture,
                FullName = user.name,
                gender = (user.gender.ToLower() == "female") ? gender.Female : (user.gender.ToLower() == "male") ? gender.Male : gender.Unknown,
                EmailConfirmed = true,
                uuid = user.uuid
            });
            return res;
        }
        [HttpPost]
        public async Task<IHttpActionResult> ResgisterAppUser(AppUser user)
        {
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var res = await acTools.CreateAppUser(user.email, user.password, user.uuid, baseUrl);
            return Json(res);
        }
        [HttpPost]
        public async Task<AuthorizedUser> AuthorizeAppUser(AppUser user)
        {
            var res = await acTools.AutorizeAppUser(user.email, user.password, user.uuid);
            return res;
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
