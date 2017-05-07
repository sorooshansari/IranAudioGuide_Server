  
var data = {
    trackId : '5ad3f2e9-2efc-4b20-b6b2-f863ca61c72c',
    email: 'testuser@iranaudioguide.com',
    uuid: "1212",
    isAudio : true
};

$http.post('/api/AppManagerV2/GetUrl', data).then(function (result) {        
    console.log(result.data)
}, function (error) {
    if (error.status==400 && typeof(error.data)!=="undefined" && typeof(error.data.Message)!== "undefined", error.data.Message.lenght>0) {
    
        console.log(error.data.Message);
    }
    ////error.status 
    //error.statusText
});

//when error :
switch(error.data.Message ==1) {
    case   1:
        console.log( notUser)
    case 2:
        console.log( unauthorizedUser) 
    case 4:
        console.log( unknownError = 4)
}
//output if success
http://iranaudioguide.net/test/Files/Audios/5ad3f2e9-2efc-4b20-b6b2-f863ca61c72c.mp3





/////////////////////////////////////////////////////////////////////////////////////////


    var data2 = {
        username: 'testuser@iranaudioguide.com',
        uuid: "1212",
    };
$http.post('/api/AppManagerV2/GetAutorizedCities', data2).then(function (result) {
      
    console.log(result)
}, function (error) {
    console.log(error.data.ModelState.error[0]);// return number *
    console.log(error.data.ModelState.ex[0]); // return error server if  throw Exception
         
});
//output if success
{
    cities:{LangId:1, LangTitle:"en", cityID:7}
    errorMessage:null,
    status:1
}

//output if error line *:

notUser = 2,
 uuidMissMatch = 3,
 notConfirmed = 4,
 unknownError = 5


/////////////////////////////////////////////////////////////////////////////////////////


var data3 = {
    CityId: '8',
    LangId:1
};
$http.post('/api/AppManagerV2/GetPackages', data3).then(function (result) {
    console.log("GetPackages");
    console.log(result.data);
}, function (error) {
    console.log(error.data.ModelState.ex[0]);

});

//output if success

IsForeign: true ,
    cities: {
    AudiosCount: 1 ,
    CityId: 7 ,
    CityName: "cit1-en" ,
    PackageId: "ae2806a5-0433-e711-9032-002215efcd58" ,
    PlacesCount: 1 ,
    StoriesCount: 1
    }
errorMessage: null ,
    packages: {
    Id: "ae2806a5-0433-e711-9032-002215efcd58" ,
    Name: "pac2" ,
    Price: 12 ,
    PriceD: 12 
    }


/////////////////////////////////////////////////////////////////////////////////////////

$http.post('/api/AppManagerV2/GetAll', { uuid: "1212"})
       .success(function (result) {           
           console.log(result.data);
       }, function (error) {
           console.log(error.data.ModelState.ex[0]);
       });


