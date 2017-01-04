using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;

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
                    Destination = "danialby@gmail.com",
                    Subject = model.name + " - " + model.email
                });

                return Json(new Respond());
            }
            catch (Exception ex)
            {
                return Json(new Respond(ex.Message, status.unknownError));
            }
        }
        public ActionResult test()
        {
            return View("Error");
        }
    }
}