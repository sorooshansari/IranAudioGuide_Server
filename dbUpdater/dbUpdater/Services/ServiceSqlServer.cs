using System;

using System.Data;
using System.Data.SqlClient;

namespace dbUpdater.Services
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
        public static void RunStoredProc(string nameSP)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connString))
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
                using (SqlConnection sqlConnection = new SqlConnection(connString))
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
        }public static void StoredProcedureExists()
        {
            //    var query = string.Format("SELECT COUNT(0) FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = '{0}'", sp);

            using (var conn = new SqlConnection(connString))
            {
                var query = @"SELECT COUNT(0) FROM sys.procedures WHERE (name = N'Download_LinkRemove')";
                conn.Open();
                var cmd = new SqlCommand(query, conn);
                if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0)
                {
                    var sbSP = new System.Text.StringBuilder();
                    sbSP.AppendLine("CREATE PROCEDURE [dbo].[Download_LinkRemove] AS	BEGIN		DELETE FROM DownloadLinks		WHERE        (IsDisable = 1) AND (TimeToVisit <=  DATEADD (mi , 20 , TimeToVisit))	END");
                    ExecuteCommand(sbSP.ToString(), conn);
                }
                query = @"SELECT COUNT(0) FROM sys.procedures WHERE (name = N'Download_GetPathForDelete')";
                cmd = new SqlCommand(query, conn);
                if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0)
                {
                    var q = @"CREATE PROCEDURE [dbo].[Download_GetPathForDelete]		AS		BEGIN		 SELECT DownloadLinks.Path		FROM DownloadLinks		WHERE (IsDisable = 1) AND (TimeToVisit <= DATEADD (mi , 20 , TimeToVisit)) END";
                    ExecuteCommand(q, conn);

                }
                query = @"SELECT COUNT(0) FROM sys.procedures WHERE (name = N'Download_LinkDisable')";
                cmd = new SqlCommand(query, conn);
                if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0)
                {
                    var sbSP = new System.Text.StringBuilder();
                    sbSP.AppendLine("CREATE PROCEDURE [dbo].[Download_LinkDisable] AS	BEGIN	UPDATE       DownloadLinks		SET    IsDisable = 'True'	where TimeToVisit <=  DATEADD (mi , 10 , TimeToVisit)	END");
                    ExecuteCommand(sbSP.ToString(), conn);

                }
                cmd.Dispose();
                conn.Close();

            }
        }
    }
}