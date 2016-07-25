angular.module('app.services', [])

.factory('SlideShows', function () {
    var SlideShow = [
        {
            id: 0,
            URL: 'img/1.jpg'
        },
    {
        id: 1,
        URL: 'img/2.jpg'
    },
    {
        id: 2,
        URL: 'img/3.jpg'
    },
    {
        id: 3,
        URL: 'img/6.jpg'
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
.service('ApiServices', ['$http', '$rootScope', function ($http, $rootScope) {
    return {
        GetAll: function (LUN) {
            method = 'post';
            url = 'http://api.iranaudioguide.com/api/update?LastUpdateNumber=' + LUN;
            $http({ method: method, url: url }).
              then(function (response) {
                  //var Tables = angular.copy(response.data);
                  $rootScope.$broadcast('PopulateTables', { Data: response.data });
              }, function (response) {
                  $rootScope.$broadcast('ServerConnFailde', { error: response.data });
              });
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
            CREATE TABLE IF NOT EXISTS Images\
            (\
            Img_Id blob PRIMARY KEY,\
            Img_PlaceId blob,\
            Img_Url text,\
            Img_desc text,\
            Img_Dirty integer\
            )");
            $cordovaSQLite.execute(db, "\
            CREATE TABLE IF NOT EXISTS Cities\
            (\
            Cit_Id integer PRIMARY KEY,\
            Cit_Name text,\
            Cit_Dirty integer\
            )");
        },
        populatePlaces: function (Places) {
            $rootScope.waitingUpdates = Places.length;
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
                        console.log("INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
                FileServices.DownloadTumbNail(Places[i].TNImgUrl, Places[i].Id);
            }
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
                    [Audios[i].Id
                    , Audios[i].PlaceId
                    , Audios[i].Name
                    , Audios[i].Url
                    , Audios[i].Desc
                    , 1])
                    .then(function (result) {
                        console.log("INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        populateImages: function (Images) {
            var query = "INSERT OR REPLACE INTO Audios\
                    (Aud_Id\
                    ,Aud_PlaceId\
                    ,Aud_Url\
                    ,Aud_desc\
                    ,Aud_Dirty)\
                    VALUES (?,?,?,?,?)";
            for (var i = 0; i < Images.length; i++) {
                $cordovaSQLite.execute(db, query,
                    [Images[i].Id
                    , Images[i].PlaceId
                    , Images[i].Url
                    , Images[i].Desc
                    , 1])
                    .then(function (result) {
                        console.log("INSERT ID -> " + result.insertId);
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
                        console.log("INSERT ID -> " + result.insertId);
                    }, function (error) {
                        console.error(error);
                    });
            }
        },
        CleanPlaceTumbnail: function (PlaceID) {
            var query = "\
            UPDATE Places\
            SET Pla_Dirty_TNImgUrl = 0\
            WHERE Pla_Id = ?";
            $cordovaSQLite.execute(db, query, [PlaceID])
                    .then(function (result) {
                        $rootScope.waitingUpdates--;
                    }, function (error) {
                        console.error(error);
                    });
        },
        CleanPlaceImage: function (PlaceID) {
            var query = "\
            UPDATE Places\
            SET Pla_Dirty_imgUrl = 0\
            WHERE Pla_Id = ?";
            $cordovaSQLite.execute(db, query, [PlaceID])
                    .then(function (result) {
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
        }
    }
}])
.service('FileServices', ['$rootScope', '$cordovaFile', '$cordovaFileTransfer', function ($rootScope, $cordovaFile, $cordovaFileTransfer) {
    return {
        createDirs: function () {
            Dirs = ["TumbNameil_dir", "PlacePic_dir", "PlaceAudio_dir", "Extras_dir"];
            for (var i = 0; i < Dirs.length; i++) {
                $cordovaFile.createDir(cordova.file.dataDirectory, Dirs[i], false)
                  .then(function (success) {
                      console.log(success);
                  }, function (error) {
                      console.log(error);
                  });
            }
        },
        DownloadTumbNail: function (fileName, placeId) {
            console.log('star: ' + fileName);
            var url = "http://iranaudioguide.com/images/Places/TumbnailImages/" + fileName;
            var targetPath = cordova.file.dataDirectory + "/TumbNameil_dir/" + fileName;
            var trustHosts = true;
            var options = {};

            $cordovaFileTransfer.download(url, targetPath, options, trustHosts)
              .then(function (result) {
                  $rootScope.$broadcast('CleanPlaceTumbnail', { placeId: placeId });
                  // Success!
              }, function (err) {
                  console.log(err);
                  // Error
              }, function (progress) {
                  //$timeout(function () {
                  //    $scope.downloadProgress = (progress.loaded / progress.total) * 100;
                  //});
              });
        },
        DownloadImage: function (fileName, placeId) {
            var url = "http://iranaudioguide.com/images/Places/" + fileName;
            var targetPath = cordova.file.dataDirectory + "/PlacePic_dir/" + fileName;
            var trustHosts = true;
            var options = {};

            $cordovaFileTransfer.download(url, targetPath, options, trustHosts)
              .then(function (result) {
                  //dbServices.CleanPlaceImage(placeId);
                  // Success!
              }, function (err) {
                  console.log(err);
                  // Error
              }, function (progress) {
                  //$timeout(function () {
                  //    $scope.downloadProgress = (progress.loaded / progress.total) * 100;
                  //});
              });
        }
    }
}]);

