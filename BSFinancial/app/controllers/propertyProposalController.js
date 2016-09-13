
// proposal list screen controller
function propertyProspectsController($scope, $http, $modal, propertyProposalService, notificationService, $location) {
    $scope.data = propertyProposalService;
    $scope.isBusy = false;
    $scope.prospects = [];
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


    $scope.orderByField = 'sentOn';
    $scope.reverseSort = false;

   
    propertyProposalService.getProspects()
        .then(function (result) {
            $scope.prospects = propertyProposalService.prospects;
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
        propertyProposalService.delProposal(delid)
        .then(function () {
            $scope.prospects = propertyProposalService.prospects;
        },
        function () {
        });
    };
    $scope.sendEmailProposal = function (proposalId) {
        propertyProposalService.sendEmailProposal(proposalId)
        .then(function () {
            notificationService.success("Email has been successfully sent!!!");
        },
        function () {
        });
    };

    $scope.openNewProposalDlg = function (item) {
        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/proposalModal.html',
            controller: 'SelProposalModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                origin: function () {
                    return "";
                }
            }
        });
    };

    $scope.$on('onOpenProposalDlg', function (event, args) {
        $scope.onOpenProposalDlg(null);
    });

    $scope.onOpenProposalDlg = function (item) {
        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/newProspectModal.html',
            controller: 'NewProposalModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                proposal: function () {
                    return item;

                }
            }
        });
        modalInstance.result.then(function (retnewpropertieslist) {
            notificationService.success("Successfully saved.");

            $scope.loan.properties = retnewpropertieslist;
            if ($scope.loan.properties.length > 0) {
                $scope.emptyProperty = false;
            }

        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save payment.");
            }
        });
    };

}

function editpropertyProspectsController($scope, $http, $modal, propertyProposalService, notificationService, $location, $routeParams) {
    $scope.data = propertyProposalService;
    $scope.isBusy = false;
    $scope.proposal = new Object();
    $scope.properties = [];

    $scope.titleBtnSave = "Save Proposal";
    $scope.models = {
        formTitle: 'Edit Proposal'
    };
    propertyProposalService.getProposalById($routeParams.id)
        .then(function (result) {
            $scope.proposal = propertyProposalService.proposal;
            $scope.properties = propertyProposalService.properties;
        },
        function () {
            //alert("could not proposals");
        })
        .then(function () {
        });
    
    $scope.openSelPropertyDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/selectProperty.html',
            controller: 'SelPropertyModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                origin: function () {
                    return "";
                }
            }
        });
    };

    $scope.submitProposal = function () {
        $location.path('/proposals');
    };
    
    $scope.viewAnalytics = function (id) {
        $location.path('/proposalstats/' + id);
    };
    $scope.cancel = function () {
        window.history.back();
    };

    $scope.deleteProperty = function (delItem) {
        var model = {
            Id: $routeParams.id,
            ChildId: delItem.id
        };
        propertyProposalService.removeProperty(model)
        .then(function () {
            notificationService.success("Successfully deleted!");

            $scope.properties = propertyProposalService.properties;

            //if ($scope.properties.length == 0) {
            //    $scope.emptyProperty = true;
            //}
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };
}
app.controller('propertyProspectsController', propertyProspectsController);

app.controller('editpropertyProspectsController', editpropertyProspectsController);

angular.module('BSFinancialApp').controller('SelProposalModalCtrl', function ($scope, $modalInstance, $location, propertyProposalService, userService) {

    $scope.titleBtnSave = "Save";

    $scope.prospect = new Object();
    $scope.user = new Object();
    $scope.prospects = [];
    $scope.users = [];
    $scope.dlg = {};
    $scope.propertyProposal = new Object();

    propertyProposalService.getProposals()
        .then(function (result) {
            $scope.prospects = propertyProposalService.proposals;
        },
        function () {
            //alert("could not proposals");
        })
        .then(function () {
        });
    userService.getUsers()
        .then(function (result) {
            $scope.users = userService.users;
        },
        function () {
            //alert("could not load users");
        })
        .then(function () {
        });

    $scope.goToNewProposal = function () {
        propertyProposalService.onOpenNewProposalDlg();
        $modalInstance.dismiss('cancel');
    };
    
    $scope.submitSelectedProspects = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Saving...";
        $scope.prospect = $scope.dlg.selectedProspectList[0];
        $scope.user = $scope.dlg.selectedUserList[0];
        $scope.propertyProposal.userId = $scope.user.id;
        $scope.propertyProposal.prospectId = $scope.prospect.id;
        $scope.propertyProposal.proposalFor = $scope.proposal.proposalFor;
        //alert($scope.propertyProposal.prospectId);
        propertyProposalService.addProspects($scope.propertyProposal)
        .then(function (result) {
            $scope.prospects = propertyProposalService.prospects;

            $modalInstance.close(propertyProposalService.prospects);
        },
        function () {
            $scope.submitting = false;
            $scope.titleBtnSave = "Save";
        });
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});

angular.module('BSFinancialApp').controller('NewProposalModalCtrl', function ($scope, $modalInstance, proposal, propertyProposalService, $routeParams) {

    $scope.proposal = new Object();
    $scope.proposal = proposal;

    $scope.isModal = true;
    $scope.openDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.openedDate = true;
    };

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };
    $scope.format = 'yyyy/MM/dd';
    $scope.titleBtnSave = "Save Prospect";

    $scope.models = {
        formTitle: 'New Prospect'
    };
    $scope.submitting = false;

    if (proposal == null) {
        proposal = new Object();
        //proposal.loanId = $routeParams.id;
    }
    $scope.submitNewProposal = function () {
        if ($("#prospectForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";

            var actType = "add";
            var pIndex = 0;

            if (proposal.id > 0) {
                actType = "edit";
                pIndex = $scope.proposals.indexOf(proposal);
            }
            propertyProposalService.addProposal($scope.proposal, actType, pIndex)
            .then(function () {
                $modalInstance.close(propertyProposalService.proposals)
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

angular.module('BSFinancialApp').controller('SelPropertyModalCtrl', function ($scope, $filter, $modalInstance, propertyService,propertyProposalService, $routeParams) {

    $scope.titleBtnSave = "Save";
    $scope.properties = [];
    propertyService.getProperties()
       .then(function (result) {
           $scope.properties = propertyService.properties;
       },
       function () {
       });

    $scope.dlg = {};

    $scope.submitSelectedProperty = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Saving...";
        $scope.properties = $scope.dlg.selectedList;
        $scope.proposalId = $routeParams.id;
        propertyProposalService.saveProperties($scope.proposalId, $scope.properties)
        .then(function (result) {
            $scope.properties = propertyProposalService.properties;
            $modalInstance.close(propertyProposalService.properties);
        },
        function () {
            $scope.submitting = false;
            $scope.titleBtnSave = "Save";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});
