angular.module('app.controllers', [])
.controller('primaryPageCtrl', function ($scope, $rootScope, $ionicPlatform, ApiServices, dbServices, FileServices, $ionicHistory, $state) {
    console.log("Primary Page");
    var start = 0, SplashTime = 1000;
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
            if (LstUpdtNum == 0) {
                FileServices.createDirs();
                dbServices.initiate();
                var networkState = navigator.connection.type;
                while (networkState == Connection.NONE)
                    alert('check internet connection');
                //$cordovaDialogs.alert('check your internet connection and try again.', 'Network error', 'Try again')
                ApiServices.GetAll(0);
            }
            else {
                if (navigator.connection.type == Connection.NONE)
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
        console.log("RemovedEntries.Places: ", RemovedEntries.Places);
        dbServices.deleteFromTable('Places', 'Pla_Id', RemovedEntries.Places);
        dbServices.removeAudioFiles(RemovedEntries.Audios);
        dbServices.deleteFromTable('Audios', 'Aud_Id', RemovedEntries.Audios);
        dbServices.removeStoryFiles(RemovedEntries.Stories);
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
        console.log("waitingUpdates: ", $rootScope.waitingUpdates);
        if ($rootScope.waitingUpdates == 0) {
            window.localStorage.setItem("LastUpdateNumber", updateNumber);
            GoHome();
        }
    });
    $rootScope.$on('ServerConnFailed', function (event, error) {
        console.log(error);
        alert("Cannot connect to server. Pleas check your internet connection and try again.");
        var LstUpdtNum = window.localStorage.getItem("LastUpdateNumber") || 0;
        if (LstUpdtNum == 0)
            ApiServices.GetAll(LstUpdtNum);
        else
            GoHome();
        //$cordovaDialogs.alert("Couldn’t connect to server, check your internet connection and try again.", 'Network error', 'Try again');
    });
    var GoHome = function () {
        dbServices.LoadTopPlayerHistory();
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
        AuthServices.Register(user.email, user.password, device.uuid)
        .then(function (data) {
            console.log(data);
            switch (data.data) {
                case 0: { //success
                    window.localStorage.setItem("User_Email", email);
                    window.localStorage.setItem("Skipped", false);
                    window.localStorage.setItem("Authenticated", true);
                    $rootScope.$broadcast('LoadDefaultUser', {});
                    break;
                }
                case 1: { //userExists
                    $ionicLoading.hide();
                    alert("This email is already regestered. Please go to log in.");
                    $state.go('login');
                    break;
                }
                case 2: { //fail
                    $ionicLoading.hide();
                    alert("Connecting to server failed.");
                    break;
                }
                case 3: { //uuidMissMatch
                    $ionicLoading.hide();
                    alert("This email registered on another device, try another account.");
                    break;
                }
                case 4: { //googleUser
                    $ionicLoading.hide();
                    alert("you allready registered with google. Please sign in with it.");
                    AuthServices.Google(device.uuid);
                    break;
                }
                default:
            }
        });
    }
})

