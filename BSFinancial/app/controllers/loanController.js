app.filter('getValidApplication', function () {
    return function (alist) {
        var rst = [];
        var i = 0, len = alist.length;
        for (; i < len; i++) {
            if (0 != alist[i].applicantId) {
                rst.push(alist[i]);
            }
        }
        return rst;
    };
});

app.filter('getApplicationById', function () {
    return function (input, id) {
        var i = 0, len = input.length;
        for (; i < len; i++) {
            if (+input[i].id == id) {
                return input[i];
            }
        }
        return null;
    };
});

app.filter('getValidApplication', function () {
    return function (alist) {
        var rst = [];
        var i = 0, len = alist.length;
        for (; i < len; i++) {
            if (0 != alist[i].applicantId) {
                rst.push(alist[i]);
            }
        }
        return rst;
    };
});

app.filter('getSelectedProperties', function () {
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
    };
});

app.filter('offset', function () {
    return function (input, start) {
        input = input || '';
        start = parseInt(start, 10);
        return input.slice(start);
    };
});

function loanController($scope, $http, $filter, loanService) {

    $scope.data = loanService;
    $scope.isBusy = false;
    $scope.loans = [];
    $scope.search = "";
    $scope.LoanFilter = [];


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

    loanService.getLoans()
        .then(function (result) {
            $scope.loans = loanService.loans;
            checkData();
        },
        function () {
            //alert("could not load inquiries");
        })
        .then(function () {
        });

    bindFilterDDL();


    $scope.onSelectPage = function () {
        if ($scope.pager.currentPage < 1 || $scope.pager.currentPage > Math.ceil($scope.pager.totalItems / $scope.pager.maxSize)) {
            return;
        }

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

        if ($scope.loans.length <= 0 || $scope.loans == null) {
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

    $scope.deleteLoan = function (delid) {
        loanService.deleteLoan(delid)
        .then(function () {
            $scope.loans = loanService.loans;
        },
        function () {
        });
    };

    $scope.addFormFilter = function () {
        return $scope.LoanFilter.length;
    };

}


function editLoanController($scope, $filter, $http, $modal, action, loanService, applicantService, propertyService, notificationService, $location, $window, $routeParams, subscriptionService) {

    $scope.format = 'yyyy/MM/dd';

    ///Pager
    $scope.pager = {};
    $scope.pager.currentPage = 1;
    $scope.pager.maxSize = 10;
    $scope.pager.noOfButtons = 5;

    //sorting 
    $scope.orderByFieldPayment = 'id';
    $scope.reverseSortPayment = false;
    $scope.searchPayment = '';

    //sorting Escrow
    $scope.orderByFieldEscrow = 'id';
    $scope.reverseSortEscrow = false;
    $scope.searchEscrow = '';

    //sorting Doc
    $scope.orderByFieldDoc = 'id';
    $scope.reverseSortDoc = false;
    $scope.searchDoc = '';

    $scope.orderByField = 'id';

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };
    $scope.openBeginDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.openedBeginDate = true;
    };

    $scope.openMaturityDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.openedMaturityDate = true;
    };

    $scope.models = {
        btnSubmitTitle: 'Save Loan',
        formTitle: (action == "add" ? "Add" : "Edit") + " Loan"
    };


    $scope.panelName = "properties";
    $scope.showPanel = function (panel) {
        $scope.panelName = panel;
        console.log($scope.panelName);
    }

    $scope.editMode = action == "add" ? false : true;

    $scope.submitting = false;
    $scope.loan = new Object();
    $scope.loan.beginDate = "";
    $scope.loan.principal = 0;
    $scope.showErrors = false;
    $scope.application = {};
    $scope.applications = [];
    $scope.subscription = new Object();
    $scope.emptyProperty = true;
    $scope.emptyPayment = true;
    $scope.emptyDocument = true;
    $scope.emptyDisbursement = true
    $scope.monthlyPayment = 0;
    $scope.selectedProperties = [];

    applicantService.getApplicants()
        .then(function (result) {
            $scope.applicants = applicantService.applicants;
        },
        function () {
            //alert("could not load inquiries");
        });

    subscriptionService.getLoanSubscription()
        .then(function (result) {
            // var flist = $filter('getValidApplication')(loanService.applications);
            $scope.subscription = subscriptionService.subscription;
            if ($scope.subscription.isSubscribed != undefined && !$scope.subscription.isSubscribed) {
                $location.lastPath = '/loan';
                $location.path('/subscription');
            }
            if (!angular.isUndefinedOrNull($routeParams.selaid)) {
                //$scope.application.selected = $filter('getApplicationById')($scope.applications, $routeParams.selaid);
            }
        },

        function () {
            //alert("could not load inquiries");
        });
    loanService.getApplications()
        .then(function (result) {
            var flist = $filter('getValidApplication')(loanService.applications);
            $scope.applications = flist;

            if (!angular.isUndefinedOrNull($routeParams.selaid)) {
                $scope.application.selected = $filter('getApplicationById')($scope.applications, $routeParams.selaid);
            }
        },
        function () {
            //alert("could not load inquiries");
        });

    $scope.showApplicants = function (item, smodel) {
        var selApplicant = $filter('getApplicantById')($scope.applicants, item.applicantId);
        //var selCoApplicant = $filter('getApplicantById')($scope.applicants, item.coApplicantId);
        $scope.applicantName = selApplicant.firstName + " " + selApplicant.lastName;
        //$scope.coApplicantName = selCoApplicant.firstName + " " + selCoApplicant.lastName;
    };

    $scope.totalPayment = function () {
        var total = 0;
        for (count = 0; count < $scope.loan.payments.length; count++) {
            total += $scope.loan.payments[count].principalAmt;
        }
        return total;
    };

    $scope.totalEscrowPayment = function () {
        var totalEsc = 0;
        for (count = 0; count < $scope.loan.payments.length; count++) {
            totalEsc += $scope.loan.payments[count].escrowAmt;
        }
        return totalEsc;
    };

    $scope.totalDisbursement = function () {
        var totalDis = 0;
        for (count = 0; count < $scope.loan.disbursements.length; count++) {
            totalDis += $scope.loan.disbursements[count].amount;
        }
        return totalDis;
    }

    $scope.sortByColumn = function (name) {

        $scope.reverseSort = ($scope.orderByField === name) ? !$scope.reverseSort : false;
        $scope.orderByField = name;
    };

    $scope.sortByColumnPayment = function (name) {
        $scope.reverseSortPayment = ($scope.orderByFieldPayment === name) ? !$scope.reverseSortPayment : false;
        $scope.orderByFieldPayment = name;
    };


    $scope.onSelectPagePayment = function () {
        if ($scope.pager.currentPage < 1 || $scope.pager.currentPage > Math.ceil($scope.pager.totalItems / $scope.pager.maxSize)) {
            return;
        }

    };

    $scope.sortByColumnEscrow = function (name) {
        $scope.reverseSortEscrow = ($scope.orderByFieldEscrow === name) ? !$scope.reverseSortEscrow : false;
        $scope.orderByFieldEscrow = name;
    };


    $scope.onSelectPageEscrow = function () {
        if ($scope.pager.currentPage < 1 || $scope.pager.currentPage > Math.ceil($scope.pager.totalItems / $scope.pager.maxSize)) {
            return;
        }

    };

    $scope.sortByColumnDoc = function (name) {
        $scope.reverseSortDoc = ($scope.orderByFieldDoc === name) ? !$scope.reverseSortDoc : false;
        $scope.orderByFieldDoc = name;
    };

    $scope.onSelectPageDoc = function () {
        if ($scope.pager.currentPage < 1 || $scope.pager.currentPage > Math.ceil($scope.pager.totalItems / $scope.pager.maxSize)) {
            return;
        }

    };

    if (action == "edit") {
        loanService.getLoanById($routeParams.id)
            .then(function (loan) {
                if (loanService.loan.payments.length > 0) {
                    $scope.emptyPayment = false;
                }
                if (loanService.loan.properties.length > 0) {
                    $scope.emptyProperty = false;
                }
                if (loanService.loan.documents.length > 0) {
                    $scope.emptyDocument = false;
                }
                if (loanService.loan.disbursements.length > 0) {
                    $scope.emptyDisbursement = false;
                }
                $scope.loan = loanService.loan;
                $scope.loan.pbalance = $scope.loan.principal - $scope.totalPayment();
                $scope.application.selected = loanService.loan.application;

                var monthlyRate = $scope.loan.rate / 100 / 12;
                var topVal = Math.pow((1 + (monthlyRate)), $scope.loan.months);
                var bottomVal = Math.pow((1 + (monthlyRate)), $scope.loan.months);

                $scope.monthlyPayment = $scope.loan.principal * ((monthlyRate * topVal) / (bottomVal - 1))
                $scope.escrowBalanace = $scope.totalEscrowPayment() - $scope.totalDisbursement();
                //$scope.showApplicants($scope.application.selected, null);
            },
            function () {
                $window.location = "#/";
                $scope.isBusy = false;
            });
    }

    $scope.submitLoan = function (loanForm) {

        $scope.showErrors = true;
        console.log('adfafasdfsafa');
        if (validate(loanForm)) {

            $scope.showErrors = false;
            $scope.loan.application = $scope.application.selected;

            if ($scope.loan.application == null) {
                notificationService.error("Pleas select an application");
                return;
            }

            $scope.submitting = true;
            $scope.models.btnSubmitTitle = "Submitting...";

            loanService.addLoan($scope.loan)
            .then(function () {
                $location.path("loanlist");
            },
            function (result) {
                notificationService.error(result.data.message);
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
                //$scope.error = "Sorry! Fail to submit inquiry. Please check your input value and try again...";
            });
        }
    };

    function validate(loanForm) {

        if (!validateField($scope.loan.maturityDate, '')) {

            $scope.loanForm.cdlastdate.$setValidity('required', false);
            return false;
        }
        if (!validateField($scope.loan.principal, '-1')) {

            $scope.loanForm.cdprincipal.$setValidity('required', false);
            return false;
        }
        if (!validateField($scope.loan.beginDate, '')) {

            $scope.loanForm.cddate.$setValidity('required', false);
            return false;
        }
        if (!validateField($scope.loan.months, '-1')) {

            $scope.loanForm.cdmonth.$setValidity('required', false);
            return false;
        }
        //if (!validateField($scope.loan.escrowInsAmount, '-1')) {

        // $scope.loanForm.cdtaxamount.$setValidity('required', false);
        //}
        if (!validateField($scope.loan.rate, '-1')) {

            $scope.loanForm.cdrate.$setValidity('required', false);
            return false;
        }
        //if (!validateField($scope.loan.escrowTaxAmount, '-1')) {

        // $scope.loanForm.cdinsamount.$setValidity('required', false);
        //}
        console.log("Valid: ", loanForm.$valid);
        return true;
    }

    function validateField(value, placeholder) {

        if (value <= 0) {
            value = '';
        }
        value = stripPrompt(value, placeholder);

        if (isNullOrWhiteSpace(value)) {
            return false;
        }

        return true;
    }

    function stripPrompt(value, prompt) {
        if (isNullOrEmpty(prompt)) {
            prompt = '[prompt]';
        }

        if (value === prompt) {
            return '';
        }

        return trim(value);
    };

    function isNullOrWhiteSpace(str) {
        return (!str || /^\s*$/.test(str));
    };

    function isNullOrEmpty(str) {
        return (!str || 0 == str.length || -1 == str);
    };

    function trim(value) {
        if (isNullOrEmpty(value)) {
            return $.trim(value);
        }

        return value;
    };

    $scope.calcMonthlyPayment = function () {
        if ($scope.loan.rate == null)
            return;
        if ($scope.loan.months == null)
            return;
        if ($scope.loan.principal == null)
            return;


        var monthlyRate = $scope.loan.rate / 100 / 12;
        var topVal = Math.pow((1 + (monthlyRate)), $scope.loan.months);
        var bottomVal = Math.pow((1 + (monthlyRate)), $scope.loan.months);

        $scope.monthlyPayment = $scope.loan.principal * ((monthlyRate * topVal) / (bottomVal - 1))
    };

    $scope.openSelPropertyDlg = function () {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/selectProperty.html',
            controller: 'SelPropertyModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                loan: function () {
                    return $scope.loan;
                },
                sellist: function () {
                    return $scope.selectedProperties;
                }
            }
        });
        modalInstance.result.then(function (retpropertieslist) {
            notificationService.success("Successfully saved.");
            $scope.loan.properties = retpropertieslist;
            if ($scope.loan.properties.length > 0) {
                $scope.emptyProperty = false;
            }
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save property.");
            }
        });
    };

    $scope.deleteProperty = function (delItem) {
        propertyService.removeProperty(delItem.id)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.loan.properties.indexOf(delItem);
            $scope.loan.properties.splice(index, 1);

            if ($scope.loan.properties.length == 0) {
                $scope.emptyProperty = true;
            }
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

    $scope.$on('onOpenPropertyDlg', function (event, args) {
        $scope.onOpenPropertyDlg(null);
    });

    $scope.onOpenPropertyDlg = function (item) {
        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/newPropertyModel.html',
            controller: 'PropertyModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                loan: function () {
                    return $scope.loan;
                },
                property: function () {
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

    $scope.openPaymentDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/loan/paymentModalAdd.html',
            controller: 'PaymentModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                loan: function () {
                    return $scope.loan;
                },
                pbalance: function () {
                    return $scope.loan.pbalance;
                },
                prate: function () {
                    return $scope.loan.rate;
                },
                pescinsamt: function () {
                    return $scope.loan.escrowInsAmount;
                },
                pesctaxamt: function () {
                    return $scope.loan.escrowTaxAmount;
                },
                payment: function () {
                    return item;
                }
            }
        });

        modalInstance.result.then(function (retpaylist) {
            notificationService.success("Successfully saved.");

            $scope.loan.payments = retpaylist;
            if ($scope.loan.payments.length > 0) {
                $scope.emptyPayment = false;
            }
            $scope.loan.pbalance = $scope.loan.principal - $scope.totalPayment();

        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save payment.");
            }
        });
    };

    $scope.deletePayment = function (delItem) {
        loanService.deletePayment(delItem.id)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.loan.payments.indexOf(delItem);
            $scope.loan.payments.splice(index, 1);

            if ($scope.loan.payments.length == 0) {
                $scope.emptyPayment = true;
            }

            $scope.loan.pbalance = $scope.loan.principal - $scope.totalPayment();
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

    $scope.openNewAppDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/application/newAppModal.html',
            controller: 'NewAppModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                origin: function () {
                    return $location.path();
                }
            }
        });

        modalInstance.result.then(function (newApp) {
            var orgin = $location.path();
            $location.path("editapplication/" + newApp.id).search('origin', orgin);
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save employment.");
            }
        });
    };

    $scope.openApplicationDetail = function (selected) {
        var origin = $location.path();

        $location
            .path("editapplication/" + selected.id)
            .search({
                'origin': origin,
                //'loan' : $scope.loan, 
                //'application': $scope.application.selected
            });
    };

    $scope.openDocumentDlg = function () {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/loan/documentModel.html',
            controller: 'SelDocumentModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                loan: function () {
                    return $scope.loan;
                }
            }
        });
        modalInstance.result.then(function (retDocuments) {
            notificationService.success("Successfully saved.");
            $scope.loan.documents = retDocuments;
            if ($scope.loan.documents.length > 0) {
                $scope.emptyDocument = false;
            }
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to upload file.");
            }
        });
    };

    $scope.deleteDocument = function (delItem) {
        loanService.delDocument(delItem.id)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.loan.documents.indexOf(delItem);
            $scope.loan.documents.splice(index, 1);

            if ($scope.loan.documents.length == 0) {
                $scope.emptyDocument = true;
            }
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

    $scope.PrintInvoice = function (id) {
        $http.get('/api/account/getpdf/' + id, { responseType: 'arraybuffer' })
        .success(function (data) {
            var file = new Blob([data], { type: 'application/pdf' });
            var fileURL = URL.createObjectURL(file);
            window.open(fileURL);
        });
    };

    $scope.openDisbursementDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/loan/disbursementModalAdd.html',
            controller: 'DisbursementModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                loan: function () {
                    return $scope.loan;
                },
                disbursement: function () {
                    return item;
                }
            }
        });

        modalInstance.result.then(function (retdislist) {
            notificationService.success("Successfully saved.");

            $scope.loan.disbursements = retdislist;
            if ($scope.loan.disbursements.length > 0) {
                $scope.emptyDisbursement = false;
            }

        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save Disbursement.");
            }
        });
    };

    $scope.deleteDisbursement = function (delItem) {
        loanService.deleteDisbursement(delItem.id)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.loan.disbursements.indexOf(delItem);
            $scope.loan.disbursements.splice(index, 1);

            if ($scope.loan.disbursements.length == 0) {
                $scope.emptyDisbursement = true;
            }
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

}


