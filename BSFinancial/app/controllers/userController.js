//var module = angular.module("userIndex", ['ngRoute', 'angularValidator']);

function userController($scope, $http, $modal, userService) {
    $scope.data = userService;
    $scope.isBusy = false;
    $scope.users = [];
    $scope.UsersFilter = [];

    $scope.pager = {};
    $scope.pager.currentPage = 1;
    $scope.pager.maxSize = 10;

    $scope.pager.noOfButtons = 5;
    $scope.dataDDL = "";

    $scope.isContainer = false;
    $scope.isSearch = false;
    $scope.isFilter = false;
    $scope.isMessage = false;
    $scope.Message = "";


    $scope.orderByField = 'id';
    $scope.reverseSort = false;

    $scope.registration = {
        email: "",
    };
    userService.getUsers()
        .then(function (result) {
            $scope.users = userService.users;
        },
        function () {
            //alert("could not load users");
        })
        .then(function () {
        });
    bindFilterDDL();

    $scope.sortByColumn = function (name) {
        $scope.reverseSort = ($scope.orderByField === name) ? !$scope.reverseSort : false;
        $scope.orderByField = name;
    };

    function bindFilterDDL() {
        $scope.dataDDL = [{
            text: "Show Entries",
            value: 10
        },
            {
                text: "10",
                value: 10
            },
        {
            text: "25",
            value: 25
        },
        {
            text: "50",
            value: 50
        },
        {
            text: "100",
            value: 100
        },
        ];
        console.log($scope.dataDDL);
    }

    $scope.openNewUserDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/user/inviteUser.html',
            controller: 'NewInviteUserModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                origin: function () {
                    return "";
                }
            }
        });
    };

}

angular.module('BSFinancialApp').controller('NewInviteUserModalCtrl', function ($scope, $modalInstance, userService) {
    $scope.titleBtnSave = "Invite";

    $scope.inviteNewUser = function () {
        if ($("#inviteUserForm").valid()) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Inviting...";

            userService.inviteUser($scope.registration).then(function () {
                $modalInstance.dismiss('cancel');
                alert('Invitation email sent!!!');
            },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to invite user or user was invited earlier:" + errors.join(' ');
             $modalInstance.dismiss('cancel');
             alert('Failed to invite user or user was invited earlier!!!');
         });
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});
function newUserController($scope, $http, $location, $window, userService) {
    $scope.models = {
        btnSubmitTitle: 'Submit User'
    };
    $scope.sending = false;

    $scope.newUser = {
        CompanyId: 1
    };

    $scope.submitUser = function () {
        $scope.sending = true;
        $scope.models.btnSubmitTitle = "Submitting...";

        userService.addUser($scope.newUser)
        .then(function () {
            $location.path("newusersucc");
        },
        function () {
            $scope.models.btnSubmitTitle = 'Submit User';
            $scope.sending = false;
            $scope.error = "Sorry! Fail to submit user. Please check your input value and try again...";
        });
    };
}

function singleUserController($scope, userService, $window, $routeParams) {
    $scope.user = new Object();
    userService.getUserById($routeParams.id)
    .then(function (user) {
        $scope.user = user;
    },
    function () {
        $window.location = "#/";
        $scope.isBusy = false;
    });
}

app.controller('newUserController', newUserController);
app.controller('userController', userController);
app.controller('singleUserController', singleUserController);