.controller('recoverPasswordCtrl', function ($scope) {

})
.controller('homeCtrl', function ($scope, dbServices, $ionicPlatform, $ionicLoading, $ionicSlideBoxDelegate) {
    $scope.PageTitle = 'Iran Audio Guide'
    $scope.SlideShows = {};
    $scope.Places = {};
    $ionicLoading.show({
        template: 'Loading...'
    });
    $ionicPlatform.ready(function () {
        dbServices.LoadPrimaryPlaces();
    });
    $scope.$on("$ionicView.beforeEnter", function (event, data) {

    });
    $(".rslides").responsiveSlides({
        auto: true,             // Boolean: Animate automatically, true or false
        speed: 1000,            // Integer: Speed of the transition, in milliseconds
        timeout: 5000,          // Integer: Time between slide transitions, in milliseconds
        //pager: false,           // Boolean: Show pager, true or false
        //nav: false,             // Boolean: Show navigation, true or false
        //random: false,          // Boolean: Randomize the order of the slides, true or false
        //pause: false,           // Boolean: Pause on hover, true or false
        //pauseControls: true,    // Boolean: Pause when hovering controls, true or false
        //prevText: "Previous",   // String: Text for the "previous" button
        //nextText: "Next",       // String: Text for the "next" button
        //maxwidth: "",           // Integer: Max-width of the slideshow, in pixels
        //navContainer: "",       // Selector: Where controls should be appended to, default is after the 'ul'
        //manualControls: "",     // Selector: Declare custom pager navigation
        namespace: "rslides"   // String: Change the default namespace used
        //before: function () { },   // Function: Before callback
        //after: function () { }     // Function: After callback
    });
    $scope.$on("PrimaryPlacesLoaded", function (event, data) {
        var AllPlaces = [];
        
        console.log("data", data);
        var res = data.result.rows;
        for (var i = 0; i < res.length; i++) {
            AllPlaces.push({
                Id: res.item(i).Pla_Id,
                name: res.item(i).Pla_Name,
                logo: cordova.file.dataDirectory + "/TumbNameil_dir/" + res.item(i).Pla_TNImgUrl,
                address: res.item(i).Pla_address,
                city: res.item(i).Cit_Name,
                CityId: res.item(i).Pla_CityId,
                bookmarked: res.item(i).Pla_bookmarked
            });
            //SlideShows.push({
            //    id: res.item(i).Pla_Id,
            //    title: res.item(i).Pla_Name,
            //    URL: cordova.file.dataDirectory + "/PlacePic_dir/" + res.item(i).Pla_imgUrl
            //});
        }
        //$scope.SlideShows = angular.copy(SlideShows);

        //$ionicSlideBoxDelegate.update();
        $scope.Places = angular.copy(AllPlaces);
        $ionicLoading.hide();
        //console.log($scope.SlideShows);
    });
    $scope.bookmark = function (placeId) {
        dbServices.bookmarkePlace(placeId);
    };
    $scope.$on("PlaceBookmarked", function (event, data) {
        for (var i = 0; i < $scope.Places.length; i++) {
            if ($scope.Places[i].Id == data.placeId) {
                $scope.Places[i].bookmarked = 1;
                break;
            }
        }
    });
    $scope.$on("PlaceUnbookmarked", function (event, data) {
        for (var i = 0; i < $scope.Places.length; i++) {
            if ($scope.Places[i].Id == data.placeId) {
                $scope.Places[i].bookmarked = 0;
                break;
            }
        }
    });
})
.controller('favoritsCtrl', function ($scope, dbServices) {
    $scope.PageTitle = "Bookmarks"
    //$scope.$on("$ionicView.beforeEnter", function (event, data) {
    //    updatePlaces();
    //});
    //var updatePlaces = function () {
    //    dbServices.LoadBookmarkedPlaces();
    //};
    dbServices.LoadBookmarkedPlaces();
    $scope.$on("BookmarkedPlacesLoaded", function (event, data) {
        AllPlaces = [];
        var res = data.result.rows;
        for (var i = 0; i < res.length; i++) {
            AllPlaces.push({
                Id: res.item(i).Pla_Id,
                name: res.item(i).Pla_Name,
                logo: cordova.file.dataDirectory + "/TumbNameil_dir/" + res.item(i).Pla_TNImgUrl,
                address: res.item(i).Pla_address,
                city: res.item(i).Cit_Name,
                CityId: res.item(i).Pla_CityId,
                bookmarked: res.item(i).Pla_bookmarked
            });
        }
        $scope.Places = AllPlaces;
    });
    $scope.defavorito = function (placeId) {
        dbServices.unbookmarkePlace(placeId);
    };
    $scope.$on("PlaceUnbookmarked", function (event, data) {
        console.log(data);
        for (var i = 0; i < $scope.Places.length; i++) {
            if ($scope.Places[i].Id == data.placeId) {
                $scope.Places.splice(i, 1);
                $scope.$apply();
                break;
            }
        }
    });
    $scope.$on("PlaceBookmarked", function (event, data) {
        dbServices.LoadBookmarkedPlaces();
        $scope.$apply();
    });
})
.controller('packagesCtrl', function ($rootScope, $ionicHistory) {
    console.log("soroosh");
    goBack = function () {
        $ionicHistory.backView();
    }
})
.controller('searchCtrl', function ($scope, dbServices) {
    $scope.Cities = [{ Cit_Id: "0", Cit_Name: "All" }];
    $scope.selectedPlaces = {};
    var AllPlaces = [];
    var SelectedCityId = 0;
    document.addEventListener('deviceready', function () {
        $scope.selectedPlaces = [];
        dbServices.LoadAllCities();
        dbServices.LoadAllPlaces();
        $scope.PageTitle = "Search"
        $scope.Clear = function () {
            $scope.search = '';
        }
        $scope.CitySelected = function (id) {
            SelectedCityId = id;
            var tempList = [];
            for (var i = 0; i < AllPlaces.length; i++)
                if (id == 0 || AllPlaces[i].CityId == id)
                    tempList.push(AllPlaces[i]);

            $scope.selectedPlaces = angular.copy(tempList);
        };
        $scope.searchPlaces = function (word) {
            var tempList = [];
            $scope.selectedPlaces = [];
            if (word !== '') {
                var re = new RegExp(word, 'i');
                for (var i = 0; i < AllPlaces.length; i++) {
                    if (re.test(AllPlaces[i].name) &&
                        (SelectedCityId == 0 || AllPlaces[i].CityId == SelectedCityId))
                        tempList.push(AllPlaces[i]);
                }
            }
            else {
                for (var j = 0; j < AllPlaces.length; j++)
                    if (SelectedCityId == 0 || AllPlaces[j].CityId == SelectedCityId)
                        tempList.push(AllPlaces[j]);
            }

            $scope.selectedPlaces = angular.copy(tempList);
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

            $scope.selectedPlaces = angular.copy(AllPlaces);
        });
    });
})
.controller('playerCtrl', function ($rootScope, $ionicHistory) {
    console.log("soroosh");

})

