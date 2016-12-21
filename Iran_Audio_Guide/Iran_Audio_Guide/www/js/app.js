
angular.module('app', ['ionic', 'ionic.service.core', 'app.controllers', 'app.routes', 'app.services', 'app.directives', 'ngCordova', 'ngCordovaOauth'])

.run(function ($ionicPlatform, $rootScope, $ionicLoading, $ionicHistory, $state, AuthServices, dbServices, player) {
    $ionicPlatform.ready(function () {
        //checkConnection();

        // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
        // for form inputs)
        if (window.cordova && window.cordova.plugins && window.cordova.plugins.Keyboard) {
            cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
            cordova.plugins.Keyboard.disableScroll(true);
        }
        if (window.StatusBar) {
            // org.apache.cordova.statusbar required
            StatusBar.styleDefault();
        }
        $rootScope.audio = {};
        $rootScope.audio.media = null;
        console.log(device.uuid);
    });

    //sidePlayer
    $rootScope.SPinfo = player.info();
    $rootScope.$on('playerUpdated', function () {
        $rootScope.SPinfo = player.info();
    });
    $rootScope.SPchooseClass = function (isPlaying) {
        return (isPlaying) ? 'ion-pause' : 'ion-play';
    };
    $rootScope.SPplayPause = function () {
        if ($rootScope.SPinfo.playing)
            player.pause();
        else
            player.play();
    };



    $rootScope.$on('LoadDefaultUser', function () {
        console.log("Load Default User");
        window.localStorage.setItem("User_Name", "");
        window.localStorage.setItem("User_Img", 'img/defaultProfile.png');
        $ionicHistory.nextViewOptions({
            disableBack: true
        });
        $ionicLoading.hide();
        console.log("Go to Primary Page");
        $state.go('primaryPage', null, { reload: true });
    });
    $rootScope.Skip = function () {
        console.log("Authentication skiped");
        $rootScope.$broadcast('LoadDefaultUser', {});
        window.localStorage.setItem("Authenticated", false);
        window.localStorage.setItem("Skipped", true);
        $ionicHistory.nextViewOptions({
            disableBack: true
        });
        console.log("Go to Primary Page");
        $state.go('primaryPage', null, { reload: true });
    };

    $rootScope.googleLogin = function () {
        AuthServices.Google(device.uuid);
    };
    $rootScope.ShowPackage = function () {
        $ionicHistory.nextViewOptions({
            disableBack: false
        });
        $state.go('packages');
    };
    $rootScope.$on('callDbServicesFunctions', function (event, Data) {
        switch (Data.functionName) {
            case 'CleanPlaceImage':
                dbServices.CleanPlaceImage(Data.params[0]);
                break;
            case 'CleanPlaceTumbnail':
                dbServices.CleanPlaceTumbnail(Data.params[0]);
                break;
            case 'CleanPlaceExtraImage':
                dbServices.CleanPlaceExtraImage(Data.params[0]);
                break;
            default:
                break;
        }
    });
});
function checkConnection() {
    var networkState = navigator.connection.type;

    var states = {};
    states[Connection.UNKNOWN] = 'Unknown connection';
    states[Connection.ETHERNET] = 'Ethernet connection';
    states[Connection.WIFI] = 'WiFi connection';
    states[Connection.CELL_2G] = 'Cell 2G connection';
    states[Connection.CELL_3G] = 'Cell 3G connection';
    states[Connection.CELL_4G] = 'Cell 4G connection';
    states[Connection.CELL] = 'Cell generic connection';
    states[Connection.NONE] = 'No network connection';

    alert('Connection type: ' + states[networkState]);
}