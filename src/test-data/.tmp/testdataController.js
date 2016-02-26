(function () {
  'use strict';

  angular.module('test-data')
    .controller('testdataController', ['$scope', '$http', 'apiBase', '$window', function ($scope, $http, apiBase, $window) {
      $scope.models = {};

      $http.get(apiBase + '/testdata')
        .success(function (data) {
          $scope.datasets = {};
          data.forEach(function (n) {
            $scope.datasets[n.fullName] = n;
          });
        })
        .error($window.alert);

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
            $window.alert('DataSet Complete - ' + messages, 'success');
          })
          .error($window.alert);
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