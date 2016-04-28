angular.module('DownloadVoucheryApp').controller('VoucherController', ['$scope', '$filter', '$route', 'FileFactory', 'UploadFactory', 'VoucherFactory', function ($scope, $filter, $route, FileFactory, UploadFactory, VoucherFactory) {

    $scope.Download = function (blobId) {
        $http.get('/blobs/' + $scope.download.VoucherCode + '/download');
    }
}]);