using dbUpdater.Services;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace dbUpdater
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ServiceSqlServer.StoredProcedureExists();
            var job = new ScheduleDownloadLink();
            job.Run();
        }
    }
}
