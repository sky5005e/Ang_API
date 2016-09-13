var indexOf = function (needle) {
    if (typeof Array.prototype.indexOf === 'function') {
        indexOf = Array.prototype.indexOf;
    } else {
        indexOf = function (needle) {
            var i = -1, index = -1;

            for (i = 0; i < this.length; i++) {
                if (this[i] === needle) {
                    index = i;
                    break;
                }
            }

            return index;
        };
    }

    return indexOf.call(this, needle);
};

app.filter('getApplicantById', function () {
    return function (input, id) {
        var i = 0, len = input.length;
        for (; i < len; i++) {
            if (+input[i].id == id) {
                return input[i];
            }
        }
        return null;
    }
});

app.filter('getSelectedApplicants', function () {
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

app.filter('propsFilter', function () {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            items.forEach(function (item) {
                var itemMatches = false;

                var keys = Object.keys(props);
                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }

                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            // Let the output be the input untouched
            out = items;
        }

        return out;
    };
});


function applicationController($scope, $filter, $http, $modal, $location, applicationService, applicantService) {
    $scope.data = applicationService;
    $scope.isBusy = false;
    $scope.applications = [];
    $scope.ApplicationsFilter = [];

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


    $scope.orderByField = 'applicationName';
    $scope.reverseSort = false;


    applicantService.getApplicants()
        .then(function (result) {
            $scope.applicants = applicantService.applicants;

            applicationService.getApplications()
                .then(function (result) {
                    for (var i = 0; i < applicationService.applications.length; i++) {
                        var selItem = $filter('getApplicantById')($scope.applicants, applicationService.applications[i].applicantId);

                        if (selItem != null) {
                            applicationService.applications[i].applicantName = selItem.firstName + " " + selItem.lastName;
                        }

                        selItem = $filter('getApplicantById')($scope.applicants, applicationService.applications[i].coApplicantId);

                        if (selItem != null) {
                            applicationService.applications[i].coApplicantName = selItem.firstName + " " + selItem.lastName;
                        }
                    }

                    $scope.applications = applicationService.applications;
                },
                function () {
                    //alert("could not load applications");
                })
                .then(function () {
                });
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

    $scope.deleteApplication = function (delid) {
        applicationService.deleteApplication(delid)
        .then(function () {
            $scope.applications = applicationService.applications;
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
    $scope.openNewAppDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/application/newAppModal.html',
            controller: 'NewAppModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                origin: function () {
                    return "";
                }
            }
        });

        modalInstance.result.then(function (newApp) {
            $location.path("editapplication/" + newApp.id).search('origin', '');
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save employment.");
            }
        });
    };
}

