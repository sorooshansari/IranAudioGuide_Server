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
                res = BarService.Creatbarcode(model.PriceId, model.quantity, name);
            }
            return RedirectToAction("Index", "Seller", res);
        }
        public ActionResult DownloadPDF()
        {
                var model = new GeneratePDFModel();
                BarcodeServices br = new BarcodeServices();
                model = br.DownloadPDF1();
            


            //try
            //{
            //    var model = new GeneratePDFModel();

            //    //get the information to display in pdf from database
            //    //for the time
            //    //Hard coding values are here, these are the content to display in pdf 
            //    var content = "<h2>WOW Rotativa<h2>" +
            //     "<p>Ohh This is very easy to generate pdf using Rotativa <p>";
            //    var logoFile = @"/Images/as.png";

            //    model.PDFContent = content;
            //    model.PDFLogo = Server.MapPath(logoFile);

            //    //Use ViewAsPdf Class to generate pdf using GeneratePDF.cshtml view
            //    return new Rotativa.ViewAsPdf("GeneratePDF", model) { FileName = "firstPdf.pdf" };
            //}

            //catch (Exception ex)
            //{

            //    throw;
            //}
            return new Rotativa.ViewAsPdf("GeneratePDF", model) { FileName = "firstPdf.pdf" };
        }





    }
}