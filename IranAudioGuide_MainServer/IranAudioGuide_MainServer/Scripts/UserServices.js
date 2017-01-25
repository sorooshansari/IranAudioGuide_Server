userApp.service('dataServices', ['$http', '$q', function ($http, $q) {

    var callbackErrorFun = function (error, deferred, status, headers, config) {
        console.log("error::", error);
    }

    this.login = function (url, data, config) {
        var deferred = $q.defer();
        $http.post(url, data, config).success(function (response) {
            deferred.resolve(response);
        }).error(function (error, status, headers, config) {
            callbackErrorFun(error, deferred, status, headers, config)
        });
        return deferred.promise;
    }


    this.post = function (url, data) {
        var deferred = $q.defer();

        //   ({ method: $scope.method, url: $scope.url, cache: $templateCache }).

        $http.post(url, data).success(function (result) {
            deferred.resolve(result);
        }).error(function (error, status, headers, config) {
            callbackErrorFun(error, deferred, status, headers, config)
        });
        return deferred.promise;
    };
    this.put = function (url, data) {

        var deferred = $q.defer();
        $http.put(url, data).success(function (result) {
            deferred.resolve(result);
        }).error(function (error, status, headers, config) {
            callbackErrorFun(error, deferred, status, headers, config)
        });
        return deferred.promise;
    };
    this.get = function (url) {

        var deferred = $q.defer();
        $http.get(url).success(function (result) {
            deferred.resolve(result);
        })
            .error(function (error, status, headers, config) {
                callbackErrorFun(error, deferred, status, headers, config)
            });
        return deferred.promise;
    };


}])
    .service('userServices', ['dataServices', function (dataServices) {
        this.getUser = function () {
            return dataServices.get('/api/AppManager/GetCurrentUserInfo');
        };
        this.getPackages = function () {
            return dataServices.get('/api/AppManager/GetPackages');
        }
        this.getPalaceForCity = function () {
            return dataServices.get();
        }
    }]);
    //var AllPackages = [];
    //var Packages = [];
    //var Success = false;
    //return {
    //    All: function () {
    //        method = 'POST';
    //        url = '/Admin/GetPackages';
    //        data = { PageNum: -1 };
    //        $http({ method: method, url: url, data: data }).
    //          then(function (response) {
    //              angular.copy(response.data, AllPackages);
    //          }, function (response) {
    //              console.log("Request failed");
    //              console.log("status:" + response.status);
    //          });
    //        return AllPackages;
    //    },
    //    Get: function (PageNum) {
    //        method = 'POST';
    //        url = '/Admin/GetPackages';
    //        data = { PageNum: PageNum };
    //        $http({ method: method, url: url, data: data }).
    //          then(function (response) {
    //              $rootScope.CityPagesLen = response.data.PagesLen;
    //              $rootScope.CityCurrentPage = PageNum;
    //              angular.copy(response.data.Packages, Packages);
    //          }, function (response) {
    //              console.log("Request failed");
    //              console.log("status:" + response.status);
    //          });
    //        return Packages;
    //    },
    //    AddPackage: function (NewPackage) {
    //        method = 'POST';
    //        url = '/Admin/AddPackage';
    //        data = { PackageName: NewPackage.Name, PackagePrice: NewPackage.Price, cities: NewPackage.Cities };
    //        $http({ method: method, url: url, data: data }).
    //          then(function (response) {
    //              switch (response.data.status) {
    //                  case respondstatus.success:
    //                      $rootScope.$broadcast('packageAdded', {});
    //                      break;
    //                  default:
    //                      console.log("Server failed to add Package.");
    //                      break;
    //              }
    //          }, function (response) {
    //              console.log("Request failed");
    //              console.log("status:" + response.status);
    //          });
    //    },
        
    //};
