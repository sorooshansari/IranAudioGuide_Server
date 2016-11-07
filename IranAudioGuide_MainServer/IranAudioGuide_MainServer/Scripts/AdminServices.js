angular.module('AdminPage.services', [])
.service('TipServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var Tips = []
    return {
        getPlaceTips: function (placeId) {
            method = 'POST';
            data = { placeId: placeId };
            url = '/Admin/GetPlaceTips';
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  angular.copy(response.data, Tips);
                  console.log(response.data);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Tips;
        },
        AddTip: function (newTip) {
            method = 'POST';
            url = '/Admin/AddTip';
            var data = {
                'PlaceId': newTip.placeId,
                'content': newTip.content,
                'TipCategoryId': newTip.categoryId
            }
            $http({
                method: method,
                url: url,
                data: data
            }).
              then(function (response) {
                  if (response.data) {
                      $rootScope.$broadcast('TipAdded', { PlaceId: newTip.placeId });
                  }
                  else {
                      alert("Server failed to add Tip.");
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        RemoveTip: function (TipId, placeId) {
            method = 'POST';
            url = '/Admin/RemoveTip';
            data = { Id: TipId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  if (response.data) {
                      $rootScope.$broadcast('TipRemoved', { PlaceId: placeId });
                  }
                  else {
                      alert("Server failed to add Tip.");
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        GetTipCategories: function () {
            method = 'POST';
            url = '/Admin/GetTipCategories';
            $http({ method: method, url: url }).
              then(function (response) {
                  $rootScope.allTipCategories = angular.copy(response.data);
                  console.log(response.data);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        }
    }
}])
.service('PlaceServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var Places = [];
    var OnlinePlaces = [];
    var ExtraImages = [];
    return {
        GetAll: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetAllPlaces';
            $http({ method: method, url: url }).
              then(function (response) {
                  $rootScope.allPlaces = angular.copy(response.data);
                  $rootScope.$broadcast('LoadPlaces', {});
                  $rootScope.$broadcast('LoadFirstPlaceAudios', {});
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        Get: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetPlaces';
            data = { PageNum: PageNum };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $rootScope.PlacePagesLen = response.data.PagesLen;
                  $rootScope.PlaceCurrentPage = PageNum;
                  angular.copy(response.data.Places, Places);
                  $rootScope.$broadcast('LoadFirstPlaceAudios', {});
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Places;
        },
        OnlineGet: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetPlaces';
            data = { PageNum: PageNum, isOnline: true };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $rootScope.PlacePagesLen = response.data.PagesLen;
                  $rootScope.PlaceCurrentPage = PageNum;
                  angular.copy(response.data.Places, OnlinePlaces);
                  $rootScope.$broadcast('OnlineLoadFirstPlaceAudios', {});
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return OnlinePlaces;
        },
        AddPlace: function (NewPlace) {
            method = 'POST';
            url = '/Admin/AddPlace';
            var fd = new FormData();
            fd.append('Image', NewPlace.Image);
            fd.append('PlaceName', NewPlace.PlaceName);
            fd.append('PlaceDesc', NewPlace.PlaceDesc || "");
            fd.append('PlaceAddress', NewPlace.PlaceAddress || "");
            fd.append('PlaceCordinates', NewPlace.PlaceCordinates || "");
            fd.append('PlaceCityId', NewPlace.PlaceCityId);
            $http({
                method: method,
                url: url,
                data: fd,
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).
              then(function (response) {
                  if (response.data.status == 0) {
                      $rootScope.$broadcast('placeAdded', {});
                  }
                  else {
                      $rootScope.$broadcast('UpdatePlaceValidationSummery', {
                          data: response.data.content
                      });
                      console.log("Server failed to add Place.");
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        RemovePlace: function (PlaceID, PlaceName) {
            method = 'POST';
            url = '/Admin/DelPlace';
            data = { Id: PlaceID };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdatePlaces', {});
                          break;
                      case 1:
                          $rootScope.$broadcast('PlaceForignKeyError', {
                              PlaceID: PlaceID,
                              PlaceName: PlaceName
                          });
                          break;
                      case 2:
                          $rootScope.$broadcast('PlaceUnknownError', {});
                          console.log("Server failed to remove Place.");
                          break;
                      case 3:
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        OnlineRemovePlace: function (PlaceID) {
            method = 'POST';
            url = '/Admin/DeactiveOnlinePlace';
            data = { Id: PlaceID };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('OnlineUpdatePlaces', {});
                          break;
                      case 2:
                          console.log("Server failed to remove Place. Invalid Place Id.");
                          console.log(response.data.content);
                          break;
                      case 3:
                          $rootScope.$broadcast('PlaceUnknownError', {});
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        Edit: function (EditPlaceVM) {
            method = 'POST';
            url = '/Admin/EditPlace';
            $http({ method: method, url: url, data: EditPlaceVM }).
              then(function (response) {
                  $rootScope.EditOverlay = false;
                  $rootScope.hide('#EditPlaceModal');
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdatePlaces', {});
                          break;
                      case 1:
                          $rootScope.$broadcast('EditPlaceValidationSummery', {
                              data: response.data.content
                          });
                          break;
                      case 2:
                          $rootScope.$broadcast('EditPlaceUnknownError', {});
                          console.log("Server failed to remove Place.");
                          break;
                      case 3:
                          console.log(response.data.content);
                          break;
                      default:
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        ChangeImage: function (ImageName, NewImage, id) {
            method = 'POST';
            url = '/Admin/ChangePlaceImage';
            var fd = new FormData();
            fd.append('ImageName', ImageName);
            fd.append('NewImage', NewImage);
            fd.append('PlaceId', id);
            $http({
                method: method,
                url: url,
                data: fd,
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).
              then(function (response) {
                  $rootScope.EditOverlay = false;
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdatePlaceImage', {});
                          break;
                      case 1:
                          $rootScope.$broadcast('UpdateImageValidationSummery', {
                              data: response.data.content
                          });
                          break;
                      case 3:
                          console.log(response.data.content);
                          break;
                      default:
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        GetExtraImages: function (PlaceId) {
            method = 'POST';
            url = '/Admin/GetExtraImages';
            data = { placeId: PlaceId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  angular.copy(response.data, ExtraImages);
              }, function (response) {
                  console.log("Request failed");
              });
            return ExtraImages;
        },
        AddExtraImage: function (image, placeId) {
            method = 'POST';
            url = '/Admin/AddPlaceExtraImage';
            var fd = new FormData();
            fd.append('NewImage', image);
            fd.append('PlaceId', placeId);
            $http({
                method: method,
                url: url,
                data: fd,
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).
              then(function (response) {
                  if (response.data.status == 0) {
                      $rootScope.$broadcast('UpdateExtraImg', {});
                  }
                  else {
                      $rootScope.$broadcast('ExtraImgUnknownError', {
                          data: response.data.content
                      });
                      console.log("Server failed to add Place.");
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        DelExtraImage: function (imgId) {
            method = 'POST';
            url = '/Admin/DelPlaceExtraImage';
            data = { imgId: imgId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdateExtraImg', {});
                          break;
                      case 2:
                          $rootScope.$broadcast('Invalid Id', {});
                          console.log(response.data.content);
                          break;
                      case 3:
                          $rootScope.$broadcast('PlaceUnknownError', {});
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        EditEIDesc: function (EditEIDescVM) {
            method = 'POST';
            url = '/Admin/EditPlaceExtraImageDesc';
            data = { ImageId: EditEIDescVM.ImageId, ImageDesc: EditEIDescVM.Desc };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case 0:
                          $('#EditEIDescModal').modal('hide');
                          $rootScope.$broadcast('UpdateExtraImg', {});
                          break;
                      case 2:
                          $rootScope.$broadcast('InvalidId', {});
                          console.log(response.data.content);
                          break;
                      case 3:
                          $rootScope.$broadcast('PlaceUnknownError', {});
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        GoOnline: function (PlaceId) {
            method = 'POST';
            url = '/Admin/GoOnline';
            data = { PlaceId: PlaceId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $('#GoOfflineModal').modal('show');
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdatePlaces', {});
                          break;
                      case 2:
                          $rootScope.$broadcast('InvalidId', {});
                          console.log(response.data.content);
                          break;
                      case 3:
                          $rootScope.$broadcast('PlaceUnknownError', {});
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        GoOffline: function (PlaceId) {
            method = 'POST';
            url = '/Admin/GoOffline';
            data = { PlaceId: PlaceId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $('#GoOfflineModal').modal('hide');
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdatePlaces', {});
                          break;
                      case 2:
                          $rootScope.$broadcast('InvalidId', {});
                          console.log(response.data.content);
                          break;
                      case 3:
                          $rootScope.$broadcast('PlaceUnknownError', {});
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        }
    }
}])
.service('CityServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var AllCities = [];
    var Cities = [];
    var Success = false;
    return {
        All: function () {
            method = 'POST';
            url = '/Admin/GetCities';
            data = { PageNum: -1 };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  angular.copy(response.data, AllCities);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return AllCities;
        },
        Get: function (PageNum) {
            method = 'POST';
            url = '/Admin/GetCities';
            data = { PageNum: PageNum };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  $rootScope.CityPagesLen = response.data.PagesLen;
                  $rootScope.CityCurrentPage = PageNum;
                  angular.copy(response.data.Cities, Cities);
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return Cities;
        },
        AddCity: function (NewCity) {
            method = 'POST';
            url = '/Admin/AddCity';
            data = { CityName: NewCity.CityName, CityDesc: NewCity.CityDesc };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case respondstatus.success:
                          $rootScope.$broadcast('cityAdded', {});
                          break;
                      default:
                          console.log("Server failed to add City.");
                          break;
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        },
        RemoveCity: function (CityID, CityName) {
            method = 'POST';
            url = '/Admin/DelCity';
            data = { Id: CityID };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case respondstatus.success:
                          $rootScope.$broadcast('UpdateCities', {});
                          break;
                      case respondstatus.forignKeyError:
                          $rootScope.$broadcast('CityForignKeyError', {
                              CityID: CityID,
                              CityName: CityName
                          });
                          break;
                      default:
                          $rootScope.$broadcast('CityUnknownError', {});
                          console.log("Server failed to remove City.");
                          break;
                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        }
    };
}])
.service('AudioServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        angular.forEach(data, function (value, key) {
            dataAsFormData.append(key, value);
        });
        return dataAsFormData;
    };
    return {
        Get: function (PlaceId) {
            method = 'POST';
            url = '/Admin/Audios';
            data = { PlaceId: PlaceId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  var temp = [];
                  $rootScope.placeimage = response.data.PlaceImage;
                  $rootScope.audios = angular.copy(response.data.audios);
                  $rootScope.$broadcast('FillFirstAudio', {});
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        OnlineGet: function (PlaceId) {
            method = 'POST';
            url = '/Admin/Audios';
            data = { PlaceId: PlaceId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  var temp = [];
                  $rootScope.OnlinePlaceimage = response.data.PlaceImage;
                  $rootScope.OnlineAudios = angular.copy(response.data.audios);
                  $rootScope.$broadcast('OnlineFillFirstAudio', {});
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        Add: function (model, placeId) {
            method = 'POST';
            url = '/Admin/AddAudio';
            var fd = new FormData();
            fd.append('PlaceId', placeId);
            fd.append('AudioName', model.AudioName);
            fd.append('AudioFile', model.AudioFile);
            console.log(model.AudioFile);
            for (var pair of fd.entries()) {
                console.log(pair[0] + ', ' + pair[1]);
            }
            $http({
                method: method,
                url: url,
                data: fd,
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).
              then(function (response) {
                  $rootScope.ShowOverlay = false;
                  $rootScope.hide('#NewAudioModal');
                  if (response.data.status == 0) {
                      $rootScope.$broadcast('UpdateAudios', {});
                  }
                  else {
                      $rootScope.$broadcast('UpdateAudioValidationSummery', {
                          data: response.data.content
                      });
                      console.log("Server failed to add Audio.");
                  }
              }, function (response) {
                  $rootScope.ShowOverlay = false;
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        Remove: function (AudioId) {
            method = 'POST';
            url = '/Admin/DelAudio';
            data = { Id: AudioId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdateAudios', {});
                          break;
                      case 1:
                          $rootScope.$broadcast('RemoveAudioError', {
                              content: response.data.content
                          });
                          console.log("Server failed to remove audio.");
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        }
    }
}])

.service('StoryServices', ['$rootScope', '$http', function ($rootScope, $http) {
    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        angular.forEach(data, function (value, key) {
            dataAsFormData.append(key, value);
        });
        return dataAsFormData;
    };
    return {
        Get: function (PlaceId) {
            method = 'POST';
            url = '/Admin/Storys';
            data = { PlaceId: PlaceId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  var temp = [];
                  $rootScope.placeimage = response.data.PlaceImage;
                  $rootScope.Storys = angular.copy(response.data.Storys);
                  $rootScope.$broadcast('FillFirstStory', {});
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        OnlineGet: function (PlaceId) {
            method = 'POST';
            url = '/Admin/Storys';
            data = { PlaceId: PlaceId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  var temp = [];
                  $rootScope.OnlinePlaceimage = response.data.PlaceImage;
                  $rootScope.OnlineStorys = angular.copy(response.data.Storys);
                  $rootScope.$broadcast('OnlineFillFirstStory', {});
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        Add: function (model, placeId) {
            method = 'POST';
            url = '/Admin/AddStory';
            var fd = new FormData();
            fd.append('PlaceId', placeId);
            fd.append('StoryName', model.StoryName);
            fd.append('StoryFile', model.StoryFile);
            console.log(model.StoryFile);
            for (var pair of fd.entries()) {
                console.log(pair[0] + ', ' + pair[1]);
            }
            $http({
                method: method,
                url: url,
                data: fd,
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).
              then(function (response) {
                  $rootScope.ShowOverlay = false;
                  $rootScope.hide('#NewStoryModal');
                  if (response.data.status == 0) {
                      $rootScope.$broadcast('UpdateStorys', {});
                  }
                  else {
                      $rootScope.$broadcast('UpdateStoryValidationSummery', {
                          data: response.data.content
                      });
                      console.log("Server failed to add Story.");
                  }
              }, function (response) {
                  $rootScope.ShowOverlay = false;
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
            return;
        },
        Remove: function (StoryId) {
            method = 'POST';
            url = '/Admin/DelStory';
            data = { Id: StoryId };
            $http({ method: method, url: url, data: data }).
              then(function (response) {
                  switch (response.data.status) {
                      case 0:
                          $rootScope.$broadcast('UpdateStorys', {});
                          break;
                      case 1:
                          $rootScope.$broadcast('RemoveStoryError', {
                              content: response.data.content
                          });
                          console.log("Server failed to remove story.");
                          console.log(response.data.content);
                          break;
                      default:

                  }
              }, function (response) {
                  console.log("Request failed");
                  console.log("status:" + response.status);
              });
        }
    }
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