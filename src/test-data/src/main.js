angular.module('test-data', ['ngRoute', 'template-partials'])
  .config(function ($routeProvider) {
    'use strict';

    $routeProvider
      .when('/testdata', {
        controller: 'testdataController',
        templateUrl: '/test-data/src/test-data.html'
      });
  });