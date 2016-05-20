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
        var page = 0;
        var size = 10;
        $scope.pageIndex = 0;

        $scope.CreateVoucher = function (fileId, index) {
            $scope.newVoucher.VoucherAmount = $scope.voucherAmount[index];
            $scope.newVoucher.$save({ id: fileId }, function () { window.location.reload(); });
        }

        $scope.GetVouchers = function (fileId, page, size) {
            page = 0;
            $scope.vouchers = [];
            $scope.pageIndex = 0;
            $scope.vouchers.push(VoucherFactory.query({ id: fileId, pageIndex: page, pageSize: size }));
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
            $scope.pageIndex--;
            if ($scope.pageIndex <= 0) {
                $scope.pageIndex = 0;
            }
        };

        $scope.nextPage = function (fileId) {
            page++;
            $scope.vouchersTemp = VoucherFactory.query({ id: fileId, pageIndex: page, pageSize: size }, function () {
                if ($scope.vouchersTemp.length > 0) {
                    $scope.vouchers.push($scope.vouchersTemp);
                    $scope.pageIndex++;
                }
            });
        };

        $scope.GetValueAtIndex = function (index) {
            var str = window.location.href;
            console.log(str.split("/")[index])
            return str.split("/")[index];
        }
    }
})();