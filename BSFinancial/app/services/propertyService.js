app.factory("propertyService", function ($rootScope, $http, $q) {
    var _properties = [];
    var _property = new Object();
    var _nonAssctLoanProperties = [];
    var _documents = [];
    var _photos = [];
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };
    var reqAddPropertyDlg = function () {
        $rootScope.$broadcast('onOpenPropertyDlg');
    };
    var _getProperties = function () {
        var deferred = $q.defer();

        $http.get("/api/property")
        .then(function (result) {
            //success
            angular.copy(result.data, _properties);
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
    var _getNonAssLoanProperties = function (id) {
        var deferred = $q.defer();

        $http.get("/api/property/nonassctloanproperties/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _nonAssctLoanProperties);
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
    var _addProperty = function (newProperty) {

        var deferred = $q.defer();
        $http.post("/api/property", newProperty)
            .then(function (result) {
                //success
                var newlyCreatedProperty = result.data;
                _properties.splice(0, 0, newlyCreatedProperty);
                deferred.resolve(newlyCreatedProperty);
            },
            function () {
                //error
            });
        return deferred.promise;

    };

    function _findProperty(id) {
        var found = null;

        $.each(_properties, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }
    var _delProperty = function (id) {
        var deferred = $q.defer();

        $http.get("/api/property/delete/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _properties);
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
    var _removeProperty = function (id) {
        var deferred = $q.defer();

        $http.get("/api/property/remove/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _properties);
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
    var _getPropertyById = function (id) {
        var deferred = $q.defer();

        $http.get("/api/property/" + id)
         .then(function (result) {
             //success
             angular.copy(result.data, _property);
             angular.copy(result.data.documents, _documents);
             angular.copy(result.data.photos, _photos);

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

    var _delDocument = function (id,propertyId) {
        var model = {
            id: id,
            childId: propertyId
        };
        
        var deferred = $q.defer();

        $http.post("/api/property/deleteDocument/" + id, model)
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

    var _saveDocument = function (propertyId, description, file, uploadUrl, actType, pIndex) {

        var deferred = $q.defer();

        var fd = new FormData();
        fd.append('file', file);
        fd.append('propertyId', propertyId);
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
    var _savePhoto = function (propertyId, description, file, uploadUrl, actType, pIndex) {

        var deferred = $q.defer();

        var fd = new FormData();
        fd.append('file', file);
        fd.append('propertyId', propertyId);
        fd.append('description', description);
        $http.post(uploadUrl, fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (result) {
            //success
            var savedPhoto = result.data;

            if (actType == "add") {
                _photos.splice(0, 0, savedPhoto);
            } else {
                _photos.splice(pIndex, 1, savedPhoto);
            }
            deferred.resolve(savedPhoto);
        },
            function () {
                //error
            });
        return deferred.promise;
    };


    return {
        properties: _properties,
        property: _property,
        nonAssctLoanProperties: _nonAssctLoanProperties,
        getProperties: _getProperties,
        getNonAssLoanProperties: _getNonAssLoanProperties,
        addProperty: _addProperty,
        isReady: _isReady,
        getPropertyById: _getPropertyById,
        deleteProperty: _delProperty,
        openNewPropertyDlg: reqAddPropertyDlg,
        removeProperty: _removeProperty,
        documents: _documents,
        photos: _photos,
        saveDocument: _saveDocument,
        savePhoto: _savePhoto,
        delDocument: _delDocument


    };
});
