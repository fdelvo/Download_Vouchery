using System.Web;
using System.Web.Optimization;

namespace Download_Vouchery
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/Site.css"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            BundleTable.EnableOptimizations = true;
        }

        public static void RegisterScriptBundles(BundleCollection bundles)
        {
            const string ANGULAR_APP_ROOT = "~/Scripts/Angular/";

            var scriptBundle = new ScriptBundle("~/bundles/angular")
                .Include(ANGULAR_APP_ROOT + "DownloadVoucheryApp.js", ANGULAR_APP_ROOT + "Controllers/AdminController.js", ANGULAR_APP_ROOT + "Controllers/VoucherController.js", ANGULAR_APP_ROOT + "Controllers/OnlineVoucherController.js", ANGULAR_APP_ROOT + "Directives/InterceptorDirective.js", ANGULAR_APP_ROOT + "Factories/FileFactory.js", ANGULAR_APP_ROOT + "Factories/InterceptorFactory.js", ANGULAR_APP_ROOT + "Factories/UploadFactory.js", ANGULAR_APP_ROOT + "Factories/VoucherFactory.js");


            bundles.Add(scriptBundle);
        }
    }
}
