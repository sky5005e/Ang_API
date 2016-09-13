
function propertyController($scope, $http, $modal, propertyService,$location, subscriptionService) {
    $scope.data = propertyService;
    $scope.isBusy = false;
    $scope.properties = [];
    $scope.PropertiesFilter = [];
    $scope.mapView = true;
    $scope.gridView = false;

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

    propertyService.getProperties()
        .then(function (result) {
            $scope.properties = propertyService.properties;
            bindPropertiesOnMap();
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
    $scope.deleteProperty = function (delid) {
        propertyService.deleteProperty(delid)
        .then(function () {
            $scope.properties = propertyService.properties;
        },
        function () {
        });
    };

    $scope.GetMapView = function () {
        $scope.mapView = true;
        $scope.gridView = false;
    };
    $scope.GetGridView = function () {
        $scope.mapView = false;
        $scope.gridView = true;
    };

    function bindPropertiesOnMap() {
        var properties = $scope.properties;
        var locations = [];
        for (var i = 0; i < properties.length; i++) {
            locations.push([properties[i].address, properties[i].latitude, properties[i].longitude, i + 1]);
        }
        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 10,
            center: new google.maps.LatLng(29.76, -95.36),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });
        var infowindow = new google.maps.InfoWindow();
        var marker, i;
        for (i = 0; i < locations.length; i++) {
            marker = new google.maps.Marker({
                position: new google.maps.LatLng(locations[i][1], locations[i][2]),
                map: map
            });
            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infowindow.setContent(locations[i][0]);
                    infowindow.open(map, marker);
                }
            })(marker, i));
        }
    }

    subscriptionService.getPropertySubscription()
     .then(function (result) {
         // var flist = $filter('getValidApplication')(loanService.applications);
         $scope.propertySubscription = subscriptionService.propertySubscription;
         if (!angular.isUndefinedOrNull($scope.propertySubscription.isSubscribed) && !$scope.propertySubscription.isSubscribed) {
             $location.lastPath = '/property';
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
}

function newPropertyController($scope, propertyService, $http, $location, $window) {
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
    $scope.emptyDocument = true;
    $scope.emptyPhoto = true;

    $scope.submitProperty = function () {
        if ($("#propertyForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";
            propertyService.addProperty($scope.property)
            .then(function () {
                $location.path("properties");
            },
            function () {
                $scope.titleBtnSave = 'Save Property';
                $scope.submitting = false;
            });
        }
    };

    $scope.cancel = function () {
        window.history.back();
    };

    $scope.getGeocode = function () {
        var geocoder23 = new google.maps.Geocoder();
        var geocoder = new google.maps.Geocoder();
        var address = $scope.property.address;
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                $scope.property.latitude = results[0].geometry.location.lat();
                $scope.property.longitude = results[0].geometry.location.lng();
                $scope.$apply();
                //alert("Latitude: " + latitude + "\nLongitude: " + longitude);
            } else {
                alert("Request failed.")
            }
        });
    };

}

