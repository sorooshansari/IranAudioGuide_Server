angular.module('HomePage', [])
 .config(['$sceProvider', function ($sceProvider) {
     $sceProvider.enabled(false);
 }])
 .controller('HomeController', ['$scope', '$http', '$timeout', function ($scope, $http, $timeout) {
     //var data = {
     //    trackId: '5ad3f2e9-2efc-4b20-b6b2-f863ca61c72c',
     //    email: 'testuser@iranaudioguide.com',
     //    uuid: "1212",
     //    isAudio: true
     //};


     //$http.post('/api/AppManagerV2/GetUrl', data).then(function (result) {
     //    console.log(result.data)
     //}, function (error) {
     //    if (error.status == 400 && typeof (error.data) !== "undefined" && typeof (error.data.Message) !== "undefined", error.data.Message != null) {

     //        console.log(error.data.Message);
     //    }
     //    ////error.status 
     //    //error.statusText
     //})

     var data2 = {
         username: 'testuser@iranaudioguide.com',
         uuid: "1212",
     };
     //$http.post('/api/AppManagerV2/GetAutorizedCities', data2).then(function (result) {
     //    console.log("GetAutorizedCities");
     //    console.log(result.data);
     //}, function (error, status, headers, config) {
     //    console.log(error, status, headers, config);
     //});



     //var data3 = {
     //    CityId: '8',
     //    LangId: 1
     //};
     //$http.post('/api/AppManagerV2/GetPackages', data3).then(function (result) {
     //    console.log("GetPackages");
     //    console.log(result.data);
     //}, function (error) {
     //    console.log(error.data.ModelState.ex[0]);

     //});


     //$http.post('/api/AppManagerV2/GetUpdates', {
     //    uuid: "1212",
     //    LastUpdateNumber: 2
     //})
     //    .success(function (result) {
     //        console.log("GetUpdates");
     //        console.log(result.data)
     //    }, function (error) {
     //        console.log(error.data.ModelState.ex[0]);

     //    });
     //$http.post('/api/AppManagerV2/GetAll', { uuid: "1212" })
     //    .success(function (result) {
     //        console.log("GetAll");
     //        console.log(result.data);
     //    }, function (error) {
     //        console.log(error.data.ModelState.ex[0]);

     //    });

     $scope.user = {
         isAutintication: false,
         username: "test",
         isAdmin: false,
     };
     var init = function () {
         if (angular.element('#chechLogin').length !== 0) {
             $http.get('/api/UserApi/IsTheFirstLogin').then(function (result) {
                 $scope.isTheFirstLogin = result.data;
             }, function (error, status, headers, config) {
                 $scope.isTheFirstLogin = false;
             });
         }
     };
     init();
     $scope.sendEmailConfirmedAgain = function () {
         $http.post('/Account/SendEmailConfirmedAgain').then(function (data) {
             console.log(data);
             if (data.status == 0) {
                 $scope.isMsg = true;
                 $timeout(function () {
                     $scope.isMsg = false;
                 }, 50000);
             }
         }, function (error) {
             $scope.isMsg = false;
         });
     };
     $scope.newsletters = {};
     $scope.showPopup = function (nameDevice) {
         $scope.newsletters.nameDevice = nameDevice;
         $('.form-request').addClass('form-request-show');
     };
     $scope.sendEmail = function () {
         $scope.formRequest.isError = false;
         if ($scope.formRequest.$invalid) {
             $scope.formRequest.isError = true;
             return;
         }
         $('.form-request').removeClass('form-request-show');
         //$('#myModal').modal('hide');
         var user = {
             NameDevice: $scope.newsletters.nameDevice,
             Email: $scope.newsletters.emailForApp
         };
         $http.post('/api/UserApi/SaveRequest', user).success(function (result) {
         }).error(function (error, status, headers, config) {
         });
     };
     $scope.ContactUs = function (model, form) {
         $scope.overlay = true;
         if (form.$valid && !model.invalidFile) {
             method = 'POST';
             url = '/Home/ContactEmailSender';
             data = {
                 email: model.email,
                 message: model.message,
                 name: model.name,
                 subject: model.subject
             };
             $scope.sent = false;
             $scope.overlay = true;
             $http({ method: method, url: url, data: data }).
                 then(function (response) {
                     switch (response.data.status) {
                         case respondstatus.success:
                             $scope.sentMessage = response.data.content;
                             $scope.sent = true;
                             break;
                         default:
                             $scope.sentMessage = response.data.content;
                             $scope.sent = true;
                             break;
                     }
                     $timeout(function () {
                         $scope.overlay = false;
                         $scope.contactForm.$setPristine();
                         $scope.ContactFormModel = {};
                     }, 1000);
                 }, function (response) {
                     console.log("Request failed");
                     console.log("status:" + response.status);
                 });
         }
     };
     valid = [true, true, true, true];
     $scope.ValidationClass = function (i) {
         return (valid[i]) ? '' : 'sorooshInvalid';
     };
     $scope.validator = function (e, i) {
         valid[i] = (e.target.classList.contains('ng-invalid')) ? false : true;
     };
 }]);
