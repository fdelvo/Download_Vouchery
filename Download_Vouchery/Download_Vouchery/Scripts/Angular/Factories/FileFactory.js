angular.module('DownloadVoucheryApp').factory('FileFactory', ['$resource', function ($resource) {
    return $resource('/api/blobs/:id'); 
}]);