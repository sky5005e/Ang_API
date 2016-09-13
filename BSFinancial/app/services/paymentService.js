app.factory("paymentService", function ($http, $q) {
    var _payments = [];
    var _isInit = false;
    var _planType = new Object();;
    var _isReady = function () {
        return _isInit;
    };

    var _getPayments = function () {
        var deferred = $q.defer();

        $http.get("/api/payment")
        .then(function (result) {
            //success
            angular.copy(result.data, _payments);
            _isInit = true;
            deferred.resolve();
        },
        function () {
            //error
            //alert("could not load payments");
            deferred.reject();
        });
        return deferred.promise;
    };
    return {
        payments: _payments,
        getPayments: _getPayments,
        isReady: _isReady,
        planType: _planType
    };
});
