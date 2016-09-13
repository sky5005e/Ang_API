function subscriptionController($scope, $http, $modal, $location, subscriptionService, notificationService) {
    $scope.data = subscriptionService;
    $scope.isBusy = false;
    $scope.subscription = new Object();
    $scope.upgrade = false;

    $scope.plan1Upgrade = false;
    $scope.plan2Upgrade = false;
    $scope.plan3Upgrade = false;
    $scope.plan4Upgrade = false;

    $scope.planDowngrade = false;
    $scope.plan2Downgrade = false;
    $scope.plan3Downgrade = false;
    $scope.plan4Downgrade = false;

    $scope.companyPlans = true;
    subscriptionService.getSubscription()
        .then(function (result) {
            $scope.subscription = subscriptionService.subscription;

            // $scope.GetCompanyPlans = function () {
            $scope.upgrade = false;
            $scope.companyPlans = true;
            switch ($scope.subscription.planType) {
                case 'Free':
                    $('#divPlan1').addClass(' selected');
                    $scope.plan2Upgrade = true;
                    $scope.plan3Upgrade = true;
                    $scope.plan4Upgrade = true;
                    break;
                case 'Plan2':
                    $('#divPlan2').addClass(' selected');
                    $scope.plan1Downgrade = true;
                    $scope.plan3Upgrade = true;
                    $scope.plan4Upgrade = true;
                    break;
                case 'Plan3':
                    $('#divPlan3').addClass(' selected');
                    $scope.plan1Downgrade = true;
                    $scope.plan2Downgrade = true;
                    $scope.plan4Upgrade = true;
                    break;
                case 'Plan4':
                    $('#divPlan4').addClass(' selected');
                    $scope.plan1Downgrade = true;
                    $scope.plan2Downgrade = true;
                    $scope.plan3Downgrade = true;
                    break;
            }

        },
        function () {
            //alert("could not load inquiries");
        })
        .then(function () {
        });
    $scope.Upgrade = function (planName) {
        if ($scope.subscription.paymentTokenNo != null && $scope.subscription.paymentTokenNo > 0) {

            var cardDetail = {
                paymentTokenNo: $scope.subscription.paymentTokenNo
            };
            subscriptionService.updateSubscription(planName, cardDetail)
            .then(function (result) {
                notificationService.success("Successfully saved.");
                //$location.path($location.lastPath);
                $location.path('/upgradenotice');
                // $scope.openMessageDlg(result);
            },
            function () {
                notificationService.error("Error in update the plan.");
            });

        }
        else {
            $location.path('/payment/' + planName);
            //subscriptionService.updateSubscription(planName);
        }
    };
    $scope.Downgrade = function (planName) {
        if (!$scope.subscription.isAllowedDegrade) {
            notificationService.error("You are not allowed to degrade , as you are already cross the limit for lower plan.");
        } else {
            var cardDetail = {
                paymentTokenNo: $scope.subscription.paymentTokenNo
            };
            subscriptionService.updateSubscription(planName, cardDetail)
            .then(function (result) {
                notificationService.success("Successfully update the plan.");
                $location.path('/upgradenotice');
              //  $scope.openMessageDlg(result);
            },
            function () {
                notificationService.error("Error in update the plan.");
            });
        }

    };


    $scope.openMessageDlg = function (result) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/subscription/subscriptionmessage.html',
            controller: 'MsgModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {result:result}
        });
    };

    subscriptionService.getFundSubscription()
        .then(function (result) {
            $scope.fundSubscription = subscriptionService.fundSubscription;
        },
        function () {
            //alert("could not load inquiries");
        })
        .then(function () {
        });
}

function subscriptionmsgController($scope, $http, $filter, subscriptionService) {
    $scope.message = 'You have Successfully subscribed your packages';

}
app.controller('subscriptionController', subscriptionController)
app.controller('subscriptionmsgController', subscriptionmsgController)


angular.module('BSFinancialApp').controller('MsgModalCtrl', function ($scope, $filter, $modalInstance, $location,result, subscriptionService, $routeParams) {

    $scope.message = 'You have Successfully subscribed your packages';
    $scope.distribution = new Object();
    $scope.goto = function () {
        $location.path('/home');
    };
    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});