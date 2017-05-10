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
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
             ServiceCulture.SetCulture();    
            return base.BeginExecuteCore(callback, state);
        }
        //protected override bool DisableAsyncSupport
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        //protected override void ExecuteCore()
        //{
        //     string isAllowed = Request.QueryString["lang"];
        //    if (RouteData.Values["lang"] != null &&
        //        !string.IsNullOrWhiteSpace(RouteData.Values["lang"].ToString()))
        //    {
        //        // set the culture from the route data (url)
        //        var lang = RouteData.Values["lang"].ToString();
        //        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
        //    }
        //    else
        //    {
        //        // load the culture info from the cookie
        //        var cookie = HttpContext.Request.Cookies["MvcLocalization.CurrentUICulture"];
        //        var langHeader = string.Empty;
        //        if (cookie != null)
        //        {
        //            // set the culture by the cookie content
        //            langHeader = cookie.Value;
        //            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
        //        }
        //        else
        //        {
        //            // set the culture by the location if not speicified
        //            langHeader = HttpContext.Request.UserLanguages[0];
        //            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
        //        }
        //        // set the lang value into route data
        //        RouteData.Values["lang"] = langHeader;
        //    }

        //    // save the location into cookie
        //    HttpCookie _cookie = new HttpCookie("MvcLocalization.CurrentUICulture", Thread.CurrentThread.CurrentUICulture.Name);
        //    _cookie.Expires = DateTime.Now.AddYears(1);
        //    HttpContext.Response.SetCookie(_cookie);

        //  base.ExecuteCore();
        //}
    }
}