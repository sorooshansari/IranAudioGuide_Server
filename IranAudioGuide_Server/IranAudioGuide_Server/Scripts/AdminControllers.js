angular.module('AdminPage.controllers', [])
.controller('AdminController', ['$scope', '$rootScope', '$sce', 'PlaceServices', 'CityServices', 'AudioServices', function ($scope, $rootScope, $sce, PlaceServices, CityServices, AudioServices) {
    //global
    function scroll(id) {
        var dest = 0;
        if ($(id).offset().top > $(document).height() - $(window).height()) {
            dest = $(document).height() - $(window).height();
        } else {
            dest = $(id).offset().top;
        }
        //go to destination
        $('html,body').animate({
            scrollTop: dest
        }, 2000, 'swing');
    };
    $scope.SetImageName = function (o) {
        NewPlaceForm.imgUrl.value = o.files[0].name;
    }

    //Player stuff
    $scope.selectedPlaceId;
    $rootScope.placeimage = "160x100.png";
    $scope.AudioTitle = "...";
    $rootScope.audios;
    $scope.PlayStatus = "play";
    //$scope.timeElapsed = "";
    //$scope.audioProgress = 0;
    var audio;
    var audioStatus = "empty"; //empty, play, puase
    $scope.LoadPlaceAudios = function (PlaceId) {
        $scope.selectedPlaceId = PlaceId;
        $scope.AudioTitle = "..."
        audioStatus = "empty";
        AudioServices.Get(PlaceId);
    };
    $scope.$on('FillFirstAudio', function (event) {
        $scope.loadAudio(1);
    });
    $scope.loadAudio = function (audioIndex) {
        angular.forEach($rootScope.audios, function (value, key) {
            if (value.Index == audioIndex) {
                if (audioStatus != "empty") {
                    audio.pause();
                    $scope.PlayStatus = "play";
                }
                $scope.AudioTitle = value.Aud_Name;
                var src = "../Audios/" + value.Aud_Url;
                audio = new Audio(src);
                audioStatus = "pause";
                return;
            }
        });
    }
    $scope.Audio_Play = function () {
        switch (audioStatus) {
            case "empty":
                break;
            case "play":
                //clearInterval(timer);
                audio.pause();
                //$scope.timeElapsed = "";
                //$scope.audioProgress = 0;
                $scope.PlayStatus = "play";
                audioStatus = "pause";
                break;
            case "pause":
                audio.play();
                //var timer = setInterval(updateProgress, 1000);
                $scope.PlayStatus = "pause";
                audioStatus = "play";
                break;
            default:

        }
    };
    $scope.Audio_prev = function () { };
    $scope.Audio_next = function () { };
    $scope.removeAudio = function (audioId) {
        console.log(audioId);
        console.log($rootScope.placeimage);
    }
    var updateProgress = function () {
        if (audio.duration == 'Infinity') { //If duration = infinity set value to 100
            $scope.audioProgress = 100;
        } else if (audio.currentTime > 0) { //else if it is > 0 calculate percentage to highlight
            $scope.audioProgress = "{'width': " + Math.floor((100 / audio.duration) * audio.currentTime).toString() + "% !important}";
            console.log($scope.audioProgress);
        }
        $scope.timeElapsed = formatTime(audio.currentTime);
    };
    var formatTime = function (seconds) {
        minutes = Math.floor(seconds / 60);
        minutes = (minutes >= 10) ? minutes : "" + minutes;
        seconds = Math.floor(seconds % 60);
        seconds = (seconds >= 10) ? seconds : "0" + seconds;
        return minutes + ":" + seconds;
    }

    //Place stuff
    $rootScope.PlacePagesLen;
    $rootScope.PlaceCurrentPage;
    $scope.places = PlaceServices.Get(0);
    $scope.PreviousPlace = function () {
        if ($rootScope.PlaceCurrentPage > 0)
            $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage - 1);
    };
    $scope.NextPlace = function () {
        if ($rootScope.PlacePagesLen - $rootScope.PlaceCurrentPage > 1)
            $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage + 1);
    };
    $scope.AddPlace = function (NewPlace, form) {
        if (form.$valid) {
            PlaceServices.AddPlace(NewPlace);

            $scope.NewPlace.PlaceName = "";
            NewPlaceForm.imgUrl.value = "";
            $scope.NewPlace.Image = "";
            $scope.NewPlace.PlaceCityId = "";
            $scope.NewPlace.PlaceCordinates = "";
            $scope.NewPlace.PlaceDesc = "";
            $scope.NewPlace.PlaceAddress = "";
        }
    };
    $scope.RemovePlace = function (PlaceID, PlaceName) {
        PlaceServices.RemovePlace(PlaceID, PlaceName);
    };
    $scope.$on('UpdatePlaces', function (event) {
        $scope.places = PlaceServices.Get(0);
        scroll("#PlaceList");
    });
    $scope.$on('UpdatePlaceValidationSummery', function (event, data) {
        $scope.additionalError = data.data;
        scroll("#NewPlace");
    });
    $scope.$on('PlaceForignKeyError', function (event, PlaceID, PlaceName) {
        $scope.ForignKeyErrorBody = 'This place (<span class="text-danger">' + PlaceName + '</span>) has one or more audios.<br />To remove this place, first you have to delete it\'s audios.'
        $scope.DelSubsBtn = "hidden";
        $('#ForignKeyErrorModal').modal('show');
    });
    $scope.$on('PlaceUnknownError', function (event) {
        $scope.ForignKeyErrorBody = 'Unknown error prevent removing place. Contact site developer to get more information.'
        $scope.DelSubsBtn = "hidden";
        $('#ForignKeyErrorModal').modal('show');
    });

    //City stuff
    $scope.CityNameValidator = "hidden";
    $rootScope.CityPagesLen;
    $rootScope.CityCurrentPage;
    $scope.cities = CityServices.Get(0);
    $scope.PreviousCity = function () {
        if ($rootScope.CityCurrentPage > 0)
            $scope.cities = CityServices.Get($rootScope.CityCurrentPage - 1);
    };
    $scope.NextCity = function () {
        if ($rootScope.CityPagesLen - $rootScope.CityCurrentPage > 1)
            $scope.cities = CityServices.Get($rootScope.CityCurrentPage + 1);
    };
    $scope.AddCity = function (NewCity, form) {
        if (form.$valid) {
            CityServices.AddCity(NewCity);
            $scope.NewCity.CityName = "";
            $scope.NewCity.CityDesc = "";
        }
    };
    $scope.RemoveCity = function (CityID, CityName) {
        CityServices.RemoveCity(CityID, CityName);
    };
    $scope.$on('UpdateCities', function (event) {
        $scope.cities = CityServices.Get(0);
        scroll("#Cities");
    });
    $scope.$on('CityForignKeyError', function (event, CityID, CityName) {
        $scope.ForignKeyErrorBody = 'This city (<span class="text-danger">' + CityName + '</span>) has one or more places.<br />To remove this city, first you have to delete it\'s places.'
        $scope.DelSubsBtn = "hidden";
        $('#ForignKeyErrorModal').modal('show');
    });
    $scope.$on('CityUnknownError', function (event) {
        $scope.ForignKeyErrorBody = 'Unknown error prevent removing city. Contact site developer to get more information.'
        $scope.DelSubsBtn = "hidden";
        $('#ForignKeyErrorModal').modal('show');
    });

}]);
