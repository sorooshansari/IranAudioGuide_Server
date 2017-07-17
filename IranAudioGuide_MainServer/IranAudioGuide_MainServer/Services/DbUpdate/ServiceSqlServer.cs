using IranAudioGuide_MainServer;
using System;

using System.Data;
using System.Data.SqlClient;

namespace DbUpdate
{
    public static class ServiceSqlServer
    {
        private static readonly string connString = GlobalPath.ConnectionString;
        private static void ExecuteCommand(string command, SqlConnection conn)
        {
            var cmd = new SqlCommand(command, conn);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
        //public static SqlDataReader RunStoredProc(string nameSP, bool returnvalue = true)
        //{
        //    try
        //    {
        //        using (SqlConnection sqlConnection = new SqlConnection(connString))
        //        {
        //            SqlCommand cmd = new SqlCommand(nameSP, sqlConnection);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddRange(sqlparam);
        //            sqlConnection.Open();
        //           return cmd.ExecuteReader();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
        //        return null;
        //    }
        //}
        public static DataTable RunStoredProc(string nameSP, bool returnvalue = false, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(nameSP, sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    sqlConnection.Open();
                    var reader = cmd.ExecuteReader();

                    if (!returnvalue)
                        return null;

                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                return null;

            }
        }
        public static void StoredProcedureExists()
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    var query = @"SELECT COUNT(0) FROM sys.procedures WHERE (name = N'Download_Link_Remove_ById')";
                    conn.Open();
                    var cmd = new SqlCommand(query, conn);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0)
                    {
                        var sbSP = new System.Text.StringBuilder();
                        sbSP.AppendLine("CREATE PROCEDURE [dbo].[Download_Link_Remove_ById] @Id uniqueidentifier AS   BEGIN  DELETE FROM DownloadLinks  WHERE Dow_Id= @Id  END");
                        ExecuteCommand(sbSP.ToString(), conn);
                    }
                    query = @"SELECT COUNT(0) FROM sys.procedures WHERE (name = N'Download_GetPathForDelete')";
                    cmd = new SqlCommand(query, conn);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0)
                    {
                        var q = @"CREATE PROCEDURE [dbo].[Download_GetPathForDelete] AS	BEGIN    SELECT DownloadLinks.Path  as Path , DownloadLinks.Dow_Id   as DowId   FROM DownloadLinks WHERE (IsDisable = 1) AND (TimeToVisit <= DATEADD (mi , 20 , TimeToVisit)) END";
                        ExecuteCommand(q, conn);

                    }
                    query = @"SELECT COUNT(0) FROM sys.procedures WHERE (name = N'Download_LinkDisable')";
                    cmd = new SqlCommand(query, conn);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0)
                    {
                        var sbSP = new System.Text.StringBuilder();
                        sbSP.AppendLine("CREATE PROCEDURE [dbo].[Download_LinkDisable] AS	BEGIN	UPDATE	DownloadLinks    SET  IsDisable = 'True'  WHERE(GETDATE() > DATEADD(mi, 10, TimeToVisit))    END");
                        ExecuteCommand(sbSP.ToString(), conn);

                    }
                    cmd.Dispose();
                    conn.Close();

                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
            }
        }
    }
}