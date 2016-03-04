(function () {
  /* globals describe, it, beforeEach, inject, expect, spyOn, angular */
  'use strict';

  describe('ngTestData', function () {
    angular.module('ngTestData').constant('apiBase', 'api');

    beforeEach(module('ngTestData'));

    describe('testdataController', function () {
      var dataSetPayload = [{
        'dependencies': [],
        'name': 'Cities',
        'description': 'A list of cities',
        'fullName': 'Template.Tests.DataSets.CityDataSet',
        'typeName': 'CityDataSet',
        'properties': []
      }, {
        'dependencies': ['Template.Tests.DataSets.CityDataSet'],
        'name': 'States',
        'description': 'A list of states',
        'fullName': 'Template.Tests.DataSets.StateDataSet',
        'typeName': 'StateDataSet',
        'properties': [{
          'fieldName': 'StartsWith',
          'name': 'Starts With',
          'description': 'Filter the states inserted',
          'dataType': 'String'
        }, {
          'fieldName': 'CreatedOn',
          'name': 'Created On',
          'description': 'Date the states were created on',
          'dataType': 'Date'
        }, {
          'fieldName': 'Count',
          'name': 'Count',
          'description': 'The count',
          'dataType': 'Number'
        }]
      }];

      it('should index available datasets onload', inject(function ($rootScope, $controller, $httpBackend) {
        $httpBackend.expectGET('api/testdata').respond(200, dataSetPayload);

        var scope = $rootScope.$new();

        $controller('testdataController', {
          $scope: scope
        });

        $httpBackend.flush();

        expect(scope.datasets['Template.Tests.DataSets.CityDataSet']).toEqual(dataSetPayload[0]);
        expect(scope.datasets['Template.Tests.DataSets.StateDataSet']).toEqual(dataSetPayload[1]);
      }));

      it('should submit its dataset request and report messages', inject(function ($rootScope, $controller, $httpBackend, testdataAlertService) {
        $httpBackend.expectGET('api/testdata').respond(200, dataSetPayload);

        var scope = $rootScope.$new();

        $controller('testdataController', {
          $scope: scope
        });

        $httpBackend.flush();

        spyOn(testdataAlertService, 'addMessage');
        $httpBackend.expectPOST('api/testdata').respond(200, ['Inserted 1 State']);
        scope.models['Template.Tests.DataSets.StateDataSet'] = {
          'Count': 1
        };
        scope.submit(scope.datasets['Template.Tests.DataSets.StateDataSet']);
        $httpBackend.flush();

        expect(testdataAlertService.addMessage).toHaveBeenCalledWith('DataSet Complete - Inserted 1 State', 'success');
      }));

      it('should build list of relevent datasets when dataset is selected', inject(function ($rootScope, $controller, $httpBackend) {
        $httpBackend.expectGET('api/testdata').respond(200, dataSetPayload);

        var scope = $rootScope.$new();

        $controller('testdataController', {
          $scope: scope
        });

        $httpBackend.flush();

        scope.selectDataSet(scope.datasets['Template.Tests.DataSets.StateDataSet']);
        expect(scope.selectedDataSetList).toEqual([scope.datasets['Template.Tests.DataSets.StateDataSet'], scope.datasets['Template.Tests.DataSets.CityDataSet']]);
      }));

      it('should return true when set has properties', inject(function ($rootScope, $controller, $httpBackend) {
        $httpBackend.expectGET('api/testdata').respond(200, dataSetPayload);

        var scope = $rootScope.$new();

        $controller('testdataController', {
          $scope: scope
        });

        $httpBackend.flush();

        expect(scope.dataSetsWithProperties({
          properties: [1]
        })).toEqual(true);
      }));
    });
  });
})();