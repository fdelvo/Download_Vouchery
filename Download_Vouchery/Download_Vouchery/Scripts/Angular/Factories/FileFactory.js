(function () { 
angular.module('DownloadVoucheryApp').factory('FileFactory', FileFactory);

FileFactory.$inject = ['$resource'];

function FileFactory ($resource) {
    return $resource('/api/blobs/:id'); 
}
})();