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
        public static string convertToString(this object obj)
        {
            return (obj == DBNull.Value) ? string.Empty : obj.ToString();
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







        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field != null)
                    {

                        //if (propertyInfos.PropertyType == typeof(DateTime))
                        //{
                        //    propertyInfos.SetValue
                        //    (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        //}
                        //else if (propertyInfos.PropertyType == typeof(int))
                        //{
                        //    propertyInfos.SetValue
                        //    (classObj, ConvertToInt(dataRow[dtField.Name]), null);
                        //}
                        //else if (propertyInfos.PropertyType == typeof(long))
                        //{
                        //    propertyInfos.SetValue
                        //    (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                        //}
                        //else if (propertyInfos.PropertyType == typeof(decimal))
                        //{
                        //    propertyInfos.SetValue
                        //    (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                        //}
                        //else if (propertyInfos.PropertyType == typeof(String))
                        //{
                        //    if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                        //    {
                        //        propertyInfos.SetValue
                        //        (classObj, ConvertToDateString(dataRow[dtField.Name]), null);
                        //    }
                        //    else
                        //    {
                        //        propertyInfos.SetValue
                        //        (classObj, ConvertToString(dataRow[dtField.Name]), null);
                        //    }
                        //}
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }

        //private static string ConvertToDateString(object date)
        //{
        //    if (date == null)
        //        return string.Empty;

        //    return SpecialDateTime.ConvertDate(Convert.ToDateTime(date));
        //}

        //private static string ConvertToString(object value)
        //{
        //    return Convert.ToString(HelperFunctions.ReturnEmptyIfNull(value));
        //}

        //private static int ConvertToInt(object value)
        //{
        //    return Convert.ToInt32(HelperFunctions.ReturnZeroIfNull(value));
        //}

        //private static long ConvertToLong(object value)
        //{
        //    return Convert.ToInt64(HelperFunctions.ReturnZeroIfNull(value));
        //}

        //private static decimal ConvertToDecimal(object value)
        //{
        //    return Convert.ToDecimal(HelperFunctions.ReturnZeroIfNull(value));
        //}

        //private static DateTime convertToDateTime(object date)
        //{
        //    return Convert.ToDateTime(HelperFunctions.ReturnDateTimeMinIfNull(date));
        //}
    }


}
