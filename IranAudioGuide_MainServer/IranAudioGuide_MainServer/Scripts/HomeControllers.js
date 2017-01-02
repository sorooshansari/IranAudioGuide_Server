//Developed by Soroosh Ansari
angular.module('HomePage.controllers', [])
.controller('HomeController', ['$scope', '$http', '$timeout',
    function ($scope, $http, $timeout) {
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
                $scope.sent = false;
                $scope.overlay = true;
                $http({ method: method, url: url, data: data }).
                  then(function (response) {
                      switch (response.data.status) {
                          case respondstatus.success:
                              $scope.sentMessage = "Thanks! Your message sent successfully!.";
                              $scope.sent = true;
                              break;
                          default:
                              $scope.sentMessage = "Email sending failed: " + response.data.content;
                              $scope.sent = true;
                              break;
                      }
                      $timeout(function () {
                          $scope.overlay = false;
                          $scope.contactForm.$setPristine();
                          $scope.ContactFormModel = {};
                      }, 1000);
                  }, function (response) {
                      console.log("Request failed");
                      console.log("status:" + response.status);
                  });
            }
        };
        valid = [true, true, true, true];
        $scope.ValidationClass = function (i) {
            return (valid[i]) ? '' : 'sorooshInvalid';
        }
        $scope.validator = function (e, i) {
            valid[i] = (e.target.classList.contains('ng-invalid')) ? false : true;

        }
        $scope.$on('elementScrolledIntoView', function (event, data) {
            if (data === 'accurate_information_section') {
                // do something
            }
            if (data === 'whattolisten_section') {
                // do something
            }
            if (data === 'yourownpace_section') {
                // do something
            }
        });
        $scope.$on('elementScrolledOutOfView', function (event, data) {
            if (data === 'accurate_information_section') {
                // do something
            }
            if (data === 'whattolisten_section') {
                // do something
            }
            if (data === 'yourownpace_section') {
                // do something
            }
        });
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