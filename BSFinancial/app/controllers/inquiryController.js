//var module = angular.module("inquiryIndex", ['ngRoute', 'angularValidator']);

function inquiryController($scope, $http, inquiryService) {
    $scope.data = inquiryService;
    $scope.isBusy = false;
    $scope.inquiries = [];
    $scope.InquiriesFilter = [];

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


    $scope.orderByField = 'sentOn';
    $scope.reverseSort = false;

    inquiryService.getInquiries()
        .then(function (result) {
            $scope.inquiries = inquiryService.inquries;
        },
        function () {
            //alert("could not load inquiries");
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
}

function newInquiryController($scope, $http, $location, $window, inquiryService) {
    $scope.models = {
        btnSubmitTitle: 'Submit Inquiry'
    };
    $scope.sending = false;

    $scope.newInquiry = {
        CompanyId: 1
    };

    $scope.submitInquiry = function () {
        $scope.sending = true;
        $scope.models.btnSubmitTitle = "Submitting...";

        inquiryService.addInquiry($scope.newInquiry)
        .then(function () {
            $location.path("newinquirysucc");
        },
        function () {
            $scope.models.btnSubmitTitle = 'Submit Inquiry';
            $scope.sending = false;
            $scope.error = "Sorry! Fail to submit inquiry. Please check your input value and try again...";
        });
    };
}

function singleInquiryController($scope, inquiryService, $window, $routeParams) {
    $scope.inquiry = new Object();
    inquiryService.getInquiryById($routeParams.id)
    .then(function (inquiry) {
        $scope.inquiry = inquiry;
    },
    function () {
        $window.location = "#/";
        $scope.isBusy = false;
    });
}

app.controller('newInquiryController', newInquiryController);
app.controller('inquiryController', inquiryController);
app.controller('singleInquiryController', singleInquiryController);