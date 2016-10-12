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
        $rootScope.User_Img = window.localStorage.getItem("User_Img");
        $rootScope.User_Name = window.localStorage.getItem("User_Name");
        $rootScope.User_Email = window.localStorage.getItem("User_Email");
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
            navigator.splashscreen.show();
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
    }
    $scope.$on('$ionicView.enter', function () {
        // code to run each time view is entered
        console.log("tester");
        AutenticateAndLoadData();
    });
    $rootScope.$on('PopulateTables', function (event, Data) {
        console.log(Data);
        dbServices.populatePlaces(Data.Data.Places);
        dbServices.populateAudios(Data.Data.Audios);
        dbServices.populateCities(Data.Data.Cities);
        dbServices.populateImages(Data.Data.Images);
        updateNumber = Data.Data.UpdateNumber;
    });
    $rootScope.$on('CheckWaitingUpdates', function (event) {
        if ($rootScope.waitingUpdates == 0) {
            window.localStorage.setItem("LastUpdateNumber", updateNumber);
            GoHome();
        }
    });
    //var waitForUpdate = function (updateNumber) {
    //    if ($rootScope.waitingUpdates == 0) {
    //        window.localStorage.setItem("LastUpdateNumber", updateNumber);
    //        GoHome();
    //    }
    //    else
    //        setTimeout(waitForUpdate, 100, updateNumber);
    //};
    $rootScope.$on('ServerConnFailed', function (event, error) {
        console.log(error);
        alert("try again.");
        var LstUpdtNum = window.localStorage.getItem("LastUpdateNumber") || 0;
        if (LstUpdtNum == 0)
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
    }
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
                if (id == 0 || AllPlaces[i].CityId == id)
                    tempList.push(AllPlaces[i]);
            $scope.selectedPlaces = tempList;
        };
        $scope.searchPlaces = function (word) {
            $scope.selectedPlaces = [];
            if (word != '') {
                var tempList = [];
                for (var i = 0; i < AllPlaces.length; i++) {
                    if (AllPlaces[i].name.indexOf(word) > -1 &&
                        (SelectedCityId == 0 || AllPlaces[i].CityId == SelectedCityId))
                        tempList.push(AllPlaces[i]);
                    $scope.selectedPlaces = tempList;
                }
            }
            else {
                for (var i = 0; i < AllPlaces.length; i++)
                    if (SelectedCityId == 0 || AllPlaces[i].CityId == SelectedCityId)
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

.controller('palaceCtrl', function ($scope, AudioServices, $rootScope, $ionicLoading, $stateParams) {
    console.log($stateParams.id);
    $scope.PageTitle = "Tomb of Hafez"

    $scope.Audios = AudioServices.all();
    var playNewAudio = function (audio) {
        var audioPath = "file:///android_asset/www/audio/" + audio.URL;
        $rootScope.audio.media = new Media(audioPath, null, null, mediaStatusCallback);
        $rootScope.audio.index = audio.index;
        $rootScope.audio.title = audio.title;
        $rootScope.audio.media.play();
    }
    $scope.playPause = function (audio) {
        if ($rootScope.audio.media == null) { //No audio loaded yet
            playNewAudio(audio);
            audio.icoStatus = 'pause';
        }
        else if ($rootScope.audio.index != audio.index) { //another audio is playing, so first pause the playing one
            $rootScope.audio.media.release();
            $scope.Audios[$rootScope.audio.index].icoStatus = 'play';
            playNewAudio(audio);
            audio.icoStatus = 'pause';
        }
        else if (audio.icoStatus == 'pause') {//same audio is playing
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
        if (status == 1) {
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