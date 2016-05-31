(function () { 
angular.module('DownloadVoucheryApp').factory('VoucherFactory', VoucherFactory);

VoucherFactory.$inject = ['$resource'];

function VoucherFactory ($resource) {
    return $resource('/api/vouchers', null, {
            GetVouchersPaged: { method: 'GET', url: '/api/vouchers/:id/:pageIndex/:pageSize', isArray: false },
            Reset: { method: 'PUT', url: '/api/vouchers/voucher/:id/reset' },
            GetVouchersInfo: { metod: 'GET', url: '/api/vouchers/getvouchersinfo/:id' },
            GetVoucherDetails: { method: 'GET', url: '/api/vouchers/voucher/:id' },
            PostVouchers: { method: 'POST', url: '/api/vouchers/new/:voucherAmount/:voucherFileId' },
            DeleteVouchers: { method: 'DELETE', url: '/api/vouchers/delete' }
        });
}
})();