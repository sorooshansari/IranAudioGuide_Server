angular.module('app.services', [])
.factory('SlideShows', function () {
    var SlideShow = [
        {
            id: 0,
            URL: 'img/1.jpg',
            title: 'soroosh'
        },
    {
        id: 1,
        URL: 'img/2.jpg',
        title: 'ansari'
    },
    {
        id: 2,
        URL: 'img/3.jpg',
        title: 'mehr'
    },
    {
        id: 3,
        URL: 'img/6.jpg',
        title: 'Hafez'
    }];

    return {
        all: function () {
            return SlideShow;
        },
        get: function (Slide_id) {
            for (var i = 0; i < SlideShow.length; i++) {
                if (SlideShow[i].id === parseInt(Slide_id)) {
                    return SlideShow[i];
                }
            }
            return null;
        }
    };
})
.factory('Places', function () {

    var Places = [{
        id: 0,
        name: 'Eram Garden',
        city: 'Shiraz',
        address: 'Fars, Shiraz, District 1, Eram Street',
        logo: 'img/1.jpg'
    }, {
        id: 1,
        name: 'Naranjestan Qavam',
        city: 'Shiraz',
        address: 'Fars, Shiraz, Lotf Ali Khan Zand St',
        logo: 'img/2.jpg'
    }, {
        id: 2,
        name: 'Nasir ol Molk Mosque',
        city: 'Shiraz',
        address: 'Fars, Shiraz, Lotf Ali Khan Zand St',
        logo: 'img/3.jpg'
    },
    {
        id: 5,
        name: 'Tomb of hafez',
        city: 'Shiraz',
        address: 'Fars, Shiraz, District 3',
        logo: 'img/6.jpg'
    }];

    return {
        all: function () {
            return Places;
        },
        remove: function (place) {
            Places.splice(Places.indexOf(place), 1);
        },
        get: function (id_local) {
            for (var i = 0; i < Places.length; i++) {
                if (Places[i].id === parseInt(id_local)) {
                    return Places[i];
                }
            }
            return null;
        },
        range: function (from, to) {
            return Places.slice(from, to);
        }
    };
})
.factory('AudioServices', [function () {
    var Audios = [
        {
            index: 0,
            URL: '1.mp3',
            title: "Who was hafez"
        },
        {
            index: 1,
            URL: '2.mp3',
            title: "Who was Soroosh"
        }];

    return {
        all: function () {
            return Audios;
        },
        get: function (Audio_ID) {
            for (var i = 0; i < Audios.length; i++) {
                if (Audios[i].id === parseInt(Audio_ID)) {
                    return Audios[i];
                }
            }
            return null;
        }
    };
}])
.service('AuthServices', ['$http', '$rootScope', '$cordovaOauth', 'FileServices', '$ionicLoading',
    function ($http, $rootScope, $cordovaOauth, FileServices, $ionicLoading) {
        var AutenticateUser = function (user, profilePath) {
            $http({
                url: 'http://iranaudioguide.com/api/AppManager/AutenticateGoogleUser',
                method: 'POST',
                data: user
            }).then(function (data) {
                console.log(data);
                switch (data.data) {
                    case 0: {//     Creating user was successful
                        window.localStorage.setItem("Skipped", false);
                        window.localStorage.setItem("Authenticated", true);
                        window.localStorage.setItem("User_Name", user.name);
                        window.localStorage.setItem("User_Email", user.email);
                        window.localStorage.setItem("User_GoogleId", user.google_id);
                        FileServices.DownloadProfilePic(user.picture, profilePath);
                        break;
                    }
                    case 1: {//     This user was existed
                        alert("Authenticating user failed");
                        break;
                    }
                    case 2: {//     Creating user failed
                        alert("Authenticating user failed");
                        break;
                    }
                    case 3: {//     Creating user with different uuid
                        alert("You have already signed in with a different device.");
                        break;
                    }
                    default:
                }
            });
        };
        return {
            Register: function (email, password, uuid) {
                var AppUser = { email: email, password: password, uuid: uuid };
                console.log(AppUser);
                $http({
                    url: 'http://iranaudioguide.com/api/AppManager/ResgisterAppUser',
                    method: 'POST',
                    data: AppUser
                }).then(function (data) {
                    console.log(data);
                    switch (data.data) {
                        case 0: {
                            window.localStorage.setItem("User_Email", email);
                            window.localStorage.setItem("Skipped", false);
                            window.localStorage.setItem("Authenticated", true);
                            $rootScope.$broadcast('LoadDefaultUser', {});
                            break;
                        }
                        case 1: {
                            $ionicLoading.hide();
                            alert("This email is already regestered. Please go to log in.")
                            break;
                        }
                        case 2: {
                            $ionicLoading.hide();
                            alert("Connecting to server failed.")
                            break;
                        }
                        default:
                    }
                });
            },
            logIn: function (email, password, uuid) {
                var AppUser = { email: email, password: password, uuid: uuid };
                console.log(AppUser);
                $http({
                    url: 'http://iranaudioguide.com/api/AppManager/AuthorizeAppUser',
                    method: 'POST',
                    data: AppUser
                }).then(function (data) {
                    $ionicLoading.hide();
                    console.log(data);
                    switch (data.data.Result) {
                        case 0: {
                            window.localStorage.setItem("Skipped", false);
                            window.localStorage.setItem("Authenticated", true);
                            var user = data.data;
                            if (user.GoogleId !== null) {
                                window.localStorage.setItem("User_Name", user.FullName);
                                window.localStorage.setItem("User_Email", user.Email);
                                window.localStorage.setItem("User_GoogleId", user.GoogleId);
                                FileServices.DownloadProfilePic(user.Picture, user.GoogleId)
                            }
                            else {
                                window.localStorage.setItem("User_Email", email);
                                $rootScope.$broadcast('LoadDefaultUser', {});
                            }
                            break;
                        }
                        case 1: {
                            alert("LockedOut");
                            break;
                        }
                        case 2: {
                            alert("RequiresVerification");
                            break;
                        }
                        case 3: {
                            alert("Wrong username or password.");
                            break;
                        }
                        case 4: {
                            alert("You have already signed in with a different device.");
                            break;
                        }
                        default:
                    }
                },
                function (err) {
                    console.log(err);
                    alert("error");
                });
            },
            Google: function (uuid) {
                $cordovaOauth.google("751762984773-tpuqc0d67liqab0809ssvjmgl311r1is.apps.googleusercontent.com",
                    ["https://www.googleapis.com/auth/urlshortener",
                    "https://www.googleapis.com/auth/userinfo.email",
                    "https://www.googleapis.com/auth/userinfo.profile"])
                    .then(function (result) {
                        $ionicLoading.show({
                            template: 'Loading...'
                        });
                        console.log(result.access_token);
                        $http({
                            url: 'https://www.googleapis.com/oauth2/v3/userinfo',
                            method: 'GET',
                            params: {
                                access_token: result.access_token,
                                format: 'json'
                            }
                        }).then(function (user_data) {
                            console.log(user_data);
                            var profilePath = user_data.data.sub + '.jpg';
                            var user = {
                                name: user_data.data.name,
                                gender: user_data.data.gender,
                                email: user_data.data.email,
                                google_id: user_data.data.sub,
                                picture: user_data.data.picture,
                                profile: user_data.data.profile,
                                uuid: uuid
                            };
                            console.log(user);
                            AutenticateUser(user, profilePath);
                        }, function (err) {
                            alert("There was a problem getting your profile.");
                            console.log(err);
                        });
                    });
            }
        }
    }])