function editApplicationController($scope, $filter, $modal, action, applicationService, applicantService, notificationService, $location, $window, $routeParams) {
    $scope.models = {
        btnSubmitTitle: 'Save application',
        formTitle: (action == "add" ? "Add" : "Edit") + ' Application'
    };

    $scope.editMode = action == "add" ? false : true;
    $scope.submitting = false;

    $scope.applicant = {};
    $scope.co_applicant = {};
    $scope.applicants = [];
    $scope.application = new Object();
    $scope.emptyEmp = true;
    $scope.emptyAddr = true;
    $scope.emptyApplicant = true;
    $scope.selectedApplicants = [];
    $scope.mainApplicant = new Object();

    applicantService.getApplicants()
        .then(function (result) {
            $scope.applicants = applicantService.applicants;

            if (action == "edit") {
                applicationService.getApplicationById($routeParams.id)
                    .then(function (application) {
                        $scope.application = applicationService.application;

                        if ($scope.application.employments.length > 0) {
                            $scope.emptyEmp = false;
                        }

                        if ($scope.application.addrs.length > 0) {
                            $scope.emptyAddr = false;
                        }

                        if ($scope.application.applicants.length > 0) {
                            $scope.emptyApplicant = false;
                        }

                        $scope.mainApplicant = $scope.application.mainApplicant;
                        $scope.selectedApplicants = $scope.application.applicants;
                    },
                    function () {
                        $window.location = "#/";
                        $scope.isBusy = false;
                    });
            }

        },
        function () {
            //alert("could not load inquiries");
        })
        .then(function () {
        });


    $scope.submitApplication = function () {
        if ($("#applicationForm").valid() && $scope.submitting == false) {
            if ($scope.mainApplicant == null) {
                notificationService.error("Please select main applicant");
                return;
            }

            $scope.submitting = true;
            $scope.models.btnSubmitTitle = "Submitting...";

            $scope.application.applicantId = $scope.mainApplicant.id;
            $scope.application.mainApplicant = $scope.mainApplicant;
            $scope.application.applicants = $scope.selectedApplicants;

            applicationService.saveApplication($scope.application)
            .then(function () {
                if ($routeParams.origin == "" || $routeParams.origin == undefined) {
                    $location.path("applicationlist");
                } else if ($routeParams.origin.indexOf("newloan") > -1 || $routeParams.origin.indexOf("editloan") > -1) {
                    $location.path($routeParams.origin).search('selaid', $scope.application.id);
                }
            },
            function (result) {
                notificationService.error(result.data.message);
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
                //$scope.error = "Sorry! Fail to submit inquiry. Please check your input value and try again...";
            });
        }
    };

    $scope.openEmpDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/application/employmentModalAdd.html',
            controller: 'EmploymentModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                application: function () {
                    return $scope.application;
                },
                applicants: function () {
                    return $scope.selectedApplicants;
                },
                employment: function () {
                    return item;
                }
            }
        });

        modalInstance.result.then(function (retemplist) {
            notificationService.success("Successfully saved.");

            $scope.application.employments = retemplist;
            if ($scope.application.employments.length > 0) {
                $scope.emptyEmp = false;
            }

        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save employment.");
            }
        });
    };

    $scope.deleteEmploy = function (delItem) {
        applicationService.deleteEmploy(delItem.id)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.application.employments.indexOf(delItem);
            $scope.application.employments.splice(index, 1);

            if ($scope.application.employments.length == 0) {
                $scope.emptyEmp = true;
            }

        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

    $scope.openAddrDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/application/addrModalAdd.html',
            controller: 'AddressModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                application: function () {
                    return $scope.application;
                },
                applicants: function () {
                    return $scope.selectedApplicants;
                },
                addr: function () {
                    return item;
                }
            }
        });

        modalInstance.result.then(function (retemplist) {
            notificationService.success("Successfully saved.");

            $scope.application.addrs = retemplist;
            if ($scope.application.addrs.length > 0) {
                $scope.emptyAddr = false;
            }

        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save employment.");
            }
        });
    };

    $scope.deleteAddr = function (delItem) {
        applicationService.deleteAddr(delItem.id)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.application.addrs.indexOf(delItem);
            $scope.application.addrs.splice(index, 1);

            if ($scope.application.addrs.length == 0) {
                $scope.emptyAddr = true;
            }

        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

    $scope.openSelApplicantDlg = function () {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/application/selectApplicant.html',
            controller: 'SelApplicantModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                application: function () {
                    return $scope.application;
                },
                mainApplicant: function () {
                    return $scope.mainApplicant;
                },
                applicants: function () {
                    return $scope.applicants;
                },
                sellist: function () {
                    //var plist = [];
                    //angular.copy($scope.selectedApplicants, plist);
                    //return plist;
                    return $scope.selectedApplicants;
                }
            }
        });

        modalInstance.result.then(function (application) {
            notificationService.success("Applicants successfully added.");
            $scope.emptyApplicant = false;
            $scope.application = application;
            $scope.selectedApplicants = application.applicants;
            $scope.mainApplicant = application.mainApplicant;
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save employment.");
            }
        });
    };

    $scope.$on('onOpenApplicantDlg', function (event, args) {
        $scope.openApplicantDlg(null);
    });

    $scope.openApplicantDlg = function (item) {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/applicant/applicantAdd.html',
            controller: 'ApplicantModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                application: function () {
                    return $scope.application;
                },
                mainApplicant: function () {
                    return $scope.mainApplicant;
                },
                applicants: function () {
                    return $scope.applicants;
                },
                sellist: function () {
                    return $scope.selectedApplicants;
                },
                action: function () {
                    return "modal";
                }
            }
        });

        modalInstance.result.then(function (rst) {
            notificationService.success("Successfully added.");
            $scope.emptyApplicant = false;

            $scope.applicants.splice(0, 0, rst.applicant);
            //$scope.applicants = application.applicants;
            $scope.mainApplicant = rst.mainApplicant;
            $scope.refreshApplicants($scope.applicants, $scope.selectedApplicants, rst.savedApp.applicants);
            $scope.selectedApplicants = rst.savedApp.applicants;
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to save employment.");
            }
        });
    };

    $scope.refreshApplicants = function (orglist, sellist, newsellist) {
        for (var i = 0; i < sellist.length; i++) {
            var delitem = $filter('getApplicantById')(orglist, sellist[i].id);

            orglist.splice(orglist.indexOf(delitem), 1);
        }

        for (var j = 0; j < newsellist.length; j++) {
            orglist.push(newsellist[j]);
        }

        $scope.applicants = orglist;
    }

    $scope.removeApplicant = function (delInd) {
        if (delInd > -1) {
            var isMain = false;
            //////////If main applicant, remove it.//////////
            if ($scope.selectedApplicants[delInd].id == $scope.mainApplicant.id) {
                isMain = true;
            }

            applicationService.removeApplicant($scope.application.id, $scope.selectedApplicants[delInd].id)
            .then(function (result) {
                notificationService.success("Successfully removed.");
                var deleteditem = $scope.selectedApplicants[delInd];
                $scope.refreshHistory(deleteditem);

                $scope.selectedApplicants.splice(delInd, 1);

                if (isMain) {
                    if ($scope.selectedApplicants.length >= 1) {
                        $scope.mainApplicant = $scope.selectedApplicants[0];
                    } else {
                        $scope.mainApplicant = null;
                    }
                }
            },
            function () {
                notificationService.error("Fail to remove applicant.");
            });
        }
    };

    $scope.refreshHistory = function (deleteditem) {
        var result = $.grep($scope.application.employments, function (e) {
            return e.applicantId != deleteditem.id;
        });

        $scope.application.employments = result;

        var result = $.grep($scope.application.addrs, function (e) {
            return e.applicantId != deleteditem.id;
        });

        $scope.application.addrs = result;
    }

    $scope.selectMainApplicant = function (selItem) {
        $scope.mainApplicant = selItem;
        applicationService.setMainApplicant($scope.application.id, $scope.mainApplicant.id)
        .then(function (result) {
            notificationService.success("Successfully changed.");
        },
        function () {
            notificationService.error("Fail to change main applicant.");
        });
    };
}

