
angular.module("DownloadVoucheryApp").factory("FileFactory", FileFactory);

FileFactory.$inject = ["$resource"];

function FileFactory($resource) {
    return $resource("/api/blobs/",
        null,
        {
            GetVoucherImageUrl: { method: "GET", url: "/api/blobs/voucherimage/displayurl" },
            query: { method: "GET", url: "/api/blobs/getfiles", isArray: true }
        });
}