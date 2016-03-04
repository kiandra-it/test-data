angular.module('ngTestData', ['ngRoute', 'template-partials'])
  .config(['$routeProvider', function ($routeProvider) {
    'use strict';

    $routeProvider
      .when('/testdata', {
        controller: 'testdataController',
        templateUrl: '/test-data/src/test-data.html'
      });
  }]);
(function (module) {
  try {
    module = angular.module('template-partials');
  } catch (e) {
    module = angular.module('template-partials', []);
  }
  module.run(['$templateCache', function ($templateCache) {
      $templateCache.put('/test-data/src/test-data.html',
        '<div><h1>Data Sets</h1><ul><li ng-repeat="ds in datasets"><a ng-click="selectDataSet(ds)">{{::ds.name}}</a></li></ul><div ng-if="selectedDataSet"><form class="form-horizontal" name="{{\'form_\' + selectedDataSet.fullName}}" role="form" ng-submit="submit(selectedDataSet)" novalidate=""><legend ng-init="form = this[\'form_\' + selectedDataSet.fullName]">{{::ds.name}}</legend><p>{{selectedDataSet.description}}</p><div ng-if="selectedDataSet.dependencies.length > 0"><h3>Dependencies</h3><ul><li ng-repeat="dep in selectedDataSet.dependencies">{{::datasets[dep].name}}</li></ul></div><fieldset ng-repeat="ds in selectedDataSetList | filter : dataSetsWithProperties"><legend>Options - {{::ds.name}}</legend><div class="form-group" ng-repeat="property in ds.properties" ng-class="{ required: property.required }"><label class="col-md-4 control-label" for="{{::property.fieldName}}">{{::property.name}}</label><div class="col-md-6" ng-switch="" on="property.dataType"><input ng-switch-when="String" id="{{::property.fieldName}}" ng-required="{{::property.required}}" name="{{::property.fieldName}}" type="text" placeholder="{{::property.description}}" class="form-control input-md" ng-model="models[ds.fullName][property.fieldName]"> <input ng-switch-when="Date" id="{{::property.fieldName}}" ng-required="{{::property.required}}" name="{{::property.fieldName}}" type="date" placeholder="{{::property.description}}" class="form-control input-md" ng-model="models[ds.fullName][property.fieldName]"> <input ng-switch-when="DateTime" id="{{::property.fieldName}}" ng-required="{{::property.required}}" name="{{::property.fieldName}}" type="datetime" placeholder="{{::property.description}}" class="form-control input-md" ng-model="models[ds.fullName][property.fieldName]"> <input ng-switch-when="Number" id="{{::property.fieldName}}" ng-required="{{::property.required}}" name="{{::property.fieldName}}" type="number" placeholder="{{::property.description}}" class="form-control input-md" ng-model="models[ds.fullName][property.fieldName]"> <input ng-switch-when="Email" id="{{::property.fieldName}}" ng-required="{{::property.required}}" name="{{::property.fieldName}}" type="email" placeholder="{{::property.description}}" class="form-control input-md" ng-model="models[ds.fullName][property.fieldName]"></div></div></fieldset><button id="submit" name="submit" class="btn btn-primary" ng-disabled="form.$invalid">Execute</button></form></div></div>');
    }
  ]);
})();
(function () {
  'use strict';

  angular.module('ngTestData')
    .service('testdataAlertService', ['$window', function ($window) {
      return {
        addMessage: function (message) {
          $window.alert(message);
        }
      };
    }])
    .controller('testdataController', ['$scope', '$http', 'apiBase', 'testdataAlertService', function ($scope, $http, apiBase, testdataAlertService) {
      $scope.models = {};

      $http.get(apiBase + '/testdata')
        .success(function (data) {
          $scope.datasets = {};
          data.forEach(function (n) {
            $scope.datasets[n.fullName] = n;
          });
        })
        .error(testdataAlertService.addMessage);

      function submit(dataSet) {
        var model = {};
        model[dataSet.fullName] = $scope.models[dataSet.fullName];
        dataSet.dependencies.forEach(function (d) {
          model[d] = $scope.models[d];
        });

        $http.post(apiBase + '/testdata', JSON.stringify({
          dataSet: dataSet.fullName,
          properties: model
        }))
          .success(function (data) {
            var messages = data.join(', ');
            testdataAlertService.addMessage('DataSet Complete - ' + messages, 'success');
          })
          .error(testdataAlertService.addMessage);
      }

      function selectDataSet(ds) {
        $scope.selectedDataSet = ds;
        $scope.selectedDataSetList = [$scope.datasets[ds.fullName]];
        ds.dependencies.forEach(function (d) {
          $scope.selectedDataSetList.push($scope.datasets[d]);
        });
      }

      function dataSetsWithProperties(value) {
        return value.properties.length > 0;
      }

      $scope.submit = submit;
      $scope.selectDataSet = selectDataSet;
      $scope.dataSetsWithProperties = dataSetsWithProperties;
    }]);
})();