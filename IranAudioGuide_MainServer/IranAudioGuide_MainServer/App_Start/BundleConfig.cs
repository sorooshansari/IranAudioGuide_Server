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
                        "~/Content/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/Scripts/bootstrap.js",
                      "~/Content/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/site.css"));
            bundles.Add(new StyleBundle("~/Content/css/minCss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/bootstrap-social.min.css",
                      "~/Content/global/plugins/font-awesome/font-awesome.min.css",
                      "~/Content/css/toggle.min.css",
                      "~/Content/css/styles.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/minJs").Include(
                        "~/Content/Scripts/jquery.min.js",
                        "~/Content/Scripts/jquery.scrollex.min.js",
                        "~/Content/Scripts/jquery.scrolly.min.js",
                        "~/Content/Scripts/skel.min.js",
                        "~/Content/Scripts/util.min.js",
                        "~/Content/Scripts/main.min.js",
                        "~/Content/Scripts/bootstrap.min.js",
                        "~/Content/global/plugins/angularjs/angular.min.js"));

            bundles.Add(new StyleBundle("~/Content/css/paymentCss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/bootstrap-social.min.css",
                      "~/Content/css/toggle.min.css",
                      "~/Content/css/main2.css",
                      "~/Content/css/Custom4.css"));

            bundles.Add(new ScriptBundle("~/bundles/paymentJs").Include(
                        "~/Content/Scripts/jquery.min.js",
                        "~/Content/Scripts/bootstrap.min.js"));
        }
    }
}
