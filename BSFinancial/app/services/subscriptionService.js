app.factory("subscriptionService", function ($http, $q) {
    var _subscription = new Object();
    var _fundSubscription = new Object();
    var _propertySubscription = new Object();
    var _loanSubscription = new Object();

    var _getFundSubscription = function () {
        var deferred = $q.defer();
        $http.get("/api/subscription/GetFund")
        .then(function (result) {
            //success
            if (result.data != null) {
                angular.copy(result.data, _fundSubscription);
            }
            _isInit = true;
            deferred.resolve();
        },
        function () {
            //error
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    }
    var _getPropertySubscription = function () {
        var deferred = $q.defer();
        $http.get("/api/subscription/GetProperty")
        .then(function (result) {
            //success
            if (result.data != null) {
                angular.copy(result.data, _propertySubscription);
            }
            _isInit = true;
            deferred.resolve();
        },
        function () {
            //error
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    }

    var _getLoanSubscription = function () {
        var deferred = $q.defer();
        $http.get("/api/subscription/GetLoan")
        .then(function (result) {
            //success
            if (result.data != null) {
                angular.copy(result.data, _loanSubscription);
            }
            _isInit = true;
            deferred.resolve();
        },
        function () {
            //error
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    }

    var _updateSubscription = function (planType,cardDetails) {
        var deferred = $q.defer();
        var model;
        if (cardDetails.paymentTokenNo == null) {
            model = {
                cardNumber: cardDetails.cardNumber,
                CardExpiryMonth: cardDetails.cardMonthExpiry,
                FirstName: cardDetails.cardFirstName,
                LastName: cardDetails.cardLastName,
                CardExpiryYear: cardDetails.cardYearExpiry,
                FirstName: cardDetails.firstName,
                LastName: cardDetails.lastName,
                EmailAddress: cardDetails.emailAddress,
                CountryId: cardDetails.countryId,
                Address: cardDetails.address,
                Name: cardDetails.name,
                ZipCode: cardDetails.zipCode
            };
        }
        else {
            model = {
                PaymentTokenNo: cardDetails.paymentTokenNo
            }
        }
       

        $http.post("/api/subscription/UpdateSubscription/" + planType, model)
            .then(function (result) {
                //success
                
                deferred.resolve();
            },
            function (a,b,c) {
                //error
            });
        return deferred.promise;
    }
    var _getSubscription = function () {
        var deferred = $q.defer();
        $http.get("/api/subscription")
        .then(function (result) {
            //success
            if (result.data != null) {
                angular.copy(result.data, _subscription);
            }
            _isInit = true;
            deferred.resolve();
        },
        function () {
            //error
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    }

    return {
        subscription: _subscription,
        getSubscription: _getSubscription,
        getFundSubscription: _getFundSubscription,
        getPropertySubscription: _getPropertySubscription,
        getLoanSubscription: _getLoanSubscription,
        fundSubscription: _fundSubscription,
        propertySubscription: _propertySubscription,
        loanSubscription: _loanSubscription,
        updateSubscription: _updateSubscription

    };
});