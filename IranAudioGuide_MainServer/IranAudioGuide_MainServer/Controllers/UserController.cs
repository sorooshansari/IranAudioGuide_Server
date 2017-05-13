using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    //[Localization]

    public class UserController: Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        // GET: User
        [Authorize(Roles = "AppUser")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "AppUser")]
        public ActionResult UserProfile()
        {
            return View();
        }

        [Authorize(Roles = "AppUser")]
        public ActionResult PackagesPurchased()
        {
            return View();
        }
        [Authorize(Roles = "AppUser")]
        //[LogActionFilter]
        public ActionResult Packages()
        {
            return View();
        }
        [Authorize(Roles = "AppUser")]
        //[LogActionFilter]
        public ActionResult DeactivateMobile()
        {
            return View();
        }

        public ActionResult LogOff()
        {
          //  var AutheticationManager = HttpContext.GetOwinContext().Authentication;
           // AuthenticationManager.SignOut();
           AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}