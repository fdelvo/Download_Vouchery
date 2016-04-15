angular.module('DownloadVoucheryApp').controller('FileController', ['$scope', 'FileFactory', 'UploadFactory', 'VoucherFactory', function ($scope, FileFactory, UploadFactory, VoucherFactory) {
    $scope.files = [];
    $scope.vouchers = [];
    $scope.newVoucher = new VoucherFactory();

    $scope.CreateVoucher = function (fileId) {
        $scope.newVoucher.$save({ id: fileId });
    }

    $scope.GetVouchers = function () {
        $scope.vouchers = VoucherFactory.query();
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
}]);