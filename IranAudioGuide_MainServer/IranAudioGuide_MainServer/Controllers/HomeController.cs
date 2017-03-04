using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace IranAudioGuide_MainServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.View = Views.Index;
            return View();
        }

        [HttpPost]
        public JsonResult ContactEmailSender(ContactEmailVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Respond("Check input fields", status.invalidInput));
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
                EmailService.SendAsync(new Microsoft.AspNet.Identity.IdentityMessage()
                {
                    Body = body,
                    Destination = "iranaudioguide@gmail.com",
                    Subject = model.name + " - " + model.email
                });

                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }

        //public ActionResult Error()
        //{
        //    return View("Error");
        //}
        //public ActionResult CheckOut()
        //{
        //    return View("CheckOut");
        //}
        //public ActionResult PaymentRef()
        //{
        //    return View("PaymentRef");
        //}
        //public ActionResult PurchaseDetails()
        //{
        //    return View("PurchaseDetails");
        //}
        //public ActionResult test()
        //{
        //    var url = "https://merchant.wmtransfer.com/lmi/payment.asp?at=authtype_2";

        //    Response.Clear();
        //    var sb = new System.Text.StringBuilder();
        //    sb.Append("<html>");
        //    sb.AppendFormat("<body onload='document.forms[0].submit()'>");
        //    sb.AppendFormat("<form action='{0}' method='post'>", url);
        //    sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_NO' value='{0}'>", 3);
        //    sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_AMOUNT' value='{0}'>", "0.05");
        //    sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_DESC' value='{0}'>", "salam");
        //    sb.AppendFormat("<input type='hidden' name='LMI_PAYEE_PURSE' value='{0}'>", "Z945718891756");
        //    sb.AppendFormat("<input type='hidden' name='LMI_SIM_MODE' value='{0}'>", 0);
        //    sb.Append("</form>");
        //    sb.Append("</body>");
        //    sb.Append("</html>");
        //    Response.Write(sb.ToString());
        //    Response.End();

        //    return RedirectToAction("index");
        //}
        //public ActionResult test()
        //{
            //var WMTools = new WebMoneyTools();
            //bool res = WMTools.ControlSignature(new ControlSignatureVM()
            //{
            //    LMI_HASH2 = "8394AB2B012536D8535D3E1CB5917AB31E91EE891898B3AADD1FA887C9B2A0C2",
            //    LMI_MODE = "1",
            //    LMI_PAYEE_PURSE = "Z945718891756",
            //    LMI_PAYER_PURSE = "Z945718891756",
            //    LMI_PAYER_WM = "059551878952",
            //    LMI_PAYMENT_AMOUNT = "0.05",
            //    LMI_PAYMENT_NO = "3",
            //    LMI_SECRET_KEY = "AD15BDAF-864A-4909-81E7-F034FFD0625C",
            //    LMI_SYS_INVS_NO = "821",
            //    LMI_SYS_TRANS_DATE = "20170221 20:25:01",
            //    LMI_SYS_TRANS_NO = "458"
            //});


            //var sb = new System.Text.StringBuilder();
            //sb.Append("Z945718891756");
            //sb.Append("0.05");
            //sb.Append("3");
            //sb.Append("1");
            //sb.Append("821");
            //sb.Append("458");
            //sb.Append("20170221 20:25:01");
            //sb.Append("AD15BDAF-864A-4909-81E7-F034FFD0625C");
            //sb.Append("Z945718891756");
            //sb.Append("059551878952");
            //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

            //var sha256 = SHA256.Create();
            //byte[] hashBytes = sha256.ComputeHash(bytes);

            //string res = HexStringFromBytes(hashBytes);
            

            //var res2 = res.ToLower() == "8FE265B681239AD127FE6A0B552D8737DFAA1C275C67E8B9189A56C0AE68126A".ToLower();
            //Dictionary<string, string> Res = new Dictionary<string, string>();
            //Res.Add("res", res);
            //Res.Add("res2", res2.ToString());
            //return Json(res, JsonRequestBehavior.AllowGet);
        //}
        //public static string HexStringFromBytes(byte[] bytes)
        //{
        //    var sb = new System.Text.StringBuilder();
        //    foreach (byte b in bytes)
        //    {
        //        var hex = b.ToString("x2");
        //        sb.Append(hex);
        //    }
        //    return sb.ToString();
        //}
    }
}