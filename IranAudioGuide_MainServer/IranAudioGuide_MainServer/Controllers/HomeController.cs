using System;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.App_GlobalResources;

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
            System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/Views/Shared/HTMLPage3.html"));

            string body = sr.ReadToEnd();
            var msg = new Microsoft.AspNet.Identity.IdentityMessage() { Body = body, Destination = dest, Subject = "salaam" };
            EmailService es = new EmailService();
            es.SendWithoutTemplateAsync(msg);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        
    }
}