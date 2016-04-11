angular.module('DownloadVoucheryApp').controller('VoucherController', ['$scope', 'VoucherFactory', function ($scope, VoucherFactory) {
    $scope.vouchers = [];
    $scope.newVoucher = new VoucherFactory();

    $scope.GetVouchers = function () {
        $scope.vouchers = VoucherFactory.query();
    }

    $scope.CreateVoucher = function () {
        
        $scope.newVoucher.$save();
    }
}]);