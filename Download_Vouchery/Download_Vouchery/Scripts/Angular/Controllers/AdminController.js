angular.module("DownloadVoucheryApp").controller("AdminController", AdminController);

AdminController.$inject = [
    "$scope", "$rootScope", "$filter", "$route", "$http", "FileFactory", "UploadFactory", "VoucherFactory"
];

function AdminController($scope, $rootScope, $filter, $route, $http, FileFactory, UploadFactory, VoucherFactory) {
    $scope.files = [];
    $scope.vouchers = [];
    $scope.newVoucher = new VoucherFactory();
    $scope.voucherAmount = [];
    $scope.voucherOptions = false;
    $scope.fileName = "";
    $scope.fileId = "";
    $scope.vouchersInfo = [];
    var pageIndex;
    var pageTotal;
    var totalCount;

    /* Voucher functions */

    $scope.CreateVoucher = function(fileId, index) {
        $scope.newVoucher.$PostVouchers({ voucherAmount: $scope.voucherAmount[index], voucherFileId: fileId },
            function() { $rootScope.status = "Vouchers created." },
            function(error) {
                $rootScope.status = error.data.Message;
            });
    };
    $scope.DisplayVoucherImageUrl = function() {
        FileFactory.GetVoucherImageUrl(null,
            function(response) {
                $scope.voucherImage = response.FileUrl;
            });

    };
    $scope.GetVouchers = function(fileId, page, size) {
        if (typeof (page) === "undefined") page = 0;
        if (typeof (size) === "undefined") size = 10;
        $scope.vouchers = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: page, pageSize: size },
            function() {
                pageTotal = $scope.vouchers.TotalPages;
                totalCount = $scope.vouchers.TotalCount;
                pageIndex = 0;
            });
        console.log($scope.vouchers);
    };
    $scope.GetVoucherBatch = function(fileId, page, size) {
        if (typeof (page) === "undefined") page = 0;
        if (typeof (size) === "undefined") size = 10;
        $scope.vouchers = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: page, pageSize: size },
            function() {
                pageTotal = $scope.vouchers.TotalPages;
                totalCount = $scope.vouchers.TotalCount;
                pageIndex = 0;
                $rootScope.status = "Batch " + (page + 1) + " ready.";
            });
    };
    $scope.GetVouchersInfo = function(fileId) {
        $scope.vouchersInfo = VoucherFactory.GetVouchersInfo({ id: fileId });
    };
    $scope.GetAllVouchers = function(fileId) {
        $scope.vouchers = [];
        $scope.vouchersInfo = $http.get(" http://localhost:54809/api/vouchers/" + fileId + "/all")
            .then(function(response) { $scope.vouchers = response.data });
    };
    $scope.ShowVoucherOptions = function(fileName, fileId) {
        $scope.voucherOptions = true;
        $scope.fileName = fileName;
        $scope.fileId = fileId;
    };
    $scope.ResetVoucher = function(voucherId) {
        var voucher = VoucherFactory.GetVoucherDetails({ id: voucherId },
            function() {
                voucher.VoucherRedeemed = false;
                voucher.VoucherRedemptionDate = null;
                voucher.VoucherRedemptionCounter = 0;
                $id = voucherId;
                VoucherFactory.Reset({ id: $id },
                    voucher,
                    function() {
                        $rootScope.status = "Voucher reset.";
                    });
            }
        );
    };
    $scope.DeleteVouchers = function(voucherId) {
        VoucherFactory.DeleteVouchers({ id: voucherId },
            function() {
                $rootScope.status = "Voucher deleted.";
            });
    };

    /* Pagination */

    $scope.prevPage = function(fileId) {
        if (pageIndex <= 0) {
            pageIndex = 0;
        } else {
            pageIndex--;
            if (typeof (size) === "undefined") size = 10;
            $scope.vouchersTemp = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: pageIndex, pageSize: size },
                function() {
                    $scope.vouchers = $scope.vouchersTemp;
                });
        }
    };

    $scope.nextPage = function(fileId) {
        pageIndex++;
        if (typeof (size) === "undefined") size = 10;
        if (pageIndex < pageTotal) {
            $scope.vouchersTemp = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: pageIndex, pageSize: size },
                function() {
                    $scope.vouchers = $scope.vouchersTemp;
                });
        } else {
            pageIndex--;
        }
    };

    $scope.GetValueAtIndex = function(index) {
        var str = window.location.href;
        console.log(str.split("/")[index]);
        return str.split("/")[index];
    };

    /* Print vouchers functions */

    $scope.GetVouchersForPrint = function(fileId, page) {
        if (typeof (page) === "undefined") page = 0;
        var size = 1000;
        $scope.vouchers = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: page, pageSize: size },
            function() {
                pageTotal = $scope.vouchers.TotalPages;
                totalCount = $scope.vouchers.TotalCount;
                pageIndex = 0;
                $scope.ShowBatchOptions();
                $rootScope.status = "Batch 1 ready to print.";
            });
        console.log($scope.vouchers);
    };
    $scope.ShowBatchOptions = function() {
        var optionsAmount = Math.ceil(totalCount / 1000);
        $scope.showOptions = [];
        for (i = 0; i < optionsAmount; i++) {
            $scope.showOptions.push(i);
        }
        console.log($scope.showOptions);
    };

    /* File functions */

    $scope.GetFiles = function() {
        $scope.files = FileFactory.query();
    };
    $scope.UploadFile = function() {
        var file = $scope.myFile;

        console.log("file is ");
        console.dir(file);

        var uploadUrl = "/api/blobs/upload";
        UploadFactory.uploadFileToUrl(file, uploadUrl);
    };

    $scope.UploadVoucherImage = function() {
        var file = $scope.myFile;


        if (file.type === "image/png") {
            var uploadUrl = "/api/blobs/uploadvoucherimage";
            UploadFactory.uploadFileToUrl(file, uploadUrl);
        } else {
            $rootScope.status = "Only PNG files are allowed";
        }
    };
}