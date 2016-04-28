(function () { 
angular.module('DownloadVoucheryApp').factory('VoucherFactory', VoucherFactory);

VoucherFactory.$inject = ['$resource'];

function VoucherFactory ($resource) {
    return $resource('/api/vouchers/:id');
}
})();