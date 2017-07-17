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
            List<SelectListItem> prices = new List<SelectListItem>();
            using (var service = new BarcodeServices())
            {
                prices = service.getPrices();
            }
            return View(new CreatBarCodeVM() { prices = prices });
        }
        [Authorize(Roles = "Seller")]
        public ActionResult CreatNewBarcode(CreatBarCodeVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.View = Views.Seller;
                return View(model);
            }
            bool res = false;
            string name = User.Identity.Name;
            using (var BarService = new BarcodeServices())
            {
                res = BarService.Creatbarcode(model.PriceId, model.quantity, name);
            }
            return RedirectToAction("Index", "Seller", res);
        }
        // GET: Seller
        [Authorize(Roles = "Seller")]
        public ActionResult DownloadPDF()
        {
            List<SelectListItem> prices = new List<SelectListItem>();
            using (var service = new BarcodeServices())
            {
                prices = service.getPrices();
            }
            return View(new Pageing(){ prices = prices });
        }
        public ActionResult GetPDF(Pageing page)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.View = Views.Seller;
                return View(ModelState);
            }
            var model = new GeneratePDFModel();
            BarcodeServices br = new BarcodeServices();
            model = br.DownloadPDF1(page);
            return new Rotativa.ViewAsPdf("GeneratePDF", model) { FileName = "firstPdf.pdf" };
        }
    }
}