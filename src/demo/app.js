(function () {
  'use strict';

  angular.module('app', ['ngRoute', 'test-data'])
    .constant('apiBase', 'api')
    .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
      $locationProvider.html5Mode(true);
      $routeProvider.when('/', {
        template: 'Hello World! - <a ng-href="/testdata">test data</a>'
      })
      .otherwise({
        template: 'Hello World! - <a ng-href="/testdata">test data</a>'
      });
    }]);
})();
