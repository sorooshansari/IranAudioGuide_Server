userApp

    .service('dataServices', ['$http', '$q', function ($http, $q) {

     
        this.login = function (url, data, config) {
            var deferred = $q.defer();
            $http.post(url, data, config).success(function (response) {
                deferred.resolve(response);
            }).error(function (error, status, headers, config) {
                deferred.reject(error);
            });
            return deferred.promise;
        }


        this.post = function (url, data) {
            var deferred = $q.defer();

            //   ({ method: $scope.method, url: $scope.url, cache: $templateCache }).

            $http.post(url, data).success(function (result) {
                deferred.resolve(result);
            }).error(function (error, status, headers, config) {
                deferred.reject(error);
            });
            return deferred.promise;
        };
        this.put = function (url, data) {

            var deferred = $q.defer();
            $http.put(url, data).success(function (result) {
                deferred.resolve(result);
            }).error(function (error, status, headers, config) {
                deferred.reject(error);
            });
            return deferred.promise;
        };
        this.get = function (url) {

            var deferred = $q.defer();
            $http.get(url).success(function (result) {
                deferred.resolve(result);
            })
                .error(function (error, status, headers, config) {
                    deferred.reject(error);
                });
            return deferred.promise;
        };


    }])
    .service('userServices', ['dataServices', function (dataServices) {
        this.packages = [];
        this.getUser = function () {
            return dataServices.post('/api/userApi/GetCurrentUserInfo');
        };
        this.getPackagesPurchased = function () {
            return dataServices.post('/api/userApi/GetPackagesPurchased');
        }
        this.getPackages = function () {    
            return dataServices.post('/api/userApi/GetPackages');
        }
        this.getPalaceForCity = function () {
            return dataServices.get();
        }
        this.deactivateMobile = function () {
            return dataServices.get('/api/userApi/DeactivateMobile')
        }
    }])
    .service('notificService', [function () {
        //jquery-notific8
        var optionsDefault = {
            positionClass: 'toast-top-center', //'toast-bottom-full-width ',// 'toast-top-center',
        };
        toastr.options = optionsDefault;
        this.success = function (header, content) {
            toastr.remove();
            toastr.clear();
            toastr.success(content, header);
            //$.notific8(content, {
            //    //life: 5000,
            //    heading: header,
            //    theme: 'lime'
            //});
            return;
        }
        this.error = function (header, content) {
            toastr.error(content, header);
            //$.notific8(content, {
            //    // life: 5000,
            //    heading: header,
            //    theme: 'ruby'
            //});
            return;
        }
        this.info = function (header, content) {
            toastr.remove();
            toastr.clear();
            toastr.info(content, header);
            return;
        }
        this.infoMessage = function (header, content) {
            // toastr.options = optionsCenter;
            toastr.remove();
            toastr.clear();
            toastr.info(content, header);
            return;
        }
        this.warning = function (header, content) {
            toastr.remove();
            toastr.clear();
            toastr.warning(content, header);
            return;
        }
        this.clear = function () {
            toastr.clear();
        }
    }])