//output if success
{
    "UpdateNumber": 134,
    "Places": [
      {
          "Id": "545d6f84-0230-e711-9032-002215efcd58",
          "Name": "plc1",
          "ImgUrl": "545d6f84-0230-e711-9032-002215efcd58.jpg",
          "TNImgUrl": "545d6f84-0230-e711-9032-002215efcd58.jpg",
          "Desc": "12",
          "CX": 0,
          "CY": 0,
          "Address": "12",
          "CityId": 7,
          "UpdateNumber": 0,
          "isPrimary": true,
          "OrderItem": 0
      },
      {
          "Id": "13b6a34f-e432-e711-9032-002215efcd58",
          "Name": "plc-cit2",
          "ImgUrl": "13b6a34f-e432-e711-9032-002215efcd58.jpg",
          "TNImgUrl": "13b6a34f-e432-e711-9032-002215efcd58.jpg",
          "Desc": "",
          "CX": 0,
          "CY": 0,
          "Address": "",
          "CityId": 8,
          "UpdateNumber": 0,
          "isPrimary": true,
          "OrderItem": 0
      }
    ],
    "Audios": [
      {
          "ID": "ed713aa0-0230-e711-9032-002215efcd58",
          "Name": "qqqqqqqqqqqq",
          "Desc": "",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 0,
          "LangId": 1
      }
    ],
    "Stories": [
      {
          "ID": "e218e8aa-0230-e711-9032-002215efcd58",
          "Name": "ssssssssss",
          "Desc": "",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 0,
          "LangId": 1
      }
    ],
    "Images": [
      {
          "ID": "d7174d8b-0230-e711-9032-002215efcd58",
          "Url": null,
          "Desc": "",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 1,
          "Name": "d7174d8b-0230-e711-9032-002215efcd58.jpg"
      },
      {
          "ID": "9d509542-1833-e711-9032-002215efcd58",
          "Url": null,
          "Desc": "wwwwwwwwwwwww",
          "PlaceId": "13b6a34f-e432-e711-9032-002215efcd58",
          "OrderItem": 1,
          "Name": "9d509542-1833-e711-9032-002215efcd58.jpg"
      },
      {
          "ID": "2f3ec34a-1833-e711-9032-002215efcd58",
          "Url": null,
          "Desc": "wwwwwwwwwwwwwwwww",
          "PlaceId": "13b6a34f-e432-e711-9032-002215efcd58",
          "OrderItem": 2,
          "Name": "2f3ec34a-1833-e711-9032-002215efcd58.gif"
      }
    ],
    "Tips": [
      {
          "ID": "d6174d8b-0230-e711-9032-002215efcd58",
          "Content": "12",
          "CategoryId": "e2cc05c3-2a2f-e711-9031-002215efcd58",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 0,
          "LangId": 1
      }
    ],
    "Cities": [
      {
          "Id": 7,
          "Name": "cit1-en",
          "ImageUrl": "7.jpg",
          "Desc": "sssssssssss",
          "OrderItem": 0
      },
      {
          "Id": 8,
          "Name": "cit2-en",
          "ImageUrl": "8.jpg",
          "Desc": "cit2-en",
          "OrderItem": 0
      },
      {
          "Id": 10,
          "Name": "cit3-en",
          "ImageUrl": "10.jpg",
          "Desc": "aaaaaaaaaaaa",
          "OrderItem": 0
      },
      {
          "Id": 11,
          "Name": "cit4-en",
          "ImageUrl": "11.gif",
          "Desc": "aaaaaaaaaaaaaa",
          "OrderItem": 0
      }
    ],
    "TipCategories": [
      {
          "ID": "e2cc05c3-2a2f-e711-9031-002215efcd58",
          "Class": "ion-android-walk",
          "Unicode": "&#xf3bb;",
          "Name": "Transportation",
          "Priority": 1
      },
      {
          "ID": "e3cc05c3-2a2f-e711-9031-002215efcd58",
          "Class": "ion-ios-pulse-strong",
          "Unicode": "&#xf492;",
          "Name": "Rough track",
          "Priority": 2
      },
      {
          "ID": "e4cc05c3-2a2f-e711-9031-002215efcd58",
          "Class": "ion-android-time",
          "Unicode": "&#xf3b3;",
          "Name": "Time",
          "Priority": 3
      },
      {
          "ID": "e5cc05c3-2a2f-e711-9031-002215efcd58",
          "Class": "ion-android-checkbox-outline",
          "Unicode": "&#xf373;",
          "Name": "Stuff",
          "Priority": 4
      },
      {
          "ID": "e6cc05c3-2a2f-e711-9031-002215efcd58",
          "Class": "ion-ios-nutrition-outline",
          "Unicode": "&#xf46f;",
          "Name": "Other",
          "Priority": 5
      }
    ],
    "TranslateCities": [
      {
          "Id": "7bdbf565-0230-e711-9032-002215efcd58",
          "Name": "city1",
          "Description": "aqqqqq",
          "CityId": 7,
          "LangId": 2
      },
      {
          "Id": "ccee076c-0230-e711-9032-002215efcd58",
          "Name": "cit1-en",
          "Description": "sssssssssss",
          "CityId": 7,
          "LangId": 1
      },
      {
          "Id": "b689de62-e332-e711-9032-002215efcd58",
          "Name": "city2",
          "Description": "",
          "CityId": 8,
          "LangId": 2
      },
      {
          "Id": "ba139282-e332-e711-9032-002215efcd58",
          "Name": "cit2-en",
          "Description": "cit2-en",
          "CityId": 8,
          "LangId": 1
      },
      {
          "Id": "5d852074-0433-e711-9032-002215efcd58",
          "Name": "cit3-en",
          "Description": "",
          "CityId": 10,
          "LangId": 2
      },
      {
          "Id": "ccefe37b-0433-e711-9032-002215efcd58",
          "Name": "cit3-en",
          "Description": "aaaaaaaaaaaa",
          "CityId": 10,
          "LangId": 1
      },
      {
          "Id": "c021af8b-0433-e711-9032-002215efcd58",
          "Name": "cit-fa",
          "Description": "",
          "CityId": 11,
          "LangId": 2
      },
      {
          "Id": "988aab97-0433-e711-9032-002215efcd58",
          "Name": "cit4-en",
          "Description": "aaaaaaaaaaaaaa",
          "CityId": 11,
          "LangId": 1
      }
    ],
    "TranslateImages": [
      {
          "Id": "2e3ec34a-1833-e711-9032-002215efcd58",
          "Name": "",
          "Description": "wwwwwwwwwwwww",
          "ImageId": 0,
          "LangId": 1
      },
      {
          "Id": "6f834854-1833-e711-9032-002215efcd58",
          "Name": "",
          "Description": "wwwwwwwwwwwwwwwww",
          "ImageId": 0,
          "LangId": 1
      }
    ],
    "TranslatePlaces": [
      {
          "Id": "555d6f84-0230-e711-9032-002215efcd58",
          "Name": "plc1",
          "Description": "12",
          "Adr": "12",
          "LangId": 1,
          "PlaceId": 0
      },
      {
          "Id": "14b6a34f-e432-e711-9032-002215efcd58",
          "Name": "plc-cit2",
          "Description": "",
          "Adr": "",
          "LangId": 1,
          "PlaceId": 0
      }
    ],
    "ErrorMessage": ""
}
////////////////////////////////////////////////////////////////////////////////////////////////////////


