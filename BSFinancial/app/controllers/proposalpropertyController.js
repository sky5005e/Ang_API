function proposalpropertyController($scope, $http, $modal,authService, propertyProposalService, $location, $routeParams) {
    $scope.data = propertyProposalService;
    $scope.isBusy = false;
    $scope.properties = [];
    $scope.mapView = true;
    $scope.gridView = false;

    $scope.ShowHeader = {};
    authService.checkMenuDisplay($location.path()).then(function (result) {
        $scope.ShowHeader = authService.showHeader;
    },
        function () {
            //alert("could not load users");
        })
        .then(function () {
        });

    $scope.ShowHeader = authService.showHeader;

    propertyProposalService.getProposalById($routeParams.id)
        .then(function (result) {
            $scope.properties = propertyProposalService.properties;
            bindPropertiesOnMap();
        },
        function () {
            //alert("could not load users");
        })
        .then(function () {
        });
    
    $scope.GetMapView = function () {
        $scope.mapView = true;
        $scope.gridView = false;
    };
    $scope.GetGridView = function () {
        $scope.mapView = false;
        $scope.gridView = true;
    };
    $scope.SubmitViewInfo = function (propertyid) {

        $scope.proposalId = $routeParams.id;
        propertyProposalService.savePropertyViewDetails($scope.proposalId, propertyid)
        .then(function () {
            $location.path('/proposalstats/' + $scope.proposalId);

        },
        function () {
            
        });
    };
    function bindPropertiesOnMap() {
        var properties = $scope.properties;
        var locations = [];
        for (var i = 0; i < properties.length; i++) {
            var iteminfo = properties[i].address + ',' + properties[i].city + '<br/> Build Sq ft:' + properties[i].buildSquareFt + '<br/> Sales Price :' + properties[i].askingPrice + '<br/> <a href="#/propertyinfo/' + properties[i].id + '/' + $routeParams.id + ' ">Details</a>';
            var title = properties[i].address;
            var icon = '';
            if (i == 0)
            {
                icon = 'http://labs.google.com/ridefinder/images/mm_20_red.png';
            }
            else
            {
                icon = 'http://labs.google.com/ridefinder/images/mm_20_green.png'
            }
            locations.push([iteminfo, properties[i].latitude, properties[i].longitude, icon, title, i + 1]);
           // alert(iteminfo);
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
                map: map,
                icon: locations[i][3],
                title: locations[i][4]
            });
            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infowindow.setContent(locations[i][0]);
                    infowindow.open(map, marker);
                }
            })(marker, i));
        }
    }
    

}

function propertystatsController($scope, $http, propertyProposalService, notificationService, $location, $routeParams) {
    $scope.data = propertyProposalService;
    $scope.isBusy = false;
    $scope.properties = [];
    propertyProposalService.getPropertyViewDetails($routeParams.id)
        .then(function (result) {
            $scope.properties = propertyProposalService.viewedproperties;
        },
        function () {
        })
        .then(function () {
        });
    $scope.SubmitViewInfo = function (propertyid) {

        $scope.proposalId = $routeParams.id;
        propertyProposalService.savePropertyViewDetails($scope.proposalId, propertyid)
        .then(function () {
            $location.path('/propertyinfo/' + propertyid + '/' + $scope.proposalId);

        },
        function () {

        });
    };
}

