using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class DisplayNameLocalizedAttribute : DisplayNameAttribute
    {
        public DisplayNameLocalizedAttribute(Type resourceManagerProvider, string resourceKey)
           : base(LookupResource(resourceManagerProvider, resourceKey))
        {
        }
        internal static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }
    }

    public class LocalizedDisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(string resourceId)
            : base(GetMessageFromResource(resourceId))
        { }
        private static string GetMessageFromResource(string resourceId)
        {
            // TODO: Return the string from the resource file
            return Resources.Global.ResourceManager.GetString(resourceId);
        }
    }
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
                var cookie = filterContext.HttpContext.Request.Cookies["test.Models.CurrentUICulture"];
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
            HttpCookie _cookie = new HttpCookie("test.Models.CurrentUICulture", Thread.CurrentThread.CurrentUICulture.Name);
            _cookie.Expires = DateTime.Now.AddYears(1);
            filterContext.HttpContext.Response.SetCookie(_cookie);

            base.OnActionExecuting(filterContext);
        }
    }
}