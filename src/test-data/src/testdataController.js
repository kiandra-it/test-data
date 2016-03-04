(function () {
  'use strict';

  angular.module('ngTestData')
    .service('testdataAlertService', function ($window) {
      return {
        addMessage: function (message) {
          $window.alert(message);
        }
      };
    })
    .controller('testdataController', function ($scope, $http, apiBase, testdataAlertService) {
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
    });
})();