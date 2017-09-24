using IranAudioGuide_MainServer.App_GlobalResources;
using IranAudioGuide_MainServer.Models_v2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer.Controllers
{
    public abstract class BaseController : Controller
    {

       
        public int GetlangFromCookie
        {
            get
            {
                return ServiceCulture.GeLangFromCookie();
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
           
            ServiceCulture.SetCulture();
            return base.BeginExecuteCore(callback, state);
        }

        public void SendEmailForPurcument(SendEmailForPaymentVM Message)
        {
            try
            {
                var sr = new System.IO.StreamReader(Server.MapPath("~/Views/Shared/UserEmailTemplatePayment.html"));
                var body = sr.ReadToEnd();
                sr.Close();
                body = body.Replace("#ReturnPaymentReferenceID#", Global.ReturnPaymentReferenceID);
                body = body.Replace("#PaymentPage27#", Global.PaymentPage27);
                body = body.Replace("#PriceTite#", Global.Price);
                body = body.Replace("#Cities#", Global.Cities);
                body = body.Replace("#ReceiptForInvoice#", Global.ReceiptForInvoice);
                body = body.Replace("#ThankYouForYourPurchase#", Global.ThankYouForYourPurchase);
                body = body.Replace("#PaymentMsg#", Global.Paymentsuccessfully + Global.PaymentMsg5);
                body = body.Replace("#DateTite#", Global.DateTite);
                body = body.Replace("#ReferenceID#", Message.ReferenceID);
                body = body.Replace("#PackageName#", Message.Pac_Name);
                body = body.Replace("#Price#", Message.Price);
                body = body.Replace("#PrivacyPolicy#", Global.PrivacyPolicy);
                body = body.Replace("#TermsConditions#", Global.TermsConditions);
                body = body.Replace("#lang#", Global.Lang);
                if (Global.Lang.Contains("fa"))
                {

                    var pCalendar = new System.Globalization.PersianCalendar();
                    DateTime a = DateTime.Now;
                    int year = pCalendar.GetYear(Message.Date);
                    int month = pCalendar.GetMonth(Message.Date);
                    int day = pCalendar.GetDayOfMonth(Message.Date);

                    body = body.Replace("#Date#", year + "/" + month + "/" + day);
                    body = body.Replace("#langStyle#", $"style=\"font-family: Tahoma;  direction:rtl\"");
                }
                else
                {
                    body = body.Replace("#Date#", Message.Date.ToString());
                    body = body.Replace("#langStyle#", $"style=\"font-family:'Open Sans'\"");
                }


                var listCity = "";
                foreach (var item in Message.Cities)
                {
                    listCity = listCity + string.Format("<p style='font-size: 21px;background-color:#15437f;color:#ffffff;margin:3% auto;padding:4px 12px;box-shadow: 1px 1px 1px #15437f;font-weight:bold;display: table;min-width:201px;text-shadow: 1px 1px 2px #171514'>{0}<p>", item);
                }
                body = body.Replace("#CityItem#", listCity);

                Message.Body = body;
                Message.Subject = Global.ReceiptForInvoice;
                Message.Destination = User.Identity.Name;
                EmailService es = new EmailService();
                es.SendEmail(Message);

            }
            catch
            {

            }
        }
    }
}