function propertyInfoController($scope, $http, $modal, authService, propertyProposalService, $location, $routeParams) {
    $scope.data = propertyProposalService;
    $scope.isBusy = false;
    $scope.properties = [];
    $scope.property = new Object();
    $scope.infoView = true;
    $scope.mapView = false;
    $scope.photosView = false;
    $scope.photos = [];
    $scope.titleBtnSave = "Save Rating";

    $scope.start = new Date();
    


    function init() {
        var id = $routeParams.id;
        var proposalId = $routeParams.proposalId;
       // alert($scope.start);
        // alert(id);
        //alert(proposalId);
    }

    init();

    $scope.GetInfoView = function () {
        $scope.infoView = true;
        $scope.mapView = false;
        $scope.photosView = false;
    };
    $scope.GetMapView = function () {
        $scope.mapView = true;
        $scope.infoView = false;
        $scope.photosView = false;

        window.setTimeout(function () {

            google.maps.event.trigger(map, 'resize');
        }, 100);
     };
    $scope.GetPhotoView = function () {
        $scope.infoView = false;
        $scope.mapView = false;
        $scope.photosView = true;

    };


    // array = [{key:value},{key:value}]
    function objectFindByKey(array, key, value) {
        for (var i = 0; i < array.length; i++) {
            if (array[i][key] == value) {
                return array[i];
            }
        }
        return null;
    }

    propertyProposalService.getProposalById($routeParams.proposalId)
        .then(function (result) {
            $scope.properties = propertyProposalService.properties;
            bindPropertiesOnMap();

            if ($scope.properties.length > 0) {
                $scope.property = objectFindByKey($scope.properties, 'id', $routeParams.id);
                // Get Rating info
                propertyProposalService.getRatingInfo($routeParams.id).then(function () {
                    $scope.rating = propertyProposalService.rating;
                },
                 function () {

                 });

                // load photo
                propertyProposalService.getPhotosbyPropoertyId($routeParams.id).then(function () {
                    $scope.photos = propertyProposalService.photos;
                },
                 function () {

                 });
            }
           

        },
        function () {
            //alert("could not load users");
        })
        .then(function () {
        });

    
    $scope.getSelectedRating = function (rating) {
        console.log(rating);
    }

    $scope.SaveRatingProperty = function () {
        $scope.rating = $scope.rating;
        $scope.rating.propertyid = $routeParams.id;
        propertyProposalService.saveRatingInfo($routeParams.id,$scope.rating)
        .then(function () {
            $scope.rating = propertyProposalService.rating;
        },
        function () {

        });
    };

    function bindPropertiesOnMap() {
        var properties = $scope.properties;
        var locations = [];
        for (var i = 0; i < properties.length; i++) {
            var iteminfo = properties[i].address + ',' + properties[i].city + '<br/> Build Sq ft:' + properties[i].buildSquareFt + '<br/> Sales Price :' + properties[i].askingPrice + '<br/> <a href="#/propertyinfo/' + properties[i].id + '/'+ $routeParams.proposalId +' ">Details</a>';
            var title = properties[i].address;
            var icon = '';
            if (properties[i].id == $routeParams.id) {
                icon = 'http://labs.google.com/ridefinder/images/mm_20_red.png';
            }
            else {
                icon = 'http://labs.google.com/ridefinder/images/mm_20_green.png'
            }
            locations.push([iteminfo, properties[i].latitude, properties[i].longitude, icon, title, i + 1]);
            // alert(iteminfo);
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
                map: map,
                icon: locations[i][3],
                title: locations[i][4]
            });
            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infowindow.setContent(locations[i][0]);
                    infowindow.open(map, marker);
                }
            })(marker, i));
        }
    }

    $scope.$on('$routeChangeStart', function () {
        //alert(window.location.href);
        $scope.endTime = new Date();
        $scope.timespent = (($scope.endTime - $scope.start) / (1000 * 60)) % 60; // to store time in minutes
        //alert($scope.timespent);
        $scope.propertyViewInfo = new Object();
        $scope.propertyViewInfo.propertyId = $routeParams.id;
        $scope.propertyViewInfo.ProposalId = $routeParams.proposalId;
        $scope.propertyViewInfo.lastViewDate = new Date();
        $scope.propertyViewInfo.viewDuration = $scope.timespent;
        propertyProposalService.savePropertyViewTime($routeParams.id, $scope.propertyViewInfo)
        .then(function () {
        },
            function () {

            });
        });
}

app.controller('proposalpropertyController', proposalpropertyController);
app.controller('propertystatsController', propertystatsController);
app.controller('propertyInfoController', propertyInfoController);
