using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var lang = filterContext.RequestContext.HttpContext.Request["lang"] == null ? null : filterContext.RequestContext.HttpContext.Request["lang"].ToString();
            if (!string.IsNullOrEmpty(lang))

            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            }
            else
            {
                // load the culture info from the cookie
                var cookie = filterContext.HttpContext.Request.Cookies["IranAudioGuide.Models.CurrentUICulture"];
                var langHeader = string.Empty;
                if (cookie != null)
                {
                    // set the culture by the cookie content
                    langHeader = cookie.Value;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                else
                {
                    // set the culture by the location if not speicified
                    langHeader = filterContext.HttpContext.Request.UserLanguages[0];
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                // set the lang value into route data
                filterContext.RouteData.Values["lang"] = langHeader;
            }

            // save the location into cookie
            HttpCookie _cookie = new HttpCookie("IranAudioGuide.Models.CurrentUICulture", Thread.CurrentThread.CurrentUICulture.Name);
            _cookie.Expires = DateTime.Now.AddYears(1);
            filterContext.HttpContext.Response.SetCookie(_cookie);

            base.OnActionExecuting(filterContext);
        }
    }
}