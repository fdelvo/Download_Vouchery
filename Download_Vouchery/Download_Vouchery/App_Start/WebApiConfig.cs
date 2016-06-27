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

            // Blobs Controller

            config.Routes.MapHttpRoute(
                "PostBlobUpload",
                "api/blobs/upload",
                new { controller = "Blobs", action = "PostBlobUpload" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );

            config.Routes.MapHttpRoute(
                "PostVoucherImage",
                "api/blobs/uploadvoucherimage",
                new { controller = "Blobs", action = "PostVoucherImage" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );

            config.Routes.MapHttpRoute(
                "GetBlobDownload",
                "api/blobs/{voucherCode}/download/{check}",
                new { controller = "Blobs", action = "GetBlobDownload" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                "GetVoucherImage",
                "api/blobs/voucherimage/displayurl",
                new { controller = "Blobs", action = "VoucherImage" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                "GetFiles",
                "api/blobs/getfiles",
                new { controller = "Blobs", action = "GetBlobs" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                "DeleteFile",
                "api/blobs/delete/{blobId}",
                new { controller = "Blobs", action = "DeleteBlob" },
                new { httpMethod = new HttpMethodConstraint("DELETE") }
            );

            // Vouchers Controller

            config.Routes.MapHttpRoute(
                "GetVouchersPaged",
                "api/vouchers/{id}/{pageIndex}/{pageSize}",
                new { controller = "Vouchers", action = "GetVouchers" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                "GetVouchersInfo",
                "api/vouchers/getvouchersinfo/{id}",
                new { controller = "Vouchers", action = "GetVouchersInfo" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                "GetVoucherDetails",
                "api/vouchers/voucher/{id}",
                new { controller = "Vouchers", action = "GetVoucherDetails" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                "ResetVoucher",
                "api/vouchers/voucher/{id}/reset",
                new { controller = "Vouchers", action = "PutVoucher" },
                new { httpMethod = new HttpMethodConstraint("PUT") }
            );

            config.Routes.MapHttpRoute(
                "PostVoucher",
                "api/vouchers/new/{voucherAmount}/{id}",
                new { controller = "Vouchers", action = "PostVoucher" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );

            config.Routes.MapHttpRoute(
                "Delete",
                "api/vouchers/voucher/{id}/delete",
                new { controller = "Vouchers", action = "DeleteVoucher" },
                new { httpMethod = new HttpMethodConstraint("DELETE") }
            );

            config.Routes.MapHttpRoute(
                "GenerateOnlineVoucher",
                "api/vouchers/generateonlinevoucher/{id}",
                new { controller = "Vouchers", action = "GenerateOnlineVoucher" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );

            config.Routes.MapHttpRoute(
                "Test",
                "api/vouchers/test/{test}",
                new { controller = "Vouchers", action = "Test" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );
        }
    }
}
