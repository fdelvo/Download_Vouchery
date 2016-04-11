angular.module('DownloadVoucheryApp').factory('VoucherFactory', ['$resource', function ($resource) {
    return $resource('/api/Vouchers/:id'); 
}]);