.service('ApiServices', ['$http', '$rootScope', function ($http, $rootScope) {
    return {
        GetAll: function (LUN) {
            if (LUN === 0) {
                method = 'post';
                url = 'http://iranaudioguide.com/api/AppManager/GetAll';
                $http({ method: method, url: url }).
                  then(function (response) {
                      console.log("getAll:");
                      console.log(response);
                      $rootScope.$broadcast('PopulateTables', { Data: response.data });
                  }, function (response) {
                      $rootScope.$broadcast('ServerConnFailed', { error: response.data });
                  });
            }
            else {
                method = 'post';
                data = { LastUpdateNumber: LUN };
                url = 'http://iranaudioguide.com/api/AppManager/GetUpdates?LastUpdateNumber=' + LUN;
                $http({ method: method, url: url, data: data }).
                  then(function (response) {
                      console.log("GetUpdates:");
                      console.log(response);
                      //var Tables = angular.copy(response.data);
                      $rootScope.$broadcast('UpdateTables', { Data: response.data });
                  }, function (response) {
                      $rootScope.$broadcast('ServerConnFailed', { error: response.data });
                  });
            }
        }
    }
}])
.service('dbServices', ['$rootScope', '$cordovaSQLite', 'FileServices', function ($rootScope, $cordovaSQLite, FileServices) {
    return {
        openDB: function () {
            db = $cordovaSQLite.openDB({ name: 'app.db', iosDatabaseLocation: 'default' });
        },
        initiate: function () {
            //if (isAndroid) {
            //    // Works on android but not in iOS
            //    _sqlLiteDB = $cordovaSQLite.openDB({ name: "app.db", iosDatabaseLocation: 'default' });
            //} else {
            //    // Works on iOS 
            //    _sqlLiteDB = window.sqlitePlugin.openDatabase({ name: "app.db", location: 2, createFromLocation: 1 });
            //}
            $cordovaSQLite.execute(db,
                "CREATE TABLE IF NOT EXISTS Places\
            (\
            Pla_Id blob PRIMARY KEY,\
            Pla_Name text,\
            Pla_imgUrl text,\
            Pla_TNImgUrl text,\
            Pla_desc text,\
            Pla_c_x real,\
            Pla_c_y real,\
            Pla_address text,\
            Pla_CityId integer,\
            Pla_Dirty_imgUrl integer,\
            Pla_Dirty_TNImgUrl integer\
            )");
            $cordovaSQLite.execute(db, "\
            CREATE TABLE IF NOT EXISTS Audios\
            (\
            Aud_Id blob PRIMARY KEY,\
            Aud_PlaceId blob,\
            Aud_Name text,\
            Aud_Url text,\
            Aud_desc text,\
            Aud_Dirty integer\
            )");
            $cordovaSQLite.execute(db, "\
            CREATE TABLE IF NOT EXISTS Stories\
            (\
            Sto_Id blob PRIMARY KEY,\
            Sto_PlaceId blob,\
            Sto_Name text,\
            Sto_Url text,\
            Sto_desc text,\
            Sto_Dirty integer\
            )");
            $cordovaSQLite.execute(db, "\
            CREATE TABLE IF NOT EXISTS Images\
            (\
            Img_Id blob PRIMARY KEY,\
            Img_PlaceId blob,\
            Img_Url text,\
            Img_desc text,\
            Img_Dirty integer\
            )");
            $cordovaSQLite.execute(db, "\
            CREATE TABLE IF NOT EXISTS Tips\
            (\
            Tip_Id blob PRIMARY KEY,\
            Tip_PlaceId blob,\
            Tip_CategoryId blob,\
            Tip_Contetnt text\
            )");
            $cordovaSQLite.execute(db, "\
            CREATE TABLE IF NOT EXISTS Cities\
            (\
            Cit_Id integer PRIMARY KEY,\
            Cit_Name text,\
            Cit_Dirty integer\
            )");
            $cordovaSQLite.execute(db, "\
            CREATE TABLE IF NOT EXISTS TipCategories\
            (\
            TiC_Id blob PRIMARY KEY,\
            TiC_Class text,\
            TiC_Unicode text,\
            TiC_Name text,\
            Cit_Priiority integer\
            )");
        },
        populatePlaces: function (Places) {
            $rootScope.waitingUpdates = Places.length || 0;
            var query = "INSERT OR REPLACE INTO Places\
                    (Pla_Id,\
                    Pla_Name,\
                    Pla_imgUrl,\
                    Pla_TNImgUrl,\
                    Pla_desc,\
                    Pla_c_x,\
                    Pla_c_y,\
                    Pla_address,\
                    Pla_CityId,\
                    Pla_Dirty_imgUrl,\
                    Pla_Dirty_TNImgUrl)\
                    VALUES (?,?,?,?,?,?,?,?,?,?,?)";
            for (var i = 0; i < Places.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [Places[i].Id
                    , Places[i].Name
                    , Places[i].ImgUrl
                    , Places[i].TNImgUrl
                    , Places[i].Desc
                    , Places[i].CX
                    , Places[i].CY
                    , Places[i].Address
                    , Places[i].CityId
                    , 1
                    , 1])
                    .then(function (result) {
                        console.log("Places INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
                FileServices.DownloadTumbNail(Places[i].TNImgUrl, Places[i].Id);
            }
            $rootScope.$broadcast('CheckWaitingUpdates');
        },
        populateAudios: function (Audios) {
            var query = "INSERT OR REPLACE INTO Audios\
                    (Aud_Id\
                    ,Aud_PlaceId\
                    ,Aud_Name\
                    ,Aud_Url\
                    ,Aud_desc\
                    ,Aud_Dirty)\
                    VALUES (?,?,?,?,?,?)";
            for (var i = 0; i < Audios.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [Audios[i].ID
                    , Audios[i].PlaceId
                    , Audios[i].Name
                    , Audios[i].Url
                    , Audios[i].Desc
                    , 1])
                    .then(function (result) {
                        console.log("Audios INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        populateStories: function (Stories) {
            var query = "INSERT OR REPLACE INTO Stories\
                    (Sto_Id\
                    ,Sto_PlaceId\
                    ,Sto_Name\
                    ,Sto_Url\
                    ,Sto_desc\
                    ,Sto_Dirty)\
                    VALUES (?,?,?,?,?,?)";
            for (var i = 0; i < Stories.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [Stories[i].ID
                    , Stories[i].PlaceId
                    , Stories[i].Name
                    , Stories[i].Url
                    , Stories[i].Desc
                    , 1])
                    .then(function (result) {
                        console.log("Stories INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        populateImages: function (Images) {
            var query = "INSERT OR REPLACE INTO Images\
                    (Img_Id,\
                    Img_PlaceId,\
                    Img_Url,\
                    Img_desc,\
                    Img_Dirty)\
                    VALUES (?,?,?,?,?)";
            for (var i = 0; i < Images.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [Images[i].ID
                    , Images[i].PlaceId
                    , Images[i].Url
                    , Images[i].Desc
                    , 1])
                    .then(function (result) {
                        console.log("Images INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        populateTips: function (Tips) {
            var query = "INSERT OR REPLACE INTO Tips\
                    (Tip_Id,\
                    Tip_PlaceId,\
                    Tip_CategoryId,\
                    Tip_Contetnt)\
                    VALUES (?,?,?,?)";
            for (var i = 0; i < Tips.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [Tips[i].ID
                    , Tips[i].PlaceId
                    , Tips[i].CategoryId
                    , Tips[i].Content])
                    .then(function (result) {
                        console.log("Tips INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        populateTipCategories: function (TipCategories) {
            var query = "INSERT OR REPLACE INTO TipCategories\
                    (\
                    TiC_Id,\
                    TiC_Class,\
                    TiC_Unicode,\
                    TiC_Name,\
                    Cit_Priiority)\
                    VALUES (?,?,?,?,?)";
            for (var i = 0; i < TipCategories.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [TipCategories[i].ID
                    , TipCategories[i].Class
                    , TipCategories[i].Unicode
                    , TipCategories[i].Name
                    , TipCategories[i].Priority])
                    .then(function (result) {
                        console.log("TipCategories INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        populateCities: function (Cities) {
            var query = "INSERT OR REPLACE INTO Cities\
                    (Cit_Id\
                    ,Cit_Name\
                    ,Cit_Dirty)\
                    VALUES (?,?,?)";
            for (var i = 0; i < Cities.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [Cities[i].Id
                    , Cities[i].Name
                    , 1])
                    .then(function (result) {
                        console.log("Cities INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        deleteFromTable: function (tableName, tableIdColumn, IDs) {
            var stringIDs = IDs.map(String);
            var args = stringIDs.join(", ");
            var query = "\
            DELETE FROM " + tableName + "\
            WHERE " + tableIdColumn + " IN (" + args + ")";
            $cordovaSQLite.execute(db, query)
                    .then(function (result) {
                        console.log("deleted IDs --> " + args);
                    }, function (error) {
                        console.error(error);
                    });
        },
        CleanPlaceTumbnail: function (PlaceID) {
            var query = "\
            UPDATE Places\
            SET Pla_Dirty_TNImgUrl = 0\
            WHERE Pla_Id = ?";
            $cordovaSQLite.execute(db, query, [PlaceID])
                    .then(function (result) {
                        console.log("placeTumbImageCleaned");
                        $rootScope.waitingUpdates--;
                        $rootScope.$broadcast('CheckWaitingUpdates');
                    }, function (error) {
                        console.error(error);
                        $rootScope.waitingUpdates--;
                        $rootScope.$broadcast('CheckWaitingUpdates');
                    });
        },
        CleanPlaceImage: function (PlaceID) {
            var query = "\
            UPDATE Places\
            SET Pla_Dirty_imgUrl = 0\
            WHERE Pla_Id = ?";
            $cordovaSQLite.execute(db, query, [PlaceID])
                    .then(function (result) {
                        console.log("placeImageCleaned");
                    }, function (error) {
                        console.error(error);
                    });
        },
        CleanPlaceExtraImage: function (ImageId) {
            var query = "\
            UPDATE Images\
            SET Img_Dirty = 0\
            WHERE Img_Id = ?";
            $cordovaSQLite.execute(db, query, [ImageId])
                    .then(function (result) {
                        console.log("placeExtraImageCleaned");
                    }, function (error) {
                        console.error(error);
                    });
        },
        CleanAudio: function (AudioId) {
            var query = "\
            UPDATE Audios\
            SET Aud_Dirty = 0\
            WHERE Aud_Id = ?";
            $cordovaSQLite.execute(db, query, [AudioId])
                    .then(function (result) {
                        console.log("Audio Cleaned: ", result);
                    }, function (error) {
                        console.error(error);
                    });
        },
        dbTest: function () {
            console.log("places");
            var query = "SELECT * FROM Places";
            $cordovaSQLite.execute(db, query).then(function (result) {
                for (var i = 0; i < result.rows.length; i++) {
                    console.log(result.rows.item(i));
                }
            }, function (error) {
                console.error(error);
            });
            console.log("Audios");
            query = "SELECT * FROM Audios";
            $cordovaSQLite.execute(db, query).then(function (result) {
                for (var i = 0; i < result.rows.length; i++) {
                    console.log(result.rows.item(i));
                }
            }, function (error) {
                console.error(error);
            });
            console.log("Images");
            query = "SELECT * FROM Images";
            $cordovaSQLite.execute(db, query).then(function (result) {
                for (var i = 0; i < result.rows.length; i++) {
                    console.log(result.rows.item(i));
                }
            }, function (error) {
                console.error(error);
            });
            console.log("cities");
            query = "SELECT * FROM Cities";
            $cordovaSQLite.execute(db, query).then(function (result) {
                for (var i = 0; i < result.rows.length; i++) {
                    console.log(result.rows.item(i));
                }
            }, function (error) {
                console.error(error);
            });
        },
        LoadAllCities: function () {
            var Cities = [];
            var query = "\
                SELECT\
                    Cit_Id,\
                    Cit_Name\
                FROM Cities";
            $cordovaSQLite.execute(db, query).then(function (result) {
                $rootScope.$broadcast('FillCities', { result: result });
            }, function (error) {
                console.error(error);
            });
        },
        LoadAllPlaces: function () {
            var query = "\
                SELECT\
                    Pla_Id,\
                    Pla_Name,\
                    Pla_TNImgUrl,\
                    Pla_address,\
                    Cit_Name,\
                    Pla_CityId\
                FROM Places JOIN Cities\
                ON Places.Pla_CityId = Cities.Cit_Id";
            $cordovaSQLite.execute(db, query).then(function (result) {
                $rootScope.$broadcast('FillPlaces', { result: result });
            }, function (error) {
                console.error(error);
            });
        },
        LoadPlaceInfos: function (Id) {
            var query = "\
                SELECT *\
                FROM Places\
                WHERE Pla_Id = ?";
            return $cordovaSQLite.execute(db, query, [Id]);
            //.then(function (result) {
            //    $rootScope.$broadcast('PlaceInfoesLoaded', { result: result });
            //}, function (error) {
            //    console.error(error);
            //});
        },
        LoadPlaceAudios: function (PlaceId) {
            var query = "\
                SELECT *\
                FROM Audios\
                WHERE Aud_PlaceId = ?";
            return $cordovaSQLite.execute(db, query, [PlaceId]);
            //.then(function (result) {
            //    $rootScope.$broadcast('PlaceAudiosLoaded', { result: result });
            //}, function (error) {
            //    console.error(error);
            //});
        },
        LoadPlaceStories: function (PlaceId) {
            var query = "\
                SELECT *\
                FROM Stories\
                WHERE Sto_PlaceId = ?";
            return $cordovaSQLite.execute(db, query, [PlaceId]);
            //.then(function (result) {
            //    $rootScope.$broadcast('PlaceStoriesLoaded', { result: result });
            //}, function (error) {
            //    console.error(error);
            //});
        },
        LoadPlaceImages: function (PlaceId) {
            var query = "\
                SELECT *\
                FROM Images\
                WHERE Img_PlaceId = ?";
            return $cordovaSQLite.execute(db, query, [PlaceId]);
            //.then(function (result) {
            //    $rootScope.$broadcast('PlaceImagesLoaded', { result: result });
            //}, function (error) {
            //    console.error(error);
            //});
        },
        LoadPlaceTips: function (PlaceId) {
            var query = "\
                SELECT Tip_Id, TiC_Class, Tip_Contetnt\
                FROM Tips JOIN TipCategories\
                ON Tips.Tip_CategoryId = TipCategories.TiC_Id\
                WHERE Tip_PlaceId = ?\
                ORDER BY\
                    Cit_Priiority ASC,\
                    Tip_Id ASC;";
            return $cordovaSQLite.execute(db, query, [PlaceId]);
            //.then(function (result) {
            //    $rootScope.$broadcast('PlaceTipsLoaded', { result: result });
            //}, function (error) {
            //    console.error(error);
            //});
        }
    }
}])
.service('FileServices', ['$rootScope', '$cordovaFile', '$cordovaFileTransfer', function ($rootScope, $cordovaFile, $cordovaFileTransfer) {
    return {
        createDirs: function () {
            Dirs = ["TumbNameil_dir", "PlacePic_dir", "PlaceAudio_dir", "PlaceStory_dir", "Extras_dir", "ProfilePic_dir"];
            for (var i = 0; i < Dirs.length; i++) {
                $cordovaFile.createDir(cordova.file.dataDirectory, Dirs[i], false)
                  .then(function (success) {
                      console.log(success);
                  }, function (error) {
                      console.log(error);
                  });
            }
        },
        DownloadExtraImage: function (fileName, ImageId, imgDesc) {
            var url = "http://iranaudioguide.com/images/Places/Extras/" + fileName;
            var targetPath = cordova.file.dataDirectory + "Extras_dir/" + fileName;
            var trustHosts = true;
            var options = {};

            $cordovaFileTransfer.download(url, targetPath, options, trustHosts)
                .then(function (result) {// Success!
                    $rootScope.$broadcast('PlaceExtraImageDownloaded', {
                        Img_Url: targetPath,
                        Img_Id: ImageId,
                        Img_desc: imgDesc
                    });
                }, function (err) {// Error
                    console.log(err);
                }, function (progress) {
                    //$timeout(function () {
                    //    $scope.downloadProgress = (progress.loaded / progress.total) * 100;
                    //});
                });
        },
        DownloadTumbNail: function (fileName, placeId) {
            console.log('star: ' + fileName);
            var url = "http://iranaudioguide.com/images/Places/TumbnailImages/" + fileName;
            var targetPath = cordova.file.dataDirectory + "/TumbNameil_dir/" + fileName;
            var trustHosts = true;
            var options = {};

            $cordovaFileTransfer.download(url, targetPath, options, trustHosts)
              .then(function (result) {
                  //dbServices.CleanPlaceTumbnail(placeId);
                  $rootScope.$broadcast('callDbServicesFunctions', { functionName: 'CleanPlaceTumbnail', params: [placeId] });
                  // Success!
              }, function (err) {
                  console.log(err);
                  $rootScope.waitingUpdates--;
                  $rootScope.$broadcast('CheckWaitingUpdates');
                  // Error
              }, function (progress) {
                  //$timeout(function () {
                  //    $scope.downloadProgress = (progress.loaded / progress.total) * 100;
                  //});
              });
        },
        DownloadPlaceImage: function (fileName, placeId) {
            var url = "http://iranaudioguide.com/images/Places/" + fileName;
            var targetPath = cordova.file.dataDirectory + "/PlacePic_dir/" + fileName;
            var trustHosts = true;
            var options = {};

            return $cordovaFileTransfer.download(url, targetPath, options, trustHosts);
            //.then(function (result) {// Success!
            //    //dbServices.CleanPlaceImage(placeId);
            //    $rootScope.$broadcast('callDbServicesFunctions', { functionName: 'CleanPlaceImage', params: [placeId] });
            //    $rootScope.$broadcast('PlaceImageDownloaded', { PlaceImgPath: targetPath });
            //}, function (err) {// Error
            //    console.log(err);
            //}, function (progress) {
            //    //$timeout(function () {
            //    //    $scope.downloadProgress = (progress.loaded / progress.total) * 100;
            //    //});
            //});
        },
        DownloadProfilePic: function (url, dest) {
            console.log(url);
            console.log(dest);
            var targetPath = cordova.file.dataDirectory + "/ProfilePic_dir/" + dest;
            var trustHosts = true;
            var options = {};
            window.localStorage.setItem("Authenticated", true);
            $cordovaFileTransfer.download(url, targetPath, options, trustHosts)
              .then(function (result) {
                  window.localStorage.setItem("User_Img", targetPath);
                  $rootScope.$broadcast('loadProfilePicCommpleted');
                  // Success!
              }, function (err) {
                  $rootScope.$broadcast('loadProfilePicFailed');
                  // Error
              }, function (progress) {
                  //$timeout(function () {
                  //    $scope.downloadProgress = (progress.loaded / progress.total) * 100;
                  //});
              });
        },
        DownloadAudio: function (fileName) {
            var url = "http://iranaudioguide.com/Audios/" + fileName;
            var targetPath = cordova.file.dataDirectory + "/PlaceAudio_dir/" + fileName;
            var trustHosts = true;
            var options = {};

            return $cordovaFileTransfer.download(url, targetPath, options, trustHosts);
        },
        DownloadStory: function (fileName) {
            var url = "http://iranaudioguide.com/Stories/" + fileName;
            var targetPath = cordova.file.dataDirectory + "/PlaceStory_dir/" + fileName;
            var trustHosts = true;
            var options = {};

            return $cordovaFileTransfer.download(url, targetPath, options, trustHosts);
        }
    }
}]);

