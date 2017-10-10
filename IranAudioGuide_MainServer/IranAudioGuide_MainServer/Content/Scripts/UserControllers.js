//Developed by pourmand

userApp.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

userApp.service('fileUpload', ['$http', function ($http) {
    this.uploadFileToUrl = function (file, uploadUrl) {
        var fd = new FormData();
        fd.append('file', file);
        $http.post(uploadUrl, fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        })
            .success(function () {
            })
            .error(function () {
            });
    }
}]);
userApp.controller('userCtrl', ['$window', '$scope', 'userServices', '$timeout', 'notificService', '$http', '$state',
    function ($window, $scope, userServices, $timeout, notific, $http, $state) {

        $scope.profile = {
            istest: true,
            packages: [],
            //packagesPurchased: [],
            isCompletedLoading: true,
            waiting: 0,
            page: 1
        };
        $(".dropdown-button").dropdown();



        $scope.$watch("profile.waiting", function (newval, oldval) {
            if (typeof newval == undefined)
                return;
            if ($scope.profile.waiting < 3)
                return;

            $scope.profile.isCompletedLoading = false;
        });
        //var stopeLoading = function () {
            
        //    console.log(" $scope.profile.waiting", $scope.profile.waiting);
        //    if ($scope.profile.waiting >= 3) {

        //        $scope.profile.isCompletedLoading = false;

        //    }
        //    else {
        //        stopeLoading();
        //    }
        //};
        var initMenu = function (side) {
            $('.button-collapse').sideNav({
                menuWidth: 300, // Default is 300
                edge: side, // Choose the horizontal origin
                closeOnClick: true, // Closes side-nav on <a> clicks, useful for Angular/Meteor
                draggable: true, // Choose whether you can drag to open on touch screens,
                onOpen: function (el) { /* Do Stuff*/ }, // A function to be called when sideNav is opened
                onClose: function (el) { /* Do Stuff*/ }, // A function to be called when sideNav is closed
            });
        };

        userServices.getLang().then(function (lang) {
            userServices.baseUrl = "/" + lang;
            var side = 'left';
            if (lang == "fa")
                side = 'right';
            initMenu(side);
            $scope.profile.waiting++;
        }, function (lang) {
            userServices.baseUrl = "/en";
            initMenu('left');
            $scope.profile.waiting++;
        });

        var timeoutID = window.setTimeout(function () {
            $(".userPreloader.progress").addClass("hidden");
        }, [1000]);
        $('.button-collapse').sideNav({
            menuWidth: 300, // Default is 300
            edge: 'left', // Choose the horizontal origin
            closeOnClick: true, // Closes side-nav on <a> clicks, useful for Angular/Meteor
            draggable: true, // Choose whether you can drag to open on touch screens,
            onOpen: function (el) { /* Do Stuff*/ }, // A function to be called when sideNav is opened
            onClose: function (el) { /* Do Stuff*/ }, // A function to be called when sideNav is closed
        });
        // end menu 
        $('.modal').modal({
            dismissible: true, // Modal can be dismissed by clicking outside of the modal
            //opacity: .5, // Opacity of modal background
            //inDuration: 300, // Transition in duration
            //outDuration: 200, // Transition out duration
            //startingTop: '4%', // Starting top style attribute
            //endingTop: '10%', // Ending top style attribute
            ready: function (modal, trigger) { // Callback for Modal open. Modal and trigger parameters available.
                console.log("modal Ready");
                //console.log(modal, trigger);
            },
            complete: function () {
                console.log('modal Closed');
            } // Callback for Modal close
        }
        );
        $scope.OpenModal = function (item, isPalce) {

            if (isPalce) {
                $scope.itemPack = {
                    PackageName: item.PlaceName,
                    PackageId: item.PlaceId
                };
                if (item.isPrimary) {
                    $scope.itemPack.PackagePriceDollar = 0;
                    $scope.itemPack.PackagePrice = 0;
                }
                else {
                    $scope.itemPack.PackagePriceDollar = item.PriceDollar;
                    $scope.itemPack.PackagePrice = item.Price;
                }

            }
            else
                $scope.itemPack = item;
            $scope.itemPack.isPalce = isPalce;
            $('#modal1').modal('open');

        };
        $scope.closeModal = function () {
            $('#modal1').modal('close');
        }

        $scope.buyPakages = function (bank) {
            $('#modal1').modal('close');
            $scope.profile.isCompletedLoading = true;

            $state.go('Payment', {
                PackageId: $scope.itemPack.PackageId,
                ChooesBank: bank,
                isPlace: $scope.itemPack.isPalce
            });
        }


        //end modal


        $scope.isloadImage = true;
        $('.collapsible').collapsible({
            accordion: false, // A setting that changes the collapsible behavior to expandable instead of the default accordion style
            onOpen: function (el) {
                var that = el;
                $timeout(function () {
                    that.city.IsloadImage = false;
                }, 10000);


            }, // Callback for Collapsible open
            onClose: function (el) { console.log(el, 'Closed'); } // Callback for Collapsible close
        });
        $scope.OpenCollapsibleBody = function (city) {
            console.log("click", city);
            $scope.city
        }
        var refreshPack = function () {
            $timeout(function () {
                $('.collapsible').collapsible();
                $scope.profile.waiting++;
            }, 1000);
            $timeout(function () {
                $scope.isloadImage = false;
            }, 100000);

            $scope.listPakages = angular.copy($scope.profile.packages)
        }
        //if ($scope.profile.packagesPurchased.length === 0) {
        //    userServices.getPackagesPurchased().then(function (data) {
        //        $scope.profile.packagesPurchased = data;
        //        stopeLoading();

        //    }, function () {
        //        stopeLoading();
        //    });
        //}
        if ($scope.profile.packages.length === 0) {
            userServices.getPackages().then(function (data) {
                $scope.profile.packages = data;
                refreshPack();
            }, function () {
                refreshPack();

            });
        }
        else {
            refreshPack();
        }
        $scope.uploadFile = function (files) {
            var fd = new FormData();
            //Take the first selected file
            fd.append("file", files[0]);

            $http.post("/api/UserApi/UploadFile", fd, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).then(function () {
                //console.log("test success")
            }, function (error) {
                //console.log("error", error)
            });

        };
        $scope.user = {
            isAutintication: false,
            IsEmailConfirmed: true,
        }
        //$scope.LogOff = function () {
        //    userServices.LogOff();
        //    $window.location.href = 'http://iranaudioguide.com';
        //}
        $scope.notImage = true;

        userServices.getUser().then(function (data) {
            try {
                if (typeof data.FullName != undefined || data.FullName != null)
                    $scope.user.FullName = data.FullName;

                if (typeof data.Email != undefined)
                    $scope.user.Email = data.Email;

                if (typeof data.imgUrl != undefined || data.imgUrl === null) {
                    $scope.user.imgUrl = data.imgUrl;
                    $scope.notImage = false;

                }

                if (typeof data.IsEmailConfirmed != undefined)
                    $scope.user.IsEmailConfirmed = data.IsEmailConfirmed;

                if (typeof data.IsSetuuid != undefined)
                    $scope.user.IsSetuuid = data.IsSetuuid;

                if (typeof data.IsAccessChangeUuid != undefined)
                    $scope.user.IsAccessChangeUuid = data.IsAccessChangeUuid;

                if (typeof data.TimeSetUuid != undefined)
                    $scope.user.TimeSetUuid = data.TimeSetUuid;

                if (typeof data.IsForeign != undefined)
                    $scope.user.IsForeign = data.IsForeign;

                $scope.user.isAutintication = true;
                $scope.profile.waiting++;


            }
            catch (error) {
                $scope.user.FullName = data.FullName;
                $scope.user.Email = data.Email;
                $scope.user.IsSetuuid = false;
                $scope.user.IsAccessChangeUuid = false;
                $scope.user.IsForeign = data.IsForeign;
                $scope.profile.waiting++;


            } 
        });



        //$timeout(function () {
        //    angular.element('#btnGetPakage').triggerHandler('click');
        //}, 0);

        $scope.sendEmailConfirmed = false;

        $scope.sendEmailConfirmedAgain = function () {

            $scope.sendEmailConfirmed = true;
            userServices.sendEmailConfirmedAgain().then(function (data) {
                $scope.sendEmailConfirmed = false;
                if (data.status === 0)
                    notific.success("", data.content);

            }, function (error) {

                $scope.sendEmailConfirmed = false;

            });
        }


        //var initPakage = function () {

        //    $scope.pakageForSelected = [];
        //    $scope.selectId = 0;
        //    $scope.pakageForSelected = [];
        //    _.each($scope.profile.packages, function (pak) {
        //        _.each(pak.PackageCities, function (city) {
        //            var findItem = false;
        //            for (var i = 0; i < $scope.pakageForSelected.length; i++) {
        //                var itemEntry = $scope.pakageForSelected[i];
        //                if (itemEntry.idItem == city.CityID) {
        //                    itemEntry.packages.push(pak)
        //                    findItem = true;
        //                    break;
        //                }
        //            }
        //            if (!findItem)
        //                $scope.pakageForSelected.push({
        //                    id: $scope.selectId++,
        //                    idItem: city.CityID,
        //                    // این گزینه برای این است که در جستجو  پکبج ها مشکلی پیش نیاد
        //                    CityID: city.CityID,// 
        //                    type: $scope.typeEachItemFoSelection[0].type,
        //                    title: city.CityName,
        //                    description: "There are " + city.Places.length + " locations for this city",
        //                    packages: [pak]
        //                })
        //            _.each(city.Places, function (plc) {
        //                var findItem = false;
        //                for (var i = 0; i < $scope.pakageForSelected.length; i++) {
        //                    var itemEntry = $scope.pakageForSelected[i];
        //                    if (itemEntry.idItem == plc.PlaceId) {
        //                        itemEntry.packages.push(pak)
        //                        findItem = true;
        //                        break;
        //                    }
        //                }
        //                if (!findItem)
        //                    $scope.pakageForSelected.push({
        //                        id: $scope.selectId++,
        //                        idItem: plc.PlaceId,
        //                        CityID: city.CityID,
        //                        type: $scope.typeEachItemFoSelection[1].type,
        //                        title: plc.PlaceName,
        //                        description: "There is this place in " + city.CityName,
        //                        packages: [pak]
        //                    })
        //            });//end each place
        //        });//end each pack.PackageCities

        //    })//end each $scope.profile.package

        //    $scope.listPakages = angular.copy($scope.profile.packages);
        //    refreshPackCss();
        //    //console.log($scope.listPakages);
        //}


        // ******************************** end getPackages

        //$scope.allContacts = [
        //    {
        //        name: 'Marina Augustine',
        //    }, {
        //        name: 'Oddr Sarno',
        //    }, {
        //        name: 'Nick Giannopoulos',
        //    }, {
        //        name: 'Narayana Garner',
        //    }, {
        //        name: 'Anita Gros',
        //    }, {
        //        name: 'Megan Smith',
        //    }, {
        //        name: 'Tsvetko Metzger',
        //    }, {
        //        name: 'Hector Simek',
        //    }, {
        //        name: 'Some-guy withalongalastaname'
        //    }
        //];


        //var pendingSearch, cancelSearch = angular.noop;
        //var lastSearch;


        // $scope.contacts = [$scope.allContacts[0]];
        //$scope.asyncContacts = [];
        $scope.filterSelected = true;

        //    $scope.querySearch = querySearch;
        //    $scope.delayedQuerySearch = delayedQuerySearch;

        /**
         * Search for contacts; use a random delay to simulate a remote call
         */
        $scope.querySearch = function (criteria) {
            var list = criteria ? $scope.allContacts.filter(createFilterFor(criteria)) : [];
            return list;
        }


        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);
            return function filterFn(contact) {
                return (angular.lowercase(contact.name).indexOf(lowercaseQuery) != -1);
            };
        }
        // ******************************************************end search




        $scope.typeEachItemFoSelection = [
            { type: 0, name: 'City', icon: "fa fa-map-marker", className: "itemSelcted box-city" },
            { type: 1, name: 'Place', icon: "fa fa-map-pin", className: "itemSelcted box-place" }
        ];
        $scope.searhPakage = {
            item: []
        };



        //$scope.$watch("profile.packages", function (newval, oldval) {
        //    console.log("$scope.profile.packages", $scope.profile.packages);
        //    if (typeof newval != undefined) {
        //        initPakage();
        //    }
        //});

        var intersection = function (searchItem) {

            for (var i = 0; i < $scope.listPakages.length; i++) {
                var pak = $scope.listPakages[i];
                var findPak = false;
                for (var inewp = 0; inewp < searchItem.packages.length; inewp++)//&& $scope.listPakages.length != 0
                {
                    var newPak = searchItem.packages[inewp];
                    if (pak.PackageId === newPak.PackageId) {
                        findPak = true;
                        break;
                    }
                }//end search 2
                if (!findPak)
                    $scope.listPakages.pop(pak);
            }

            if ($scope.listPakages.length == 0)
                $scope.listPakages = searchItem.packages;
        }

        var setPakages = function (searchItem) {

            var isFindCity = false;
            //چک می کنیم که این شهر قبلا انتخاب شده یا نه
            for (var i = 0; i < arrayCity.length; i++) {
                if (arrayCity[i].CityID == searchItem.CityID) {
                    isFindCity = true;
                    break;
                }
            }
            if (arrayCity.length === 0) {
                arrayCity.push({ CityID: searchItem.CityID, packages: searchItem.packages });
                $scope.listPakages = searchItem.packages;
            }
            else if (!isFindCity) {
                arrayCity.push({ CityID: searchItem.CityID, packages: searchItem.packages });
                intersection(searchItem);
            }
        }
        var arrayCity = [];
        $scope.listPakages = [];

        $scope.$watch("searhPakage.item", function (listSelected, oldval) {

            if (typeof listSelected === undefined || listSelected.length === 0) {
                arrayCity = [];
                $scope.listPakages = angular.copy($scope.profile.packages);
                return;
            }

            // delete city of search
            if (listSelected.length < oldval.length) {
                var findItem = false;
                var searchItem = _.difference(listSelected, oldval);
                //چک می کنیم که این شهر قبلا انتخاب شده یا نه
                for (var i = 0; i < listSelected.length; i++) {
                    if (searchItem.CityID == listSelected[i].CityID) {
                        findItem = true;
                        break;
                    }
                }
                if (findItem)
                    return;
                arrayCity = [];
                $scope.listPakages = [];
                for (var i = 0; i < listSelected.length; i++) {
                    var searchItem = angular.copy(listSelected[i]);
                    setPakages(searchItem);
                }
            } // end delete city of search

            else {
                //add search
                var searchItem = angular.copy(listSelected[listSelected.length - 1]);
                setPakages(searchItem);
            }
        });

        //_
        //$scope.profile.city = data[0].PackageCities[0];
        //    console.log("item"item)


        //    return num * 3;
        //}); 

        //$scope.pakageForSelected = [{
        //    id: 1,
        //    idItem: 0,
        //    type: typeEachItemFoSelection[0],
        //    title: "Siraz",
        //    description: "There are 3 locations for this city",


        //}, {
        //    id: 2,
        //    idItem: 0,
        //    type: typeEachItemFoSelection[1],
        //    title: "Jundishapur",
        //    description: "There is this place in Shiraz",
        //    // iconClass: ,

        //}, {
        //    id: 3,
        //    idItem: 0,
        //    type: typeEachItemFoSelection[1],
        //    title: "hafezieh",
        //    description: "There is this place in Shiraz",
        //    iconClass: "fa fa-leaf",

        //}]
        //$scope.productStatuce = $scope.productStatuse;
        //$scope.modelItem = {
        //    StatusType: $scope.pakageForSelected[0]
        //};
        //$scope.getProductStatus = function () {
        //    var deferred = $q.defer();
        //    deferred.resolve($scope.productStatuce);
        //    return deferred;
        //};




    }]);

