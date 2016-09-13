'use strict';
app.controller('indexController', ['$scope', '$location', 'authService', 'Idle', function ($scope, $location, authService, Idle, Keepalive, modal) {

    $scope.logOut = function () {
        authService.logOut();
        $location.path('/home');
    }

    $scope.events = [];
    $scope.idle = 1000;
    $scope.timeout = 100;

    $scope.$on('$routeChangeSuccess', function (event, current) {
        getPageClass(current.originalPath);
    });



    $scope.$on('IdleTimeout', function () {
        authService.logOut();
        $location.path('/login');
        $scope.$apply();
    });

    $scope.$on('$keepalive', function () {
        // do something to keep the user's session alive
        console.log('$keepalive');
    })

    $scope.$watch('idle', function (value) {
        if (value !== null) Idle.setIdle(value);
    });

    $scope.$watch('timeout', function (value) {
        if (value !== null) Idle.setTimeout(value);
    });

    $scope.authentication = authService.authentication;
    $scope.ShowHeader = authService.showHeader;

    function getPageClass(path) {
        switch (path) {
            case "/signup":
                $scope.bodyClass = "cover-bg";
                $scope.containerClass = "container-fluid";
                break;
            case "/login":
                $scope.bodyClass = "cover-bg";
                $scope.containerClass = "container";
                break;
            default:
                $scope.bodyClass = "";
                $scope.containerClass = "container";
                break;
        }
    }
}]);