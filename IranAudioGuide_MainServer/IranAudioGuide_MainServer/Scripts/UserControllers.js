//Developed by pourmand

userApp.controller('userCtrl', ['$scope', 'userServices', '$timeout', 'notificService', function ($scope, userServices, $timeout, notific) {
    $scope.user = {
        isAutintication: false,
        username: "test"
    }

    $scope.profile = {
        istest: true,
        packages: [],
        packagesPurchased:[]
    };
    userServices.getUser().then(function (data) {
        $scope.user = data;
        $scope.user.isAutintication = true;
    });
    $scope.deactivateMobile = function () {
        userServices.deactivateMobile();
    };
    $scope.getPalaceForCity = function (city) {
        $scope.profile.city = city;
    };

    $scope.profile.isCompletedLoading = false;
    $scope.getPackagesPurchased = function (event) {
        if ($scope.profile.packagesPurchased.length == 0) {

            var calssName = $(event.target).find("i").attr('class');
            var s = $(event.target).find("i").removeAttr('class').addClass("fa fa-spinner fa-spin");

            userServices.getPackagesPurchased().then(function (data) {
                if (data.length == 0) {
                    $scope.profile.packagesPurchased = [];
                    $scope.profile.city = "";
                    $scope.IsShowMessage = true;
                }
                else {
                    $scope.profile.packagesPurchased = data;
                    $scope.profile.city = data[0].PackageCities[0];
                }
                $(event.target).find("i").removeAttr('class').addClass(calssName);

                //$timeout(function () {
                //    nav();
                //    $(event.target).find("i").removeAttr('class').addClass(calssName);
                //}, 2000);
            })
        }

    }
    $scope.deactivateMobile = function () {
        userServices.deactivateMobile()
            .then(function (data) {
                console.log("deactivateMobile: suc", data);
            }, function (error) {
                notific.error("ERROR", error.Message);
            });
    }
    $scope.getPackages = function (event) {

        if ($scope.profile.packages.length == 0) {
            var element = $(event.target).find("i");
            var calssName = element.attr('class');
            var s = element.removeAttr('class').addClass("fa fa-spinner fa-spin");
            userServices.getPackages().then(function (data) {
                $scope.profile.packages = data;
                element.removeAttr('class').addClass(calssName);
                $scope.profile.isCompletedLoading = true;
                $(event.target).find("i").removeAttr('class').addClass(calssName);
            })
        }

    }
    //$scope.profile.isCompletedLoading = true;

    $timeout(function () {
        angular.element('#btnGetPakage').triggerHandler('click');
    }, 0);
    var nav = function () {
        $('.gw-nav > li > a').click(function () {
            var gw_nav = $('.gw-nav');
            gw_nav.find('li').removeClass('active');
            $('.gw-nav > li > ul > li').removeClass('active');

            var checkElement = $(this).parent();
            var ulDom = checkElement.find('.gw-submenu')[0];

            if (ulDom == undefined) {
                checkElement.addClass('active');
                $('.gw-nav').find('li').find('ul:visible').slideUp();
                return;
            }
            if (ulDom.style.display != 'block') {
                gw_nav.find('li').find('ul:visible').slideUp();
                gw_nav.find('li.init-arrow-up').removeClass('init-arrow-up').addClass('arrow-down');
                gw_nav.find('li.arrow-up').removeClass('arrow-up').addClass('arrow-down');
                checkElement.removeClass('init-arrow-down');
                checkElement.removeClass('arrow-down');
                checkElement.addClass('arrow-up');
                checkElement.addClass('active');
                checkElement.find('ul').slideDown(300);
            } else {
                checkElement.removeClass('init-arrow-up');
                checkElement.removeClass('arrow-up');
                checkElement.removeClass('active');
                checkElement.addClass('arrow-down');
                checkElement.find('ul').slideUp(300);

            }
        });
        $('.gw-nav > li > ul > li > a').click(function () {
            $(this).parent().parent().parent().removeClass('active');
            $('.gw-nav > li > ul > li').removeClass('active');
            $(this).parent().addClass('active')
        });
    };
}]);
userApp.controller('PackagesCtrl', ['$scope', 'userServices', '$timeout', function ($scope, userServices, $timeout) {
    //if (typeof $scope.profile.packages == undefined)
    //    return;
    //var form = document.getElementById("PurchaseForm")
    //    //,            select = document.getElementById("pt");

    //form.addEventListener("submit", function (event) {
    //    event.preventDefault();
    //    if (select.value == 2) {
    //        var element = document.createElement("INPUT");
    //        element.setAttribute("type", "hidden");
    //        element.setAttribute("name", "PackageId");
    //        element.setAttribute("value", $scope.packagesId);
    //        form.appendChild(element);
    //    }
    //    form.submit();
    //});


    $scope.buyPakages = function (pak) {
        var form = document.getElementById("PurchaseForm");
        var element = document.createElement("INPUT");
        element.setAttribute("type", "hidden");
        element.setAttribute("name", "PackageId");
        element.setAttribute("value", pak.PackageId);
        form.appendChild(element);
        form.submit();
    }


    $scope.typeEachItemFoSelection = [
        { type: 0, name: 'City', icon: "fa fa-bolt", className: "itemSelcted box-city" },
        { type: 1, name: 'Place', icon: "fa fa-leaf", className: "itemSelcted box-place" }
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
        if (arrayCity.length == 0 || !isFindCity) {
            arrayCity.push({ CityID: searchItem.CityID, packages: searchItem.packages });
            intersection(searchItem);

        }
    }
    var arrayCity = [];
    $scope.listPakages = [];

    $scope.$watch("searhPakage.item", function (listSelected, oldval) {

        //var tempList = angular.copy(listSelected);
        //angular.copy(listSelected, tempList);
        //var tlist = [];
        //tlist = tlist.concat(listSelected);
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

    //$scope.$watch("listPakages", function (newval) {
    //    if (newval.length != 0 )
    //        $timeout(function () {

    //            $(".slider6").responsiveSlides({
    //                auto: false,
    //                pager: false,
    //                nav: true,
    //                speed: 500,
    //                maxwidth: 800,
    //                namespace: "centered-btns"
    //            });
    //        }, 5000)
    //});

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
    console.log($scope.p);


    $scope.$watch("profile.packagesPurchased", function (newval, oldval) {
        if (typeof newval != undefined) {
            $scope.packagesPurchased = angular.copy($scope.profile.packagesPurchased);

        }
    });
}]);