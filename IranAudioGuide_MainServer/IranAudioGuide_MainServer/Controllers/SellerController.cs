using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Services;



namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class SellerController : Controller
    {
        // GET: Seller
        [Authorize(Roles = "Seller")]
        public ActionResult Index(bool isError = false)
        {
            ViewBag.isError = isError;
            return View();
        }
        [Authorize(Roles = "Seller")]
        public ActionResult CreatNewBarcode(CreatBarCodeVM model) {
            if (!ModelState.IsValid)
            {
                ViewBag.View = Views.Seller;
                return View(model);
            }
            bool res = false;
            string name = User.Identity.Name;
            using (var BarService = new BarcodeServices())
            {
                res = BarService.Creatbarcode(model.price, model.quantity, name);
            }
            return RedirectToAction("Index", "Seller", res);
        }


    }
}