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
        AccountTools acTools = new AccountTools();
        [HttpPost]
        // POST: api/AppManager/GetUpdates/5
        public GetAllVM GetUpdates(int LastUpdateNumber)
        {
            return dbTools.GetUpdate(LastUpdateNumber);
        }
        [HttpPost]
        public JsonResult<bool> AutenticateGoogleUser(GoogleUserInfo user)
        {
            var res = acTools.CreateGoogleUser(new ApplicationUser()
            {
                Email = user.email,
                GoogleId = user.google_id,
                UserName = user.email,
                Picture = user.picture,
                FullName = user.name,
                gender = (user.gender.ToLower() == "female") ? gender.Female : (user.gender.ToLower() == "male") ? gender.Male : gender.Unknown,
                EmailConfirmed = true
            });
            return Json(res);
        }
        public async Task<IHttpActionResult> ResgisterAppUser(AppUser user)
        {
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            CreateingUserResult res = await acTools.CreateAppUser(user.email, user.password, baseUrl);
            return Json(res);
        }
        public JsonResult<Microsoft.AspNet.Identity.Owin.SignInStatus> AuthoruzeAppUser(AppUser user)
        {
            Microsoft.AspNet.Identity.Owin.SignInStatus res = acTools.AutorizeAppUser(user.email, user.password);
            return Json(res);
        }
    }
}
