angular.module('app.routes', [])

.config(function ($stateProvider, $urlRouterProvider) {

    // Each state's controller can be found in controllers.js
    $stateProvider
    .state('primaryPage', {
        url: '/primary',
        templateUrl: 'templates/primaryPage.html',
        controller: 'primaryPageCtrl'
    })
    .state('firstPage', {
        url: '/first',
        templateUrl: 'templates/firstPage.html',
        controller: 'firstPageCtrl'
    })

    .state('secondPage', {
        url: '/second ',
        templateUrl: 'templates/secondPage.html',
        controller: 'secondPageCtrl'
    })
    .state('login', {
        url: '/login',
        templateUrl: 'templates/login.html',
        controller: 'loginCtrl'
    })

    .state('signup', {
        url: '/signup',
        templateUrl: 'templates/signup.html',
        controller: 'signupCtrl'
    })
    .state('recoverPassword', {
        url: '/recoverPass',
        templateUrl: 'templates/recoverPassword.html',
        controller: 'recoverPasswordCtrl'
    })

    .state('tabsController', {
        url: '/page1',
        templateUrl: 'templates/tabsController.html',
        abstract: true
    })

    .state('tabsController.home', {
        url: '/page2',
        views: {
            'tab1': {
                templateUrl: 'templates/home.html',
                controller: 'homeCtrl'
            }
        }
    })

    .state('tabsController.favorits', {
        url: '/page3',
        views: {
            'tab2': {
                templateUrl: 'templates/favorits.html',
                controller: 'favoritsCtrl'
            }
        }
    })

    .state('tabsController.search', {
        url: '/page4',
        views: {
            'tab3': {
                templateUrl: 'templates/search.html',
                controller: 'searchCtrl'
            }
        }
    })

    .state('tabsController.palace', {
        url: '/page5',
        views: {
            'tab1': {
                templateUrl: 'templates/palace.html',
                controller: 'palaceCtrl'
            }
        }
    })
    .state('tabsController.palaceSearched', {
        url: '/palaceSearched',
        params: {
            id: 'salam'
        },
        views: {
            'tab3': {
                templateUrl: 'templates/palace.html',
                controller: 'palaceCtrl'
            }
        }
    })

    $urlRouterProvider.otherwise('/primary')
});