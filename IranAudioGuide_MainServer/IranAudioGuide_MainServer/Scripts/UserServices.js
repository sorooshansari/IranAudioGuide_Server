angular.module('UserPage.services', [])
.service('factory', ['$rootScope', '$http', function ($rootScope, $http) {

}])
.service('PackageServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var AllPackages = [];
    var Packages = [];
    var Success = false;
    return {
        All: function () {
            method = 'POST';
            url = '/Admin/GetPackages';
            data = { PageNum: -1 };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  angular.copy(response.data, AllPackages);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return AllPackages;
        },
        Get: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetPackages';
            data = { PageNum: PageNum };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $rootScope.CityPagesLen = response.data.PagesLen;
                  $rootScope.CityCurrentPage = PageNum;
                  angular.copy(response.data.Packages, Packages);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Packages;
        },
        AddPackage: function (NewPackage) {
            method = 'POST';
            url = '/Admin/AddPackage';
            data = { PackageName: NewPackage.Name, PackagePrice: NewPackage.Price, cities: NewPackage.Cities };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case respondstatus.success:
                          $rootScope.$broadcast('packageAdded', {});
                          break;
                      default:
                          console.log("Server failed to add Package.");
                          break;
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        
    };
}]) ;