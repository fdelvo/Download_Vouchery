(function () {
    angular.module('DownloadVoucheryApp').controller('FileController', FileController);

    FileController.$inject = ['$scope', '$filter', '$route', '$http', 'FileFactory', 'UploadFactory', 'VoucherFactory'];

    function FileController($scope, $filter, $route, $http, FileFactory, UploadFactory, VoucherFactory) {
        $scope.files = [];
        $scope.vouchers = [];
        $scope.newVoucher = new VoucherFactory();
        $scope.voucherOptions = false;
        $scope.fileName = "";
        $scope.fileId = "";
        $scope.vouchersInfo = [];
        var page = 0;
        var size = 10;

        $scope.CreateVoucher = function (fileId) {
            $scope.newVoucher.$save({ id: fileId }, function () { window.location.reload(); });
        }

        $scope.GetVouchers = function (fileId) {
            page = 0;
            $scope.vouchers = VoucherFactory.query({ id: fileId, pageIndex: page, pageSize: size });           
        }

        $scope.GetVouchersInfo = function (fileId) {
            $scope.vouchersInfo = $http.get(' http://localhost:54809/api/vouchers/getvouchersinfo/' + fileId).then(function (response) { $scope.vouchersInfo = response.data});
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
            page--;
            $scope.vouchers = VoucherFactory.query({ id: fileId, pageIndex: page, pageSize: size });
        };

        $scope.nextPage = function (fileId) {
            page++;
            $scope.vouchers = VoucherFactory.query({ id: fileId, pageIndex: page, pageSize: size });
        };

        $scope.GetValueAtIndex = function (index) {
            var str = window.location.href;
            console.log(str.split("/")[index])
            return str.split("/")[index];
        }
    }
})();