app.controller('loanController', loanController);

app.controller('editLoanController', editLoanController);

angular.module('BSFinancialApp').controller('PaymentModalCtrl', function ($scope, $modalInstance, loan, pbalance, prate, pescinsamt, pesctaxamt, payment, loanService) {
    $scope.format = 'yyyy/MM/dd';

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.pbalance = pbalance;
    $scope.prate = prate;
    $scope.pescinsamt = pescinsamt / 12;
    $scope.pesctaxamt = pesctaxamt / 12;

    $scope.openPaymentDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.openedPaymentDate = true;
    };

    $scope.loan = loan;
    if (payment == null) {
        payment = new Object();
        payment.loanId = $scope.loan.id;
    }

    $scope.payment = new Object();
    angular.copy(payment, $scope.payment);
    $scope.titleBtnSave = "Save";

    $scope.submitPayment = function () {
        if ($("#paymentForm").valid()) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Saving...";

            var actType = "add";
            var pIndex = 0;

            if (payment.id > 0) {
                actType = "edit";
                pIndex = $scope.loan.payments.indexOf(payment);
            }

            loanService.savePayment($scope.payment, actType, pIndex)
            .then(function () {
                $modalInstance.close(loanService.payments);
            },
            function () {
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
                $scope.titleBtnSave = "Save";
                //$scope.error = "Sorry! Fail to submit inquiry. Please check your input value and try again...";
            });
        }
    };


    $scope.calcPayBreakdown = function () {
        $scope.payment.interestAmt = $scope.pbalance * ($scope.prate / 100 / 12);
        if ($scope.payment.interestAmt > $scope.payment.totalAmt) {
            $scope.payment.interestAmt = $scope.payment.totalAmt;
            $scope.payment.principalAmt = 0;
            $scope.payment.escrowAmt = 0;
            return;
        }
        $scope.payment.interestAmt = parseFloat($scope.payment.interestAmt.toFixed(2));
        $scope.payment.escrowAmt = ($scope.payment.totalAmt - $scope.payment.interestAmt) < 0 ? 0 : (
                ($scope.pescinsamt + $scope.pesctaxamt) > ($scope.payment.totalAmt - $scope.payment.interestAmt)
                ? ($scope.payment.totalAmt - $scope.payment.interestAmt)
                : ($scope.pescinsamt + $scope.pesctaxamt)
            );
        $scope.payment.escrowAmt = parseFloat($scope.payment.escrowAmt.toFixed(2));
        $scope.payment.principalAmt = ($scope.payment.totalAmt - $scope.payment.interestAmt - $scope.payment.escrowAmt) < 0 ? 0 : (
                $scope.payment.totalAmt - $scope.payment.interestAmt - $scope.payment.escrowAmt
            );
        $scope.payment.principalAmt = parseFloat($scope.payment.principalAmt.toFixed(2));
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});

