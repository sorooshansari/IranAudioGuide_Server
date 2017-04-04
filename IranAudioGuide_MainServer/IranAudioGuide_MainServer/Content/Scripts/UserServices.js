userApp.service('dataServices', ['$http', '$q', function ($http, $q) {
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


}]);
userApp.service('userServices', ['dataServices', function (dataServices) {
    this.LogOff = function () {
        dataServices.get('/User/LogOff');
    }
    this.getpayment = function () {

    }
    this.packages = [];
    this.sendEmailConfirmedAgain = function () {
        return dataServices.get('/Account/SendEmailConfirmedAgain');
    }
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
    this.buyPakages = function (pak) {

        return dataServices.get('/Payment/PaymentWeb', pak);
    }
}]);
userApp.service('notificService', [function () {
    //jquery-notific8
    var optionsDefault = {
        positionClass: 'toast-bottom-full-width', //'toast-bottom-full-width ',// 'toast-top-center',
        life: 5000,
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
}]);
userApp.filter('propsFilter', [function () {
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
}]);
userApp.service('localezationService', ['$http', '$q', function ($http, $q) {
    that = this;
    this.currentLocale = null;
    this.setCurrentLocale = function (callfunctin, localeId) {
        try {
            if (typeof localeId == 'undefined' || localeId == 'fa-ir') {
                var deferred = $q.defer();
                $http.get('/Content/Scripts/locales/fa-ir.js').success(function (result) {
                    currentLocale = result[0]
                    deferred.resolve(result);
                }).error(function (error, status, headers, config) {
                    deferred.reject(error);
                });
                return deferred.promise;
            }
        }
        catch (e) {
            console.log('error: dont add local , has this error:', e)
        }
    };
    this.getLocale = function () {
        if (that.currentLocale == null)
            that.setCurrentLocale().then(function (data) {
                return that.currentLocale;
            });
        return that.currentLocale;
    };
    //return {
    //    getLocale: getLocale,
    //    setCurrentLocale: setCurrentLocale,
    //}
}]);
userApp.filter('localezationFilter', function (locale) {
    return function (input) {
        if (input && locale.length != 0) {
            parts = input.toLowerCase().split('.');
            localizedString = locale[0];
            for (_i = 0, _len = parts.length; _i < _len; _i++) {
                var part = parts[_i];
                localizedString = localizedString[part];
                if (!localizedString) {
                    break;
                }
            }
            if (localizedString) {
                return localizedString;
            }
        }
    };
});