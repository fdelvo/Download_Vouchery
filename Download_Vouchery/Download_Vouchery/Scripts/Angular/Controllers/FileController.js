(function () {
    angular.module('DownloadVoucheryApp').controller('FileController', FileController);

    FileController.$inject = ['$scope', '$filter', '$route', 'FileFactory', 'UploadFactory', 'VoucherFactory'];

    function FileController($scope, $filter, $route, FileFactory, UploadFactory, VoucherFactory) {
        $scope.files = [];
        $scope.vouchersRaw = [];
        $scope.vouchers = [];
        $scope.newVoucher = new VoucherFactory();
        $scope.voucherOptions = false;
        $scope.fileName = "";
        $scope.voucherAmount = 0;
        $scope.voucherAmountRedeemed = 0;
        $scope.voucherAmountNotRedeemed = 0;

        $scope.CreateVoucher = function (fileId) {
            $scope.newVoucher.$save({ id: fileId }, function () { window.location.reload(); });
        }

        $scope.GetVouchers = function () {
            $scope.vouchersRaw = VoucherFactory.query();
        }

        $scope.ShowVoucherOptions = function (fileName) {
            $scope.voucherOptions = true;
            $scope.fileName = fileName;
        }

        $scope.FilterVouchers = function (fileId) {
            $scope.vouchers = [];
            $scope.voucherAmountRedeemed = 0;
            $scope.voucherAmountNotRedeemed = 0;
            $scope.vouchersRaw.filter(function (item) {
                if (item.VoucherFileId.FileId === fileId) {
                    if (item.VoucherRedeemed === true) {
                        $scope.voucherAmountRedeemed++;
                    } else {
                        $scope.voucherAmountNotRedeemed++;
                    }
                    $scope.vouchers.push(item);
                }
            });
            $scope.voucherAmount = $scope.vouchers.length;
            $scope.search();
        }

        $scope.GetFiles = function () {
            $scope.files = FileFactory.query();
        }

        $scope.uploadFile = function () {
            var file = $scope.myFile;

            console.log('file is ');
            console.dir(file);

            var uploadUrl = "/blobs/upload";
            UploadFactory.uploadFileToUrl(file, uploadUrl);
        };

        $scope.sort = {
            sortingOrder: 'VoucherCreationDate',
            reverse: true
        };

        $scope.gap = 5;

        $scope.filteredItems = [];
        $scope.groupedItems = [];
        $scope.itemsPerPage = 10;
        $scope.pagedItems = [];
        $scope.currentPage = 0;

        var searchMatch = function (haystack, needle) {
            if (!needle) {
                return true;
            }
            return haystack.toLowerCase().indexOf(needle.toLowerCase()) !== -1;
        };

        // init the filtered vouchers
        $scope.search = function () {
            $scope.filteredItems = $filter('filter')($scope.vouchers, function (item) {
                for (var attr in item) {
                    if (searchMatch(item[attr], $scope.query))
                        return true;
                }
                return false;
            });
            // take care of the sorting order
            if ($scope.sort.sortingOrder !== '') {
                $scope.filteredItems = $filter('orderBy')($scope.filteredItems, $scope.sort.sortingOrder, $scope.sort.reverse);
            }
            $scope.currentPage = 0;
            // now group by pages
            $scope.groupToPages();
        };


        // calculate page in place
        $scope.groupToPages = function () {
            $scope.pagedItems = [];

            for (var i = 0; i < $scope.filteredItems.length; i++) {
                if (i % $scope.itemsPerPage === 0) {
                    $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)] = [$scope.filteredItems[i]];
                } else {
                    $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)].push($scope.filteredItems[i]);
                }
            }
        };

        $scope.range = function (size, start, end) {
            var ret = [];
            console.log(size, start, end);

            if (size < end) {
                end = size;
                start = size - $scope.gap;
            }
            for (var i = start; i < end; i++) {
                ret.push(i);
            }
            console.log(ret);
            return ret;
        };

        $scope.prevPage = function () {
            if ($scope.currentPage > 0) {
                $scope.currentPage--;
            }
        };

        $scope.nextPage = function () {
            if ($scope.currentPage < $scope.pagedItems.length - 1) {
                $scope.currentPage++;
            }
        };

        $scope.setPage = function () {
            $scope.currentPage = this.n;
        };
    }
})();