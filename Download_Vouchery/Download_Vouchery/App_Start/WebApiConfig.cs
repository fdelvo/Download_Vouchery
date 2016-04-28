using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;

namespace Download_Vouchery
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
            "PostBlobUpload",
            "blobs/upload",
            new { controller = "Blobs", action = "PostBlobUpload" },
            new { httpMethod = new HttpMethodConstraint("POST") }
        );

            config.Routes.MapHttpRoute(
                "GetBlobDownload",
                "blobs/{voucherCode}/download",
                new { controller = "Blobs", action = "GetBlobDownload" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                "PostVouchers",
                "voucher/new/{voucherAmount}/{voucherFileId}",
                new { controller = "Vouchers", action = "PostVoucher" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
