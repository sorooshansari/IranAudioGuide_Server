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

        private List<PackageVM> GetPackages()
        {
            List<PackageVM> packages = new List<PackageVM>();

            //packages =(from p in db.Packages
            //                       orderby p.Pac_Id descending
            //                       select new PackageVM()
            //                       {
            //                           PackageDesc = p.Pac_Description,
            //                           PackageId = p.Pac_Id,
            //                           PackageName = p.Pac_Name
            //                       }).ToList();

            return packages;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddPackage(NewPackage model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
            }
            //var package = new package()
            //{
            //    Pac_Name = model.PackageName, 
            //    Pac_Price = model.PackagePrice
            //};
            //using (var dbTran = db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        db.Packages.Add(package);
            //        db.UpdateLogs.Add(new UpdateLog() { Pac_ID = package.Pac_Id });
            //        db.SaveChanges();
            //        dbTran.Commit();
            //        return Json(new Respond());
            //    }
            //    catch (Exception ex)
            //    {
            //        dbTran.Rollback();
            //        return Json(new Respond(ex.Message, status.unknownError));
            //    }
            //}
            return Json(new Respond("Not implemented yet!", status.unknownError));
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