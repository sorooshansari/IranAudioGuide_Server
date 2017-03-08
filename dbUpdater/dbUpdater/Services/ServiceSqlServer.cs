using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace dbUpdater.Services
{
    public static class ServiceSqlServer
    {
        private static readonly string connstring = GlobalPath.ConnectionString;


        public static void RunStoredProc(string nameSP)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connstring))
                {
                    SqlCommand cmd = new SqlCommand(nameSP, sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    cmd.ExecuteReader();

                }
            }
            catch
            {

            }
        }
        public static DataTable RunStoredProc(string nameSP, bool returnvalue = true)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connstring))
                {
                    SqlCommand cmd = new SqlCommand(nameSP, sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    var reader = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            catch
            {
                return null;
            }
        }
        //public IList<PathVM>  RunStoredProc(string nameSP)
        //{
        //    try
        //    {
        //        using (SqlConnection sqlConnection = new SqlConnection(connstring))
        //        {
        //            SqlCommand cmd = new SqlCommand(nameSP, sqlConnection);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            sqlConnection.Open();
        //            var reader = cmd.ExecuteReader();
        //            var dt = new DataTable();
        //            dt.Load(reader);
        //            return dt.AsEnumerable().Select(x => new PathVM
        //            {
        //                Path = x["path"].ToString()
        //            }).ToList();
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}