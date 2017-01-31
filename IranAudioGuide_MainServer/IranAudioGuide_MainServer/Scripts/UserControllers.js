//Developed by pourmand
userApp.filter('propsFilter', function () {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            var keys = Object.keys(props);

            items.forEach(function (item) {
                var itemMatches = false;

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
}).controller('userCtrl', ['$scope', 'userServices', '$timeout', 'notificService', function ($scope, userServices, $timeout, notific) {
    $scope.user = {
        isAutintication: false,
        username: "test"
    }

    $scope.profile = {
        istest: true
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

        var calssName = $(event.target).find("i").attr('class');
        var s = $(event.target).find("i").removeAttr('class').addClass("fa fa-spinner fa-spin");

        userServices.getPackagesPurchased().then(function (data) {
            if (data.length == 0) {
                $scope.profile.packages = [];
                $scope.profile.city = "";
                $scope.IsShowMessage = true;
            }
            else {
                $scope.profile.packages = data;
                $scope.profile.city = data[0].PackageCities[0];
            }
            $timeout(function () {
                nav();
                $(event.target).find("i").removeAttr('class').addClass(calssName);
            }, 2000);
        })

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

        var element = $(event.target).find("i");
        var calssName = element.attr('class');
        var s = element.removeAttr('class').addClass("fa fa-spinner fa-spin");

        userServices.getPackages().then(function (data) {
            $scope.profile.packages = data;
            element.removeAttr('class').addClass(calssName);
            $scope.profile.isCompletedLoading = true;
        })
     
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
}])
.filter('propsFilter', function () {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            var keys = Object.keys(props);

            items.forEach(function (item) {
                var itemMatches = false;

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
userApp.controller('PackagesCtrl', ['$scope', 'userServices', '$timeout', function ($scope, userServices, $timeout) {
    if (typeof $scope.profile.packages == undefined)
        return;
    $scope.$watch("profile.packages", function (newval, oldval) {
        if (typeof newval != undefined) {
            initPakage();
        }
    });
    var initPakage = function () {
        //console.log($scope.profile.packages);
        $scope.pakageForSelected = [];
        _.each($scope.profile.packages, function (item) {
            //set city
            //$scope.pakageForSelected.push({

            //})
            console.log("item:",item)
        })//end each $scope.profile.package
    }
    //_
    //$scope.profile.city = data[0].PackageCities[0];
    //    console.log("item"item)


    //    return num * 3;
    //}); 

    var typeEachItemFoSelection = ['City', 'Place'];
    $scope.pakageForSelected = [{
        id: 1,
        idItem:0,
        type: typeEachItemFoSelection[0],
        title: "Siraz",
        description: "There are 3 locations for this city",
        iconClass: "fa fa-bolt",
       
    },{
        id: 2,
        idItem:0,
        type: typeEachItemFoSelection[1],
        title: "Jundishapur",
        description: "There is this place in Shiraz",
        iconClass:  "fa fa-leaf",
       
    }, {
        id: 3,
        idItem: 0,
        type: typeEachItemFoSelection[1],
        title: "hafezieh",
        description: "There is this place in Shiraz",
        iconClass: "fa fa-leaf",

    }]
    //$scope.productStatuce = $scope.productStatuse;
    $scope.modelItem = {
        StatusType: $scope.pakageForSelected[0]
    };
    //$scope.getProductStatus = function () {
    //    var deferred = $q.defer();
    //    deferred.resolve($scope.productStatuce);
    //    return deferred;
    //};




}]);

userApp.controller('pakagePurchasedCtrl', ['$scope', 'userServices', '$timeout', function ($scope, userServices, $timeout) {
    console.log($scope.p);

}]);