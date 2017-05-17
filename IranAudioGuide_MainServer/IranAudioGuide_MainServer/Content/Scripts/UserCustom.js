
//, 'ui.select'
var userApp = angular.module('userApp', ["ui.router", 'ui.select'])
    .config(['$urlRouterProvider', '$stateProvider', '$sceProvider', function ($urlRouterProvider, $stateProvider, $sceProvider) {
        $sceProvider.enabled(false);

        //
        // For any unmatched url, redirect to /state1
        $urlRouterProvider.otherwise("/profile");
        //
        // Now set up the states
        $stateProvider
          .state('user', {
              url: "/profile",
              //abstract: true,
              templateUrl: "/fa/user/UserProfile",
              controller: "userCtrl",
              //resolve: {
              //    locale: ['$http', function ($http) {
              //        return $http.get('/Areas/AdminPanel/Modules/locales/fa-ir.js');
              //    }]
              //},
              //onEnter: 'localezationService', function (localezationService) {
              //    console.log('onEnter');
              //    localezationService.getLocale();
                 
              //},
          })

          .state('PackagesPurchased', {
              url: "/pakagePurchased",
              templateUrl: "/fa/user/PackagesPurchased",
              controller: "pakagePurchasedCtrl"


          })
            .state('Packages', {
                url: "/pakages",
                templateUrl: "/fa/user/Packages",
                controller: "PackagesCtrl"


            })
             .state('Payment', {
                 url: "/payment?PackageId&IsChooesZarinpal",
                 templateUrl: function ($stateParams) {
                     try {
                         var model = "?pacId=" + $stateParams.PackageId + "&IsZarinpal=" + $stateParams.IsChooesZarinpal;
                         console.log(model)
                         return "/fa/Payment/PaymentWeb" +model;
                     }
                     catch (error) {

                     }
                 },
                 controller: "paymentCtrl"
             })
         .state('deactivateMobile', {
             url: "/deactivateMobile",
             templateUrl: "/fa/user/deactivateMobile",
             controller: "PackagesCtrl"
         })
    }])


;