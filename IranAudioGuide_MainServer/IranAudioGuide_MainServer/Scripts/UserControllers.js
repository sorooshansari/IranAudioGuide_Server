//Developed by Soroosh Ansari
angular.module('UserPage.controllers', [])
.controller('UserController', ['$scope',
    function ($scope) {

        $scope.Items = [{
            Name: "Shiraz",
            id: 0
        }, {
            Name: "Esfahan",
            id: 1
        }, {
            Name: "Tehran",
            id: 2
        }, {
            Name: "Yazd",
            id: 3
        }];
        //$scope.checkAll = function () {
        //    if ($scope.selectedAll) {
        //        $scope.selectedAll = true;
        //    } else {
        //        $scope.selectedAll = false;
        //    }

        //    angular.forEach($scope.Items, function (item) {
        //        item.Selected = $scope.selectedAll;
        //    });

        //};

    }]);
