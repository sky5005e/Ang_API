
function investorController($scope, $http, $filter, investorService) {
    $scope.data = investorService;
    $scope.isBusy = false;
    $scope.investors = [];
    $scope.InvestorsTemp = [];

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


    investorService.getInvestors()
        .then(function (result) {
            $scope.investors = investorService.investors;
        },
        function () {
        })
        .then(function () {
        });
    bindFilterDDL();

    $scope.deleteInvestor = function (delid) {
        investorService.deleteInvestor(delid)
        .then(function () {
            $scope.investors = investorService.investors;
        },
        function () {
        });
    };

    function bindFilterDDL() {
        $scope.dataDDL = [{
            text: "Show Enteries",
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

function newInvestorController($scope, $http, $location, $window, investorService) {
    $scope.titleBtnSave = "Save Investor";

    $scope.models = {
        formTitle: 'New Investor'
    };
    $scope.submitting = false;

    $scope.submitInvestor = function () {
        if ($("#investorForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";
            investorService.addInvestor($scope.investor)
            .then(function () {
                $location.path("investorlist");
            },
            function () {
                $scope.titleBtnSave = 'Save Investor';
                $scope.submitting = false;
            });
        }
    };

    $scope.cancel = function () {
        window.history.back();
    };
}
function editInvestorController($scope, $filter, $http, $modal, action, investorService, $location, $window, $routeParams) {

    $scope.titleBtnSave = "Save Investor";

    $scope.models = {
        formTitle: 'Edit Investor'
    };
    $scope.submitting = false;
    $scope.investor = new Object();

    investorService.getInvestorById($routeParams.id)
        .then(function (investor) {
            $scope.investor = investorService.investor;
        },
        function () {
            $window.location = "#/";
            $scope.isBusy = false;
        });


    $scope.submitInvestor = function () {
        if ($("#investorForm").valid() && $scope.submitting == false) {
            $scope.investor = $scope.investor;


            $scope.submitting = true;
            $scope.btnSubmitTitle = "Submitting...";

            investorService.addInvestor($scope.investor)
            .then(function () {
                $location.path("investorlist");
            },
            function (result) {
                notificationService.error(result.data.message);
                $scope.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
            });
        }
    };

    $scope.cancel = function () {
        window.history.back();
    };

}


app.controller('investorController', investorController)
app.controller('newInvestorController', newInvestorController);
app.controller('editInvestorController', editInvestorController)