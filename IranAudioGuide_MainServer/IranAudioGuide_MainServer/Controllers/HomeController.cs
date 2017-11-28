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
                System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/Views/Shared/ContactEmailTemplate.html"));
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
                    Destination = "iranaudioguide@gmail.com",
                    Subject = model.name + " - " + model.email
                });
                return Json(new Respond(Global.ServerEmailSending, status.success));
            }
            catch (Exception ex)
            {
                return Json(new Respond(Global.ServerEmailErrorSending, status.unknownError));
            }
        }

        public JsonResult SendMailTest(string dest)
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
            body = body.Replace("#PaymentMsg#", Global.Paymentsuccessfully + Global.PaymentMsg5);
            body = body.Replace("#DateTite#", Global.DateTite);
            body = body.Replace("#ReferenceID#", "9382387437");
            body = body.Replace("#PackageName#", returnResults.Name);
            body = body.Replace("#Price#", returnResults.Price.ToString());
            body = body.Replace("#PrivacyPolicy#", Global.PrivacyPolicy);
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
        public class FCMResult
        {
            public string message_id { get; set; }
        }
        public class FCMResponse
        {
            public long multicast_id { get; set; }
            public int success { get; set; }
            public int failure { get; set; }
            public int canonical_ids { get; set; }
            public System.Collections.Generic.List<FCMResult> results { get; set; }
        }
        public ActionResult SendPushNotification()
        {

            try
            {

                string applicationID = "1:27601873693:android:5cb9dbc256a387c2";

                string serverKey = "AAAABm0zKx0:APA91bELrWPWTy4ieHpBgWGBzDKsDprA89Z1I5OhF0I-77KFYkCkZBssKStc4ri7sEFpk8BC8UbZWmCqCElew93kmAkzTLgYBIu1rwejcUBtX2EISriK2yMEXPWZJVYzNm_nectHXXA7";

                string senderId = "27601873693";

                //string appId = "1:27601873693:android:5cb9dbc256a387c2";
                string deviceId = "dIAQOYM8DqU:APA91bF2FMGGgFbDX-3xJaQHdOjwtUJpVOhnm7vpLTMuXqp73MsXyA2lzELcaWyft5ZGQL8bjyPdLDK3kQ5HxeG7EfNWh5AVV1YefJMI3w_noHyDhkytxOa-Q8jbPQP8TxWi_PqG7C5q";
                //string deviceId = "1:27601873693:android:5cb9dbc256a387c2";

                //System.Net.WebRequest tRequest = System.Net.WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                //tRequest.Method = "post";
                //tRequest.ContentType = "application/json";
                //var data = new
                //{
                //    to = deviceId,
                //    notification = new
                //    {
                //        body = "Osama",
                //        title = "AlBaami",
                //        sound = "Enabled"

                //    }
                //};

                System.Net.WebRequest tRequest = System.Net.WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var objNotification = new
                {
                    to = deviceId,
                    notification = new
                    {
                        title = "Portugal vs. Denmark",
                        body = "5 to 1",
                        sound = "Enabled"
                    },
                    data = new
                    {
                        title = "salam",
                        body = "Soroosh"
                    }
                };
                string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(objNotification);

                Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(jsonNotificationFormat);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                tRequest.ContentType = "application/json";
                using (System.IO.Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (System.Net.WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (System.IO.Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (System.IO.StreamReader tReader = new System.IO.StreamReader(dataStreamResponse))
                            {
                                String responseFromFirebaseServer = tReader.ReadToEnd();

                                FCMResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<FCMResponse>(responseFromFirebaseServer);
                                if (response.success == 1)
                                {
                                    return View();
                                }
                                else if (response.failure == 1)
                                {
                                    return View();
                                }

                            }
                        }

                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return View();
            }
        }
    }
}