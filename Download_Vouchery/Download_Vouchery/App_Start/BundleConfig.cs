using System.Web;
using System.Web.Optimization;

namespace Download_Vouchery
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/Angular/DownloadVoucheryApp.js",
                        "~/Scripts/Angular/Factories/FileFactory.js",
                        "~/Scripts/Angular/Factories/UploadFactory.js",
                        "~/Scripts/Angular/Factories/InterceptorFactory.js",
                        "~/Scripts/Angular/Directives/InterceptorDirective.js",
                        "~/Scripts/Angular/Factories/VoucherFactory.js",
                        "~/Scripts/Angular/Controllers/VoucherController.js",
                        "~/Scripts/Angular/Controllers/AdminController.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/Site.css"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
