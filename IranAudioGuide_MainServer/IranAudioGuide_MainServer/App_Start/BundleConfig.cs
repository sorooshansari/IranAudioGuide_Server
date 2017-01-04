using System.Web;
using System.Web.Optimization;

namespace IranAudioGuide_MainServer
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/minCss").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap-social.min.css",
                      "~/Content/toggle.min.css",
                      "~/Content/styles.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/minJs").Include(
                        "~/Scripts/jquery.min.js",
                        "~/Scripts/jquery.scrollex.min.js",
                        "~/Scripts/jquery.scrolly.min.js",
                        "~/Scripts/skel.min.js",
                        "~/Scripts/util.min.js",
                        "~/Scripts/main.min.js",
                        "~/Scripts/angular.min.js"));
        }
    }
}
