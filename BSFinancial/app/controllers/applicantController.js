function applicantController($scope, $http, applicantService) {
    $scope.data = applicantService;
    $scope.isBusy = false;
    $scope.applicants = [];
    $scope.ApplicantsFilter = [];

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



    applicantService.getApplicants()
        .then(function (result) {
            $scope.applicants = applicantService.applicants;
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

    $scope.deleteApplicant = function (delid) {
        applicantService.deleteApplicant(delid)
        .then(function () {
            $scope.applicants = applicantService.applicants;
        },
        function () {
        });
    };
}

function newApplicantController($scope, $http, $location, $window, applicantService) {

    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.opened = true;
    };

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.format = 'yyyy/MM/dd';

    $scope.titleBtnSave = "Submit";

    $scope.models = {
        formTitle: 'New applicant'
    };
    $scope.submitting = false;

    $scope.submitApplicant = function () {
        if ($("#applicantForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";

            applicantService.addApplicant($scope.applicant)
            .then(function () {
                $location.path("applicantlist");
            },
            function () {
                $scope.titleBtnSave = 'Submit';
                $scope.submitting = false;
                //$scope.error = "Sorry! Fail to submit inquiry. Please check your input value and try again...";
            });
        }
    };

    $scope.cancel = function () {
        window.history.back();
    };
}

function editApplicantController($scope, applicantService, $location, $window, $routeParams) {
    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.opened = true;
    };

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.format = 'yyyy/MM/dd';

    $scope.titleBtnSave = "Submit";
    $scope.models = {
        formTitle: 'Edit applicant'
    };
    $scope.submitting = false;

    $scope.applicant = null;
    applicantService.getApplicantById($routeParams.id)
        .then(function (applicant) {
            $scope.applicant = applicantService.applicant;
        },
        function () {
            $window.location = "#/";
            $scope.isBusy = false;
        });

    $scope.submitApplicant = function () {
        if ($("#applicantForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";

            applicantService.editApplicant($scope.applicant)
            .then(function () {
                $location.path("applicantlist");
            },
            function () {
                $scope.titleBtnSave = 'Submit';
                $scope.submitting = false;
                //$scope.error = "Sorry! Fail to submit inquiry. Please check your input value and try again...";
            });
        }
    };

    $scope.cancel = function () {
        window.history.back();
    };
}

app.controller('applicantController', applicantController);
app.controller('newApplicantController', newApplicantController);
app.controller('editApplicantController', editApplicantController);