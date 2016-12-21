//Developed by Soroosh Ansari
angular.module('HomePage.controllers', [])
.controller('HomeController', ['$scope', '$http',
    function ($scope, $http) {
        $scope.ContactUs = function (model, form) {
            $scope.overlay = true;
            if (form.$valid && !model.invalidFile) {
                method = 'POST';
                url = '/Home/ContactEmailSender';
                data = {
                    email: model.email,
                    message: model.message,
                    name: model.name,
                    subject: model.subject
                };
                $http({ method: method, url: url, data: data }).
                  then(function (response) {
                      switch (response.data.status) {
                          case respondstatus.success:
                              alert("eamil sent.");
                              break;
                          case respondstatus.invalidInput:
                              alert("invalid input: " + response.data.content);
                              break;
                          default:
                              alert("email sending failed: " + response.data.content);
                              break;
                      }
                  }, function (response) {
                      console.log("Request failed");
                      console.log("status:" + response.status);
                  });
            }
        };

    }]);


var respondstatus =
{
    success: 0,
    invalidInput: 1,
    ivalidCordinates: 2,
    invalidFileFormat: 3,
    unknownError: 4,
    dbError: 5,
    invalidId: 6,
    forignKeyError: 7
}