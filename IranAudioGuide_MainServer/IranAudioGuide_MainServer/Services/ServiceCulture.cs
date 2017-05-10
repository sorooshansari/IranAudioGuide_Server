using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.App_GlobalResources;
using System.Collections.Generic;

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

        public static string Find(string str)
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
        public static void SetCulture()
        {
            var lang = HttpContext.Current.Request.RequestContext.RouteData.Values["lang"];

            if (lang == null || lang == DBNull.Value)
            {
                lang = HttpContext.Current.Request.Cookies["IranAudioGuide.Models.CurrentUICulture"].Value;
            }

            if (lang == null || lang == DBNull.Value)
            {
                lang = HttpContext.Current.Request.UserLanguages[0];
            }

            string _eLang = Find(lang.ToString());

            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(_eLang);
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
}