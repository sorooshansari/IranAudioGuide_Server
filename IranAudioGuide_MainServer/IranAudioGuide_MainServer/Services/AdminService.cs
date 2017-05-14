using IranAudioGuide_MainServer.App_GlobalResources;
using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IranAudioGuide_MainServer.Services
{

    public class AdminService
    {
        public object RunStoreProcedure(string nameStoreProcdeure, SqlParameter[] param)
        {
            try
            {
                var dataTable = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(GlobalPath.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(nameStoreProcdeure, sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var item in param)
                    {
                        
                    }cmd.Parameters.Add(new SqlParameter("@langId", 1));
                    sqlConnection.Open();
                    var reader = cmd.ExecuteReader();
                    dataTable.Load(reader);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }



        public object GetAll()
        {
            var prm = new[] { new SqlParameter("@langId", Global.Lang) };
            var dt1 = (List<DataTable>)RunStoreProcedure("GetPackages_website", prm);

            var links = dt1[0].AsEnumerable().Select(x => new
            {
                pathDestination = x["d"],
                FileName = x["f"],

            }).FirstOrDefault();
            return links;
        }
    }
    //internal void addEditPackage(NewPackage model)
    //{
    //   // return null;
    //    //using (var db = new ApplicationDbContext())
    //    //{
    //    //    using (var dbTran = db.Database.BeginTransaction())
    //    //    {
    //    //        try
    //    //        {
    //    //            var package = new Package()
    //    //            {
    //    //                Pac_Name = model.PackageName,
    //    //                Pac_Price = model.PackagePrice,
    //    //                Pac_Price_Dollar = model.PackagePrice_Dollar

    //    //            };

    //    //            var cities = db.Cities.Where(x => model.Cities.Any(y => y == x.Cit_Id)).ToList();
    //    //            package.Pac_Cities = cities;

    //    //            db.Packages.Add(package);
    //    //            db.SaveChanges();
    //    //            dbTran.Commit();
    //    //            return Json(new Respond());

    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            //ErrorSignal.FromCurrentContext().Raise(ex);
    //    //            //dbTran.Rollback();
    //    //            //return Json(new Respond(ex.Message, status.unknownError));
    //    //        }
    //    //    }

    //    //            }
    //}
}