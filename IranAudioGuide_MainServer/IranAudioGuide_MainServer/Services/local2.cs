using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IranAudioGuide_MainServer
{
    public static class ServiceCulture
    {
        public static string GetCurrentCultureFromcookie()
        {
            // load the culture info from the cookie
            var cookie = HttpContext.Current.Request.Cookies["IranAudioGuide.Models.CurrentUICulture"];
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
                langHeader = HttpContext.Current.Request.UserLanguages[0];
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
            }

            HttpCookie _cookie = new HttpCookie("IranAudioGuide.Models.CurrentUICulture", Thread.CurrentThread.CurrentUICulture.Name);
            _cookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Response.SetCookie(_cookie);

            return langHeader;

        }
    }

    public class StringLengthTranclateAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute
    {
        public StringLengthTranclateAttribute(int maximumLength) : base(maximumLength)
        {

        }

        public StringLengthTranclateAttribute(int length, string resourceId) : base(length)
        {
            var c = Thread.CurrentThread.CurrentUICulture;
            var lang = ServiceCulture.GetCurrentCultureFromcookie();
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);

            ErrorMessage = Resources.Global.ResourceManager.GetString(resourceId);
        }


        //public override string FormatErrorMessage(string name)
        //{
        //    return String.Format(CultureInfo.CurrentCulture,
        //      ErrorMessageString, name);
        //}


    }

    //public class MyMaxLengthAttribute : System.ComponentModel.DataAnnotations.MaxLengthAttribute
    //{
    //    private static String CustomErrorMessage = "{0} length should not be more than {1}";
    //    public MyMaxLengthAttribute(int length) : base(length)
    //    {
    //        ErrorMessage = "What should I input here"
    //    }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        if (!String.IsNullOrEmpty(ErrorMessage))
    //        {
    //            ErrorMessage = MyErrorMessage;
    //        }
    //        return String.Format(CultureInfo.CurrentUICulture, CustomErrorMessage, name);
    //    }
    //}
    public class RequiredTranclateAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        public RequiredTranclateAttribute(string resourceId)
        {
            ErrorMessage = Resources.Global.ResourceManager.GetString(resourceId);
        }
    }
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    //public class DisplayNameLocalizedAttribute : DisplayNameAttribute
    //{
    //    public DisplayNameLocalizedAttribute(Type resourceManagerProvider, string resourceKey)
    //       : base(LookupResource(resourceManagerProvider, resourceKey))
    //    {
    //    }
    //    internal static string LookupResource(Type resourceManagerProvider, string resourceKey)
    //    {
    //        foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
    //        {
    //            if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
    //            {
    //                System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
    //                return resourceManager.GetString(resourceKey);
    //            }
    //        }

    //        return resourceKey; // Fallback with the key name
    //    }
    //}
    public class DisplayNameTranclateAttribute : System.ComponentModel.DisplayNameAttribute
    {
        public DisplayNameTranclateAttribute(string resourceId)
            : base(GetMessageFromResource(resourceId))
        { }
        private static string GetMessageFromResource(string resourceId)
        {
            // TODO: Return the string from the resource file
            return Resources.Global.ResourceManager.GetString(resourceId);
        }
    }
    public class LogActionFilter : ActionFilterAttribute

    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var lang = filterContext.RouteData.Values["lang"] == null ? null : filterContext.RouteData.Values["lang"].ToString();
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

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log("OnActionExecuted", filterContext.RouteData);
        }
        

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Log("OnResultExecuting", filterContext.RouteData);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Log("OnResultExecuted", filterContext.RouteData);
        }


        private void Log(string methodName, RouteData routeData)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var message = String.Format("{0} controller:{1} action:{2}", methodName, controllerName, actionName);
            System.Diagnostics.Debug.WriteLine(message, "Action Filter Log");
        }

    }
    public class LocalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var lang = filterContext.RouteData.Values["lang"] == null ? null : filterContext.RouteData.Values["lang"].ToString();
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