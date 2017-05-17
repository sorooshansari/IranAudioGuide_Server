using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IranAudioGuide_MainServer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{lang}/{controller}/{action}/{id}", // URL with parameters
                new { lang = "en", controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );


            //routes.MapRoute(name: "error",
            //    url: "ErrorPage", defaults: new {  controller = "Home", action = "Error" });
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "DefaultLocalized",
            //    url: "{lang}/{controller}/{action}/{id}",
            //    constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" },   // en or en-US
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //    );
        }
    }
}
