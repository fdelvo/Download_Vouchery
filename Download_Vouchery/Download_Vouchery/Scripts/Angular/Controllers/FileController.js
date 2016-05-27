(function () {
    angular.module('DownloadVoucheryApp').controller('FileController', FileController);

    FileController.$inject = ['$scope', '$filter', '$route', '$http', 'FileFactory', 'UploadFactory', 'VoucherFactory'];

    function FileController($scope, $filter, $route, $http, FileFactory, UploadFactory, VoucherFactory) {
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

        $scope.CreateVoucher = function (fileId, index) {
            $scope.newVoucher.VoucherAmount = $scope.voucherAmount[index];
            $scope.newVoucher.$save({ id: fileId }, function () { window.location.reload(); });
        }

        $scope.GetVouchers = function (fileId, page, size) {
            if (typeof (page) === 'undefined') page = 0;
            if (typeof (size) === 'undefined') size = 10;
            $scope.vouchers = VoucherFactory.query({ id: fileId, pageIndex: page, pageSize: size }, function () {
                pageTotal = $scope.vouchers.TotalPages;
                totalCount = $scope.vouchers.TotalCount;
                pageIndex = 0;
            });
            console.log($scope.vouchers);
        }

        $scope.GetVouchersInfo = function (fileId) {
            $scope.vouchersInfo = $http.get(' http://localhost:54809/api/vouchers/getvouchersinfo/' + fileId).then(function (response) { $scope.vouchersInfo = response.data});
        }

        $scope.GetAllVouchers = function (fileId) {
            $scope.vouchers = [];
            $scope.vouchersInfo = $http.get(' http://localhost:54809/api/vouchers/' + fileId + '/all').then(function (response) { $scope.vouchers = response.data });
        }

        $scope.ShowVoucherOptions = function (fileName, fileId) {
            $scope.voucherOptions = true;
            $scope.fileName = fileName;
            $scope.fileId = fileId;
        }

        $scope.GetFiles = function () {
            $scope.files = FileFactory.query();
        }

        $scope.uploadFile = function () {
            var file = $scope.myFile;

            console.log('file is ');
            console.dir(file);

            var uploadUrl = "/blobs/upload";
            UploadFactory.uploadFileToUrl(file, uploadUrl);
        };   

        $scope.prevPage = function (fileId) {
            if (pageIndex <= 0) {
                pageIndex = 0;
            } else {
                pageIndex--;
                if (typeof (size) === 'undefined') size = 10;
                $scope.vouchersTemp = VoucherFactory.query({ id: fileId, pageIndex: pageIndex, pageSize: size });
                $scope.vouchers = $scope.vouchersTemp;
            }
        };

        $scope.nextPage = function (fileId) {
            pageIndex++;
            if (typeof (size) === 'undefined') size = 10;
            if (pageIndex < pageTotal) {
                $scope.vouchersTemp = VoucherFactory.query({ id: fileId, pageIndex: pageIndex, pageSize: size });
                $scope.vouchers = $scope.vouchersTemp;
            } else {
                pageIndex--;
            }
        };

        $scope.GetValueAtIndex = function (index) {
            var str = window.location.href;
            console.log(str.split("/")[index])
            return str.split("/")[index];
        }

        $scope.GetVouchersForPrint = function (fileId, page) {
            if (typeof (page) === 'undefined') page = 0;
            var size = 1000;
            $scope.vouchers = VoucherFactory.query({ id: fileId, pageIndex: page, pageSize: size }, function () {
                pageTotal = $scope.vouchers.TotalPages;
                totalCount = $scope.vouchers.TotalCount;
                pageIndex = 0;
                $scope.ShowBatchOptions();
            });
            console.log($scope.vouchers);
        }

        $scope.ShowBatchOptions = function () {
            var optionsAmount = Math.ceil(totalCount / 1000);
            $scope.showOptions = [];
            for (i = 0; i < optionsAmount; i++) {
                $scope.showOptions.push(i);
            }
            console.log($scope.showOptions);
        }
    }
})();