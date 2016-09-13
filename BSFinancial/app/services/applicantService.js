app.factory("applicantService", function ($http, $q) {
    var _applicants = [];
    var _applicant = new Object();
    var _applications = [];
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };

    var _getApplicants = function () {
        var deferred = $q.defer();

        $http.get("/api/applicant")
        .then(function (result) {
            //success
            angular.copy(result.data, _applicants);
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

    var _addApplicant = function (newApplicant) {

        var deferred = $q.defer();
        $http.post("/api/applicant", newApplicant)
            .then(function (result) {
                //success
                var newlyCreated = result.data;
                _applicants.splice(0, 0, newlyCreated);
                deferred.resolve(newlyCreated);
            },
            function () {
                //error
                deferred.reject();
            });
        return deferred.promise;

    };

    var _editApplicant = function (applicant) {

        var deferred = $q.defer();
        $http.post("/api/applicant", applicant)
            .then(function (result) {
                //success
                var edititem = result.data;
                _applicants.splice(0, 0, edititem);
                deferred.resolve(edititem);
            },
            function () {
                //error
                deferred.reject();
            });
        return deferred.promise;

    };

    function _findapplicant(id) {
        var found = null;

        $.each(_applicants, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }

    var _delApplicant = function (id) {
        var deferred = $q.defer();

        $http.get("/api/applicant/delete/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _applicants);
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

    var _getApplicantById = function (id) {
        var deferred = $q.defer();

        $http.get("/api/applicant/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _applicant);
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
        applicants: _applicants,
        applicant: _applicant,
        getApplicants: _getApplicants,
        applications: _applications,
        getApplications: _getApplications,
        addApplicant: _addApplicant,
        editApplicant: _editApplicant,
        isReady: _isReady,
        getApplicantById: _getApplicantById,
        deleteApplicant: _delApplicant
    };
});