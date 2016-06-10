(function () {
    angular.module('DownloadVoucheryApp').controller('VoucherController', VoucherController);

    VoucherController.$inject = ['$scope', '$rootScope', '$filter', '$route', '$http', 'FileFactory', 'UploadFactory', 'VoucherFactory'];

    function VoucherController($scope, $rootScope, $filter, $route, $http, FileFactory, UploadFactory, VoucherFactory) {
        $scope.Download = function () {
            $http.get('/api/blobs/' + $scope.download.VoucherCode + '/download/true').then(function (response) {
                window.open('/api/blobs/' + $scope.download.VoucherCode + '/download/false', '_blank', '');
            },
            function (response) {
                $rootScope.status = response.statusText;
                console.log(response.statusText);
            });
        }
    }
})();