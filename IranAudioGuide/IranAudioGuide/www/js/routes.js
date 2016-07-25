angular.module('app.routes', [])

.config(function($stateProvider, $urlRouterProvider) {

  // Each state's controller can be found in controllers.js
  $stateProvider
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

  .state('tabsController', {
    url: '/page1',
    templateUrl: 'templates/tabsController.html',
    abstract:true
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

$urlRouterProvider.otherwise('/page1/page2')

  

});