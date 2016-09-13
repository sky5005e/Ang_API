app.factory("fundService", function ($http, $q, $location, $modal,notificationService) {
    var _funds = [];
    var _fund = new Object();
    var _isInit = false;
    var _fundInvestors = [];
    var _investors = [];
    var _fundDistribution = new Object();
    var _fundDistributions = [];
    var _isAddNewInverstor = false;
    var _selectedFundInvestor = [];
    var _selectedRowInvestor = 0;
    var _isInvestorEdit = false;

    var _isReady = function () {
        return _isInit;
    };

    var _getfunds = function () {
        var deferred = $q.defer();

        $http.get("/api/fund")
        .then(function (result) {
            //success
            angular.copy(result.data, _funds);
            _isInit = true;
            deferred.resolve();
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };

    var _addfund = function (newfund) {

        var deferred = $q.defer();
        $http.post("/api/fund", newfund)
            .then(function (result) {
                //success
                var newlyCreatedfund = result.data;
                _funds.splice(0, 0, newlyCreatedfund);
                deferred.resolve(newlyCreatedfund);
                $location.path("editfund/" + newlyCreatedfund.id);
            },
            function (data) {
                //deferred.resolve(data);
                deferred.reject(data);
            });
        return deferred.promise;

    };

    function _findfund(id) {
        var found = null;

        $.each(_funds, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }

    var _delfund = function (id) {
        var deferred = $q.defer();

        $http.get("/api/fund/delete/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _funds);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };

    var _delInvestorDistribution = function (id) {
        var deferred = $q.defer();

        $http.get("/api/fund/deleteInvestorDistribution/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _fundDistributions);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };

    var _getfundById = function (id) {
        var deferred = $q.defer();

        $http.get("/api/fund/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _fund);
            //angular.copy(result.data.fundInvestors, _fundInvestors);
            //angular.copy(result.data.fundDistributions, _findDistributions);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };
    var _addFundInvestor = function (newInvestor, actType, pIndex) {
        var deferred = $q.defer();
        $http.post("/api/investor", newInvestor)
            .then(function (result) {
                var savedInvestors = result.data;
                if (actType == "add") {
                    _investors.splice(0, 0, savedInvestors);
                } else {
                    _investors.splice(pIndex, 1, savedInvestors);
                }
                deferred.resolve(savedInvestors);
            },
            function () {
                //error
            });
        return deferred.promise;
    };
    var _saveFundInvestors = function (fundId, investors) {
        var model = {
            FundId: fundId,
            Investors: investors
        };
        var deferred = $q.defer();
        $http.post("/api/fund/saveFundInvestors/" + fundId, model)
            .then(function (result) {
                var savedInvestors = result.data;
                angular.copy(result.data, _fundInvestors);
                deferred.resolve(savedInvestors);
            },
            function () {
                //error
            });
        return deferred.promise;
    };
    var _updateFundInvestor = function (fundId, investor) {
        var model = {
            FundId: fundId,
            Investor: investor
        };
        var deferred = $q.defer();
        $http.post("/api/fund/updateFundInvestors/" + fundId, model)
            .then(function (result) {
                var savedInvestors = result.data;
                angular.copy(result.data, _fundInvestors);
                deferred.resolve(savedInvestors);
            },
            function () {
                //error
            });
        return deferred.promise;
    };

    var _saveFundDistribution = function (fundId, distribution) {
        var deferred = $q.defer();
        $http.post("/api/fund/saveFundDistribution/" + fundId, distribution)
            .then(function (result) {
                var savedDistributions = result.data;
                angular.copy(result.data, _fundDistributions);
                deferred.resolve(savedDistributions);
            },
            function () {
                //error
            });
        return deferred.promise;
    };

    var _openAddFundInvestorDlg = function openAddFundInvestorDlg() {
        var modalInstance = $modal.open({
            //templateUrl: '/app/views/fund/selectInvestor.html',
            templateUrl: '/app/views/fund/selInvestor.html',
            controller: 'SelInvestorModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                fund: function () {
                    return _funds;// $scope.fund;
                },
                sellist: function () {
                    return '';// $scope.selectedInvestors;
                }
            }
        });
        modalInstance.result.then(function (retinvestorslist) {
            notificationService.success("Successfully saved.");
            //$scope.fund.fundInvestors = retinvestorslist;
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save property.");
            }
        });
    }

    return {
        funds: _funds,
        fund: _fund,
        getFunds: _getfunds,
        isAddNewInvestor: _isAddNewInverstor,
        addFund: _addfund,
        isReady: _isReady,
        getFundById: _getfundById,
        addFundInvestor: _addFundInvestor,
        deleteFund: _delfund,
        fundInvestors: _fundInvestors,
        investors: _investors,
        saveFundInvestors: _saveFundInvestors,
        updateFundInvestor: _updateFundInvestor,
        fundDistribution: _fundDistribution,
        fundDistributions: _fundDistributions,
        saveFundDistribution: _saveFundDistribution,
        deleteDistribution: _delInvestorDistribution,
        openAddFundInvestorDlg: _openAddFundInvestorDlg,
        selectedFundInvestor: _selectedFundInvestor,
        selectedRowInvestor: _selectedRowInvestor,
        isInvestorEdit: _isInvestorEdit
    };
});