userApp.controller('pakagePurchasedCtrl', ['$scope', '$timeout', 'userServices', function ($scope, $timeout, userServices) {

    $timeout(function () {
        $('.collapsible').collapsible();
    }, 1000);

    $timeout(function () {
        $scope.isloadImage = false;
    }, 100000);


    $scope.profile.page = 2;

    //if (typeof $scope.profile.packagesPurchased != undefined || $scope.profile.packagesPurchased.length != 0) {
    //    $scope.packagesPurchased = angular.copy($scope.profile.packagesPurchased);
    //    $scope.isShowMessage = false;
    //}
    //else
    //    $scope.isShowMessage = true;

}]);
userApp.controller('pakageCtrl', ['$scope', function ($scope) {
   // $scope.profile.isCompletedLoading = false;
    $scope.profile.page = 1;

}]);

userApp.controller('deactivateCtrl', ['$scope', 'userServices', 'notificService', function ($scope, userServices, notific) {
    $scope.profile.isCompletedLoading = false;
    $scope.profile.page = 3;

    $scope.deactivateMobile = function () {
        userServices.deactivateMobile()
            .then(function (data) {
                notific.success("", data)
                $scope.user.IsAccessChangeUuid = false;
                $scope.m.isShowMessage = false;
                // notific.success(successMsg);
                // $state.go("Packages");
            }, function (error) {
                if (typeof error != undefined && error.Message != null)
                    notific.error("", error.Message);
            });
    }
}]);

serApp.controller('paymentCtrl', ['$scope',  function ($scope, ) {
    $scope.profile.isCompletedLoading = false;
   
}]);