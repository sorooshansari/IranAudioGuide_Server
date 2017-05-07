using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class SellerController : Controller
    {
        // GET: Seller
        [Authorize(Roles = "Seller")]
        public ActionResult Index()
        {
            return View();
        }
    }
}