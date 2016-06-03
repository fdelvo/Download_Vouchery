(function () {
    angular.module('DownloadVoucheryApp').controller('VoucherController', VoucherController);

    VoucherController.$inject = ['$scope', '$rootScope', '$filter', '$route', '$http', 'FileFactory', 'UploadFactory', 'VoucherFactory'];

    function VoucherController($scope, $rootScope, $filter, $route, $http, FileFactory, UploadFactory, VoucherFactory) {
        $scope.Download = function () {
            $http.get('/api/blobs/' + $scope.download.VoucherCode + '/download').then(function (response) {
                window.open('/api/blobs/' + $scope.download.VoucherCode + '/download', '_blank', '');
            },
            function (response) {
                $rootScope.status = response.statusText;
                console.log(response.statusText);
            });
        }
    }
})();