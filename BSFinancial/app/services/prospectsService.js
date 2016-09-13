app.factory("prospectService", function ($rootScope, $http, $q) {
    var _prospects = [];
    var _prospect = new Object();
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };
    var reqAddProspectDlg = function () {
        $rootScope.$broadcast('openNewProspectDlg');
    };
    var _getProspects = function () {
        var deferred = $q.defer();

        $http.get("/api/prospect")
        .then(function (result) {
            //success
            angular.copy(result.data, _prospects);
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
    var _addProspect = function (newProspect, actType, pIndex) {

        var deferred = $q.defer();
        $http.post("/api/prospect", newProspect)
            .then(function (result) {
                //success
                var newlyCreatedProspect = result.data;
                if (actType == "add") {
                    _prospects.splice(0, 0, newlyCreatedProspect);
                } else {
                    _prospects.splice(pIndex, 1, newlyCreatedProspect);
                }
                deferred.resolve(newlyCreatedProspect);
            },
            function (data) {
                //deferred.resolve(data);
                deferred.reject(data);
            });
        return deferred.promise;

    };
    function _findProspect(id) {
        var found = null;

        $.each(_prospects, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }
    var _delProspect = function (id) {
        var deferred = $q.defer();

        $http.get("/api/prospect/delete/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _prospects);
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
        prospects: _prospects,
        prospect: _prospect,
        addProspect: _addProspect,
        isReady: _isReady,
        onOpenNewProspectDlg: reqAddProspectDlg,
        getProspects: _getProspects,
        delProspect: _delProspect
    };
});
