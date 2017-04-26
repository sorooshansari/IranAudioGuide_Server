//Developed by Soroosh Ansari
angular.module('AdminPage.controllers', [])

.controller('AdminController', ['$scope', '$rootScope', '$sce', 'PlaceServices', 'CityServices', 'AudioServices', 'TipServices', 'StoryServices', 'PackageServices',
    function ($scope, $rootScope, $sce, PlaceServices, CityServices, AudioServices, TipServices, StoryServices, PackageServices) {
        //global
        var placePaging = 10;
        var paging = 5;
        var validImgFormats = ['jpg', 'gif'];
        var validAudioFormats = ['mp3'];
        $rootScope.hide = function (modal) {
            $(modal).modal('hide');
        }
        $rootScope.ShowOverlay = false;
        $rootScope.NewStoryShowOverlay = false;
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
        $scope.selectCityImg = false;
        $scope.SetCityImageName = function (o) {
            NewCityForm.cityImgUrl.value = o.files[0].name;
            $scope.selectCityImg = true;
        }
        $scope.saveOrderExtraImg = function (model) {
            $rootScope.saveOrderBtn = true;
            PlaceServices.editeOrder(model).then(function () {
                $('#EditPlaceModal').modal('hide');
                $rootScope.saveOrderBtn = false;
            }, function () {
                $('#EditPlaceModal').modal('hide');
                $rootScope.saveOrderBtn = false;

            });
        }

        //Online Player stuff
        //$scope.OnlineselectedPlaceId;
        //$rootScope.OnlinePlaceimage = "/Content/images/160x100.png";
        //$scope.OnlineAudioTitle = "...";
        //$rootScope.OnlineAudios;
        //$scope.OnlinePlayStatus = "play";
        //var OnlinePlayingIndex;
        //var OnlineAudio;
        //var OnlineAudioStatus = "empty"; //empty, play, puase
        //$scope.OnlineLoadPlaceAudios = function (PlaceId) {
        //    $scope.OnlineSelectedPlaceId = PlaceId;
        //    $scope.OnlineAudioTitle = "...";
        //    OnlineAudioStatus = "empty";
        //    AudioServices.OnlineGet(PlaceId);
        //};
        //$scope.$on('OnlineFillFirstAudio', function (event) {
        //    $scope.OnlineLoadAudio(1);
        //});
        //$scope.OnlineLoadAudio = function (audioIndex) {
        //    OnlinePlayingIndex = audioIndex;
        //    angular.forEach($rootScope.OnlineAudios, function (value, key) {
        //        if (value.Index == audioIndex) {
        //            if (OnlineAudioStatus != "empty") {
        //                OnlineAudio.pause();
        //                $scope.OnlinePlayStatus = "play";
        //            }
        //            $scope.OnlineAudioTitle = value.Aud_Name;
        //            var src = "../Audios/" + value.Aud_Url;
        //            OnlineAudio = new Audio(src);
        //            OnlineAudioStatus = "pause";
        //            return;
        //        }
        //    });
        //}
        //$scope.OnlineAudio_Play = function () {
        //    switch (OnlineAudioStatus) {
        //        case "empty":
        //            break;
        //        case "play":
        //            OnlineAudio.pause();
        //            $scope.OnlinePlayStatus = "play";
        //            OnlineAudioStatus = "pause";
        //            break;
        //        case "pause":
        //            OnlineAudio.play();
        //            $scope.OnlinePlayStatus = "pause";
        //            OnlineAudioStatus = "play";
        //            break;
        //        default:

        //    }
        //};
        //$scope.OnlineAudio_prev = function () {
        //    if (OnlinePlayingIndex > 1) {
        //        if (OnlineAudioStatus == "play")
        //            $scope.OnlineAudio_Play(); //first pause the playing audio
        //        $scope.OnlineLoadAudio(OnlinePlayingIndex - 1);
        //        $scope.OnlineAudio_Play();
        //    }
        //};
        //$scope.OnlineAudio_next = function () {
        //    if ($rootScope.OnlineAudios.length > OnlinePlayingIndex) {
        //        if (OnlineAudioStatus == "play")
        //            $scope.OnlineAudio_Play(); //first pause the playing audio
        //        $scope.OnlineLoadAudio(OnlinePlayingIndex + 1);
        //        $scope.OnlineAudio_Play();
        //    }
        //};

        //$scope.$on('OnlineLoadFirstPlaceAudios', function (event) {
        //    if ($scope.OnlinePlaces.length > 0) {
        //        $scope.OnlineLoadPlaceAudios($scope.OnlinePlaces[0].PlaceId);
        //    }
        //});

        ////Online Place stuff
        //$rootScope.OnlinePlacePagesLen;
        //$rootScope.OnlinePlaceCurrentPage;
        //$scope.OnlinePlaces = PlaceServices.OnlineGet(0);
        //$scope.PreviousPlace = function () {
        //    if ($rootScope.OnlinePlaceCurrentPage > 0)
        //        $scope.OnlinePlaces = PlaceServices.OnlineGet($rootScope.OnlinePlaceCurrentPage - 1);
        //};
        //$scope.OnlineNextPlace = function () {
        //    if ($rootScope.OnlinePlacePagesLen - $rootScope.OnlinePlaceCurrentPage > 1)
        //        $scope.OnlinePlaces = PlaceServices.OnlineGet($rootScope.OnlinePlaceCurrentPage + 1);
        //};
        //$scope.OnlineRemovePlaceVM = {};
        //$scope.OnlineRemovePlaceModal = function (PlaceID, PlaceName) {
        //    $scope.OnlineRemovePlaceVM.PlaceID = PlaceID;
        //    $scope.OnlineRemovePlaceVM.PlaceName = PlaceName;
        //    $('#OnlineEditPlaceModal').modal('show');
        //};
        //$scope.OnlineRemovePlace = function (PlaceID) {
        //    PlaceServices.OnlineRemovePlace(PlaceID);
        //};
        //$scope.$on('OnlineUpdatePlaces', function (event) {
        //    $scope.OnlinePlaces = PlaceServices.OnlineGet(0);
        //    scroll("#OnlinePlaces");
        //});

        //Edit Place
        $scope.OnlineEditPlaceVM;
        $scope.OnlineSelectedPlace = {};
        $scope.OnlineShowEditPlaceModal = function (Place) {
            $scope.OnlineSelectedPlace.ExtraImages = PlaceServices.GetExtraImages(Place.PlaceId);
            $scope.OnlineSelectedPlace.Img = Place.ImgUrl;
            $scope.selectedPlace.TumbImgUrl = place.TumbImgUrl;
            $scope.OnlineEditPlaceVM = angular.copy(Place);
            $('#OnlineEditPlaceModal').modal('show');
        };

        //Story and Audio Stuff
        $scope.LoadPlaceAudiosAndStories = function (placeId) {
            $scope.LoadPlaceStorys(placeId);
            $scope.LoadPlaceAudios(placeId);
            for (var i = 0; i < $scope.places.length; i++) {
                if ($scope.places[i].PlaceId == placeId) {
                    $scope.places[i].selected = true;
                }
                else
                    $scope.places[i].selected = false;
            }
        };
        //Story Player stuff
        $scope.StoryTitle = "...";
        $rootScope.Storys;
        $scope.StoryPlayStatus = "play";
        var playingIndex;
        //$scope.timeElapsed = "";
        //$scope.StoryProgress = 0;
        var Story;
        var StoryStatus = "empty"; //empty, play, puase
        $scope.LoadPlaceStorys = function (PlaceId) {
            $scope.selectedPlaceId = PlaceId;
            $scope.StoryTitle = "...";
            StoryStatus = "empty";
            StoryServices.Get(PlaceId);
            //scroll("#Player");
        };
        $scope.$on('FillFirstStory', function (event) {
            $scope.loadStory(1);
        });
        $scope.loadStory = function (StoryIndex) {
            playingIndex = StoryIndex;
            angular.forEach($rootScope.Storys, function (value, key) {
                if (value.Index == StoryIndex) {
                  //  todo test getUrl
                    var m = {
                        trackId: value.Id,
                        isAudio: false
                    }
                    PlaceServices.getUrl(m);
                    if (StoryStatus != "empty") {
                        Story.pause();
                        $scope.StoryPlayStatus = "play";
                    }
                    $scope.StoryTitle = value.Name;
                    //var src = "../Stories/" + value.Url;
                    var src = value.Url;
                    Story = new Audio(src);
                    StoryStatus = "pause";
                    console.log(src);
                    return;
                }
            });
        }
        $scope.Story_Play = function () {
            switch (StoryStatus) {
                case "empty":
                    break;
                case "play":
                    //clearInterval(timer);
                    Story.pause();
                    //$scope.timeElapsed = "";
                    //$scope.StoryProgress = 0;
                    $scope.StoryPlayStatus = "play";
                    StoryStatus = "pause";
                    break;
                case "pause":
                    Story.play();
                    //var timer = setInterval(updateProgress, 1000);
                    $scope.StoryPlayStatus = "pause";
                    StoryStatus = "play";
                    break;
                default:

            }
        };
        $scope.Story_prev = function () {
            if (playingIndex > 1) {
                if (StoryStatus == "play")
                    $scope.Story_Play(); //first pause the playing Story
                $scope.loadStory(playingIndex - 1);
                $scope.Story_Play();
            }
        };
        $scope.Story_next = function () {
            if ($rootScope.Storys.length > playingIndex) {
                if (StoryStatus == "play")
                    $scope.Story_Play(); //first pause the playing Story
                $scope.loadStory(playingIndex + 1);
                $scope.Story_Play();
            }
        };

        //Story Stuff
        $scope.NewStory = function () {
            console.log("hi");
            $('#NewStoryModal').modal('show');
        };
        $scope.SetStoryName = function (o) {
            $scope.NewStoryVM.FileChanged = true;
            var StoryUrl = o.files[0].name;
            var ext = StoryUrl.substring(StoryUrl.lastIndexOf('.') + 1).toLowerCase();
            if (validAudioFormats.indexOf(ext) !== -1) {
                NewStoryForm.StoryUrl.value = StoryUrl;
                $scope.NewStoryVM.invalidFile = false;
            }
            else {
                NewStoryForm.StoryUrl.value = '';
                $scope.NewStoryVM.invalidFile = true;
            }
        }
        $scope.AddStory = function (model, form) {

            if (form.$valid && !model.invalidFile) {
                $rootScope.NewStoryShowOverlay = true;
                StoryServices.Add(model, $scope.selectedPlaceId);
            }
        };
        $scope.removeStory = function (StoryId) {
            StoryServices.Remove(StoryId);
        }
        $scope.$on('UpdateStorys', function (event) {
            $scope.LoadPlaceStorys($scope.selectedPlaceId);
        });
        $scope.$on('UpdateStoryValidationSummery', function (event, args) {
            //$scope.additionalError = args.data;
            //scroll("#NewPlace");
        });
        $scope.$on('RemoveStoryError', function (event, args) {
            $scope.ForignKeyErrorBody = 'Error on removing story.<br/>' + args.content + '<br/>Contact site developer to get more information.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });
        $scope.$on('LoadFirstPlaceStorys', function (event) {
            if ($scope.places.length > 0) {
                $scope.LoadPlaceStorys($scope.places[0].PlaceId);
            }
        });



        //Player stuff
        $scope.selectedPlaceId;
        $rootScope.placeimage = "/Content/images/160x100.png";
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
            ////todo test getUrl
            //for (var i = 0; i < 10; i++) {
            //    var m = {
            //        trackId: "6d997cee-d407-e711-80bf-20cf30561859", // value.Aud_Id,
            //        isAudio: true
            //    }
            //    PlaceServices.getUrl(m);
            //    //1 request 
            //    var m = {
            //        trackId: "7ebc1448-d907-e711-80bf-20cf30561859", // value.Aud_Id,
            //        isAudio: true
            //    }
            //    PlaceServices.getUrl(m);
            //    //2 request 
            //    var m = {
            //        trackId: "bb36dddf-d007-e711-80bf-20cf30561859", // value.Aud_Id,
            //        isAudio: true
            //    }
            //    PlaceServices.getUrl(m);
            //    //3 request 
            //    var m = {
            //        trackId: "fa04be86-e107-e711-80bf-20cf30561859", // value.Aud_Id,
            //        isAudio: true
            //    }
            //    PlaceServices.getUrl(m);
            //    //4 request 
            //    var m = {
            //        trackId: "b20c0aea-f307-e711-80bf-20cf30561859", // value.Aud_Id,
            //        isAudio: true
            //    }
            //    PlaceServices.getUrl(m);
            //    //5 request 
            //    var m = {
            //        trackId: "c050271d-d2fc-e611-80bf-20cf30561859", // value.Aud_Id,
            //        isAudio: true
            //    }
            //    PlaceServices.getUrl(m);

            //    // 6  request 
            //    var m = {
            //        trackId: "ddc58c1e-0b08-e711-80bf-20cf30561859", // value.Aud_Id,
            //        isAudio: true
            //    }
            //    PlaceServices.getUrl(m);
            //}
            playingIndex = audioIndex;
            angular.forEach($rootScope.audios, function (value, key) {
                if (value.Index == audioIndex) {
                    $scope.AudioTitle = value.Aud_Name;
                    //var src = "../Audios/" + value.Aud_Url;
                    var src = value.Aud_Url;
                    audio = new Audio(src);
                    audioStatus = "pause";
                    return;

                    if (audioStatus != "empty") {
                        audio.pause();
                        $scope.PlayStatus = "play";
                    }

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
        $scope.SetAudioName = function (o) {
            $scope.NewAudioVM.FileChanged = true;
            var AudioUrl = o.files[0].name;
            var ext = AudioUrl.substring(AudioUrl.lastIndexOf('.') + 1).toLowerCase();
            if (validAudioFormats.indexOf(ext) !== -1) {
                NewAudioForm.AudioUrl.value = AudioUrl;
                $scope.NewAudioVM.invalidFile = false;
            }
            else {
                NewAudioForm.AudioUrl.value = '';
                $scope.NewAudioVM.invalidFile = true;
            }
        }
        $scope.AddAudio = function (model, form) {
            if (form.$valid && !model.invalidFile) {
                $rootScope.ShowOverlay = true;
                AudioServices.Add(model, $scope.selectedPlaceId);
            }
        };
        $scope.removeAudio = function (audioId) {
            AudioServices.Remove(audioId);
        }
        $scope.$on('UpdateAudios', function (event) {
            $scope.LoadPlaceAudios($scope.selectedPlaceId);
        });
        $scope.$on('UpdateAudioValidationSummery', function (event, args) {
            //$scope.additionalError = args.data;
            //scroll("#NewPlace");
        });
        $scope.$on('RemoveAudioError', function (event, args) {
            $scope.ForignKeyErrorBody = 'Error on removing audio.<br/>' + args.content + '<br/>Contact site developer to get more information.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });
        $scope.$on('LoadFirstPlaceAudios', function (event) {
            if ($scope.places.length > 0) {
                $scope.LoadPlaceAudios($scope.places[0].PlaceId);
            }
        });

        //Place stuff
        $scope.PlacePagesLen = 0;
        $scope.PlaceCurrentPage = 0;
        $rootScope.allPlaces = [];
        PlaceServices.GetAll();
        $scope.SwichPrimaryStatus = function (placeId) {
            PlaceServices.SwichPrimaryStatus(placeId);
        }
        $scope.$on('LoadPlaces', function (event) {
            $scope.places = angular.copy($rootScope.allPlaces.slice(0, placePaging));
            $scope.PlacePagesLen = Math.floor($rootScope.allPlaces.length / placePaging);
            if (($rootScope.allPlaces.length / placePaging > $scope.PlacePagesLen))
                $scope.PlacePagesLen++;
        });
        $scope.NextPlace = function () {
            //if ($rootScope.PlacePagesLen - $rootScope.PlaceCurrentPage > 1)
            //    $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage + 1);
            if ($scope.PlacePagesLen - $scope.PlaceCurrentPage > 1) {
                $scope.places = angular.copy($rootScope.allPlaces.slice(($scope.PlaceCurrentPage + 1) * placePaging, ($scope.PlaceCurrentPage + 2) * placePaging));
                $scope.PlaceCurrentPage++;
            }
        };
        $scope.PreviousPlace = function () {
            //if ($rootScope.PlaceCurrentPage > 0)
            //    $scope.places = PlaceServices.Get($rootScope.PlaceCurrentPage - 1);
            if ($scope.PlaceCurrentPage > 0) {
                $scope.places = angular.copy($rootScope.allPlaces.slice(($scope.PlaceCurrentPage - 1) * placePaging, $scope.PlaceCurrentPage * placePaging));
                $scope.PlaceCurrentPage--;
            }
        };
        $scope.AddPlace = function (NewPlace, form) {
            if (form.$valid) {
                $rootScope.AddplaceLoading = true;
                PlaceServices.AddPlace(NewPlace);
            }
        };
        $scope.$on('placeAdded', function (event) {
            $scope.NewPlace = {
                PlaceName: '',
                Image: '',
                PlaceCityId: '',
                PlaceCordinates: '',
                PlaceDesc: '',
                PlaceAddress: ''
            };
            NewPlaceForm.imgUrl.value = "";
            $scope.NewPlaceForm.$setPristine();
            $scope.NewPlaceForm.$setUntouched();
            $scope.NewPlaceForm.$submitted = false;
            $scope.$broadcast('UpdatePlaces', {});
        });
        $scope.RemovePlace = function (PlaceID, PlaceName) {
            PlaceServices.RemovePlace(PlaceID, PlaceName);
        };
        $scope.$on('UpdatePlaces', function (event) {
            //console.log($scope.PlaceCurrentPage);
            PlaceServices.Get($scope.PlaceCurrentPage).
               then(function (response) {
                   $rootScope.PlacePagesLen = response.data.PagesLen;
                   // $rootScope.PlaceCurrentPage = PageNum;
                   $rootScope.$broadcast('LoadFirstPlaceAudios', {});
                   $scope.places = response.data.Places;

               }, function (response) {
                   console.log("Request failed");
                   console.log("status:" + response.status);
               });
        });
        $scope.$on('UpdatePlaceValidationSummery', function (event, args) {
            $scope.additionalError = args.data;
            scroll("#NewPlace");
        });
        $scope.$on('PlaceForignKeyError', function (event, args) {
            $scope.ForignKeyErrorBody = 'This place (<span class="text-danger">' + args.PlaceName + '</span>) has some dependencies.<br />To remove this place, first you have to remove it\'s dependencies.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });
        $scope.$on('RemoveOnlinePlaceError', function (event, args) {
            $scope.ForignKeyErrorBody = 'This place (<span class="text-danger">' + args.PlaceName + '</span>) is online.<br />Removing online places is unallowable.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });
        $scope.$on('PlaceUnknownError', function (event) {
            $scope.ForignKeyErrorBody = 'Unknown error prevent removing place. Contact site developer to get more information.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });

        //Get Online
        $scope.GetOnlineVM = {};
        $scope.ShowGetOnlineModal = function (PlaceId, PlaceName) {
            $scope.GetOnlineVM.Id = PlaceId;
            $scope.GetOnlineVM.Name = PlaceName;
            $('#GoOnlineModal').modal('show');
        };
        $scope.GoOnline = function (PlaceId) {
            PlaceServices.GoOnline(PlaceId);
            $('#GoOnlineModal').modal('hide');
        };

        //Get Offline
        $scope.GetOfflineVM = {};
        $scope.ShowGetOfflineModal = function (PlaceId, PlaceName) {
            $scope.GetOfflineVM.Id = PlaceId;
            $scope.GetOfflineVM.Name = PlaceName;
            $('#GoOfflineModal').modal('show');
        };
        $scope.GoOffline = function (PlaceId) {
            PlaceServices.GoOffline(PlaceId);
        };

        //Edit Place
        $scope.EditPlaceVM;
        $scope.selectedPlace = {};
        $scope.ShowEditPlaceModal = function (Place) {
            $scope.selectedPlace.ExtraImages = PlaceServices.GetExtraImages(Place.PlaceId);
            $scope.selectedPlace.Img = Place.ImgUrl + "?" + new Date().getMilliseconds();
            $scope.selectedPlace.TumbImgUrl = Place.TumbImgUrl + "?" + new Date().getMilliseconds();
            $scope.selectedPlace.Id = Place.PlaceId;
            $scope.selectedPlace.PlaceTips = TipServices.getPlaceTips(Place.PlaceId);
            $scope.EditPlaceVM = angular.copy(Place);
            //{ id: '1', Content: 'vgjhbkn lm vybjnk' },
            //{ id: '2', Content: 'kl;dhjv; h/dm h;gb' }];
            $('#EditPlaceModal').modal('show');
        };
        $scope.EditPlace = function (EditPlaceVM) {
            //console.log(EditPlaceVM);
            $rootScope.EditOverlay = true;
            PlaceServices.Edit(EditPlaceVM);
        };
        $scope.ClickPlaceImg = function (id) {
            $(id).click();
        };
        $scope.ChangeImg = function (NewImage) {
            $rootScope.EditOverlay = true;
            PlaceServices.ChangeImage(NewImage.files[0], $scope.selectedPlace.Id)
        };
        $scope.ChangeTumbImg = function (NewImage) {
            $rootScope.EditOverlay = true;
            PlaceServices.ChangeTumbImg(NewImage.files[0], $scope.selectedPlace.Id)
        };
        $scope.AddExtraImage = function (image) {
            PlaceServices.AddExtraImage(image.files[0], $scope.EditPlaceVM.PlaceId);
        };
        $scope.RemoveExtraImg = function (ImageId) {
            PlaceServices.DelExtraImage(ImageId);
        };
        $scope.EditEIDescVM = {};
        $scope.EditEIDescModal = function (image) {
            //console.log(image);
            $scope.EditEIDescVM.ImageId = image.ImageId;
            $scope.EditEIDescVM.Desc = image.ImageDesc;
            $('#EditEIDescModal').modal('show');
        };
        $scope.EditEIDesc = function (EditEIDescVM, form) {
            if (form.$valid) {
                PlaceServices.EditEIDesc(EditEIDescVM);
            }
        };
        $scope.$on('UpdateExtraImg', function (event) {
            $scope.selectedPlace.ExtraImages = PlaceServices.GetExtraImages($scope.EditPlaceVM.PlaceId);
        });
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
            $rootScope.placeimage = $scope.selectedPlace.Img;
        });
        $scope.$on('UpdatePlaceTumbImage', function (event) {
            $scope.selectedPlace.TumbImgUrl = $scope.selectedPlace.TumbImgUrl + "?" + new Date().getMilliseconds();
        });
        $scope.$on('UpdateImageValidationSummery', function (event, data) {
            console.log(data);
            //to be fill
        });
        //Tips
        $rootScope.allTipCategories = [];
        //{ id: '1', Class: 'ion-android-walk', name: '&#xf3bb; transportation', uniCode: '&#xf3bb;' },
        //{ id: '2', Class: 'ion-ios-pulse-strong', name: '&#xf492; rough trak', uniCode: '&#xf492;' }];
        TipServices.GetTipCategories();
        $scope.GetTipUniCode = function (id) {
            for (var i = 0; i < $scope.allTipCategories.length; i++) {
                if ($scope.allTipCategories[i].id == id) {
                    return $scope.allTipCategories[i].unicode;
                }
            }
            return '';
        }
        $scope.AddTip = function (model, placeId) {
            model.placeId = placeId;
            TipServices.AddTip(model);
        }
        $scope.$on('TipAdded', function (event, data) {
            $scope.newTip = {
                categoryId: '',
                content: ''
            };
            $scope.selectedPlace.PlaceTips = TipServices.getPlaceTips(data.PlaceId);
        });
        $scope.RemoveTip = function (TipId, placeId) {
            TipServices.RemoveTip(TipId, placeId);
        }
        $scope.$on('TipRemoved', function (event, data) {
            console.log(data);
            $scope.selectedPlace.PlaceTips = TipServices.getPlaceTips(data.PlaceId);
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
        $scope.selectCityImg = false;
        $scope.AddCity = function (NewCity, form) {
            if (form.$valid && $scope.selectCityImg) {
                $rootScope.AddCityLoading = true;
                CityServices.AddCity(NewCity);

            }
        };
        $scope.EditCity = function (cityVM, form) {
            $rootScope.EditOverlay = true;
            if (form.$valid) {
                CityServices.Edit(cityVM);
            }
        };
        $scope.ShowEditCityModal = function (City) {
            $scope.selectedCityImg = City.CityImageUrl + "?" + new Date().getMilliseconds();
            $scope.EditCityVM = angular.copy(City);
            $('#EditCityModal').modal('show');
        };
        $scope.ChangeCityImg = function (NewImage, id) {
            $rootScope.EditOverlay = true;
            CityServices.ChangeImage(NewImage.files[0], id)
        };

        $scope.$on('EditCityValidationSummery', function (event, data) {
            console.log(data.data);
            //to be fill
        });

        $scope.$on('EditCityUnknownError', function (event) {
            //$scope.ForignKeyErrorBody = 'Unknown error prevent removing place. Contact site developer to get more information.'
            //$scope.DelSubsBtn = "hidden";
            //$('#ForignKeyErrorModal').modal('show');

        });
        $scope.$on('cityAdded', function (event) {
            $scope.NewCity = {
                CityName: '',
                CityDesc: ''
            };
            $scope.NewCityForm.$setPristine();
            $scope.NewCityForm.$setUntouched();
            $scope.NewCityForm.$submitted = false;
            $scope.$broadcast('UpdateCities', {});
        });
        $scope.RemoveCity = function (CityID, CityName) {
            CityServices.RemoveCity(CityID, CityName);
        };
        $scope.$on('UpdateCities', function (event) {
            $scope.cities = CityServices.Get(0);
            $scope.AllCities = CityServices.All();
            scroll("#Cities");
        });
        $scope.$on('CityForignKeyError', function (event, args) {
            $scope.ForignKeyErrorBody = 'This city (<span class="text-danger">' + args.CityName + '</span>) has one or more places.<br />To remove this city, first you have to delete it\'s places.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });
        $scope.$on('CityUnknownError', function (event) {
            $scope.ForignKeyErrorBody = 'Unknown error prevent removing city. Contact site developer to get more information.'
            $scope.DelSubsBtn = "hidden";
            $('#ForignKeyErrorModal').modal('show');
        });



        // Amir section
        //Package stuff
        //$scope.PackageNameValidator = "hidden";
        $scope.PackagePagesLen;
        $scope.PackageCurrentPage;
        var AllPackages = [];
        $scope.packages = [];
        var setPckagesPagingInfo = function () {
            $scope.PackagePagesLen = Math.floor(AllPackages.length / paging);
            if ((AllPackages.length / paging > $scope.PackagePagesLen))
                $scope.PackagePagesLen++;
        };

        $scope.NextPackage = function () {
            if ($scope.PackagePagesLen - $scope.PackageCurrentPage > 1) {
                $scope.packages = angular.copy(AllPackages.slice(($scope.PlaceCPackageCurrentPageurrentPage + 1) * paging, ($scope.PlacePackageCurrentPageCurrentPage + 2) * paging));
                $scope.PackageCurrentPage++;
            }
        };
        $scope.PreviousPackage = function () {
            if ($scope.PackageCurrentPage > 0) {
                $scope.packages = angular.copy(AllPackages.slice(($scope.PackageCurrentPage - 1) * paging, $scope.PackageCurrentPage * paging));
                $scope.PackageCurrentPage--;
            }
        };
        var getPackages = function () {
            PackageServices.All()
                .then(function (response) {
                    AllPackages = response.data;
                    $scope.packages = AllPackages.slice(0, paging);
                    setPckagesPagingInfo();
                }, function (response) {
                    console.error("Request failed:", response.status);
                });
        };
        var resetAddPackageForm = function () {
            $scope.NewPackage = {
                PackageName: '',
                PackagePrice: ''
            };
            for (var i = 0; i < $scope.cities.length; i++)
                $scope.cities[i].isSelected = false;
            $scope.PackageForm.$setPristine();
            $scope.PackageForm.$setUntouched();
            $scope.PackageForm.$submitted = false;
            getPackages();
            scroll("#Packages");
        };

        //$scope.RemovePackage = function (PackageID, PackageName) {
        //    PackageServices.RemovePackage(PackageID, PackageName)
        //      .then(function (response) {
        //          switch (response.data.status) {
        //              case respondstatus.success:
        //                  getPackages();
        //                  scroll("#Packages");
        //                  break;
        //              case respondstatus.forignKeyError:
        //                  console.error("forign key error.")
        //                  break;
        //              default:
        //                  console.error("Server failed to remove Package.");
        //                  break;
        //          }
        //      }, function (response) {
        //          console.error("Server failed to add Package.");
        //      });
        //};

        $scope.AddPackage = function (NewPackage, form) {

            if (form.$invalid) {
                console.log("invalid form");
            }
            var cities = [];
            for (var i = 0; i < $scope.cities.length; i++) {
                if ($scope.cities[i].isSelected) {
                    cities.push($scope.cities[i].CityID);
                }
            }
            if (cities.length) {
                $scope.selectCityInvalid = false;
                NewPackage.Cities = cities;
                $scope.AddPakageLoading = true;
                PackageServices.AddPackage(NewPackage)
                  .then(function (response) {
                      $scope.AddPakageLoading = false;
                      switch (response.data.status) {
                          case respondstatus.success:
                              resetAddPackageForm();
                              break;
                          default:
                              alert(response.status, "    message::::", response.content);
                              break;
                      }
                  }, function (response) {
                      $scope.AddPakageLoading = false;
                      alert(response.status, "    message::::", response.content);
                      console.log("Request failed");
                      console.log("status:" + response.status);
                  });
            }
            else
                $scope.selectCityInvalid = true;

        };
        $scope.selectCityInvalid = true;
        $scope.pkgcitiesTouched = false;
        $scope.checkSelectedCities = function () {
            $scope.pkgcitiesTouched = true;
            for (var i = 0; i < $scope.cities.length; i++)
                if ($scope.cities[i].isSelected) {
                    $scope.selectCityInvalid = false;
                    return true;
                }
            $scope.selectCityInvalid = true;
            return false;
        };
        $scope.choosInputClass = function (invalid, touched, submitted) {
            return (invalid && (touched || submitted)) ? "sorooshInvalid" : "";
        };
        $scope.selectAllCities = function (isSelected) {
            $scope.pkgcitiesTouched = true;
            $scope.selectCityInvalid = !isSelected;
            for (var i = 0; i < $scope.cities.length; i++)
                $scope.cities[i].isSelected = isSelected;
        };

        getPackages();
    }]);

