using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dbUpdater.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                throw new ArgumentNullException("value");
                return View();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                return View();
            }
        }

    }
}