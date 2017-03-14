
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
              templateUrl: "/user/UserProfile",
              controller: "userCtrl"
          })

          .state('PackagesPurchased', {
              url: "/pakagePurchased",
              templateUrl: "/user/PackagesPurchased",
              controller: "pakagePurchasedCtrl"


          })
            .state('Packages', {
                url: "/pakages",
                templateUrl: "/user/Packages",
                controller: "PackagesCtrl"


            })
             .state('Payment', {
                 url: "/payment?PackageId&IsChooesZarinpal",
                 templateUrl: function ($stateParams) {
                     try {
                         var model = "?pacId=" + $stateParams.PackageId + "&IsZarinpal=" + $stateParams.IsChooesZarinpal;
                         console.log(model)
                         return "/Payment/PaymentWeb" + model;
                     }
                     catch (error) {

                     }
                 },
                 controller: "paymentCtrl"
             })
         .state('deactivateMobile', {
             url: "/deactivateMobile",
             templateUrl: "/user/deactivateMobile",
             controller: "PackagesCtrl"
         })
    }])

.run(function () {

})
;