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
            packagesPurchased: [],
            isCompletedLoading: true
        };

        $scope.uploadFile = function (files) {
            var fd = new FormData();
            //Take the first selected file
            fd.append("file", files[0]);

            $http.post("/api/UserApi/UploadFile", fd, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).then(function () { console.log("test success") }, function (error) { console.log("error", error) });

        };
        $scope.user = {
            isAutintication: false,
            IsEmailConfirmed: true,
        }
        $scope.LogOff = function () {
            userServices.LogOff();
            $window.location.href = 'http://iranaudioguide.com';
        }
        userServices.getUser().then(function (data) {

            $scope.user = data;
            $scope.user.FullName = data.Email;
            if (data.imgUrl == null) {
                data.imgUrl = "../images/default_avatar.png";
            }
            $scope.user.uImageUrl = data.imgUrl;
            if (data.FullName !== null)
                $scope.user.FullName = data.FullName
            $scope.user.isAutintication = true;
        });

        $scope.deactivateMobile = function () {
            userServices.deactivateMobile()
                .then(function (data) {
                    notific.success("", data)
                    $scope.user.IsAccessChangeUuid = false;
                    $scope.m.isShowMessage = false;
                    // notific.success(successMsg);
                    $state.go("Packages");
                }, function (error) {
                    notific.error("ERROR", error.Message);
                });
        }
        $scope.getPackages = function (event) {

            if ($scope.profile.packages.length == 0) {
                userServices.getPackages().then(function (data) {
                    $scope.profile.packages = data;
                    angular.forEach(data, function (item, index) {
                        if (item.isPackagesPurchased == true) {
                            $scope.profile.packagesPurchased.push(item);
                        }
                    });
                    $scope.profile.isCompletedLoading = false;
                }, function () {
                    $scope.profile.isCompletedLoading = false;

                });
            }

        }

        $timeout(function () {
            angular.element('#btnGetPakage').triggerHandler('click');
        }, 0);


        $scope.sendEmailConfirmedAgain = function () {
            userServices.sendEmailConfirmedAgain().then(function (data) {
                console.log(data);
                if (data.status == 0)
                    notific.success("", data.content);

            }, function (error) {
            });
        }

    }]);
userApp.controller('PackagesCtrl', ['$state', '$scope', 'userServices', '$timeout', function ($state, $scope, userServices, $timeout) {
    $scope.pak = {};
    $scope.showModal = function (pak) {
        $scope.pak = pak;
        $('#myModal').modal('show');
    }
    $scope.buyPakages = function (isChooesZarinpal) {
        $('#myModal').modal('hide');
        $scope.profile.isCompletedLoading = true;
        $scope.isChooesZarinpal = isChooesZarinpal;
    }
    $('#myModal').on('hidden.bs.modal', function (e) {
        $state.go('Payment', {
            PackageId: $scope.pak.PackageId,
            IsChooesZarinpal: $scope.isChooesZarinpal
        });
    })

    $scope.typeEachItemFoSelection = [
        { type: 0, name: 'City', icon: "fa fa-map-marker", className: "itemSelcted box-city" },
        { type: 1, name: 'Place', icon: "fa fa-map-pin", className: "itemSelcted box-place" }
    ];
    $scope.searhPakage = {
        item: []
    };



    $scope.$watch("profile.packages", function (newval, oldval) {
        if (typeof newval != undefined) {
            initPakage();
        }
    });

    var intersection = function (searchItem) {

        for (var i = 0; i < $scope.listPakages.length; i++) {
            var pak = $scope.listPakages[i];
            var findPak = false;
            for (var inewp = 0; inewp < searchItem.packages.length  ; inewp++)//&& $scope.listPakages.length != 0
            {
                var newPak = searchItem.packages[inewp];
                if (pak.PackageId == newPak.PackageId) {
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
        if (arrayCity.length == 0) {
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

        if (typeof listSelected == undefined || listSelected.length == 0) {
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
    var initPakage = function () {

        $scope.pakageForSelected = [];
        $scope.selectId = 0;
        $scope.pakageForSelected = [];
        _.each($scope.profile.packages, function (pak) {
            _.each(pak.PackageCities, function (city) {
                var findItem = false;
                for (var i = 0; i < $scope.pakageForSelected.length; i++) {
                    var itemEntry = $scope.pakageForSelected[i];
                    if (itemEntry.idItem == city.CityID) {
                        itemEntry.packages.push(pak)
                        findItem = true;
                        break;
                    }
                }
                if (!findItem)
                    $scope.pakageForSelected.push({
                        id: $scope.selectId++,
                        idItem: city.CityID,
                        // این گزینه برای این است که در جستجو  پکبج ها مشکلی پیش نیاد
                        CityID: city.CityID,// 
                        type: $scope.typeEachItemFoSelection[0].type,
                        title: city.CityName,
                        description: "There are " + city.Places.length + " locations for this city",
                        packages: [pak]
                    })
                _.each(city.Places, function (plc) {
                    var findItem = false;
                    for (var i = 0; i < $scope.pakageForSelected.length; i++) {
                        var itemEntry = $scope.pakageForSelected[i];
                        if (itemEntry.idItem == plc.PlaceId) {
                            itemEntry.packages.push(pak)
                            findItem = true;
                            break;
                        }
                    }
                    if (!findItem)
                        $scope.pakageForSelected.push({
                            id: $scope.selectId++,
                            idItem: plc.PlaceId,
                            CityID: city.CityID,
                            type: $scope.typeEachItemFoSelection[1].type,
                            title: plc.PlaceName,
                            description: "There is this place in " + city.CityName,
                            packages: [pak]
                        })
                });//end each place
            });//end each pack.PackageCities

        })//end each $scope.profile.package

        $scope.listPakages = angular.copy($scope.profile.packages);

    }
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

userApp.controller('pakagePurchasedCtrl', ['$scope', 'userServices', '$timeout', function ($scope, userServices, $timeout) {

    $scope.$watch("profile.packagesPurchased", function (newval, oldval) {
        if (typeof newval != undefined) {
            $scope.packagesPurchased = angular.copy($scope.profile.packagesPurchased);
            if ($scope.packagesPurchased.length == 0)
                $scope.isShowMessage = true;
            else
                $scope.isShowMessage = false;

        }
    });
}]);
userApp.controller('paymentCtrl', ['$scope', function ($scope) {
    $scope.profile.isCompletedLoading = false;
}]);