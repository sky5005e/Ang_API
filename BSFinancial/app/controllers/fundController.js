
app.filter('getSelectedInvestors', function () {
    return function (org, tgt) {
        var rst = [];
        var i = 0, j = 0, len = org.length, tlen = tgt.length;
        for (; i < tlen; i++) {
            for (j = 0; j < len; j++) {
                if (tgt[i].id == org[j].id) {
                    rst.push(org[j]);
                    break;
                }
            }
        }
        return rst;
    }
});

function fundController($scope, $http, $filter, $modal, fundService,$location, subscriptionService) {
    $scope.data = fundService;
    $scope.isBusy = false;
    $scope.funds = [];
    $scope.investors = [];
    $scope.search = "";
    $scope.fundFilter = [];


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

    bindFilterDDL();

    $scope.onSelectPage = function () {
        if ($scope.pager.currentPage < 1 || $scope.pager.currentPage > Math.ceil($scope.pager.totalItems / $scope.pager.maxSize)) {
            return;
        }

    };

    fundService.getFunds()
        .then(function (result) {
            $scope.funds = fundService.funds;
            checkData();
        },
        function () {
        })
        .then(function () {
        });

    $scope.deleteFund = function (delid) {
        fundService.deleteFund(delid)
        .then(function () {
            $scope.funds = fundService.funds;
        },
        function () {
        });
    };

    $scope.openNewFundDlg = function (item) {

        subscriptionService.getFundSubscription()
       .then(function (result) {
           // var flist = $filter('getValidApplication')(loanService.applications);
           $scope.fundSubscription = subscriptionService.fundSubscription;
           if (!angular.isUndefinedOrNull($scope.fundSubscription.isSubscribed) && !$scope.fundSubscription.isSubscribed) {
               $location.lastPath = '/fund';
               $location.path('/subscription');
               return;
           }
           //if (!angular.isUndefinedOrNull($routeParams.selaid)) {
           //    //$scope.application.selected = $filter('getApplicationById')($scope.applications, $routeParams.selaid);
           //}
       },

       function () {
           //alert("could not load inquiries");
       });

        var modalInstance = $modal.open({
            templateUrl: '/app/views/fund/fundModel.html',
            controller: 'NewFundModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                origin: function () {
                    return "";
                }
            }
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

    function checkData() {

        if ($scope.funds.length <= 0 || $scope.funds == null) {
            $scope.isContainer = false;
            $scope.isSearch = false;
            $scope.isFilter = false;
            $scope.isMessage = true;
            $scope.Message = "No Record Found";
        }
        else {
            $scope.isContainer = true;
            $scope.isSearch = true;
            $scope.isFilter = true;
            $scope.isMessage = false;

        }
    }

    $scope.sortByColumn = function (name) {
        $scope.reverseSort = ($scope.orderByField === name) ? !$scope.reverseSort : false;
        $scope.orderByField = name;
    };


    $scope.addFormFilter = function () {
        return $scope.fundFilter.length;
    };
}

angular.module('BSFinancialApp').controller('NewFundModalCtrl', function ($scope, $modalInstance, $location, fundService) {
    $scope.titleBtnSave = "Create Fund";

    $scope.createFund = function () {
        if ($("#FundForm").valid()) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Saving...";

            fundService.addFund($scope.fund).then(function () {
                $modalInstance.dismiss('cancel');
            },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to create new fund:" + errors.join(' ');
             $modalInstance.dismiss('cancel');
             alert('Failed to create new fund!!!');
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

function editfundController($scope, $http, $filter, $modal, fundService, investorService, notificationService, $location, $window, $routeParams) {

    $scope.models = {
        btnSubmitTitle: 'Save Fund',
        formTitle: "View / Edit Fund"
    };

    $scope.investors = [];
    $scope.fundDistributions = [];
    $scope.selectedInvestors = [];
    $scope.editMode = true;

    $scope.submitting = false;
    $scope.fund = new Object();
    fundService.getFundById($routeParams.id)
            .then(function (fund) {

                

                $scope.fund = fundService.fund;
                $scope.investors = fundService.fund.investors;
                $scope.fundDistributions = fundService.fund.fundDistributions;
            },
            function () {
                $window.location = "#/";
                $scope.isBusy = false;
            });
    $scope.submitFund = function () {
        if ($("#fundeditForm").valid() && $scope.submitting == false) {

            $scope.submitting = true;
            $scope.models.btnSubmitTitle = "Submitting...";

            fundService.addFund($scope.fund)
            .then(function () {
                $location.path("fundlist");
            },
            function (result) {
                notificationService.error(result.data.message);
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
            });
        }
    };

    // Investor
    $scope.openSelInvestorDlg = function () {

        fundService.openAddFundInvestorDlg();
        return;
        var modalInstance = $modal.open({
            //templateUrl: '/app/views/fund/selectInvestor.html',
            templateUrl: '/app/views/fund/selInvestor.html',
            controller: 'SelInvestorModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                fund: function () {
                    return $scope.fund;
                },
                sellist: function () {
                    return $scope.selectedInvestors;
                }
            }
        });
        modalInstance.result.then(function (retinvestorslist) {
            notificationService.success("Successfully saved.");
            $scope.fund.fundInvestors = retinvestorslist;
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save property.");
            }
        });
    };

    $scope.deleteInvestor = function (delItem) {
        investorService.removeInvestor(delItem.fundInvestorId)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.fund.fundInvestors.indexOf(delItem);
            $scope.fund.fundInvestors.splice(index, 1);
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

    $scope.$on('onOpenInvestorDlg', function (event, args) {
        $scope.onOpenInvestorDlg(null);
    });

    $scope.onOpenInvestorDlg = function (item) {
        
        var modalInstance = $modal.open({
            templateUrl: '/app/views/fund/investorModelAdd.html',
            controller: 'InvestorModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                fund: function () {
                    return $scope.fund;
                },
                investor: function () {
                    if (item != null) {
                        fundService.isInvestorEdit = true;
                    }
                    return item 
                }
            }
        });
        modalInstance.result.then(function (retnewinvestorslist) {
            notificationService.success("Successfully saved.");

            $scope.fund.investors = retnewinvestorslist;
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save payment.");
            }
        });
    };
    // Distribution
    $scope.openSelDistributionDlg = function () {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/fund/selDistribution.html',
            controller: 'DistributionModalCtrl',
            backdrop: 'static',
            resolve: {
                fund: function () {
                    return $scope.fund;
                }
            }
        });
        modalInstance.result.then(function (retDistributionlist) {
            notificationService.success("Successfully saved.");
            $scope.fund.fundDistributions = retDistributionlist;
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save property.");
            }
        });
    };

    $scope.deleteDistribution = function (delItem) {
        fundService.deleteDistribution(delItem.id)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.fund.fundDistributions.indexOf(delItem);
            $scope.fund.fundDistributions.splice(index, 1);
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };



}


