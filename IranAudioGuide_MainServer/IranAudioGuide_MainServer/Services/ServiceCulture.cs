using System;
using System.Globalization;
using System.Threading;
using System.Web;

namespace IranAudioGuide_MainServer
{
    [Flags]
    public enum EnumLang
    {
        en = 1,
        fa = 2
    }
    public static class ServiceCulture
    {
        public static int GetIntLang(string str)
        {
            foreach (EnumLang item in Enum.GetValues(typeof(EnumLang)))
            {
                if (str.Contains(item.ToString()))
                {
                    return (int)item;
                }
            }
            return (int)EnumLang.en;
        }

        public static string FindGetSting(string str)
        {
            foreach (EnumLang item in Enum.GetValues(typeof(EnumLang)))
            {
                if (str.Contains(item.ToString()))
                {
                    return item.ToString();
                }
            }
            return EnumLang.en.ToString();
        }
      
        public static string FindGetSting(int str)
        {
            foreach (EnumLang item in Enum.GetValues(typeof(EnumLang)))
            {
                if ((int)item == str)
                {
                    return item.ToString();
                }
            }
            return EnumLang.en.ToString();
        }

        public static void SetCulture(string langHeader)
        {
            langHeader = FindGetSting(langHeader);
            // set the lang value into route data
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
            // save the location into cookie
            HttpContext.Current.Request.RequestContext.RouteData.Values["lang"] = langHeader;
            HttpCookie _cookie = new HttpCookie("IranAudioGuide.CurrentUICulture", langHeader);
            _cookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Response.SetCookie(_cookie);
            //  HttpContext.Current.Response.Cookies[0].Value
            HttpContext.Current.Request.Cookies["IranAudioGuide.CurrentUICulture"].Value = langHeader;
        }

        public static void SetCulture()
        {
            var cookie = HttpContext.Current.Request.Cookies.Get("IranAudioGuide.CurrentUICulture");
            var langHeader = string.Empty;

            if (HttpContext.Current.Request.RequestContext.RouteData.Values["lang"] != null)
                langHeader = HttpContext.Current.Request.RequestContext.RouteData.Values["lang"].ToString();
            else
            {
                // load the culture info from the cookie
                if (cookie == null)
                {
                    if (HttpContext.Current.Request.UserLanguages.Length > 0)
                    {
                        // set the culture by the location if not speicified
                        langHeader = HttpContext.Current.Request.UserLanguages[0];
                    }
                    else
                    {
                        langHeader = "en";
                    }
                }
                else if (HttpContext.Current.Request.UserLanguages.Length > 0)
                {
                    // set the culture by the location if not speicified
                    langHeader = HttpContext.Current.Request.UserLanguages[0];
                }
                else
                {
                    langHeader = "en";
                }
            }
            langHeader = FindGetSting(langHeader);
            // set the lang value into route data
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
            // save the location into cookie
            HttpContext.Current.Request.RequestContext.RouteData.Values["lang"] = langHeader;
            HttpCookie _cookie = new HttpCookie("IranAudioGuide.CurrentUICulture", langHeader);
            _cookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Response.SetCookie(_cookie);
            //  HttpContext.Current.Response.Cookies[0].Value
            if (cookie != null)
                HttpContext.Current.Request.Cookies["IranAudioGuide.CurrentUICulture"].Value = langHeader;
        }
        public static void SetCultureFromCookie()
        {
            var cookie = HttpContext.Current.Request.Cookies.Get("IranAudioGuide.CurrentUICulture");
            var langHeader = string.Empty;
            if (cookie != null)
            {
                langHeader = cookie.Value.ToString();
            }
            else
                langHeader = EnumLang.en.ToString();
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(FindGetSting(langHeader));

        }
        public static int GeLangFromCookie()
        {
            // load the culture info from the cookie
            var cookie = HttpContext.Current.Request.Cookies.Get("IranAudioGuide.CurrentUICulture");
            if (cookie != null)
            {
                // set the culture by the cookie content
                return GetIntLang(cookie.Value.ToString());
            }
            return (int)EnumLang.en;
        }

        internal static object FindGetSting(object lang)
        {
            throw new NotImplementedException();
        }
    }
}

//public class StringLengthTranclateAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute
//{
//    public StringLengthTranclateAttribute(int maximumLength) : base(maximumLength)
//    {

//    }

//    public StringLengthTranclateAttribute(int length, string resourceId) : base(length)
//    {
//        ServiceCulture.SetCulture();
//        ErrorMessage = Global.ResourceManager.GetString(resourceId);
//    }


//    //public override string FormatErrorMessage(string name)
//    //{
//    //    return String.Format(CultureInfo.CurrentCulture,
//    //      ErrorMessageString, name);
//    //}


//}

//public class RequiredTranclateAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
//{
//    public RequiredTranclateAttribute(string resourceId)
//    {
//        ErrorMessage = Global.ResourceManager.GetString(resourceId);
//    }
//}

//public class DisplayNameTranslateAttribute : System.ComponentModel.DisplayNameAttribute
//{
//    public DisplayNameTranslateAttribute(string resourceId)
//        : base(GetMessageFromResource(resourceId))
//    { }
//    private static string GetMessageFromResource(string resourceId)
//    {
//        // TODO: Return the string from the resource file
//        return Global.ResourceManager.GetString(resourceId);
//    }
//}
//public class LocalizationAttribute : ActionFilterAttribute
//{
//    public override void OnActionExecuting(ActionExecutingContext filterContext)
//    {
//        ServiceCulture.SetCulture();
//        base.OnActionExecuting(filterContext);
//    }
//}


////public class MyMaxLengthAttribute : System.ComponentModel.DataAnnotations.MaxLengthAttribute
////{
////    private static String CustomErrorMessage = "{0} length should not be more than {1}";
////    public MyMaxLengthAttribute(int length) : base(length)
////    {
////        ErrorMessage = "What should I input here"
////    }

////    public override string FormatErrorMessage(string name)
////    {
////        if (!String.IsNullOrEmpty(ErrorMessage))
////        {
////            ErrorMessage = MyErrorMessage;
////        }
////        return String.Format(CultureInfo.CurrentUICulture, CustomErrorMessage, name);
////    }
////}

////[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
////public class DisplayNameLocalizedAttribute : DisplayNameAttribute
////{
////    public DisplayNameLocalizedAttribute(Type resourceManagerProvider, string resourceKey)
////       : base(LookupResource(resourceManagerProvider, resourceKey))
////    {
////    }
////    internal static string LookupResource(Type resourceManagerProvider, string resourceKey)
////    {
////        foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
////        {
////            if (staticProperty.PropertyType == typeof(System.ResourceManager))
////            {
////                System.ResourceManager resourceManager = (System.ResourceManager)staticProperty.GetValue(null, null);
////                return resourceManager.GetString(resourceKey);
////            }
////        }

////        return resourceKey; // Fallback with the key name
////    }
////}
