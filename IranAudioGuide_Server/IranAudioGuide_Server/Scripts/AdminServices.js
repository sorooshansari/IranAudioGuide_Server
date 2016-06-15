angular.module('AdminPage.services', [])
.service('PlaceServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var Places = [];
    return {
        Get: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetPlaces';
            data = { PageNum: PageNum };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  angular.copy(response.PagesLen, $rootScope.PlacePagesLen);
                  angular.copy(PageNum, $rootScope.PlaceCurrentPage);
                  angular.copy(response.data.Places, Places);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Places;
        }
    }
}])
.service('CityServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var Cities = [];
    return {
        Get: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetCities';
            data = { PageNum: PageNum };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  angular.copy(response.PagesLen, $rootScope.CityPagesLen);
                  angular.copy(PageNum, $rootScope.CityCurrentPage);
                  angular.copy(response.data.Cities, Cities);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Cities;
        }
    };
}])
.service('myService', [function () {
    return {
        all: function () {
            return 1;
        }
    }
}]);
