app.factory("userService", function ($http, $q) {
    var _users = [];
    var _isInit = false;

    var _isReady = function () {
        return _isInit;
    };

    var _getUsers = function () {
        var deferred = $q.defer();

        $http.get("/api/users")
        .then(function (result) {
            //success
            angular.copy(result.data, _users);
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
    var _inviteUser = function (registration) {

        return $http.post("api/account/inviteuser", registration).then(function (response) {
            return response;
        });

    };
    var _addUser = function (newUser) {

        var deferred = $q.defer();
        $http.post("/api/users", newUser)
            .then(function (result) {
                //success
                var newlyCreatedUser = result.data;
                _users.splice(0, 0, newlyCreatedUser);
                deferred.resolve(newlyCreatedUser);
            },
            function () {
                //error
            });
        return deferred.promise;

    };

    function _findUser(id) {
        var found = null;

        $.each(_users, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }

    var _getUserById = function (id) {
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
        _getUsers()
            .then(function () {
                //success
                var user = _findUser(id);
                if (user) {
                    deferred.resolve(user);
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
        users: _users,
        inviteUser: _inviteUser,
        getUsers: _getUsers,
        addUser: _addUser,
        isReady: _isReady,
        getUserById: _getUserById
    };
});
