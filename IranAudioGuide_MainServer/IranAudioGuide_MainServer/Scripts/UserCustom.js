
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
              data: { pageTitle: 'تغذیه سالم' },
              controller: "userCtrl"
          })

          .state('user.PackagesPurchased', {
              url: "/pakagePurchased",
              templateUrl: "/user/PackagesPurchased",
              controller: "pakagePurchasedCtrl"


          })
            .state('user.Packages', {
                url: "/pakages",
                templateUrl: "/user/Packages",
                controller: "PackagesCtrl"


            })
         .state('user.deactivateMobile', {
             url: "/deactivateMobile",
             templateUrl: "/user/deactivateMobileMobile",
         })
    }])

.run(function () {

});