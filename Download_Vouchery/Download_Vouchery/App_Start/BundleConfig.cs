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
                        "~/Scripts/Angular/Core/angular.js",
                        "~/Scripts/Angular/Core/angular-resource.js",
                        "~/Scripts/Angular/Core/angular-route.js",
                        "~/Scripts/Angular/DownloadVoucheryApp.js",
                        "~/Scripts/Angular/Factories/FileFactory.js",
                        "~/Scripts/Angular/Factories/UploadFactory.js",
                        "~/Scripts/Angular/Factories/InterceptorFactory.js",
                        "~/Scripts/Angular/Directives/InterceptorDirective.js",
                        "~/Scripts/Angular/Factories/VoucherFactory.js",
                        "~/Scripts/Angular/Controllers/FileController.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/Site.css"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
        }
    }
}
