app.factory("propertyProposalService", function ($rootScope, $http, $q) {
    var _proposals = [];
    var _proposal = new Object();
    var _prospects = [];
    var _properties = [];
    var _viewedproperties = [];
    var _photos = [];
    var _rating = new Object();
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };
    var reqAddProposalDlg = function () {
        $rootScope.$broadcast('onOpenProposalDlg');
    };
    var _getProspects = function () {
        var deferred = $q.defer();

        $http.get("/api/propertyproposal")
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
    var _getProposals = function () {
        var deferred = $q.defer();
        var id = 0; // will implement as per propertyId
        $http.get("/api/propertyproposal/getProposal/"+id)
        .then(function (result) {
            //success
            angular.copy(result.data, _proposals);
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
    var _getProposalById = function (id) {
        var deferred = $q.defer();

        $http.get("/api/propertyproposal/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _proposal);
            angular.copy(result.data.properties, _properties);
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
    var _addProspects = function (model) {
        
        var deferred = $q.defer();
        $http.post("/api/propertyproposal/savePropertyProspect/" + model.prospectId, model)
            .then(function (result) {
                //success
                var newlyCreatedProposal = result.data;
                angular.copy(result.data, _prospects);
                deferred.resolve(newlyCreatedProposal);
            },
            function (data) {
                //deferred.resolve(data);
                deferred.reject(data);
            });
        return deferred.promise;

    };
    var _addProposal = function (newProposal) {

        var deferred = $q.defer();
        $http.post("/api/propertyproposal", newProposal)
            .then(function (result) {
                //success
                var newlyCreatedProposal = result.data;
                _proposals.splice(0, 0, newlyCreatedProposal);
                deferred.resolve(newlyCreatedProposal);
            },
            function (data) {
                //deferred.resolve(data);
                deferred.reject(data);
            });
        return deferred.promise;

    };
    var _saveProperties = function (proposalId, properties) {
        var model = {
            ProposalId: proposalId,
            Properties: properties
        };
        var deferred = $q.defer();
        $http.post("/api/propertyproposal/saveProposalProperties/" + proposalId, model)
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
    var _removeProperty = function (model) {
        var deferred = $q.defer();
        $http.post("/api/propertyproposal/removeProposalProperty/" + model.Id, model)
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

    function _findProposal(id) {
        var found = null;

        $.each(_proposals, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }
    var _delProposal = function (id) {
        var deferred = $q.defer();

        $http.get("/api/propertyproposal/delete/" + id)
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
    var _sendEmailProposal = function (id) {
        var deferred = $q.defer();

        $http.get("/api/propertyproposal/sendemail/" + id)
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
    var _getPropertyViewDetails = function (proposalID) {
        var deferred = $q.defer();
        $http.get("/api/propertyproposal/getPropertyViewDetails/" + proposalID)
        .then(function (result) {
            //success
            angular.copy(result.data, _viewedproperties);
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
    var _savePropertyViewDetails = function (id, propertyId) {
        var model = {
            id: id,
            childId: propertyId
        };

        var deferred = $q.defer();

        $http.post("/api/propertyproposal/savePropertyViewDetails/" + id, model)
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
    // get photos
    var _getPhotosbyPropoertyId = function (id) {
        var deferred = $q.defer();
        $http.get("/api/propertyproposal/getPhotosbyPropoertyId/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _photos);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });

        return deferred.promise;
    };
    var _saveRatingInfo = function (id, rating) {
       
        var deferred = $q.defer();

        $http.post("/api/propertyproposal/saveRatingInfo/" + id, rating)
        .then(function (result) {
            //success
            angular.copy(result.data, _rating);
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
    var _getRatingInfo = function (id) {
        var deferred = $q.defer();
        $http.get("/api/propertyproposal/getPropertyRating/" + id)
        .then(function (result) {
            //success
            angular.copy(result.data, _rating);
            _isInit = true;
            deferred.resolve();
            return result.data;
        },
        function () {
            deferred.reject();
        });

        return deferred.promise;
    };
    var _savePropertyViewTime = function (id, propertyviewinfo) {
       
        var deferred = $q.defer();

        $http.post("/api/propertyproposal/savePropertyViewedTime/" + id, propertyviewinfo)
        .then(function (result) {
            //success
            angular.copy(result.data, _viewedproperties);
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
        proposals: _proposals,
        prospects: _prospects,
        proposal: _proposal,
        properties: _properties,
        viewedproperties: _viewedproperties,
        addProposal: _addProposal,
        addProspects: _addProspects,
        isReady: _isReady,
        onOpenNewProposalDlg: reqAddProposalDlg,
        getProposals: _getProposals,
        getProspects: _getProspects,
        delProposal: _delProposal,
        getProposalById: _getProposalById,
        saveProperties: _saveProperties,
        removeProperty: _removeProperty,
        sendEmailProposal: _sendEmailProposal,
        savePropertyViewDetails: _savePropertyViewDetails,
        getPropertyViewDetails: _getPropertyViewDetails,
        getPhotosbyPropoertyId: _getPhotosbyPropoertyId,
        photos: _photos,
        rating:_rating,
        getRatingInfo:_getRatingInfo,
        saveRatingInfo: _saveRatingInfo,
        savePropertyViewTime: _savePropertyViewTime
     };
});
