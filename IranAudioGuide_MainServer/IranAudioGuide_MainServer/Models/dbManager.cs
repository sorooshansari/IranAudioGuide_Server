using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IranAudioGuide_MainServer.Models
{
    public static class ConnectionString
    {
        readonly public static string connString = @"Data Source=.;Initial Catalog=iranaudi_test5;Integrated Security=True";

        //#if DEBUG
        //#else
       // readonly public static string connString = "Password = @aQ35cw%0@; Persist Security Info=True;User ID = iranaudi_SorooshDeveloperTeam; Initial Catalog = iranaudi_PrimaryDB; Data Source = 185.55.224.3";
        //#endif
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
                if (parameters.Length > 0)
                {
                    paramList = parameters[0].ParameterName;
                }
                for (int i = 1; i < parameters.Length; i++)
                {
                    paramList += string.Format(", {0}", parameters[i].ParameterName);
                }
                var command = string.Format("exec {0} {1}", SP, paramList);
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
        public void AddTipCategory(string name, string Class, string unicode, int priority)
        {
            TipCategory tc = new TipCategory()
            {
                TiC_Class = Class,
                TiC_Name = name,
                TiC_Unicode = unicode,
                TiC_Priority = priority
            };
            using (var db = new ApplicationDbContext())
            {
                if (db.TipCategories.Where(x => x.TiC_Priority == priority).FirstOrDefault() == default(TipCategory))
                {
                    db.TipCategories.Add(tc);
                    db.SaveChanges();
                }
            }
        }
        public void AddUser(string Email, string Pass, string ImgUrl, string FullName, string Role, ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (UserManager.FindByName(Email) == null)
            {
                var user = new ApplicationUser()
                {
                    UserName = Email,
                    Email = Email,
                    FullName = FullName,
                    ImgUrl = ImgUrl
                };
                if (Role == "AppUser")
                    user.EmailConfirmed = true;
                UserManager.Create(user, Pass);
                UserManager.AddToRole(user.Id, Role);
            }
        }
        public void RoleCreator(ApplicationDbContext context, string roleName)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            IdentityResult roleResult;

            // Check to see if Role Exists, if not create it
            if (!RoleManager.RoleExists(roleName))
            {
                roleResult = RoleManager.Create(new IdentityRole(roleName));
            }
        }
    }
}