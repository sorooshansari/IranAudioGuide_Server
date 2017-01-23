using IranAudioGuide_MainServer.Models;
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
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: User
        [Authorize(Roles = "AppUser")]
        public ActionResult Index()
        {
            ViewBag.View = Views.UserIndex;
            return View(GetCurrentUserInfo());
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

                throw ex;
            }
        }

        public JsonResult GetPackages()
        {
            List<PackageVM> packages = new List<PackageVM>();

            packages = (from p in db.Packages
                        orderby p.Pac_Id descending
                        select new PackageVM()
                        {
                            PackagePrice = p.Pac_Price,
                            PackageId = p.Pac_Id,
                            PackageName = p.Pac_Name,
                            PackageCities = (from c in db.Cities
                                             select new CityVM()
                                             {
                                                 CityName = c.Cit_Name,
                                                 CityDesc = c.Cit_Description,
                                                 CityID = c.Cit_Id,
                                                 CityImageUrl = c.Cit_ImageUrl
                                             }).ToList()
                        }).ToList();
            return Json(packages);
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