.controller('placeCtrl', function ($scope, dbServices, FileServices, AudioServices, player, $timeout, $rootScope, $ionicLoading, $stateParams, $ionicSlideBoxDelegate) {
    var defaultImageSource = 'img/default-thumbnail.jpg';


    $scope.PlaceInfo = {
        Audios: [],
        Stories: [],
        ExtraImages: [],
        Tips: []
    };
    var placeId = $stateParams.id;

    $scope.percentClass = function (percent) {
        return 'p' + percent.toString();
    };
    
    //player stuff
    var searchById = function (arr, id) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].Id == id)
                return i;
        }
    };
    var checkPlayer = function () {
        var info = player.info();
        if (info.hasMedia) {
            if (info.PlaceId == placeId) {
                if (info.isAudio) {
                    var idx = searchById($scope.PlaceInfo.Audios, info.trackInfo.Id);
                    $scope.PlaceInfo.Audios[idx].playing = info.playing;
                }
                else {
                    var idx = searchById($scope.PlaceInfo.Stories, info.trackInfo.Id);
                    $scope.PlaceInfo.Stories[idx].playing = info.playing;
                }
            }
        }
    };
    $scope.$on('playerUpdated', function () {
        var info = player.info();
        if (info.isAudio) {
            for (var i = 0; i < $scope.PlaceInfo.Audios.length; i++) {
                if ($scope.PlaceInfo.Audios[i].Id == info.trackInfo.Id)
                    $scope.PlaceInfo.Audios[i].playing = info.playing;
                else
                    $scope.PlaceInfo.Audios[i].playing = false;
            }
            for (var i = 0; i < $scope.PlaceInfo.Stories.length; i++)
                $scope.PlaceInfo.Stories[i].playing = false;
        }
        else {
            for (var i = 0; i < $scope.PlaceInfo.Stories.length; i++) {
                if ($scope.PlaceInfo.Stories[i].Id == info.trackInfo.Id)
                    $scope.PlaceInfo.Stories[i].playing = info.playing;
                else
                    $scope.PlaceInfo.Stories[i].playing = false;
            }
            for (var i = 0; i < $scope.PlaceInfo.Audios.length; i++)
                $scope.PlaceInfo.Audios[i].playing = false;
        }
    });
    var playPause = function (idx, isAudio) {
        var info = player.info();
        var track;
        track = (isAudio) ? $scope.PlaceInfo.Audios[idx] : $scope.PlaceInfo.Stories[idx];
        if (info.hasMedia &&
            track.Id == info.trackInfo.Id) {
            if (track.playing)
                player.pause();
            else
                player.play();
        }
        else {
            player.New(track, isAudio, idx, placeId);
            player.play();
        }
    }
    //var iOSPlayOptions = {
    //    numberOfLoops: 1,
    //    playAudioWhenScreenIsLocked: true
    //}

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


    $scope.$on("$ionicView.beforeEnter", function (event, data) {

    });
    $scope.$on("$ionicView.afterEnter", function (event, data) {
        checkPlayer();
    });
    $scope.$on("$ionicView.leave", function (event, data) {
        //freePlayer();
    });


    //loading place basic infos
    $scope.PlaceInfo.PlaceId = placeId;
    dbServices.LoadPlaceInfos(placeId);
    $scope.$on('PlaceInfoesLoaded', function (event, data) {
        var res = data.result.rows.item(0);
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
    });

    //loading place audios
    dbServices.LoadPlaceAudios(placeId);
    $scope.$on('PlaceAudiosLoaded', function (event, Data) {
        var res = Data.result.rows;
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
    });

    //loading place Stories
    dbServices.LoadPlaceStories(placeId);
    $scope.$on('PlaceStoriesLoaded', function (event, Data) {
        var res = Data.result.rows;
        for (var i = 0; i < res.length; i++) {
            $scope.PlaceInfo.Stories.push({
                Id: res.item(i).Sto_Id,
                Name: res.item(i).Sto_Name,
                url: res.item(i).Sto_Url,
                description: res.item(i).Sto_desc,
                downloaded: !res.item(i).Sto_Dirty,
                downloadProgress: 0,
                downloading: false,
                playing: false
            });
        }
    });

    //loading place extra images
    dbServices.LoadPlaceImages(placeId);
    $scope.$on('PlaceImagesLoaded', function (event, Data) {
        var res = Data.result.rows;
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

            if ($scope.PlaceInfo.ExtraImages.length == 1)
                $scope.ExtraImageMarkDown = $scope.PlaceInfo.ExtraImages[0].description;
        }
    });
    $scope.$on('PlaceExtraImageDownloaded', function (event, Data) {
        dbServices.CleanPlaceExtraImage(Data.Img_Id);
        $scope.PlaceInfo.ExtraImages.push({
            Id: Data.Img_Id,
            description: Data.Img_desc,
            path: Data.Img_Url
        });
        if ($scope.PlaceInfo.ExtraImages.length == 1)
            $scope.ExtraImageMarkDown = $scope.PlaceInfo.ExtraImages[0].description;
        $ionicSlideBoxDelegate.update();
    });

    //loading place Tips
    dbServices.LoadPlaceTips($scope.PlaceInfo.PlaceId);
    $scope.$on('PlaceTipsLoaded', function (event, Data) {
        var res = Data.result.rows;
        for (var i = 0; i < res.length; i++) {
            $scope.PlaceInfo.Tips.push({
                Id: res.item(i).Tip_Id,
                Class: res.item(i).TiC_Class,
                Content: res.item(i).Tip_Contetnt
            });
        }
    });




    //Audio stuffs
    $scope.audioClicked = function (idx) {
        var audio = $scope.PlaceInfo.Audios[idx];
        if (!audio.downloaded) {
            downloadAudio(idx);
        }
        else {
            playPause(idx, true);//isAudio = True
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
                $scope.PlaceInfo.Audios[idx].downloading = false;
                $scope.PlaceInfo.Audios[idx].downloaded = false;
            }, function (progress) {
                $timeout(function () {
                    $scope.PlaceInfo.Audios[idx].downloadProgress =
                        Math.floor((progress.loaded / progress.total) * 100);
                });
            });
    }

    //Story stuffs
    $scope.StoryClicked = function (idx) {
        var story = $scope.PlaceInfo.Stories[idx];
        if (!story.downloaded) {
            downloadStory(idx);
        }
        else {
            playPause(idx, false);//isAudio = false
        }
    }
    var downloadStory = function (idx) {
        $scope.PlaceInfo.Stories[idx].downloading = true;
        var story = $scope.PlaceInfo.Stories[idx];
        FileServices.DownloadStory(story.url)
            .then(function (result) {// Success!
                dbServices.CleanStory(story.Id);
                $scope.PlaceInfo.Stories[idx].downloading = false;
                $scope.PlaceInfo.Stories[idx].downloaded = true;
            }, function (err) {// Error
                console.log(err);
                $scope.PlaceInfo.Stories[idx].downloading = false;
                $scope.PlaceInfo.Stories[idx].downloaded = false;
            }, function (progress) {
                $timeout(function () {
                    $scope.PlaceInfo.Stories[idx].downloadProgress =
                        Math.floor((progress.loaded / progress.total) * 100);
                });
            });
    }


    //remove track
    $scope.removeTrack = function (id, idx, isAudio) {
        FileServices.RemoveTrack(id, isAudio);
        if (isAudio) {
            dbServices.DirtyAudio(id);
            $scope.PlaceInfo.Audios[idx].downloaded = false;
        }
        else {
            dbServices.DirtyStory(id);
            $scope.PlaceInfo.Stories[idx].downloaded = false;
        }
    }
});