app.factory("applicationService", function ($rootScope, $http, $q) {
    var _applications = [];
    var _application = new Object();
    var _employments = [];
    var _addrs = [];
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };

    var reqAddApplicantDlg = function () {
        $rootScope.$broadcast('onOpenApplicantDlg');
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

    var _newApplication = function (newApp) {

        var deferred = $q.defer();
        $http.post("/api/application", newApp)
            .then(function (result) {
                //success
                deferred.resolve(result.data);
            },
            function (data) {
                //error
                deferred.reject(data);
            });
        return deferred.promise;

    };

    var _saveApplication = function (app) {

        var deferred = $q.defer();
        $http.post("/api/application", app)
            .then(function (result) {
                //success
                var newlyCreatedApplication = result.data;
                _applications.splice(0, 0, newlyCreatedApplication);
                deferred.resolve(newlyCreatedApplication);
            },
            function (data) {
                //error
                deferred.reject(data);
            });
        return deferred.promise;

    };

    function _findApplication(id) {
        var found = null;

        $.each(_applications, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }

    var _delApplication = function (id) {
        var deferred = $q.defer();

        $http.get("/api/application/delete/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _applications);
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

    var _getApplicationById = function (id) {
        var deferred = $q.defer();

        $http.get("/api/application/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _application);
            angular.copy(result.data.employments, _employments);
            angular.copy(result.data.addrs, _addrs);
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

    var _saveAddr = function (addr, actType, pIndex) {

        var deferred = $q.defer();
        $http.post("/api/address", addr)
            .then(function (result) {
                //success
                var savedAddr = result.data;

                if (actType == "add") {
                    _addrs.splice(_addrs.length, 0, savedAddr);
                } else {
                    _addrs.splice(pIndex, 1, savedAddr);
                }
                deferred.resolve(savedAddr);
            },
            function () {
                //error
            });
        return deferred.promise;
    };

    var _delAddr = function (id) {
        var deferred = $q.defer();
        $http.get("/api/address/delete/" + id)
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

    var _saveEmployment = function (employment, actType, pIndex) {

        var deferred = $q.defer();
        $http.post("/api/employment", employment)
            .then(function (result) {
                //success
                var savedEmployment = result.data;

                if (actType == "add") {
                    _employments.splice(_employments.length, 0, savedEmployment);
                } else {
                    _employments.splice(pIndex, 1, savedEmployment);
                }
                deferred.resolve(savedEmployment);
            },
            function () {
                //error
            });
        return deferred.promise;
    };

    var _delEmployment = function (id) {
        var deferred = $q.defer();
        $http.get("/api/employment/delete/" + id)
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

    var _saveApplicants = function (app) {

        var deferred = $q.defer();

        $http.post("/api/application", app)
            .then(function (result) {
                //success
                deferred.resolve(result.data);
            },
            function (data) {
                //error
                deferred.reject(data);
            });
        return deferred.promise;

    };

    var _setMainApplicant = function (id, applicantId) {

        var deferred = $q.defer();

        $http.put("/api/application/setMainApplicant/" + id, applicantId)
            .then(function (result) {
                //success
                deferred.resolve(result.data);
            },
            function (data) {
                //error
                deferred.reject(data);
            });
        return deferred.promise;

    };

    var _removeApplicant = function (id, applicantId) {

        var deferred = $q.defer();

        $http.put("/api/application/removeApplicant/" + id, applicantId)
            .then(function (result) {
                //success
                deferred.resolve(result.data);
            },
            function (data) {
                //error
                deferred.reject(data);
            });
        return deferred.promise;

    };

    return {
        application: _application,
        getApplications: _getApplications,
        applications: _applications,
        newApplication: _newApplication,
        saveApplication: _saveApplication,
        isReady: _isReady,
        getApplicationById: _getApplicationById,
        deleteApplication: _delApplication,
        saveApplicants: _saveApplicants,
        setMainApplicant: _setMainApplicant,
        removeApplicant: _removeApplicant,

        employments: _employments,
        saveEmployment: _saveEmployment,
        deleteEmploy: _delEmployment,

        addrs: _addrs,
        saveAddr: _saveAddr,
        deleteAddr: _delAddr,
        openNewAppDlg: reqAddApplicantDlg
    };
});