app.controller('fundController', fundController)
app.controller('editfundController', editfundController)


angular.module('BSFinancialApp').controller('SelInvestorModalCtrl', function ($scope, $filter, $modalInstance, fundService, fund, investorService, sellist, $routeParams) {
    $scope.titleBtnSave = "Save";
    $scope.investors = [];
    $scope.fundInvestors = [];
    // initial value
    $scope.fundInvestors.push({ Id: null, fullName: null, shares: null, amountpaid: null, rowId: 0, selectedInvestorItem: {} });

    investorService.getInvestors()
       .then(function (result) {
           $scope.investors = investorService.investors;
           if (fundService.isAddNewInvestor && $scope.investors != undefined) {
               $scope.fundInvestors = fundService.selectedFundInvestor;
               $scope.fundInvestors[fundService.selectedRowInvestor].selectedInvestorItem = $scope.investors[$scope.investors.length - 1];
               fundService.isAddNewInvestor = false;
           }
           fundService.selectedFundInvestor = $scope.fundInvestors;
           for (var i = 0; i < $scope.fundInvestors.length; i++) {
               var index= getSelectedIndex($scope.fundInvestors[i].selectedInvestorItem);
               $scope.fundInvestors[i].selectedInvestorItem = investorService.investors[index];
           }
       },
       function () {
           //alert("could not investor");
       });

    function getSelectedIndex(item)
    {
        if (item.id != undefined) {
            for (var i = 0; i < investorService.investors.length; i++) {
                if (investorService.investors[i].id == item.id) {
                    return i;
                }
            }
        }
    }

    $scope.addNewFundInvestor = function () {
        if (!$("#newInvestorsForm").valid()) {
            
            return;
        }

        $scope.fundInvestors.push({ Id: null, fullName: null, shares: null, amountpaid: null, rowId: $scope.fundInvestors.length, selectedInvestorItem: {} });
        //debugger;
        var asd = $('#newInvestorsForm').find(".table");
        var asdf = $('#newInvestorsForm').find('[name="shares"]');
        $("#newInvestorsForm").validate('addField', asdf);
        fundService.selectedFundInvestor = $scope.fundInvestors;
    }

    $scope.goToNewInvestor = function (rowId) {
        fundService.selectedRowInvestor = rowId;
        investorService.openNewInvestorDlg();
        $modalInstance.dismiss('cancel');
    };

    $scope.submitSelectedInvestor = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Saving...";
        $scope.fundId = $routeParams.id;

        for (var i = 0; i < $scope.fundInvestors.length; i++) {
            $scope.fundInvestors[i].Id = $scope.fundInvestors[i].selectedInvestorItem.id;
            //$scope.fundInvestors[i].fullName = $scope.fundInvestors[i].selectedInvestorItem.fullName;
        }

        fundService.saveFundInvestors($scope.fundId, $scope.fundInvestors)
        .then(function (result) {
            $modalInstance.close(fundService.fundInvestors);
            fundService.getFundById($routeParams.id);
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
angular.module('BSFinancialApp').controller('InvestorModalCtrl', function ($scope, $modalInstance, investor, investorService, fund, fundService, $routeParams) {

    $scope.investor = new Object();
    $scope.investor = investor;

    $scope.isModal = true;
    $scope.titleBtnSave = "Save investor";

    $scope.models = {
        formTitle: 'Edit investor'
    };
    $scope.submitting = false;

    $scope.fund = fund;
    if (investor == null) {
        investor = new Object();
    }
    $scope.submitFundInvestor = function () {
        if ($("#investorSaveForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";

            var actType = "add";
            var pIndex = 0;

            if (investor.id > 0) {
                actType = "edit";
                pIndex = $scope.fund.fundInvestors.indexOf(investor);
            }
            $scope.fundId = $routeParams.id;
            if ($scope.investor.fundInvestorId > 0) {
                fundService.updateFundInvestor($scope.fundId, $scope.investor)
               .then(function () {
                   $modalInstance.close(fundService.investors);
                   //fundService.openAddFundInvestorDlg();
               },
                    function () {
                        $scope.titleBtnSave = 'Save Investor';
                        $scope.submitting = false;
                    });
            }
            else {
                fundService.addFundInvestor($scope.investor, "add", 0)
               .then(function () {
                   fundService.selectedFundInvestor[fundService.selectedRowInvestor].shares = fundService.investors[0].shares;
                   fundService.selectedFundInvestor[fundService.selectedRowInvestor].amountpaid = fundService.investors[0].amountPaid;
                   fundService.isAddNewInvestor = true;
                   $modalInstance.close(fundService.investors);

                   fundService.openAddFundInvestorDlg();
               },
                    function () {
                        $scope.titleBtnSave = 'Save Investor';
                        $scope.submitting = false;
                    });
            }

        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');

        
        if (!fundService.isInvestorEdit) {
            fundService.openAddFundInvestorDlg();
        }
        fundService.isInvestorEdit = false;
        $scope.$apply();
        
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});

angular.module('BSFinancialApp').controller('DistributionModalCtrl', function ($scope, $filter, $modalInstance, fundService, fund, $routeParams) {

    $scope.titleBtnSave = "Create Distribution";
    $scope.distribution = new Object();


    $scope.submitDistribution = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Creating...";
        $scope.distribution.fundId = $routeParams.id;

        fundService.saveFundDistribution($scope.distribution.fundId, $scope.distribution)
        .then(function (result) {
            $modalInstance.close(fundService.fundDistributions);
        },
        function () {
            $scope.submitting = false;
            $scope.titleBtnSave = "Create Distribution";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});