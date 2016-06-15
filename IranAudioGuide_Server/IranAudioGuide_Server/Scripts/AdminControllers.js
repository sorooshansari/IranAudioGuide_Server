angular.module('AdminPage.controllers', [])
.controller('AdminController', ['$scope', '$rootScope', 'PlaceServices', 'CityServices', function ($scope, $rootScope, PlaceServices, CityServices) {
    //Place stuff
    $rootScope.PlacePagesLen = 0;
    $rootScope.PlaceCurrentPage = 0;
    $scope.places = PlaceServices.Get(0);
    $scope.PreviousPlace = function () {
        if ($rootScope.PlaceCurrentPage < 0)
            $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage - 1);
    };
    $scope.NextPlace = function () {
        if ($rootScope.PlacePagesLen - $rootScope.PlaceCurrentPage > 1)
            $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage + 1);
    };
    //City stuff
    $rootScope.CityPagesLen = 0;
    $rootScope.CityCurrentPage = 0;
    $scope.cities = CityServices.Get(0);
    $scope.PreviousCity = function () {
        if ($rootScope.CityCurrentPage < 0)
            $scope.cities = CityServices.Get($rootScope.CityCurrentPage - 1);
    };
    $scope.NextCity = function () {
        if ($rootScope.CityPagesLen - $rootScope.CityCurrentPage > 1)
            $scope.cities = CityServices.Get($rootScope.CityCurrentPage + 1);
    };
}]);
