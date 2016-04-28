angular.module('DownloadVoucheryApp').directive("loader", function ($rootScope) {
    return function ($scope, element, attrs) {
        $scope.$on("loader_show", function () {
            return $scope.showLoader = true;
        });
        return $scope.$on("loader_hide", function () {
            return $scope.showLoader = false;
        });
    };
}
)