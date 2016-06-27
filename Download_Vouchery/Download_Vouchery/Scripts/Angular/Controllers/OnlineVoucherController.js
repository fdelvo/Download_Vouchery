angular.module("DownloadVoucheryApp").controller("OnlineVoucherController", OnlineVoucherController);

OnlineVoucherController.$inject = [
    "$scope", "$rootScope", "$filter", "$route", "$http", "VoucherFactory"
];

function OnlineVoucherController($scope, $rootScope, $filter, $route, $http, VoucherFactory) {
    $scope.onlineVoucher = new VoucherFactory();

    $scope.GetValueAtIndex = function (index) {
        var str = window.location.href;
        console.log(str.split("/")[index]);
        return str.split("/")[index];
    };

    $scope.SendOnlineVoucher = function(fileId) {
        $scope.onlineVoucher.$GenerateOnlineVoucher({ voucherFileId: fileId },
            function () {
                $rootScope.status = "Voucher sent.";
            },
            function (error) {
                $rootScope.status = error.data.Message;
            });
    };
}