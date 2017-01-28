using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class UserApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public IHttpActionResult GetCurrentUserInfo()
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

                return Ok(Info);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        // GET: User
        [Authorize(Roles = "AppUser")]
        public IHttpActionResult GetPackages()
        {
            try
            {
                string userName = User.Identity.Name;
                var list = db.Payments.Include("User").Include("Package").Include("Package.Pac_Cities")
                       .Where(x => x.User.UserName == userName)
                       .Where(x => x.PaymentFinished == true)
                       .Select(p => p.Package).Select(p => new PackageVM()
                       {
                           PackagePrice = p.Pac_Price,
                           PackageId = p.Pac_Id,
                           PackageName = p.Pac_Name,
                           PackageCities = p.Pac_Cities.Select(c => new CityVM()
                           {
                               CityName = c.Cit_Name,
                               CityDesc = c.Cit_Description,
                               CityID = c.Cit_Id,
                               CityImageUrl = c.Cit_ImageUrl,
                               Places = (from pl in db.Places
                                         where pl.Pla_city.Cit_Id == c.Cit_Id
                                         select new PlaceVM()
                                         {
                                             PlaceName = pl.Pla_Name,
                                             PlaceId = pl.Pla_Id,
                                             CityName = pl.Pla_Name,
                                             PlaceAddress = pl.Pla_Address,
                                             PlaceDesc = pl.Pla_Discription,
                                             ImgUrl = pl.Pla_ImgUrl,

                                         }).ToList()
                           }).ToList()
                       }).ToList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IHttpActionResult DeactivateMobile()
        {

            return Ok();
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
