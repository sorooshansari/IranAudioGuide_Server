angular.module('AdminPage.services', [])
.service('factory', ['$rootScope', '$http', function ($rootScope, $http) {
    angular.module("CheckAllModule", [])
    .controller("checkboxController", function checkboxController($scope) {


        $scope.Items = [{
            Name: "Shiraz"
        }, {
            Name: "Esfahan"
        }, {
            Name: "Tehran"
        }, {
            Name: "Yazd"
        }];
        $scope.checkAll = function () {
            if ($scope.selectedAll) {
                $scope.selectedAll = true;
            } else {
                $scope.selectedAll = false;
            }

            angular.forEach($scope.Items, function (item) {
                item.Selected = $scope.selectedAll;
            });

        };


    });
}]);