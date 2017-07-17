using System;

namespace IranAudioGuide_MainServer.Services
{
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
            try
            {
                return (obj == DBNull.Value) ? Guid.Empty : (Guid)obj;
            }
            catch
            {
                return Guid.Empty;
            }
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
