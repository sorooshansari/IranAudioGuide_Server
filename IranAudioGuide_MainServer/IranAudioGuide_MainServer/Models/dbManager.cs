using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IranAudioGuide_MainServer.Models
{
    public static class ConnectionString
    {
        readonly public static string connString = "Password = @aQ35cw%0@; Persist Security Info=True;User ID = iranaudi_SorooshDeveloperTeam; Initial Catalog = iranaudi_MainDB; Data Source = 185.55.224.3";
    }
    public class dbManager
    {
        string connstring = ConnectionString.connString;
        public DataTable TableResultSP(string SP, params SqlParameter[] parameters)
        {
            return ExecSqlCommand(SP, parameters)[0];
        }
        public int IntegerResultSP(string SP, params SqlParameter[] parameters)
        {
            var dataTable = ExecSqlCommand(SP, parameters)[0];
            return dataTable.AsEnumerable().Select(x => x.Field<int>("res")).FirstOrDefault();
        }
        public List<DataTable> MultiTableResultSP(string SP, params SqlParameter[] parameters)
        {
            return ExecSqlCommand(SP, parameters);
        }
        private List<DataTable> ExecSqlCommand(string SP, params SqlParameter[] parameters)
        {
            var res = new List<DataTable>();
            using (var con = new SqlConnection(connstring))
            {
                string paramList = "";
                foreach (var p in parameters)
                    paramList += string.Format(" {0}", p.ParameterName);
                var command = string.Format("exec {0}{1}", SP, paramList);
                con.Open();
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);
                    var dataReader = cmd.ExecuteReader();
                    while (!dataReader.IsClosed)
                    {
                        var dt = new DataTable();
                        dt.Load(dataReader);
                        res.Add(dt);
                    }
                }
            }
            return res;
        }
    }
}