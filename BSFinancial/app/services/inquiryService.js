app.factory("inquiryService", function ($http, $q) {
    var _inquries = [];
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };

    var _getInquiries = function () {
        var deferred = $q.defer();

        $http.get("/api/inquiry")
        .then(function (result) {
            //success
            angular.copy(result.data, _inquries);
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

    var _addInquiry = function (newInquiry) {

        var deferred = $q.defer();
        $http.post("/api/inquiry", newInquiry)
            .then(function (result) {
                //success
                var newlyCreatedInquiry = result.data;
                _inquries.splice(0, 0, newlyCreatedInquiry);
                deferred.resolve(newlyCreatedInquiry);
            },
            function () {
                //error
            });
        return deferred.promise;

    };

    function _findInquiry(id) {
        var found = null;

        $.each(_inquries, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }

    var _getInquiryById = function (id) {
        var deferred = $q.defer();

        /*
        if (_isReady()) {
            var inquiry = _findInquiry(id);
            if (inquiry) {
                deferred.resolve(inquiry);
            } else {
                deferred.reject();
            }
            return inquiry;
        } else {
        */
            _getInquiries()
                .then(function () {
                    //success
                    var inquiry = _findInquiry(id);
                    if (inquiry) {
                        deferred.resolve(inquiry);
                    } else {
                        deferred.reject();
                    }
                },
                function () {
                    //error
                    deferred.reject();
                });
        //}

        return deferred.promise;
    };

    return {
        inquries: _inquries,
        getInquiries: _getInquiries,
        addInquiry: _addInquiry,
        isReady: _isReady,
        getInquiryById: _getInquiryById
    };
});
