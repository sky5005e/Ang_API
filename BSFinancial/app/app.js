var app = angular.module('BSFinancialApp', [
    'ngRoute',
    'ngSanitize',
    'ngBootbox',
    'ngDialog',
    'LocalStorageModule',
    'angular-loading-bar',
    'angular-ladda',
    'angularValidator',
    'toastr',
    'datatables',
    'ui.bootstrap',
    'ui.select',
    'angularFileUpload',
    'ngTouch',
    'ngIdle'

]);

app.config(function($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/register", {
        controller: "signupController",
        templateUrl: "/app/views/registration.html"
    });

    $routeProvider.when("/confirmation", {
        controller: "signupController",
        templateUrl: "/app/views/accountconfirmation.html"
    });

    $routeProvider.when("/refresh", {
        controller: "refreshController",
        templateUrl: "/app/views/refresh.html"
    });

    $routeProvider.when("/tokens", {
        controller: "tokensManagerController",
        templateUrl: "/app/views/tokens.html"
    });

    $routeProvider.when("/associate", {
        controller: "associateController",
        templateUrl: "/app/views/associate.html"
    });

    $routeProvider.when("/inquirylist", {
        controller: "inquiryController",
        templateUrl: "/app/views/inquiry/inquiryList.html"
    });

    $routeProvider.when("/newinquiry", {
        controller: "newInquiryController",
        templateUrl: "/app/views/inquiry/newInquiryView.html"
    });
    $routeProvider.when("/newinquirysucc", {
        templateUrl: "/app/views/inquiry/newInquirySucc.html"
    });

    $routeProvider.when("/inquiry/:id", {
        controller: "singleInquiryController",
        templateUrl: "/app/views/inquiry/singleInquiryView.html"
    });

    $routeProvider.when("/loanlist", {
        controller: "loanController",
        templateUrl: "/app/views/loan/loanList.html"
    });

    $routeProvider.when("/userlist", {
        controller: "userController",
        templateUrl: "/app/views/user/userlist.html"
    });

    $routeProvider.when("/newloan", {
        controller: "editLoanController",
        templateUrl: "/app/views/loan/loanAdd.html",
        resolve: {
            action: function() {
                return "add";
            }
        }
    });

    $routeProvider.when("/editloan/:id", {
        controller: "editLoanController",
        templateUrl: "/app/views/loan/loanAdd.html",
        resolve: {
            action: function () {
                return "edit";
            }
        }
    });
    $routeProvider.when("/payments", {
        controller: "paymentController",
        templateUrl: "/app/views/loan/paymentList.html"
    });
    $routeProvider.when("/applicantlist", {
        controller: "applicantController",
        templateUrl: "/app/views/applicant/applicantList.html"
    });
    $routeProvider.when("/newapplicant", {
        controller: "newApplicantController",
        templateUrl: "/app/views/applicant/applicantAdd.html"
    });

    $routeProvider.when("/editapplicant/:id", {
        controller: "editApplicantController",
        templateUrl: "/app/views/applicant/applicantAdd.html"
    });

    $routeProvider.when("/applicationlist", {
        controller: "applicationController",
        templateUrl: "/app/views/application/applicationList.html"
    });
    /*
    $routeProvider.when("/newapplication", {
        controller: "editApplicationController",
        templateUrl: "/app/views/application/applicationAdd.html",
        resolve: {
            action: function () {
                return "add";
            }
        }
    });
    */

    $routeProvider.when("/editapplication/:id", {
        controller: "editApplicationController",
        templateUrl: "/app/views/application/applicationAdd.html",
        resolve: {
            action: function() {
                return "edit";
            }
        }
    });
    // Property
    $routeProvider.when("/properties", {
        controller: "propertyController",
        templateUrl: "/app/views/property/propertyList.html"
    });
   
    $routeProvider.when("/newproperty", {
        controller: "newPropertyController",
        templateUrl: "/app/views/property/propertyAdd.html"
    });

    $routeProvider.when("/editproperty/:id", {
        controller: "editPropertyController",
        templateUrl: "/app/views/property/propertyAdd.html"
    });

    // Property - Prospects
    $routeProvider.when("/prospects", {
        controller: "prospectsController",
        templateUrl: "/app/views/property/prospects.html"
    });
    // Property - Proposals
    $routeProvider.when("/proposals", {
        controller: "propertyProspectsController",
        templateUrl: "/app/views/property/propertyProspects.html"
    });
    $routeProvider.when("/editproposal/:id", {
        controller: "editpropertyProspectsController",
        templateUrl: "/app/views/property/propertyProposals.html"
    });

    //Proposal Properties List 
    $routeProvider.when("/proposalproperties/:id/:tokenKey", {
        controller: "proposalpropertyController",
        templateUrl: "/app/views/property/proposalproperties.html"
    });
    $routeProvider.when("/proposalstats/:id", {
        controller: "propertystatsController",
        templateUrl: "/app/views/property/proposalstats.html"
    });
    // property detail info
    $routeProvider.when("/propertyinfo/:id/:proposalId", {
        controller: "propertyInfoController",
        templateUrl: "/app/views/property/propertydetails.html"
    });


    $routeProvider.when("/fundlist", {
        controller: "fundController",
        templateUrl: "/app/views/fund/fundList.html"
    });

    $routeProvider.when("/editfund/:id", {
        controller: "editfundController",
        templateUrl: "/app/views/fund/fundAdd.html"
    });
    $routeProvider.when("/investorlist", {
        controller: "investorController",
        templateUrl: "/app/views/fund/investorList.html"
    });
    $routeProvider.when("/newinvestor", {
        controller: "newInvestorController",
        templateUrl: "/app/views/fund/investorAdd.html"
    });
    $routeProvider.when("/editinvestor/:id", {
        controller: "editInvestorController",
        templateUrl: "/app/views/fund/investorAdd.html",
        resolve: {
            action: function() {
                return "edit";
            }
        }
    });
    $routeProvider.when("/subscription", {
        controller: "subscriptionController",
        templateUrl: "/app/views/subscription/subscription.html"
    });
    $routeProvider.when("/upgradenotice", {
        controller: "subscriptionmsgController",
        templateUrl: "/app/views/subscription/upgradenotice.html"
    });
    $routeProvider.when("/payment/:plan", {
        controller: "paymentController",
        templateUrl: "/app/views/payment/payment.html"
    });
    $routeProvider.when("/payment", {
        controller: "paymentController",
        templateUrl: "/app/views/payment/payment.html"
    });
    $routeProvider.otherwise({ redirectTo: "/home" });
});

//var serviceBase = 'http://localhost:3242/';
var serviceBase = '/';
//var serviceBase = 'http://ngauthenticationapi.azurewebsites.net/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'ngAuthApp'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


app.config(function (IdleProvider, KeepaliveProvider) {
    KeepaliveProvider.interval(10);
});

app.run(function ($rootScope, Idle, $log, Keepalive) {
    Idle.watch();

    $log.debug('app started.');
});

angular.isUndefinedOrNull = function (val) {
    return angular.isUndefined(val) || val === null;
}