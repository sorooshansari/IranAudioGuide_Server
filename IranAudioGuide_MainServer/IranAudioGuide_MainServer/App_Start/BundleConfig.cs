using System.Web;
using System.Web.Optimization;

namespace IranAudioGuide_MainServer
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/ProfileCssen").Include(
                            "~/Content/global/plugins/toastr/toastr.min.css",
                            "~/Content/css/bootstrap.3.3.7.min.css",
                            "~/Content/css/select.min.css",
                            "~/Content/css/simple-sidebar.css",
                            "~/Content/global/plugins/font-awesome/css/font-awesome.min.css",
                            "~/Content/css/profile.css",
                            "~/Content/css/PakageTemplate.css"));

            bundles.Add(new StyleBundle("~/ProfileCssfa").Include(
                           "~/Content/global/plugins/toastr/toastr.min.css",
                           "~/Content/css/bootstrap.3.3.7.min.css",
                           "~/Content/css/select.min.css",
                           "~/Content/css/simple-sidebar_fa.css",
                           "~/Content/global/plugins/font-awesome/css/font-awesome.min.css",
                           "~/Content/css/profile_fa.css",
                           "~/Content/css/PakageTemplate.css"));

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
                      "~/Content/css/site.css",
                      "~/Content/global/plugins/font-awesome/css/font-awesome.min.css"));
            bundles.Add(new StyleBundle("~/HomeCss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/bootstrap-social.min.css",
                      "~/Content/global/plugins/font-awesome/css/font-awesome.min.css",
                      "~/Content/css/toggle.min.css",
                      "~/Content/css/styles.min.css",
                      "~/Content/css/HomeStyles.css",
                      "~/Content/css/Home_fa.css"));

            bundles.Add(new ScriptBundle("~/bundles/minJs").Include(
                        "~/Content/Scripts/jquery.min.js",
                        "~/Content/Scripts/jquery.scrollex.min.js",
                        "~/Content/Scripts/jquery.scrolly.min.js",
                        "~/Content/Scripts/skel.min.js",
                        "~/Content/Scripts/util.min.js",
                        "~/Content/Scripts/main.min.js",
                        "~/Content/Scripts/bootstrap.min.js",
                        "~/Content/global/plugins/angularjs/angular.min.js",
                        "~/Content/Scripts/root-app.js",
                        "~/Content/Scripts/root-function.js"));

            bundles.Add(new StyleBundle("~/Content/css/paymentCss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/bootstrap-social.min.css",
                      "~/Content/css/toggle.min.css",
                      "~/Content/css/main2.css",
                      "~/Content/global/plugins/font-awesome/css/font-awesome.min.css",
                      "~/Content/css/Custom4.css"));

            bundles.Add(new ScriptBundle("~/bundles/paymentJs").Include(
                        "~/Content/Scripts/jquery.min.js",
                        "~/Content/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/Profile/minJs")
                .Include(
                    "~/Content/Scripts/jquery-2.2.3.js",
                    "~/Content/Scripts/bootstrap.min.js",
                    "~/Content/global/plugins/angularjs/angular.min.js",
                    "~/Content/global/plugins/angularjs/angular-ui-router.min.js",
                    "~/Content/Scripts/toastr.min.js",
                    "~/Content/Scripts/select.min.js",
                    "~/Content/Scripts/Underscore.js"
                    // "~/Content/Scripts/UserCustom.js"
                    ));

            bundles.Add(new ScriptBundle("~/Profile/ang")
                .Include(
                "~/Content/Scripts/UserServices.js",
                "~/Content/Scripts/menu.js",
                "~/Content/Scripts/UserControllers.js"));
                }
    }
}