function editPropertyController($scope, $modal, propertyService, notificationService, $location, $window, $routeParams) {
    $scope.photos = [];
    $scope.documents = [];
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
        formTitle: 'Edit Property'
    };
    $scope.submitting = false;
    $scope.emptyDocument = true;
    $scope.emptyPhoto = true;
    $scope.editMode = true;
    $scope.property = null;
    propertyService.getPropertyById($routeParams.id)
        .then(function (property) {
            $scope.property = propertyService.property;
            $scope.photos = propertyService.photos;
            $scope.documents = propertyService.documents;
            if (propertyService.property.id > 0 && $scope.photos.length > 0) {
                $scope.emptyPhoto = false;
            }
            if (propertyService.property.id > 0 && $scope.documents.length > 0) {
                $scope.emptyDocument = false;
            }
        },
        function () {
            $window.location = "#/";
            $scope.isBusy = false;
        });

    $scope.submitProperty = function () {
        if ($("#propertyForm").valid() && $scope.submitting == false) {
            $scope.submitting = true;
            $scope.titleBtnSave = "Submitting...";

            propertyService.addProperty($scope.property)
            .then(function () {
                $location.path("properties");
            },
            function () {
                $scope.titleBtnSave = 'Save Property';
                $scope.submitting = false;
            });
        }
    };

    $scope.cancel = function () {
        window.history.back();
    };

    $scope.getGeocode = function () {
        var geocoder23 = new google.maps.Geocoder();
        var geocoder = new google.maps.Geocoder();
        var address = $scope.property.address;
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                $scope.property.latitude = results[0].geometry.location.lat();
                $scope.property.longitude = results[0].geometry.location.lng();
                $scope.$apply();
                //alert("Latitude: " + latitude + "\nLongitude: " + longitude);
            } else {
                alert("Request failed.")
            }
        });
    };

    $scope.openDocumentDlg = function () {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/propertyDocumentModal.html',
            controller: 'SelPropertyDocumentModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                property: function () {
                    return $scope.property;
                }
            }
        });
        modalInstance.result.then(function (retDocuments) {
            notificationService.success("Successfully saved.");
            $scope.property.documents = retDocuments;
            if ($scope.property.documents.length > 0) {
                $scope.emptyDocument = false;
            }
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to upload file.");
            }
        });
    };

    $scope.deletePropDocument = function (delItem) {
        $scope.propertyId = $routeParams.id;
        
        propertyService.delDocument(delItem.id, $scope.propertyId)
        .then(function () {
            notificationService.success("Successfully deleted!");

            var index = $scope.property.documents.indexOf(delItem);
            $scope.property.documents.splice(index, 1);

            if (propertyService.property.id > 0) {
                $scope.emptyDocument = false;
            }
        },
        function () {
            notificationService.error("Fail to delete!");
        });
    };

    $scope.openPhotoDlg = function () {

        var modalInstance = $modal.open({
            templateUrl: '/app/views/property/propertyDocumentModal.html',
            controller: 'SelPropertyPhotoModalCtrl',
            //size: size,
            backdrop: 'static',
            resolve: {
                property: function () {
                    return $scope.property;
                }
            }
        });
        modalInstance.result.then(function (retPhotos) {
            notificationService.success("Successfully saved.");
            $scope.property.photos = retPhotos;
            if ($scope.property.photos.length > 0) {
                $scope.emptyPhoto = false;
            }
        }, function (data) {
            if (data != "cancel") {
                notificationService.error("Fail to upload file.");
            }
        });
    };

    // initial image index
    $scope._Index = 0;

    // if a current image is the same as requested image
    $scope.isActive = function (index) {
        return $scope._Index === index;
    };

    // show prev image
    $scope.showPrev = function () {
        $scope._Index = ($scope._Index > 0) ? --$scope._Index : $scope.photos.length - 1;
    };

    // show next image
    $scope.showNext = function () {
        $scope._Index = ($scope._Index < $scope.photos.length - 1) ? ++$scope._Index : 0;
    };

    // show a certain image
    $scope.showPhoto = function (index) {
        $scope._Index = index;
    };
}

app.controller('propertyController', propertyController);
app.controller('newPropertyController', newPropertyController);
app.controller('editPropertyController', editPropertyController);


angular.module('BSFinancialApp').controller('SelPropertyDocumentModalCtrl', function ($scope, $modalInstance, fileUpload, propertyService, property, $routeParams) {

    $scope.titleBtnSave = "Save Document";
    $scope.models = {
        formTitle: 'Please upload document'
    };

    $scope.uploadFile = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Saving...";
        $scope.propertyId = $routeParams.id;
        $scope.desc = $scope.document.description;
        var file = $scope.myFile;
        console.log('file is ' + JSON.stringify(file));
        var uploadUrl = "/api/property/uploadDocument/1";
        var actType = "add";
        var pIndex = 0;

        if ($scope.document.id > 0) {
            actType = "edit";
            pIndex = $scope.property.documents.indexOf($scope.document);
        }
        propertyService.saveDocument($scope.propertyId, $scope.desc, file, uploadUrl, actType, pIndex)
        .then(function () {
            $modalInstance.close(propertyService.documents)
        },
        function () {
            $scope.titleBtnSave = 'Save Document';
            $scope.submitting = false;
        });

    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});


angular.module('BSFinancialApp').controller('SelPropertyPhotoModalCtrl', function ($scope, $modalInstance, fileUpload, propertyService, property, $routeParams) {

    $scope.titleBtnSave = "Save Photo";
    $scope.models = {
        formTitle: 'Please upload photo'
    };

    $scope.uploadFile = function () {
        $scope.submitting = true;
        $scope.titleBtnSave = "Saving...";
        $scope.propertyId = $routeParams.id;
        $scope.desc = $scope.document.description;
        var file = $scope.myFile;
        console.log('file is ' + JSON.stringify(file));
        var uploadUrl = "/api/property/uploadPhoto/1";
        var actType = "add";
        var pIndex = 0;

        if ($scope.document.id > 0) {
            actType = "edit";
            pIndex = $scope.property.photos.indexOf($scope.document);
        }
        propertyService.savePhoto($scope.propertyId, $scope.desc, file, uploadUrl, actType, pIndex)
        .then(function () {
            $modalInstance.close(propertyService.photos);
        },
        function () {
            $scope.titleBtnSave = 'Save Photo';
            $scope.submitting = false;
        });

    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$routeChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
});

