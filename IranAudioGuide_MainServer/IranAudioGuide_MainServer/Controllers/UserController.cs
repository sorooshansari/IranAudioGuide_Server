using IranAudioGuide_MainServer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer.Controllers
{
   [Authorize]
    public class UserController : Controller
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
        public ActionResult Packages()
        {
            return View();
        }
        [Authorize(Roles = "AppUser")]
        public ActionResult DeactivateMobile()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}