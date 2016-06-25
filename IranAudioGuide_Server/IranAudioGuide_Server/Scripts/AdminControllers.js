//Developed by Soroosh Ansari
angular.module('AdminPage.controllers', [])
.controller('AdminController', ['$scope', '$rootScope', '$sce', 'PlaceServices', 'CityServices', 'AudioServices',
    function ($scope, $rootScope, $sce, PlaceServices, CityServices, AudioServices) {
        //global
        $rootScope.hide = function (modal) {
            $(modal).modal('hide');
        }
        $rootScope.ShowOverlay = false;
        $rootScope.EditOverlay = false;
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
        var playingIndex;
        //$scope.timeElapsed = "";
        //$scope.audioProgress = 0;
        var audio;
        var audioStatus = "empty"; //empty, play, puase
        $scope.LoadPlaceAudios = function (PlaceId) {
            $scope.selectedPlaceId = PlaceId;
            $scope.AudioTitle = "...";
            audioStatus = "empty";
            AudioServices.Get(PlaceId);
            //scroll("#Player");
        };
        $scope.$on('FillFirstAudio', function (event) {
            $scope.loadAudio(1);
        });
        $scope.loadAudio = function (audioIndex) {
            playingIndex = audioIndex;
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
        $scope.Audio_prev = function () {
            if (playingIndex > 1) {
                if (audioStatus == "play")
                    $scope.Audio_Play(); //first pause the playing audio
                $scope.loadAudio(playingIndex - 1);
                $scope.Audio_Play();
            }
        };
        $scope.Audio_next = function () {
            if ($rootScope.audios.length > playingIndex) {
                if (audioStatus == "play")
                    $scope.Audio_Play(); //first pause the playing audio
                $scope.loadAudio(playingIndex + 1);
                $scope.Audio_Play();
            }
        };

        //Audio Stuff
        $scope.NewAudio = function () {
            $('#NewAudioModal').modal('show');
        };
        $scope.AddAudio = function (model) {
            //$('#NewAudioModal').modal('hide');
            $rootScope.ShowOverlay = true;
            //model.PlaceId = $scope.selectedPlaceId;
            AudioServices.Add(model, $scope.selectedPlaceId);
        };
        $scope.removeAudio = function (audioId) {
            AudioServices.Remove(audioId);
        }
        $scope.$on('UpdateAudios', function (event) {
            $scope.LoadPlaceAudios($scope.selectedPlaceId);
        });
        $scope.$on('UpdateAudioValidationSummery', function (event, data) {
            //$scope.additionalError = data.data;
            //scroll("#NewPlace");
        });
        $scope.$on('RemoveAudioError', function (event, content) {
            $scope.ForignKeyErrorBody = 'Error on removing audio.<br/>' + content + '<br/>Contact site developer to get more information.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });
        $scope.$on('LoadFirstPlaceAudios', function (event) {
            if ($scope.places.length > 0) {
                $scope.LoadPlaceAudios($scope.places[0].PlaceId);
            }
        });
        $scope.SetAudioName = function (o) {
            NewAudioForm.AudioUrl.value = o.files[0].name;
        }

        //validation stuff
        //var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
        //function Validate(oForm) {
        //    var arrInputs = oForm.getElementsByTagName("input");
        //    for (var i = 0; i < arrInputs.length; i++) {
        //        var oInput = arrInputs[i];
        //        if (oInput.type == "file") {
        //            var sFileName = oInput.value;
        //            if (sFileName.length > 0) {
        //                var blnValid = false;
        //                for (var j = 0; j < _validFileExtensions.length; j++) {
        //                    var sCurExtension = _validFileExtensions[j];
        //                    if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
        //                        blnValid = true;
        //                        break;
        //                    }
        //                }

        //                if (!blnValid) {
        //                    alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}

        //progress bar stuff
        //var updateProgress = function () {
        //    if (audio.duration == 'Infinity') { //If duration = infinity set value to 100
        //        $scope.audioProgress = 100;
        //    } else if (audio.currentTime > 0) { //else if it is > 0 calculate percentage to highlight
        //        $scope.audioProgress = "{'width': " + Math.floor((100 / audio.duration) * audio.currentTime).toString() + "% !important}";
        //        console.log($scope.audioProgress);
        //    }
        //    $scope.timeElapsed = formatTime(audio.currentTime);
        //};
        //var formatTime = function (seconds) {
        //    minutes = Math.floor(seconds / 60);
        //    minutes = (minutes >= 10) ? minutes : "" + minutes;
        //    seconds = Math.floor(seconds % 60);
        //    seconds = (seconds >= 10) ? seconds : "0" + seconds;
        //    return minutes + ":" + seconds;
        //}

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
        $scope.$on('PlaceForignKeyError', function (event, data) {
            $scope.ForignKeyErrorBody = 'This place (<span class="text-danger">' + data.PlaceName + '</span>) has one or more audios.<br />To remove this place, first you have to delete it\'s audios.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });
        $scope.$on('PlaceUnknownError', function (event) {
            $scope.ForignKeyErrorBody = 'Unknown error prevent removing place. Contact site developer to get more information.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });

        //Edit Place
        $scope.EditPlaceVM;
        $scope.selectedPlace = {};
        $scope.ShowEditPlaceModal = function (Place) {
            $scope.selectedPlace.Img = Place.ImgUrl;
            $scope.EditPlaceVM = angular.copy(Place);
            $('#EditPlaceModal').modal('show');
        };
        $scope.EditPlace = function (EditPlaceVM) {
            $rootScope.EditOverlay = true;
            PlaceServices.Edit(EditPlaceVM);
        };
        $scope.ClickPlaceImg = function () {
            $('#PlaceImage').click();
        };
        $scope.ChangeImg = function (NewImage) {
            $rootScope.EditOverlay = true;
            PlaceServices.ChangeImage($scope.selectedPlace.Img, NewImage.files[0])
        };
        $scope.$on('EditPlaceValidationSummery', function (event, data) {
            console.log(data.data);
            //to be fill
        });
        $scope.$on('EditPlaceUnknownError', function (event) {
            //$scope.ForignKeyErrorBody = 'Unknown error prevent removing place. Contact site developer to get more information.'
            //$scope.DelSubsBtn = "hidden";
            //$('#ForignKeyErrorModal').modal('show');

        });
        $scope.$on('UpdatePlaceImage', function (event) {
            $scope.selectedPlace.Img = $scope.selectedPlace.Img + "?" + new Date().getMilliseconds();
        });
        $scope.$on('UpdateImageValidationSummery', function (event, data) {
            console.log(data);
            //to be fill
        });

        //City stuff
        $scope.CityNameValidator = "hidden";
        $rootScope.CityPagesLen;
        $rootScope.CityCurrentPage;
        $scope.AllCities = CityServices.All();
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
