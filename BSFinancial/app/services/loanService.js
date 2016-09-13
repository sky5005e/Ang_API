app.factory("loanService", function ($http, $q) {
    var _loans = [];
    var _loan = new Object();
    var _properties = [];
    var _payments = [];
    var _escrowBalance = 0;
    var _escrowBal = 0;
    var _disbursements = [];
    var _documents = [];
    var _applications = [];
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };

    var _getLoans = function () {
        var deferred = $q.defer();

        $http.get("/api/loan")
        .then(function (result) {
            //success
            angular.copy(result.data, _loans);
            _isInit = true;
            deferred.resolve();
        },
        function () {
            //error
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    };

    var _getApplications = function () {
        var deferred = $q.defer();

        $http.get("/api/application")
        .then(function (result) {
            //success
            angular.copy(result.data, _applications);
            _isInit = true;
            deferred.resolve();
        },
        function () {
            //error
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    };

    var _addLoan = function (newLoan) {

        var deferred = $q.defer();
        $http.post("/api/loan", newLoan)
            .then(function (result) {
                //success
                var newlyCreatedLoan = result.data;
                _loans.splice(0, 0, newlyCreatedLoan);
                deferred.resolve(newlyCreatedLoan);
            },
            function (data) {
                //deferred.resolve(data);
                deferred.reject(data);
            });
        return deferred.promise;

    };

    function _findLoan(id) {
        var found = null;

        $.each(_loans, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }

    var _delLoan = function (id) {
        var deferred = $q.defer();

        $http.get("/api/loan/delete/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _loans);
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

    var _getLoanById = function (id) {
        var deferred = $q.defer();

        $http.get("/api/loan/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _loan);
            angular.copy(result.data.payments, _payments);
            angular.copy(result.data.properties, _properties);
            angular.copy(result.data.documents, _documents);
            angular.copy(result.data.disbursements, _disbursements);
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

    var _savePayment = function (payment, actType, pIndex) {

        var deferred = $q.defer();
        $http.post("/api/payment", payment)
            .then(function (result) {
                //success
                var savedPayment = result.data;

                if (actType == "add") {
                    _payments.splice(0, 0, savedPayment);
                } else {
                    _payments.splice(pIndex, 1, savedPayment);
                }
                deferred.resolve(savedPayment);
            },
            function () {
                //error
            });
        return deferred.promise;
    };
    var _addLoanProperty = function (newProperty, actType, pIndex) {
        var deferred = $q.defer();
        $http.post("/api/property", newProperty)
            .then(function (result) {
                var savedProperties = result.data;
                if (actType == "add") {
                    _properties.splice(0, 0, savedProperties);
                } else {
                    _properties.splice(pIndex, 1, savedProperties);
                }
                deferred.resolve(savedProperties);
            },
            function () {
                //error
            });
        return deferred.promise;
    };


    var _saveLoanProperties = function (loanId, properties) {
        var model = {
            LoanId: loanId,
            Properties: properties
        };
        var deferred = $q.defer();
        $http.post("/api/loan/saveLoanProperties/" + loanId, model)
            .then(function (result) {
                var savedProperties = result.data;
                angular.copy(result.data, _properties);
                deferred.resolve(savedProperties);
            },
            function () {
                //error
            });
        return deferred.promise;
    };

    var _delPayment = function (id) {
        var deferred = $q.defer();

        $http.get("/api/payment/delete/" + id)
        .then(function (result) {
            //success
            deferred.resolve();
            return result.data;
        },
        function () {
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    };

    var _delDocument = function (id) {
        var deferred = $q.defer();

        $http.get("/api/loan/deleteDocument/" + id)
        .then(function (result) {
            //success
            deferred.resolve();
            return result.data;
        },
        function () {
            //alert("could not load inquiries");
            deferred.reject();
        });
        return deferred.promise;
    };

    var _saveDocument = function (loanId, description, file, uploadUrl, actType, pIndex) {

        var deferred = $q.defer();

        var fd = new FormData();
        fd.append('file', file);
        fd.append('loanId', loanId);
        fd.append('description', description);
        $http.post(uploadUrl, fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (result) {
                //success
                var savedDocument = result.data;

                if (actType == "add") {
                    _documents.splice(0, 0, savedDocument);
                } else {
                    _documents.splice(pIndex, 1, savedDocument);
                }
                deferred.resolve(savedDocument);
            },
            function () {
                //error
            });
        return deferred.promise;
    };

    var _saveDisbursement = function (disbursement, actType, pIndex) {

        var deferred = $q.defer();
        $http.post("/api/disbursement", disbursement)
            .then(function (result) {
                //success
                var savedDisbursement = result.data;

                if (actType == "add") {
                    _disbursements.splice(0, 0, savedDisbursement);
                } else {
                    _disbursements.splice(pIndex, 1, savedDisbursement);
                }
                deferred.resolve(savedDisbursement);
            },
            function () {
                //error
            });
        return deferred.promise;
    };

    var _delDisbursement = function (id) {
        var deferred = $q.defer();

        $http.get("/api/disbursement/delete/" + id)
        .then(function (result) {
            //success
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };
    var _escrowBalanceByLoanId = function (id) {
        var deferred = $q.defer();

        $http.get("/api/loan/escrowbalance/" + id)
        .then(function (result) {
            //success
            _escrowBalance = result.data;
            _escrowBal = result.data;
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
        loans: _loans,
        loan: _loan,
        getLoans: _getLoans,
        applications: _applications,
        getApplications: _getApplications,
        addLoan: _addLoan,
        isReady: _isReady,
        getLoanById: _getLoanById,
        deleteLoan: _delLoan,

        payments: _payments,
        properties: _properties,
        documents: _documents,
        savePayment: _savePayment,
        deletePayment: _delPayment,
        saveLoanProperties: _saveLoanProperties,
        addLoanProperty: _addLoanProperty,
        saveDocument : _saveDocument,
        delDocument: _delDocument,
        disbursements: _disbursements,
        saveDisbursement: _saveDisbursement,
        deleteDisbursement: _delDisbursement,
        escrowBalanceByLoanId: _escrowBalanceByLoanId,
        escrowBalance: _escrowBalance,
        escrowBal: _escrowBal
        
    };
});