using System;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.App_GlobalResources;
using System.Globalization;
using System.Linq;
using IranAudioGuide_MainServer.Services;

namespace IranAudioGuide_MainServer.Controllers
{
    //[Localization]
    public class HomeController : BaseController
    {
        [Compress]
        public ActionResult Index()
        {
            ViewBag.View = Views.Index;
            return View();
        }
        [Compress]
        public ActionResult Error(string aspxerrorpath)
        {
            return View();
        }


        [HttpPost]
        public JsonResult ContactEmailSender(ContactEmailVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond(Global.ServerContactEmail, status.invalidInput));
            }
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/Views/Shared/HTMLAliTemplate.html"));
                string body = sr.ReadToEnd();
                sr.Close();
                body = body.Replace("#NameFamily#", model.name);
                body = body.Replace("#Email#", model.email);
                body = body.Replace("#message#", model.message);
                body = body.Replace("#Subject#", model.subject);
                body = body.Replace("#Date#", DateTime.Now.ToString());
                body = body.Replace("#Year#", DateTime.Now.Year.ToString());
                string Time = Convert.ToString(DateTime.Now.ToShortTimeString());
                body = body.Replace("#Time#", Time);

                EmailService EmailService = new EmailService();
                EmailService.SendWithoutTemplateAsync(new Microsoft.AspNet.Identity.IdentityMessage()
                {
                    Body = body,
                    Destination = "danialby@gmail.com",
                    Subject = model.name + " - " + model.email
                });

                return Json(new Respond(Global.ServerEmailSending, status.success));
            }
            catch (Exception ex)
            {
                return Json(new Respond(Global.ServerEmailErrorSending, status.unknownError));
            }
        }

        public JsonResult SendMailTest(string dest )
        {
         
            var lang = ServiceCulture.FindGetSting("en");
            ServiceCulture.SetCulture(lang);

            var db = new ApplicationDbContext();
            var paymentId = 12;
            var returnResults = db.Payments.Include("Pay_Procurement.Pro_Package.Pac_Cities")
                                       .Where(c => c.Pay_Id == paymentId)
                                       .Select(c => new
                                       {
                                           Price = c.Pay_Amount,
                                           Name = c.Pay_Procurement.Pro_Package.Pac_Name,
                                           Cities = c.Pay_Procurement.Pro_Package.Pac_Cities.Select(x => x.Cit_Name).ToList(),
                                           Date = c.Pay_Procurement.Pro_InsertDatetime
                                       }).FirstOrDefault();




            System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/Views/Shared/UserEmailTemplatePayment.html"));

            string body = sr.ReadToEnd();
            sr.Close();
            body = body.Replace("#ReturnPaymentReferenceID#", Global.ReturnPaymentReferenceID);
            body = body.Replace("#PaymentPage27#", Global.PaymentPage27);
            body = body.Replace("#PriceTite#", Global.Price);
            body = body.Replace("#Cities#", Global.Cities);
            body = body.Replace("#ReceiptForInvoice#", Global.ReceiptForInvoice);
            body = body.Replace("#ThankYouForYourPurchase#", Global.ThankYouForYourPurchase);
            body = body.Replace("#PaymentMsg#",  Global.Paymentsuccessfully + Global.PaymentMsg5 );
            body = body.Replace("#DateTite#", Global.DateTite);
            body = body.Replace("#ReferenceID#", "9382387437");
            body = body.Replace("#PackageName#", returnResults.Name);
            body = body.Replace("#Price#", returnResults.Price.ToString());
            body = body.Replace("#PrivacyPolicy#",  Global.PrivacyPolicy);
            body = body.Replace("#TermsConditions#", Global.TermsConditions);
            body = body.Replace("#lang#", Global.Lang);


            if (Global.Lang.Contains("fa"))
            {

                var pCalendar = new System.Globalization.PersianCalendar();
                DateTime a = DateTime.Now;
                int year = pCalendar.GetYear(returnResults.Date);
                int month = pCalendar.GetMonth(returnResults.Date);
                int day = pCalendar.GetDayOfMonth(returnResults.Date);

                body = body.Replace("#Date#", year + "/" + month + "/" + day);

                body = body.Replace("#langStyle#", $"style=\"font-family: Tahoma;  direction:rtl\"");
            }
            else
            {
                body = body.Replace("#Date#", returnResults.Date.ToString());
                body = body.Replace("#langStyle#", $"style=\"font-family:'Open Sans'\"");
            }


            var listCity = "";
            foreach (var item in returnResults.Cities)
            {
                listCity = listCity + string.Format("<p style='font-size: 21px;background-color:#15437f;color:#ffffff;margin:3% auto;padding:4px 12px;box-shadow: 1px 1px 1px #15437f;font-weight:bold;display: table;min-width:201px;text-shadow: 1px 1px 2px #171514'>{0}<p>", item);
            }
            body = body.Replace("#CityItem#", listCity);

            var msg = new Microsoft.AspNet.Identity.IdentityMessage() { Body = body, Destination = dest, Subject = Global.ReceiptForInvoice };
            EmailService es = new EmailService();
            es.SendAsync2(msg);

            return Json("ok", JsonRequestBehavior.AllowGet);
        }


        //Home/PrivacyPolicy
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        //Home/PrivacyPolicy
        public ActionResult TermsConditions()
        {
            return View();
        }
    }
}