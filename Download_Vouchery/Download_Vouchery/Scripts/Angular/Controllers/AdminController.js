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
    $scope.currentPage = 0;
    $scope.pageTotal = 0;
    $scope.totalCount = 0;
    $scope.pageSize = 10;

    /* Voucher functions */

    $scope.CreateVoucher = function(fileId, index, fileName) {
        $scope.newVoucher.$PostVouchers({ voucherAmount: $scope.voucherAmount[index], voucherFileId: fileId },
            function() {
                $rootScope.status = "Vouchers created.";
                $scope.GetVouchers(fileId);
                $scope.GetVouchersInfo(fileId);
                $scope.ShowVoucherOptions(fileName, fileId);
            },
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
    $scope.GetVouchers = function (fileId, pageIndex) {
        $scope.GetVouchersInfo(fileId);
        if (typeof pageIndex !== 'undefined') {
            $scope.currentPage = pageIndex;
        } else {
            $scope.currentPage = 0;
        }
        $scope.vouchersTemp = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: $scope.currentPage, pageSize: $scope.pageSize },
            function () {
                $scope.vouchers = $scope.vouchersTemp;
                $scope.pageTotal = $scope.vouchers.TotalPages;
                $scope.totalCount = $scope.vouchers.TotalCount;
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
                        $scope.GetVouchers(voucher.VoucherFileId.FileId, $scope.currentPage);
                    });
            }
        );
    };
    $scope.DeleteVouchers = function(voucherId, fileId) {
        VoucherFactory.DeleteVouchers({ id: voucherId },
            function() {
                $rootScope.status = "Voucher deleted.";
                $scope.GetVouchers(fileId.FileId);
            });
    };

    /* Pagination */

    $scope.prevPage = function(fileId) {
        if ($scope.currentPage <= 0) {
            $scope.currentPage = 0;
        } else {
            $scope.currentPage--;
            $scope.vouchersTemp = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: $scope.currentPage, pageSize: $scope.pageSize },
                function() {
                    $scope.vouchers = $scope.vouchersTemp;
                });
        }
    };
    $scope.GoToPage = function (fileId) {
        if ($scope.currentPage > $scope.pageTotal) {
            $scope.currentPage = $scope.pageTotal;
        }
        if ($scope.currentPage < 0) {
            $scope.currentPage = 0;
        }
        if ($scope.currentPage <= $scope.pageTotal && $scope.currentPage >= 0) {
            $scope.vouchersTemp = VoucherFactory
                .GetVouchersPaged({ id: fileId, pageIndex: $scope.currentPage, pageSize: $scope.pageSize },
                    function() {
                        $scope.vouchers = $scope.vouchersTemp;
                    });
        }
    };
    $scope.nextPage = function(fileId) {
        $scope.currentPage++;
        if ($scope.currentPage <= $scope.pageTotal) {
            $scope.vouchersTemp = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: $scope.currentPage, pageSize: $scope.pageSize },
                function() {
                    $scope.vouchers = $scope.vouchersTemp;
                });
        } else {
            $scope.currentPage--;
        }
    };

    /* Print vouchers functions */

    $scope.GetVouchersForPrint = function(fileId, page) {
        if (typeof (page) === "undefined") page = 0;
        var size = 1000;
        $scope.vouchers = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: page, pageSize: size },
            function() {
                $scope.pageTotal = $scope.vouchers.TotalPages;
                $scope.totalCount = $scope.vouchers.TotalCount;
                $scope.currentPage = 0;
                $scope.ShowBatchOptions();
                $rootScope.status = "Batch 1 ready to print.";
            });
        console.log($scope.vouchers);
    };
    $scope.GetVoucherBatch = function (fileId, page, size) {
        if (typeof (page) === "undefined") page = 0;
        if (typeof (size) === "undefined") size = 10;
        $scope.vouchers = VoucherFactory.GetVouchersPaged({ id: fileId, pageIndex: page, pageSize: size },
            function () {
                $scope.pageTotal = $scope.vouchers.TotalPages;
                $scope.totalCount = $scope.vouchers.TotalCount;
                $scope.currentPage = 0;
                $rootScope.status = "Batch " + (page + 1) + " ready.";
            });
    };
    $scope.ShowBatchOptions = function() {
        var optionsAmount = Math.ceil($scope.totalCount / 1000);
        $scope.showOptions = [];
        for (i = 0; i < optionsAmount; i++) {
            $scope.showOptions.push(i);
        }
        console.log($scope.showOptions);
    };
    $scope.GetValueAtIndex = function (index) {
        var str = window.location.href;
        console.log(str.split("/")[index]);
        return str.split("/")[index];
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
    $scope.DeleteFile = function(id) {
        FileFactory.DeleteBlob({ blobId: id }, function () {
            window.location.reload();
            $rootScope.status = "File deleted.";
        });
    }
}