$http.post('/api/AppManagerV2/GetUpdates', {
    uuid: "1212",
    LastUpdateNumber: 2
})
         .success(function (result) {
             console.log("GetUpdates");
             console.log(result.data)
         }, function (error) {
             console.log(error.data.ModelState.ex[0]);

         });

//output if success

{
    "UpdateNumber": 134,
    "Places": [
      {
          "Id": "545d6f84-0230-e711-9032-002215efcd58",
          "Name": "plc1",
          "ImgUrl": "545d6f84-0230-e711-9032-002215efcd58.jpg",
          "TNImgUrl": "545d6f84-0230-e711-9032-002215efcd58.jpg",
          "Desc": "12",
          "CX": 0,
          "CY": 0,
          "Address": "12",
          "CityId": 7,
          "UpdateNumber": 0,
          "isPrimary": true,
          "OrderItem": 0
      },
      {
          "Id": "13b6a34f-e432-e711-9032-002215efcd58",
          "Name": "plc-cit2",
          "ImgUrl": "13b6a34f-e432-e711-9032-002215efcd58.jpg",
          "TNImgUrl": "13b6a34f-e432-e711-9032-002215efcd58.jpg",
          "Desc": "",
          "CX": 0,
          "CY": 0,
          "Address": "",
          "CityId": 8,
          "UpdateNumber": 0,
          "isPrimary": true,
          "OrderItem": 0
      }
    ],
    "Audios": [
      {
          "ID": "ed713aa0-0230-e711-9032-002215efcd58",
          "Name": "qqqqqqqqqqqq",
          "Desc": "",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 0,
          "LangId": 1
      }
    ],
    "Stories": [
      {
          "ID": "e218e8aa-0230-e711-9032-002215efcd58",
          "Name": "ssssssssss",
          "Desc": "",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 0,
          "LangId": 1
      }
    ],
    "Images": [
      {
          "ID": "d7174d8b-0230-e711-9032-002215efcd58",
          "Url": null,
          "Desc": "",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 1,
          "Name": "d7174d8b-0230-e711-9032-002215efcd58.jpg"
      },
      {
          "ID": "9d509542-1833-e711-9032-002215efcd58",
          "Url": null,
          "Desc": "wwwwwwwwwwwww",
          "PlaceId": "13b6a34f-e432-e711-9032-002215efcd58",
          "OrderItem": 1,
          "Name": "9d509542-1833-e711-9032-002215efcd58.jpg"
      },
      {
          "ID": "2f3ec34a-1833-e711-9032-002215efcd58",
          "Url": null,
          "Desc": "wwwwwwwwwwwwwwwww",
          "PlaceId": "13b6a34f-e432-e711-9032-002215efcd58",
          "OrderItem": 2,
          "Name": "2f3ec34a-1833-e711-9032-002215efcd58.gif"
      }
    ],
    "Tips": [
      {
          "ID": "d6174d8b-0230-e711-9032-002215efcd58",
          "Content": "12",
          "CategoryId": "e2cc05c3-2a2f-e711-9031-002215efcd58",
          "PlaceId": "545d6f84-0230-e711-9032-002215efcd58",
          "OrderItem": 0,
          "LangId": 1
      }
    ],
    "Cities": [
      {
          "Id": 7,
          "Name": "cit1-en",
          "ImageUrl": "7.jpg",
          "Desc": "sssssssssss",
          "OrderItem": 0
      },
      {
          "Id": 10,
          "Name": "cit3-en",
          "ImageUrl": "10.jpg",
          "Desc": "aaaaaaaaaaaa",
          "OrderItem": 0
      },
      {
          "Id": 11,
          "Name": "cit4-en",
          "ImageUrl": "11.gif",
          "Desc": "aaaaaaaaaaaaaa",
          "OrderItem": 0
      }
    ],
    "TranslateCities": [
      {
          "Id": "7bdbf565-0230-e711-9032-002215efcd58",
          "Name": "city1",
          "Description": "aqqqqq",
          "CityId": 7,
          "LangId": 2
      },
      {
          "Id": "ccee076c-0230-e711-9032-002215efcd58",
          "Name": "cit1-en",
          "Description": "sssssssssss",
          "CityId": 7,
          "LangId": 1
      },
      {
          "Id": "b689de62-e332-e711-9032-002215efcd58",
          "Name": "city2",
          "Description": "",
          "CityId": 8,
          "LangId": 2
      },
      {
          "Id": "ba139282-e332-e711-9032-002215efcd58",
          "Name": "cit2-en",
          "Description": "cit2-en",
          "CityId": 8,
          "LangId": 1
      },
      {
          "Id": "5d852074-0433-e711-9032-002215efcd58",
          "Name": "cit3-en",
          "Description": "",
          "CityId": 10,
          "LangId": 2
      },
      {
          "Id": "ccefe37b-0433-e711-9032-002215efcd58",
          "Name": "cit3-en",
          "Description": "aaaaaaaaaaaa",
          "CityId": 10,
          "LangId": 1
      },
      {
          "Id": "c021af8b-0433-e711-9032-002215efcd58",
          "Name": "cit-fa",
          "Description": "",
          "CityId": 11,
          "LangId": 2
      },
      {
          "Id": "988aab97-0433-e711-9032-002215efcd58",
          "Name": "cit4-en",
          "Description": "aaaaaaaaaaaaaa",
          "CityId": 11,
          "LangId": 1
      }
    ],
    "TranslateImages": [],
    "TranslatePlaces": [
      {
          "Id": "555d6f84-0230-e711-9032-002215efcd58",
          "Name": "plc1",
          "Description": "12",
          "Adr": "12",
          "LangId": 1,
          "PlaceId": 0
      },
      {
          "Id": "14b6a34f-e432-e711-9032-002215efcd58",
          "Name": "plc-cit2",
          "Description": "",
          "Adr": "",
          "LangId": 1,
          "PlaceId": 0
      }
    ],
    "RemovedEntries": {
        "Places": [
          "1d651a26-f52f-e711-9032-002215efcd58",
          "6c315122-fa2f-e711-9032-002215efcd58",
          "99e48b5a-e432-e711-9032-002215efcd58"
        ],
        "Audios": [],
        "Stories": [],
        "Images": [
          "e6efc620-f82f-e711-9032-002215efcd58",
          "e7efc620-f82f-e711-9032-002215efcd58"
        ],
        "Tips": [
          "50778130-f52f-e711-9032-002215efcd58"
        ],
        "Cities": [
          8
        ]
    },
  "ErrorMessage": ""
}

