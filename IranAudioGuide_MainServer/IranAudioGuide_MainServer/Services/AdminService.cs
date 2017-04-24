using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IranAudioGuide_MainServer.Services
{

    public class AdminService
    {
        internal static void addEditPackage(NewPackage model)
        {
            //using (var db = new ApplicationDbContext())
            //{
            //    using (var dbTran = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var package = new Package()
            //            {
            //                Pac_Name = model.PackageName,
            //                Pac_Price = model.PackagePrice,
            //                Pac_Price_Dollar = model.PackagePrice_Dollar

            //            };

            //            var cities = db.Cities.Where(x => model.Cities.Any(y => y == x.Cit_Id)).ToList();
            //            package.Pac_Cities = cities;

            //            db.Packages.Add(package);
            //            db.SaveChanges();
            //            dbTran.Commit();
            //            return Json(new Respond());

            //        }
            //        catch (Exception ex)
            //        {
            //            //ErrorSignal.FromCurrentContext().Raise(ex);
            //            //dbTran.Rollback();
            //            //return Json(new Respond(ex.Message, status.unknownError));
            //        }
            //    }

            //            }
            throw new NotImplementedException();
        }
    }
}