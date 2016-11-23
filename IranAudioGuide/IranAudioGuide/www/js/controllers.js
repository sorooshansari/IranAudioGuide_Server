angular.module('app.controllers', [])
.controller('primaryPageCtrl', function ($scope, $rootScope, $ionicPlatform, ApiServices, dbServices, FileServices, $ionicHistory, $state) {
    console.log("Primary Page");
    var start = 0, SplashTime = 3000;
    var updateNumber = 0;

    $ionicPlatform.ready(function () {
        AutenticateAndLoadData();
    });

    var fillMenu = function () {
        //var user = window.localStorage.getItem("userInfo");
        $rootScope.$apply(function () {
            $rootScope.Authenticated = window.localStorage.getItem("Authenticated") || false;
            $rootScope.Skipped = window.localStorage.getItem("Skipped") || false;
            $rootScope.User_Img = window.localStorage.getItem("User_Img");
            $rootScope.User_Name = window.localStorage.getItem("User_Name");
            $rootScope.User_GoogleId = window.localStorage.getItem("User_GoogleId") || '';
            $rootScope.User_Email = window.localStorage.getItem("User_Email");
        });
    };

    var AutenticateAndLoadData = function () {
        start = new Date().getTime();
        var Skipped = window.localStorage.getItem("Skipped") || false;
        var Authenticated = window.localStorage.getItem("Authenticated") || false;
        console.log(Skipped);
        console.log(Authenticated);
        if (!Authenticated && !Skipped) {
            $ionicHistory.nextViewOptions({
                disableBack: true
            });
            $state.go('firstPage');
        }
        else {
            fillMenu();
            //navigator.splashscreen.show();
            $rootScope.waitingUpdates = -1;
            dbServices.openDB();
            var LstUpdtNum = window.localStorage.getItem("LastUpdateNumber") || 0;
            if (LstUpdtNum === 0) {
                FileServices.createDirs();
                dbServices.initiate();
                var networkState = navigator.connection.type;
                while (networkState === Connection.NONE)
                    alert('check internet connection');
                //$cordovaDialogs.alert('check your internet connection and try again.', 'Network error', 'Try again')
                ApiServices.GetAll(0);
            }
            else {
                if (navigator.connection.type === Connection.NONE)
                    GoHome();
                else
                    ApiServices.GetAll(LstUpdtNum);
            }
        }
    };

    $scope.$on('$ionicView.enter', function () {
        // code to run each time view is entered
        console.log("tester");
        AutenticateAndLoadData();
    });
    $rootScope.$on('PopulateTables', function (event, Data) {
        console.log(Data);
        updateNumber = Data.Data.UpdateNumber;
        dbServices.populatePlaces(Data.Data.Places);
        dbServices.populateAudios(Data.Data.Audios);
        dbServices.populateStories(Data.Data.Stories);
        dbServices.populateImages(Data.Data.Images);
        dbServices.populateTips(Data.Data.Tips);
        dbServices.populateCities(Data.Data.Cities);
        dbServices.populateTipCategories(Data.Data.TipCategories);
    });
    var DelRemovedEntities = function (RemovedEntries) {
        dbServices.deleteFromTable('Places', 'Pla_Id', RemovedEntries.Places);
        dbServices.deleteFromTable('Audios', 'Aud_Id', RemovedEntries.Audios);
        dbServices.deleteFromTable('Stories', 'Sto_Id', RemovedEntries.Stories);
        dbServices.deleteFromTable('Images', 'Img_Id', RemovedEntries.Images);
        dbServices.deleteFromTable('Tips', 'Tip_Id', RemovedEntries.Tips);
        dbServices.deleteFromTable('Cities', 'Cit_Id', RemovedEntries.Cities);
    };
    $rootScope.$on('UpdateTables', function (event, Data) {
        console.log(Data);
        updateNumber = Data.Data.UpdateNumber;
        dbServices.populatePlaces(Data.Data.Places);
        dbServices.populateAudios(Data.Data.Audios);
        dbServices.populateStories(Data.Data.Stories);
        dbServices.populateImages(Data.Data.Images);
        dbServices.populateTips(Data.Data.Tips);
        dbServices.populateCities(Data.Data.Cities);
        DelRemovedEntities(Data.Data.RemovedEntries);
    });
    $rootScope.$on('CheckWaitingUpdates', function (event) {
        if ($rootScope.waitingUpdates === 0) {
            window.localStorage.setItem("LastUpdateNumber", updateNumber);
            GoHome();
        }
    });
    $rootScope.$on('ServerConnFailed', function (event, error) {
        console.log(error);
        alert("Cannot connect to server. Pleas check your internet connection and try again.");
        var LstUpdtNum = window.localStorage.getItem("LastUpdateNumber") || 0;
        if (LstUpdtNum === 0)
            ApiServices.GetAll(LstUpdtNum);
        else
            GoHome();
        //$cordovaDialogs.alert("Couldn’t connect to server, check your internet connection and try again.", 'Network error', 'Try again');
    });
    var GoHome = function () {
        var end = new Date().getTime();
        var time = end - start;
        if (time < SplashTime)
            setTimeout(function () {
                $state.go('tabsController.home');
                navigator.splashscreen.hide();
            }, SplashTime - time);
        else {
            $state.go('tabsController.home');
            navigator.splashscreen.hide();
        }
    };
})
.controller('firstPageCtrl', function ($scope, $rootScope, $state, $ionicHistory, $ionicLoading) {
    $rootScope.$on('loadProfilePicCommpleted', function (event) {
        $ionicLoading.hide();
        console.log("loadProfilePicCommpleted");
        $ionicHistory.nextViewOptions({
            disableBack: true
        });
        console.log("Go to Primary Page");
        $state.go('primaryPage', null, { reload: true });
    });
    $rootScope.$on('loadProfilePicFailed', function (event) {
        $ionicLoading.hide();
        console.log("loadProfilePicFailed");
        window.localStorage.setItem("User_Img", 'img/defaultProfile.png');
        $ionicHistory.nextViewOptions({
            disableBack: true
        });
        alert("loading profile pic failed.")
        console.log("Go to Primary Page");
        $state.go('primaryPage', null, { reload: true });
    });
})

