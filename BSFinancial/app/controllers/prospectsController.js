function prospectsController($scope, $http, $modal, prospectService, notificationService, $location) {
    $scope.data = prospectService;
    $scope.isBusy = false;
    $scope.prospects = [];
    $scope.prospect = new Object();
    $scope.ProspectFilter = [];

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


    prospectService.getProspects()
        .then(function (result) {
            $scope.prospects = prospectService.prospects;
        },
        function () {
            //alert("could not proposals");
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
    $scope.delProposal = function (delid) {
        prospectService.delProspect(delid)
        .then(function () {
            $scope.prospects = prospectService.prospects;
        },
        function () {
        });
    };

    $scope.$on('openNewProspectDlg', function (event, args) {
        $scope.openNewProspectDlg(null);
    });
    $scope.openNewProspectDlg = function (item) {
        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/newProspectModal.html',
            controller: 'NewProspectModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                prospects: function () {
                    return $scope.prospects;
                },
                prospect: function () {
                    return item;

                }
            }
        });
    };


}

app.controller('prospectsController', prospectsController);


angular.module('BSFinancialApp').controller('NewProspectModalCtrl', function ($scope, $modalInstance, prospect, prospects, prospectService, $routeParams, notificationService) {

    $scope.prospect = new Object();
    $scope.prospect = prospect;
    $scope.isModal = true;
    
    $scope.titleBtnSave = "Save Prospect";

    $scope.models = {
        formTitle: 'Add Prospect'
    };
    $scope.submitting = false;

    if (prospect == null) {
        prospect = new Object();
    }
    $scope.prospects = prospects;
    $scope.submitNewProspects = function () {
        if ($("#prospectForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";
            var actType = "add";
            var pIndex = 0;

            if (prospect.id > 0) {
                actType = "edit";
                pIndex = $scope.prospects.indexOf(prospect);
            }
            prospectService.addProspect($scope.prospect, actType, pIndex)
            .then(function () {
                $modalInstance.close(prospectService.prospects)
            },
            function () {
                $scope.titleBtnSave = 'Save Prospect';
                $scope.submitting = false;
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