app.controller('applicationController', applicationController);
//app.controller('newApplicationController', newApplicationController);
app.controller('editApplicationController', editApplicationController);

angular.module('BSFinancialApp').controller('EmploymentModalCtrl', function ($scope, $filter, $modalInstance, application, applicants, employment, applicationService) {
    $scope.application = application;
    if (employment == null) {
        employment = new Object();
        employment.applicationId = $scope.application.id;
    }

    $scope.applicant = {};
    $scope.applicants = applicants;
    $scope.employment = new Object();

    if (employment.id > 0) {
        var selApplicant = $filter('getApplicantById')($scope.applicants, employment.applicantId);
        $scope.applicant.selected = selApplicant;
    }
    angular.copy(employment, $scope.employment);
    $scope.titleBtnSave = "Save";

    $scope.submitEmployment = function () {
        if ($("#employmentForm").valid()) {
            if ($scope.applicant.selected == null) {
                alert("please select applicant");
                return;
            }

            $scope.submitting = true;
            $scope.titleBtnSave = "Saving...";

            var actType = "add";
            var pIndex = 0;

            if (employment.id > 0) {
                actType = "edit";
                pIndex = $scope.application.employments.indexOf(employment);
            }

            $scope.employment.applicantId = $scope.applicant.selected.id;
            $scope.employment.applicant = $scope.applicant.selected;

            applicationService.saveEmployment($scope.employment, actType, pIndex)
            .then(function () {
                $modalInstance.close(applicationService.employments);
            },
            function () {
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
                $scope.titleBtnSave = "Save";
                //$scope.error = "Sorry! Fail to submit inquiry. Please check your input value and try again...";
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

angular.module('BSFinancialApp').controller('AddressModalCtrl', function ($scope, $filter, $modalInstance, application, applicants, addr, applicationService) {
    $scope.application = application;
    if (addr == null) {
        addr = new Object();
        addr.applicationId = $scope.application.id;
    }

    $scope.applicant = {};
    $scope.applicants = applicants;
    $scope.addr = new Object();

    if (addr.id > 0) {
        var selApplicant = $filter('getApplicantById')($scope.applicants, addr.applicantId);
        $scope.applicant.selected = selApplicant;
    }
    angular.copy(addr, $scope.addr);
    $scope.titleBtnSave = "Save";

    $scope.submitAddr = function () {
        if ($("#addrForm").valid()) {
            if ($scope.applicant.selected == null) {
                alert("please select applicant");
                return;
            }

            $scope.submitting = true;
            $scope.titleBtnSave = "Saving...";

            var actType = "add";
            var pIndex = 0;

            if (addr.id > 0) {
                actType = "edit";
                pIndex = $scope.application.addrs.indexOf(addr);
            }

            $scope.addr.applicantId = $scope.applicant.selected.id;
            $scope.addr.applicant = $scope.applicant.selected;

            applicationService.saveAddr($scope.addr, actType, pIndex)
            .then(function () {
                $modalInstance.close(applicationService.addrs);
            },
            function () {
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
                $scope.titleBtnSave = "Save";
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

angular.module('BSFinancialApp').controller('ApplicantModalCtrl', function ($scope, $filter, $modalInstance, application, mainApplicant, applicants, sellist, action, applicationService, applicantService) {

    $scope.applicant = new Object();

    $scope.isModal = true;
    $scope.titleBtnSave = "Save";

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.format = 'yyyy/MM/dd';
    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.opened = true;
    };

    $scope.submitApplicant = function () {
        if ($("#applicantForm").valid()) {

            $scope.submitting = true;
            $scope.titleBtnSave = "Saving...";

            sellist.splice(0, 0, $scope.applicant);
            if (mainApplicant == null) {
                mainApplicant = $scope.applicant;
            }

            application.applicantId = mainApplicant.id;
            application.mainApplicant = mainApplicant;
            application.applicants = sellist;

            applicationService.saveApplicants(application)
            .then(function (savedApp) {
                var rst = {};
                rst.savedApp = savedApp;
                rst.application = application;
                rst.applicant = $scope.applicant;
                rst.mainApplicant = mainApplicant;
                $modalInstance.close(rst);
            },
            function () {
                $scope.submitting = false;
                $scope.titleBtnSave = "Save";
            });

            /*
            applicantService.addApplicant($scope.applicant)
            .then(function (newApplicant) {
                $modalInstance.close(newApplicant);
            },
            function () {
                $scope.models.btnSubmitTitle = 'Submit';
                $scope.submitting = false;
                $scope.titleBtnSave = "Save";
            });
            */
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});

angular.module('BSFinancialApp').controller('NewAppModalCtrl', function ($scope, $modalInstance, applicationService, origin) {

    $scope.application = new Object();

    $scope.titleBtnSave = "Save";

    $scope.submitNewApp = function () {
        if ($("#newAppForm").valid()) {

            $scope.submitting = true;
            $scope.titleBtnSave = "Saving...";

            applicationService.newApplication($scope.application)
            .then(function (newAppId) {
                $modalInstance.close(newAppId);
            },
            function () {
                $scope.submitting = false;
                $scope.titleBtnSave = "Save";
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

angular.module('BSFinancialApp').controller('SelApplicantModalCtrl', function ($scope, $filter, $modalInstance, application, mainApplicant, applicants, sellist, applicationService) {

    $scope.titleBtnSave = "Save";
    $scope.applicants = applicants;

    $scope.dlg = {};
    $scope.dlg.selectedList = $filter('getSelectedApplicants')($scope.applicants, sellist);

    $scope.goToNewApp = function () {
        applicationService.openNewAppDlg();
        $modalInstance.dismiss('cancel');
    };

    $scope.submitSelectedApp = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Saving...";

        if (mainApplicant == null) {
            mainApplicant = $scope.dlg.selectedList[0];
        }

        application.applicantId = mainApplicant.id;
        application.mainApplicant = mainApplicant;
        application.applicants = $scope.dlg.selectedList;

        applicationService.saveApplicants(application)
        .then(function (newAppId) {
            $modalInstance.close(application);
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