.controller('secondPageCtrl', function ($scope) {

})

.controller('loginCtrl', function ($scope, AuthServices, $ionicLoading) {
    $scope.logIn = function (user) {
        $ionicLoading.show({
            template: 'Loading...'
        });
        AuthServices.logIn(user.email, user.password, device.uuid);
    }
})

.controller('signupCtrl', function ($scope, AuthServices, $ionicLoading, $ionicPlatform) {
    $scope.Register = function (user) {
        $ionicLoading.show({
            template: 'Loading...'
        });
        AuthServices.Register(user.email, user.password, device.uuid);
    }
})

.controller('recoverPasswordCtrl', function ($scope) {

})
.controller('homeCtrl', function ($scope, SlideShows, Places, $cordovaFile) {
    $scope.PageTitle = 'Iran Audio Guide'
    $scope.SlideShows = SlideShows.all();
    $scope.Places = Places.all();

    //$scope.insert = function (firstname, lastname) {
    //    var query = "INSERT INTO people (firstname, lastname) VALUES (?,?)";
    //    $cordovaSQLite.execute(db, query, [firstname, lastname]).then(function (result) {
    //        console.log("INSERT ID -> " + result.insertId);
    //    }, function (error) {
    //        console.error(error);
    //    });
    //};

    //$scope.select = function (lastname) {
    //    var query = "SELECT firstname, lastname FROM people WHERE lastname = ?";
    //    $cordovaSQLite.execute(db, query, [lastname]).then(function (result) {
    //        if (result.rows.length > 0) {
    //            console.log("SELECTED -> " + result.rows.item(0).firstname + " " + result.rows.item(0).lastname);
    //        } else {
    //            console.log("NO ROWS EXIST");
    //        }
    //    }, function (error) {
    //        console.error(error);
    //    });
    //};
})
.controller('favoritsCtrl', function ($scope, Places) {
    $scope.PageTitle = "Favorits"
    $scope.Places = Places.range(2, 5);

})
.controller('packagesCtrl', function ($rootScope, $ionicHistory) {
    console.log("soroosh");
    goBack = function () {
        $ionicHistory.backView();
    }
})
.controller('searchCtrl', function ($scope, dbServices) {
    $scope.Cities = [{ Cit_Id: "0", Cit_Name: "All" }];
    var AllPlaces = [];
    var SelectedCityId = 0;
    document.addEventListener('deviceready', function () {
        $scope.selectedPlaces = [];
        dbServices.LoadAllCities();
        dbServices.LoadAllPlaces();
        $scope.PageTitle = "Search"
        $scope.Clear = function () {
            console.log('clear');
        }
        $scope.CitySelected = function (id) {
            SelectedCityId = id;
            var tempList = [];
            for (var i = 0; i < AllPlaces.length; i++)
                if (id === 0 || AllPlaces[i].CityId === id)
                    tempList.push(AllPlaces[i]);
            $scope.selectedPlaces = tempList;
        };
        $scope.searchPlaces = function (word) {
            $scope.selectedPlaces = [];
            if (word !== '') {
                var tempList = [];
                for (var i = 0; i < AllPlaces.length; i++) {
                    if (AllPlaces[i].name.indexOf(word) > -1 &&
                        (SelectedCityId === 0 || AllPlaces[i].CityId === SelectedCityId))
                        tempList.push(AllPlaces[i]);
                    $scope.selectedPlaces = tempList;
                }
            }
            else {
                for (var i = 0; i < AllPlaces.length; i++)
                    if (SelectedCityId === 0 || AllPlaces[i].CityId === SelectedCityId)
                        tempList.push(AllPlaces[i]);
                $scope.selectedPlaces = tempList;
            }
        };
        $scope.$on('FillCities', function (event, result) {
            var res = result.result.rows;
            for (var i = 0; i < res.length; i++) {
                $scope.Cities.push({
                    Cit_Id: res.item(i).Cit_Id,
                    Cit_Name: res.item(i).Cit_Name
                });
            }
        });

        $scope.$on('FillPlaces', function (event, result) {
            AllPlaces = [];
            var res = result.result.rows;
            for (var i = 0; i < res.length; i++) {
                AllPlaces.push({
                    Id: res.item(i).Pla_Id,
                    name: res.item(i).Pla_Name,
                    logo: cordova.file.dataDirectory + "/TumbNameil_dir/" + res.item(i).Pla_TNImgUrl,
                    address: res.item(i).Pla_address,
                    city: res.item(i).Cit_Name,
                    CityId: res.item(i).Pla_CityId
                });
            }
            $scope.selectedPlaces = AllPlaces;
        });
    });
})

