angular.module('AdminPage.services', [])
.service('PlaceServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var Places = [];
    //var getModelAsFormData = function (data) {
    //    var dataAsFormData = new FormData();
    //    angular.forEach(data, function (value, key) {
    //        dataAsFormData.append(key, value);
    //    });
    //    return dataAsFormData;
    //};
    return {
        Get: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetPlaces';
            data = { PageNum: PageNum };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $rootScope.PlacePagesLen = response.data.PagesLen;
                  $rootScope.PlaceCurrentPage = PageNum;
                  angular.copy(response.data.Places, Places);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Places;
        },
        AddPlace: function (NewPlace) {
            method = 'POST';
            url = '/Admin/AddPlace';
            console.log(NewPlace);
            var fd = new FormData();
            fd.append('Image', NewPlace.Image);
            fd.append('PlaceName', NewPlace.PlaceName);
            fd.append('PlaceDesc', NewPlace.PlaceDesc);
            fd.append('PlaceAddress', NewPlace.PlaceAddress);
            fd.append('PlaceCordinates', NewPlace.PlaceCordinates);
            fd.append('PlaceCityId', NewPlace.PlaceCityId);
            $http({
                method: method,
                url: url,
                data: fd,
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).
              then(function (response) {
                  if (response.data.success) {
                      $rootScope.$broadcast('UpdateCities', {});
                  }
                  else
                      console.log("Server failed to add City.");
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        }
    }
}])
.service('CityServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var Cities = [];
    var Success = false;
    return {
        Get: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetCities';
            data = { PageNum: PageNum };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $rootScope.CityPagesLen = response.data.PagesLen;
                  $rootScope.CityCurrentPage = PageNum;
                  angular.copy(response.data.Cities, Cities);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Cities;
        },
        AddCity: function (NewCity) {
            method = 'POST';
            url = '/Admin/AddCity';
            data = { CityName: NewCity.CityName, CityDesc: NewCity.CityDesc };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  if (response.data.success) {
                      $rootScope.$broadcast('UpdateCities', {});
                  }
                  else
                      console.log("Server failed to add City.");
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        RemoveCity: function (CityID, CityName) {
            method = 'POST';
            url = '/Admin/DelCity';
            data = { Id: CityID };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdateCities', {});
                          break;
                      case 1:
                          $rootScope.$broadcast('CityForignKeyError', {
                              CityID: CityID,
                              CityName: CityName
                          });
                          break;
                      case 2:
                          $rootScope.$broadcast('CityUnknownError', {});
                          console.log("Server failed to add City.");
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
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
