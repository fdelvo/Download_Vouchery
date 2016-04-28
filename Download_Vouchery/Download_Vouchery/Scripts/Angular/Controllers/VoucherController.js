(function () {
    angular.module('DownloadVoucheryApp').controller('VoucherController', VoucherController);

    VoucherController.$inject = ['$scope', '$filter', '$route', 'FileFactory', 'UploadFactory', 'VoucherFactory'];

    function VoucherController($scope, $filter, $route, FileFactory, UploadFactory, VoucherFactory) {
        $scope.Download = function (blobId) {
            $http.get('/blobs/' + $scope.download.VoucherCode + '/download');
        }
    }
})();