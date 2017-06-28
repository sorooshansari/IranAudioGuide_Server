using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace IranAudioGuide_MainServer.Services
{
    public static class ExtensionMethod
    {

        //convertToGuid
        public static Guid convertToGuid(this object obj)
        {
            try
            {
                return (obj == DBNull.Value) ? Guid.Empty : (Guid)obj;
            }
            catch
            {
                return Guid.Empty;
            }
        }
        public static string convertToString(this long obj)
        {
            try
            {

                return obj.ToString();
            }
            catch
            {
                return "0";
            }
        }
        public static string convertToString(this object obj)
        {
            try
            {
                return (obj == DBNull.Value) ? string.Empty : obj.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static int convertToInt(this object obj, int d = 0)
        {
            try
            {
                return (obj == DBNull.Value) ? d : Int32.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }
        public static int convertToInt(this string str)
        {
            try
            {
                return Int32.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        public static string ConvertToHtml(this string str)
        {
            try
            {
                return System.Web.HttpUtility.HtmlEncode(str);
            }
            catch
            {
                return null;
            }
        }


    }


}
