'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService', function ($scope, $location, $timeout, authService) {

    $scope.savedSuccessfully = false;
    $scope.message = "";

    $scope.registration = {
        email: "",
        firstname: "",
        lastname: "",
        company: "",
        password: "",
        confirmPassword: "",
        accid: "",
        planType: ""
    };

    $scope.passwordValidator = function (password) {

        if (!password) {
            return;
        }
        else if (password.length < 6) {
            return "Password must be at least " + 6 + " characters long";
        }
        else if (!password.match(/[A-Z]/)) {
            return "Password must have at least one capital letter";
        }
        else if (!password.match(/[0-9]/)) {
            return "Password must have at least one number";
        }

        return true;
    };

    $scope.signUp = function () {
        $scope.registration.planType = getparams('planType');
        authService.saveRegistration($scope.registration).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "Thank you!!! Confirmation email has been sent to your register email address.";
            startTimer('/home');

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to register user due to:" + errors.join(' ');
         });
    };

    $scope.register = function () {
        var token = getparams('id');
        var useraccid = getparams('acc');
        $scope.registration.invitedtoken = token;
        $scope.registration.accid = useraccid;

        authService.saveRegistration($scope.registration).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "Thank you!!! Confirmation email has been sent to your register email address.";
            startTimer('/home');
        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to register user due to:" + errors.join(' ');
         });
    };

    $scope.acceptinguser = function () {
        var token = getparams('id');
        var useraccid = getparams('acc');
        $scope.confirmation = {
            email: 'e',
            userid: useraccid
        };

        authService.accountConfirmation($scope.confirmation).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "Your account has been activated, you will be redicted to login page in 2 seconds.";
            startTimer('/login');
        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to confirmed user due to:" + errors.join(' ');
             startTimer('/home');
         });
    };
    var startTimer = function (url) {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path(url);
        }, 2000);
    }
    var getparams = function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
}]);