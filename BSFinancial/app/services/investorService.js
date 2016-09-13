app.factory("investorService", function ($http, $q, $rootScope) {
    var _investors = [];
    var _investor = new Object();
    var _isInit = false;
    
    var _isReady = function () {
        return _isInit;
    };

    var reqAddInvestorDlg = function () {
        $rootScope.$broadcast('onOpenInvestorDlg');
    };
    var _getinvestors = function () {
        var deferred = $q.defer();

        $http.get("/api/investor")
        .then(function (result) {
            //success
            angular.copy(result.data, _investors);
            _isInit = true;
            deferred.resolve();
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };

    var _addinvestor = function (newinvestor) {

        var deferred = $q.defer();
        $http.post("/api/investor", newinvestor)
            .then(function (result) {
                //success
                var newlyCreatedinvestor = result.data;
                _investors.splice(0, 0, newlyCreatedinvestor);
                deferred.resolve(newlyCreatedinvestor);
            },
            function (data) {
                //deferred.resolve(data);
                deferred.reject(data);
            });
        return deferred.promise;

    };

    function _findinvestor(id) {
        var found = null;

        $.each(_investors, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }

    var _delinvestor = function (id) {
        var deferred = $q.defer();

        $http.get("/api/investor/delete/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _investors);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };

    var _getinvestorById = function (id) {
        var deferred = $q.defer();

        $http.get("/api/investor/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _investor);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };
    var _removeInvestor = function (id) {
        var deferred = $q.defer();

        $http.get("/api/investor/remove/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _investors);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    };

    return {
        investors: _investors,
        investor: _investor,
        getInvestors: _getinvestors,
        addInvestor: _addinvestor,
        isReady: _isReady,
        getInvestorById: _getinvestorById,
        openNewInvestorDlg: reqAddInvestorDlg,
        deleteInvestor: _delinvestor,
        removeInvestor: _removeInvestor
    };
});