.controller('placeCtrl', function ($scope, dbServices, FileServices, AudioServices, $timeout, $rootScope, $ionicLoading, $stateParams, $ionicSlideBoxDelegate) {
    var defaultImageSource = 'img/default-thumbnail.jpg';
    $scope.percentClasspercentClass = function (percent) {
        return 'p' + percent.toString();
    };
    $scope.$on("$ionicView.beforeEnter", function (event, data) {
        // handle event
        //$ionicLoading.show({
        //    template: 'Loading...'
        //});
        //$scope.ShowContent = false;

    });
    $scope.$on("$ionicView.afterEnter", function (event, data) {
        //$scope.ShowContent = true;
        //$ionicLoading.hide();
    });

    $scope.ModifyText = function (str) {
        str = str || "";
        return str.replace("\r\n", "<br />");
    }
    $scope.ChooseClass = function (isDownloaded, isPlaying) {
        if (!isDownloaded)
            return 'ion-android-download';
        if (isPlaying)
            return 'ion-pause';
        return 'ion-play';
    };
    $scope.slideHasChanged = function (index) {
        var itemIdx = index % $scope.PlaceInfo.ExtraImages.length;
        $scope.ExtraImageMarkDown = $scope.PlaceInfo.ExtraImages[itemIdx].description;
    }


    $scope.PlaceInfo = {
        Audios: [],
        Stories: [],
        ExtraImages: [],
        Tips: []
    };
    var placeId = $stateParams.id;

    //loading place basic infoes
    $scope.PlaceInfo.PlaceId = placeId;
    dbServices.LoadPlaceInfos(placeId)
        .then(function (d) {
            var res = d.rows.item(0);
            $scope.pageTitle = res.Pla_Name;
            $timeout(function () {
                $scope.pageTitle = res.Pla_Name;
            }, 500);
            if (res.Pla_Dirty_imgUrl) {
                $scope.PlaceInfo.PlaceImage = defaultImageSource;
                FileServices.DownloadPlaceImage(res.Pla_imgUrl, placeId)
                    .then(function (result) {// Success!
                        dbServices.CleanPlaceImage(placeId);
                        $scope.PlaceInfo.PlaceImage = cordova.file.dataDirectory + "/PlacePic_dir/" + res.Pla_imgUrl;
                    }, function (err) {// Error
                        console.log(err);
                    }, function (progress) {
                        //$timeout(function () {
                        //    $scope.downloadProgress = (progress.loaded / progress.total) * 100;
                        //});
                    });
            }
            else
                $scope.PlaceInfo.PlaceImage = cordova.file.dataDirectory + "/PlacePic_dir/" + res.Pla_imgUrl;

            $scope.PlaceInfo.PlaceName = res.Pla_Name;
            $scope.PlaceInfo.PlaceDescription = res.Pla_desc;
            $scope.PlaceInfo.PlaceCordinateX = res.Pla_c_x;
            $scope.PlaceInfo.PlaceCordinateY = res.Pla_c_y;
            $scope.PlaceInfo.Placeaddress = res.Pla_address;
            $scope.PlaceInfo.PlaceCityId = res.Pla_CityId;
        }, function (error) {
            console.error(error);
        });

    //loading place audios
    dbServices.LoadPlaceAudios(placeId)
        .then(function (Data) {
            var res = Data.rows;
            for (var i = 0; i < res.length; i++) {
                $scope.PlaceInfo.Audios.push({
                    Id: res.item(i).Aud_Id,
                    Name: res.item(i).Aud_Name,
                    url: res.item(i).Aud_Url,
                    description: res.item(i).Aud_desc,
                    downloaded: !res.item(i).Aud_Dirty,
                    downloadProgress: 0,
                    downloading: false,
                    playing: false
                });
            }
        }, function (error) {
            console.error(error);
        });

    //loading place Stories
    dbServices.LoadPlaceStories(placeId)
        .then(function (Data) {
            var res = Data.rows;
            for (var i = 0; i < res.length; i++) {
                $scope.PlaceInfo.Stories.push({
                    Id: res.item(i).Sto_Id,
                    Name: res.item(i).Sto_Name,
                    url: res.item(i).Sto_Url,
                    description: res.item(i).Sto_desc,
                    downloaded: !res.item(i).Sto_Dirty
                });
            }
        }, function (error) {
            console.error(error);
        });

    //loading place extra images
    dbServices.LoadPlaceImages(placeId)
        .then(function (Data) {
            var res = Data.rows;
            for (var i = 0; i < res.length; i++) {
                if (res.item(i).Img_Dirty) {
                    FileServices.DownloadExtraImage(res.item(i).Img_Url, res.item(i).Img_Id, res.item(i).Img_desc);
                }
                else {
                    $scope.PlaceInfo.ExtraImages.push({
                        Id: res.item(i).Img_Id,
                        description: res.item(i).Img_desc,
                        path: cordova.file.dataDirectory + "Extras_dir/" + res.item(i).Img_Url
                    });
                    $ionicSlideBoxDelegate.update();
                }

                if ($scope.PlaceInfo.ExtraImages.length === 1)
                    $scope.ExtraImageMarkDown = $scope.PlaceInfo.ExtraImages[0].description;
            }
            //$ionicSlideBoxDelegate.update()
        }, function (error) {
            console.error(error);
        });
    $scope.$on('PlaceExtraImageDownloaded', function (event, Data) {
        dbServices.CleanPlaceExtraImage(Data.Img_Id);
        $scope.PlaceInfo.ExtraImages.push({
            Id: Data.Img_Id,
            description: Data.Img_desc,
            path: Data.Img_Url
        });
        if ($scope.PlaceInfo.ExtraImages.length === 1)
            $scope.ExtraImageMarkDown = $scope.PlaceInfo.ExtraImages[0].description;
        $ionicSlideBoxDelegate.update();
    });

    //loading place Tips
    dbServices.LoadPlaceTips($scope.PlaceInfo.PlaceId)
    .then(function (Data) {
        var res = Data.rows;
        for (var i = 0; i < res.length; i++) {
            $scope.PlaceInfo.Tips.push({
                Id: res.item(i).Tip_Id,
                Class: res.item(i).TiC_Class,
                Content: res.item(i).Tip_Contetnt
            });
        }
    }, function (error) {
        console.error(error);
    });

    $scope.audioClicked = function (idx) {
        var audio = $scope.PlaceInfo.Audios[idx];
        if (!audio.downloaded) {
            downloadAudio(idx);
        }
    }
    var downloadAudio = function (idx) {
        $scope.PlaceInfo.Audios[idx].downloading = true;
        var audio = $scope.PlaceInfo.Audios[idx];
        FileServices.DownloadAudio(audio.url)
            .then(function (result) {// Success!
                dbServices.CleanAudio(audio.Id);
                $scope.PlaceInfo.Audios[idx].downloading = false;
                $scope.PlaceInfo.Audios[idx].downloaded = true;
            }, function (err) {// Error
                console.log(err);
            }, function (progress) {
                $timeout(function () {
                    $scope.PlaceInfo.Audios[idx].downloadProgress = 
                        Math.floor((progress.loaded / progress.total) * 100);
                });
            });
    }


    $scope.Audios = AudioServices.all();
    var playNewAudio = function (audio) {
        var audioPath = "file:///android_asset/www/audio/" + audio.URL;
        $rootScope.audio.media = new Media(audioPath, null, null, mediaStatusCallback);
        $rootScope.audio.index = audio.index;
        $rootScope.audio.title = audio.title;
        $rootScope.audio.media.play();
    }
    $scope.playPause = function (audio) {
        if ($rootScope.audio.media === null) { //No audio loaded yet
            playNewAudio(audio);
            audio.icoStatus = 'pause';
        }
        else if ($rootScope.audio.index !== audio.index) { //another audio is playing, so first pause the playing one
            $rootScope.audio.media.release();
            $scope.Audios[$rootScope.audio.index].icoStatus = 'play';
            playNewAudio(audio);
            audio.icoStatus = 'pause';
        }
        else if (audio.icoStatus === 'pause') {//same audio is playing
            $rootScope.audio.media.pause();
            audio.icoStatus = 'play';
        }
        else { //play paused audio
            $rootScope.audio.media.play();
            audio.icoStatus = 'pause';
        }
    }

    var iOSPlayOptions = {
        numberOfLoops: 1,
        playAudioWhenScreenIsLocked: true
    }
    var mediaStatusCallback = function (status) {
        if (status === 1) {
            $ionicLoading.show({ template: 'Loading...' });
        } else {
            $ionicLoading.hide();
        }
    }
    //var play = function (id) {
    //    //$cordovaMedia.play(media);
    //    media.play();
    //    $rootScope.AudioPlayed = true;
    //    document.getElementById("i-" + id).classList.remove('ion-play');
    //    document.getElementById("i-" + id).classList.add('ion-pause');
    //}
    //var pause = function (id) {
    //    //$cordovaMedia.pause(media);
    //    media.pause();
    //    $rootScope.AudioPlayed = false;
    //    document.getElementById("i-" + id).classList.remove('ion-pause');
    //    document.getElementById("i-" + id).classList.add('ion-play');
    //}
});