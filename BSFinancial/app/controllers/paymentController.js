
function paymentController($scope, $http, $filter, $modal, $routeParams, $location, paymentService, subscriptionService) {

    if ($routeParams.plan != undefined) {
        paymentService.planType = $routeParams.plan;
    }

    $scope.paymentInfo = new Object();

    $scope.data = paymentService;
    $scope.isBusy = false;
    $scope.payments = [];
    $scope.PaymentsFilter = [];

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


    paymentService.getPayments()
        .then(function (result) {
            $scope.payments = paymentService.payments;
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
    $scope.BuySubscription = function () {
        if (!$("#paymentForm").valid()) {

            return;
        }
        subscriptionService.updateSubscription(paymentService.planType, $scope.paymentInfo);
    }

}
app.controller('paymentController', paymentController);
