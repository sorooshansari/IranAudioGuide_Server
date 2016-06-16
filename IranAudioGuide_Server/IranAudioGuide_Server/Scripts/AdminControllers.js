angular.module('AdminPage.controllers', [])
.controller('AdminController', ['$scope', '$rootScope', 'PlaceServices', 'CityServices', function ($scope, $rootScope, PlaceServices, CityServices) {
    $scope.SetImageName = function (o) {
        NewPlaceForm.imgUrl.value = o.files[0].name;
    }
    //Place stuff
    $rootScope.PlacePagesLen;
    $rootScope.PlaceCurrentPage;
    $scope.places = PlaceServices.Get(0);
    $scope.PreviousPlace = function () {
        if ($rootScope.PlaceCurrentPage < 0)
            $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage - 1);
    };
    $scope.NextPlace = function () {
        if ($rootScope.PlacePagesLen - $rootScope.PlaceCurrentPage + 1 > 1)
            $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage + 1);
    };
    $scope.AddPlace = function (NewPlace, form) {
        if (form.$valid) {
            //var file = $scope.PlaceImg;
            PlaceServices.AddPlace(NewPlace);

            //$scope.NewPlace.PlaceName = "";
            //console.log(imgUrl);
            //imgUrl.value = ""
            //$scope.NewPlace.Image = "";
            //$scope.NewPlace.PlaceCityId = "";
            //$scope.NewPlace.PlaceCordinates = "";
            //$scope.NewPlace.PlaceDesc = "";
            //$scope.NewPlace.PlaceAddress = "";
        }
    };


    //City stuff
    $scope.CityNameValidator = "hidden";
    $rootScope.CityPagesLen;
    $rootScope.CityCurrentPage;
    $scope.cities = CityServices.Get(0);
    $scope.PreviousCity = function () {
        if ($rootScope.CityCurrentPage > 0)
            $scope.cities = CityServices.Get($rootScope.CityCurrentPage - 1);
    };
    $scope.NextCity = function () {
        if ($rootScope.CityPagesLen - $rootScope.CityCurrentPage + 1 > 1)
            $scope.cities = CityServices.Get($rootScope.CityCurrentPage + 1);
    };
    $scope.AddCity = function (NewCity, form) {
        if (form.$valid) {
            CityServices.AddCity(NewCity);
            $scope.NewCity.CityName = "";
            $scope.NewCity.CityDesc = "";
        }
    };
    $scope.RemoveCity = function (CityID, CityName) {
        CityServices.RemoveCity(CityID, CityName);
    };
    $scope.$on('UpdateCities', function (event) {
        $scope.cities = CityServices.Get(0);
        scroll("#Cities");
        //location.replace(location.href.split('#')[0] + "#Cities");
    });
    $scope.$on('CityForignKeyError', function (event, CityID, CityName) {
        $scope.ForignKeyErrorBody = 'This city (<span class="text-danger">' + CityName + '</span>) has one or more sub-places.<br />To remove this city, first you have to delete it\'s sub-places.'
        $scope.DelSubsBtn = "hidden";
        $('#ForignKeyErrorModal').modal('show');
    });
    $scope.$on('CityUnknownError', function (event) {
        $scope.ForignKeyErrorBody = 'Unknown error prevent removing city. Contact site developer to get more information.'
        $scope.DelSubsBtn = "hidden";
        $('#ForignKeyErrorModal').modal('show');
    });
    function scroll(id) {
        var dest = 0;
        if ($(id).offset().top > $(document).height() - $(window).height()) {
            dest = $(document).height() - $(window).height();
        } else {
            dest = $(id).offset().top;
        }
        //go to destination
        $('html,body').animate({
            scrollTop: dest
        }, 2000, 'swing');
    };
}]);