angular.module('BSFinancialApp').controller('SelPropertyModalCtrl', function ($scope, $filter, $modalInstance, loanService, loan, propertyService, sellist, $routeParams) {

    $scope.titleBtnSave = "Save";
    $scope.properties = [];


    propertyService.getNonAssLoanProperties(null)
       .then(function (result) {
           $scope.properties = propertyService.nonAssctLoanProperties;
       },
       function () {
           //alert("could not load properties");
       });


    $scope.dlg = {};

    $scope.goToNewProperty = function () {
        propertyService.openNewPropertyDlg();
        $modalInstance.dismiss('cancel');
    };

    $scope.submitSelectedProperty = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Saving...";
        $scope.properties = $scope.dlg.selectedList;
        $scope.loanId = $routeParams.id;
        loanService.saveLoanProperties($scope.loanId, $scope.properties)
        .then(function (result) {
            $scope.properties = loanService.properties;
            $modalInstance.close(loanService.properties);
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

angular.module('BSFinancialApp').controller('PropertyModalCtrl', function ($scope, $modalInstance, property, loanService, loan, $routeParams) {

    $scope.property = new Object();
    $scope.property = property;

    $scope.isModal = true;
    $scope.openPurchaseDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.openedPurchaseDate = true;
    };

    $scope.openSaleDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.openedSaleDate = true;
    };
    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };
    $scope.format = 'yyyy/MM/dd';
    $scope.titleBtnSave = "Save Property";

    $scope.models = {
        formTitle: 'New Property'
    };
    $scope.submitting = false;

    $scope.loan = loan;
    if (property == null) {
        property = new Object();
        property.loanId = $routeParams.id;
    }
    $scope.submitNewProperty = function () {
        if ($("#propertyForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";

            var actType = "add";
            var pIndex = 0;

            if (property.id > 0) {
                actType = "edit";
                pIndex = $scope.loan.properties.indexOf(property);
            }
            $scope.property.loanId = $routeParams.id;
            loanService.addLoanProperty($scope.property, actType, pIndex)
            .then(function () {
                $modalInstance.close(loanService.properties)
            },
            function () {
                $scope.titleBtnSave = 'Save Property';
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

angular.module('BSFinancialApp').controller('SelDocumentModalCtrl', function ($scope, $modalInstance, fileUpload, loanService, loan, $routeParams) {

    $scope.titleBtnSave = "Save";
    $scope.showErrors = true;
    $scope.document = {};
    $scope.document.description = "";
   

    $scope.uploadFile = function (newDocumentForm) {
        $scope.showErrors = true;
        if (validate(newDocumentForm)) {

            $scope.showErrors = false;
            $scope.submitting = false;
            $scope.titleBtnSave = "Saving...";
            $scope.loanId = $routeParams.id;
            $scope.desc = $scope.document.description;
            var file = $scope.myFile;
            console.log('file is ' + JSON.stringify(file));
            var uploadUrl = "/api/loan/uploadDocument/1";
            //var uploadUrl = "/api/files/Upload";
            //fileUpload.uploadFileToUrl($scope.loanId, $scope.desc, file, uploadUrl);
            var actType = "add";
            var pIndex = 0;

            if ($scope.document.id > 0) {
                actType = "edit";
                pIndex = $scope.loan.documents.indexOf($scope.document);
            }
            loanService.saveDocument($scope.loanId, $scope.desc, file, uploadUrl, actType, pIndex)
            .then(function () {
                $modalInstance.close(loanService.documents)
            },
            function () {
                $scope.titleBtnSave = 'Save Document';
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

    function validate(newDocumentForm) {
        if (!validateField($scope.document.description, '')) {

             newDocumentForm.description.$setValidity('required', false);
        }

        return newDocumentForm.$valid;
    }

    function validateField(value, placeholder) {

        if (value <= 0) {
            value = '';
        }
        value = stripPrompt(value, placeholder);

        if (isNullOrWhiteSpace(value)) {
            return false;
        }

        return true;
    }

    function stripPrompt(value, prompt) {
        if (isNullOrEmpty(prompt)) {
            prompt = '[prompt]';
        }

        if (value === prompt) {
            return '';
        }

        return trim(value);
    };

    function isNullOrWhiteSpace(str) {
        return (!str || /^\s*$/.test(str));
    };

    function isNullOrEmpty(str) {
        return (!str || 0 == str.length || -1 == str);
    };

    function trim(value) {
        if (isNullOrEmpty(value)) {
            return $.trim(value);
        }

        return value;
    };

});

angular.module('BSFinancialApp').controller('DisbursementModalCtrl', function ($scope, $modalInstance, disbursement, loan, loanService, $routeParams) {

    $scope.format = 'yyyy/MM/dd';
    $scope.loan = loan;

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.openPaymentDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.openedPaymentDate = true;
    };
    $scope.disbursement = new Object();


    $scope.totalEscPayment = function () {
        var totalEsc = 0;
        for (count = 0; count < $scope.loan.payments.length; count++) {
            totalEsc += $scope.loan.payments[count].escrowAmt;
        }
        return totalEsc;
    };
    $scope.totalDisbursement = function () {
        var totalDis = 0;
        for (count = 0; count < $scope.loan.disbursements.length; count++) {
            totalDis += $scope.loan.disbursements[count].amount;
        }
        return totalDis;
    }



    $scope.dEscrowBalanace = $scope.totalEscPayment() - $scope.totalDisbursement()



    if (disbursement == null) {
        disbursement = new Object();
        disbursement.loanId = $scope.loan.id;
        disbursement.escrowAmt = $scope.dEscrowBalanace;
    }
    else {
        disbursement.escrowAmt = $scope.dEscrowBalanace - disbursement.amount;
    }



    $scope.escrowBalAmt = 0;


    angular.copy(disbursement, $scope.disbursement);
    $scope.titleBtnSave = "Save";

    $scope.submitDisbursement = function () {
        if ($("#disbursementForm").valid()) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Saving...";

            var actType = "add";
            var pIndex = 0;

            if (disbursement.id > 0) {
                actType = "edit";
                pIndex = $scope.loan.disbursements.indexOf(disbursement);
            }

            loanService.saveDisbursement($scope.disbursement, actType, pIndex)
            .then(function () {
                $scope.disbursements = loanService.disbursements;
                $modalInstance.close(loanService.disbursements);
            },
            function () {
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
                $scope.titleBtnSave = "Save";
            });
        }
    };


    $scope.calcEscrow = function () {
        $scope.disbursement.escrowAmt = $scope.totalEscPayment() - $scope.totalDisbursement() - $scope.disbursement.amount;
        return;
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});
