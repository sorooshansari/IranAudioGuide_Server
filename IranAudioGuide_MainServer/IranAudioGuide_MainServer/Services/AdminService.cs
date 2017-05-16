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

    public class AdminService {
       

        public object GetAll()
        {
            var dt1 = new DataTable();
            List<DataTable> listdata = new List<DataTable>(); 
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(GlobalPath.ConnectionString))
                {
                    var ddd = Global.Lang;
                    SqlCommand cmd = new SqlCommand("GetPackages_website", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@langId", 1));
                    sqlConnection.Open();
                    var reader = cmd.ExecuteReader();

                    dt1.Load(reader);
                    listdata.Add(dt1);
                }

                foreach (var item in dt1.AsEnumerable())
                {
                    var d = item.Table.AsEnumerable().ToList();
                }
                var links = new
                {
                    pathDestination = listdata[0].AsEnumerable().Select(c => new
                    {
                        Pla_Id = c["Pla_Id"],
                        Name = c["Name"],
                        Discription = c["Discription"],
                        Address = c["Address"],
                        ImgUrl = c["ImgUrl"],
                        TumbImgUrl = c["TumbImgUrl"],
                        AudiosCount = c["AudiosCount"],
                        StoriesCount = c["StoriesCount"],
                        Cit_Id = c["Cit_Id"],
                        OrderItem = c["OrderItem"]
                    }).ToList(),
                    FileName = listdata[1].AsEnumerable().Select(c => new
                    {
                        PackageId = c["PackageId"],
                        PackageName = c["PackageName"],
                        PackagePrice = c["PackagePrice"],
                        PackagePriceDollar = c["PackagePriceDollar"],
                        PackageOrder = c["PackageOrder"],
                        CityId = c["CityId"],
                        CityName = c["CityName"],
                        CityOrder = c["CityOrder"]
                    }).ToList()
                };

                return links;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
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