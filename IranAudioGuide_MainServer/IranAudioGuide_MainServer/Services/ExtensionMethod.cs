using IranAudioGuide_MainServer.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace IranAudioGuide_MainServer.Services
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
                return "";
            //throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            var members = type.GetMember(value.ToString());
            if (members.Length == 0)
                return "";
            //throw new ArgumentException(String.Format("Member '{0}' not found in type '{1}'", value, type.Name));

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes.Length == 0)
                return "";
            //throw new ArgumentException(String.Format("'{0}.{1}' doesn't have DisplayAttribute", type.Name, value));

            var attribute = (DisplayAttribute)attributes[0];
            return attribute.GetName();
        }

        public static T ConvertToEnum<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static T ConvertToEnum<T>(this int instance) where T : struct
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                return default(T);
            }
            var success = Enum.IsDefined(enumType, instance);
            if (success)
            {
                return (T)Enum.ToObject(enumType, instance);
            }
            else
            {
                return default(T);
            }
        }

    }
    public class DisplayTextAttribute : Attribute
    {

        public string Text { get; set; }
        public DisplayTextAttribute(string text)
        {
            Text = text;
        }

    }


    public static class ExtensionMethod
    {

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        //    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}


        //convertToGuid
        public static Guid ConvertToGuid(this object obj)
        {

            var myGuid = new Guid();
            if (Guid.TryParse(obj.ToString(), out myGuid))
                return myGuid;

            return Guid.Empty;

        }
        public static string ConvertToString(this long obj)
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
        public static string ConvertToString(this object obj)
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
        public static int ConvertToInt(this object obj, int d = 0)
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
        public static int ConvertToInt(this string str)
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
        public static int StoI(this string str)
        {
            try
            {
                return Int32.Parse(str);
            }
            catch
            {
                return -9999;
            }
        }
        public static long ConvertToLong(this string str)
        {
            try
            {
                return long.Parse(str);
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
