using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IranAudioGuide_MainServer.Models
{
    public class ViewModels
    {
    }

    public class NewPackage
    {
        [Required]
        [Display(Name = "Package name")]
        public string PackageName { get; set; }

        [Display(Name = "Package description")]
        public string PackageDesc { get; set; }

        [Display(Name = "Package price")]
        public int PackagePrice { get; set; }

        [Display(Name = "Package Cities")]
        public List<int> Cities { get; set; }
    }

    public class PackageVM
    {
        public int Index { get; internal set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public int PackagePrice { get; set; }
        public List<CityVM> PackageCities { get; set; }
    }
    public class GetPackagesVM
    {
        private Package Packges;
                
        public GetPackagesVM(List<PackageVM> Packages, int PagesLen)
        {
            this.Packages = Packages;
            this.PagesLen = PagesLen;
        }
        public List<PackageVM> Packages { get; set; }
        public int PagesLen { get; set; }
    }

    public class AddTipVM
    {
        public Guid PlaceId { get; set; }
        public string content { get; set; }
        public Guid TipCategoryId { get; set; }
    }
    public class TipCategoriesVM
    {
        public Guid id { get; set; }
        public string Class { get; set; }
        public string unicode { get; set; }
        public string name { get; set; }
        public string iconicName { get; set; }
    }
    public class TipVM
    {
        public Guid id { get; set; }
        public Guid TipcategoryID { get; set; }
        public string Content { get; set; }
    }
    public class AuthorizedUser
    {
        public string GoogleId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public SignInResults Result { get; set; }
    }
    public class AppUser
    {
        public string email { get; set; }
        public string password { get; set; }
        public string uuid { get; set; }
    }
    public class GoogleUserInfo
    {
        public string name { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string google_id { get; set; }
        public string picture { get; set; }
        public string profile { get; set; }
        public string uuid { get; set; }
    }
    public enum CreateingUserResult
    {
        //
        // Summary:
        //     Creating user was successful
        success = 0,
        //
        // Summary:
        //     This user was existed
        userExists = 1,
        //
        // Summary:
        //     Creating user failed
        fail = 2,
        //
        // Summary:
        //     Creating user with different uuid
        uuidMissMatch = 3
    }
    public enum SignInResults
    {
        //
        // Summary:
        //     Sign in was successful
        Success = 0,
        //
        // Summary:
        //     User is locked out
        LockedOut = 1,
        //
        // Summary:
        //     Sign in requires addition verification (i.e. two factor)
        RequiresVerification = 2,
        //
        // Summary:
        //     Sign in failed
        Failure = 3,
        //
        // Summary:
        //     Sign in with different uuid
        uuidMissMatch = 4
    }
    public enum gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }
    public enum Views
    {
        Index = 1,
        Register,
        Login,
        AdminIndex,
        UserIndex
    }
    public class AdminIndexVM
    {
        public AdminIndexVM()
        {
            NewPlace = new NewPlace();
        }
        public UserInfo AdminInfo { get; set; }
        public NewPlace NewPlace { get; set; }
        public List<PlaceVM> Places { get; set; }
        public NewAudioVM NewAudio { get; set; }
        public NewCity NewCity { get; set; }
        public List<CityVM> Cities { get; set; }

        public List<PackageVM> Packages { get; set; }
    }
    public class UserInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string imgUrl { get; set; }
    }
    public class NewCity
    {
        [Required]
        [Display(Name = "City name")]
        public string CityName { get; set; }
        [Display(Name = "City description")]
        public string CityDesc { get; set; }
    }
    public class CityVM
    {
        [Display(Name = "#")]
        public int Index { get; set; }
        public int CityID { get; set; }
        [Display(Name = "Name")]
        public string CityName { get; set; }
        [Display(Name = "Description")]
        public string CityDesc { get; set; }
    }
    public class JNOCitiesVM
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
    }
    public class GetPlacesVM
    {
        public GetPlacesVM(List<PlaceVM> Places, int PagesLen)
        {
            this.Places = Places;
            this.PagesLen = PagesLen;
        }
        public List<PlaceVM> Places { get; set; }
        public int PagesLen { get; set; }
    }
    public class GetCitiesVM
    {
        public GetCitiesVM(List<CityVM> Cities, int PagesLen)
        {
            this.Cities = Cities;
            this.PagesLen = PagesLen;
        }
        public List<CityVM> Cities { get; set; }
        public int PagesLen { get; set; }
    }

    public class NewPlace
    {
        [Required]
        public string PlaceName { get; set; }
        public string PlaceDesc { get; set; }
        public string PlaceAddress { get; set; }
        public string PlaceCordinates { get; set; }
        [Required]
        public int PlaceCityId { get; set; }
        [Required]
        public HttpPostedFileBase Image { get; set; }
    }
    public class EditPlaceVM
    {
        [Required]
        public System.Guid PlaceId { get; set; }
        [Required]
        public string PlaceName { get; set; }
        public string PlaceDesc { get; set; }
        public string PlaceAddress { get; set; }
        public string PlaceCordinates { get; set; }
        [Required]
        public int PlaceCityId { get; set; }
    }
    public class PlaceVM
    {
        public System.Guid PlaceId { get; set; }
        public int Index { get; set; }
        public string PlaceName { get; set; }
        public string PlaceDesc { get; set; }
        public string CityName { get; set; }
        public int PlaceCityId { get; set; }
        public string ImgUrl { get; set; }
        public string TumbImgUrl { get; set; }
        public string PlaceAddress { get; set; }
        public string PlaceCordinates { get; set; }
        public bool isOnline { get; set; }
        public bool isPrimary { get; set; }
    }
    public class ImageVM
    {
        public Guid ImageId { get; set; }
        public string ImageName { get; set; }
        public int Index { get; set; }
        public string ImageDesc { get; set; }
    }
    public class EditEIDescVM
    {
        public Guid ImageId { get; set; }
        public string ImageDesc { get; set; }
    }
    public class NewImageVM
    {
        public Guid PlaceId { get; set; }
        public HttpPostedFileBase NewImage { get; set; }
    }
    public class ChangeImageVM
    {
        public Guid PlaceId { get; set; }
        public HttpPostedFileBase NewImage { get; set; }
    }
    public class StoryViewVM
    {
        public StoryViewVM()
        {
            respond = new Respond();
        }
        public List<StoryVM> Storys { get; set; }
        public string PlaceImage { get; set; }
        public Respond respond { get; set; }
    }
    public class StoryVM
    {
        public int Index { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Discription { get; set; }
    }
    public class NewStoryVM
    {
        [Required]
        public System.Guid PlaceId { get; set; }
        [Required]
        public string StoryName { get; set; }
        [Required]
        public HttpPostedFileBase StoryFile { get; set; }
    }

    public class AudioViewVM
    {
        public AudioViewVM()
        {
            respond = new Respond();
        }
        public List<AudioVM> audios { get; set; }
        public string PlaceImage { get; set; }
        public Respond respond { get; set; }
    }
    public class AudioVM
    {
        public int Index { get; set; }
        public Guid Aud_Id { get; set; }
        public string Aud_Name { get; set; }
        public string Aud_Url { get; set; }
        public string Aud_Discription { get; set; }
    }
    public class NewAudioVM
    {
        [Required]
        public System.Guid PlaceId { get; set; }
        [Required]
        public string AudioName { get; set; }
        [Required]
        public HttpPostedFileBase AudioFile { get; set; }
    }
    public class OK
    {
        public OK(bool success = true)
        {
            this.success = success;
        }
        public bool success { get; set; }
    }
    public enum updatedTable
    {
        Audio,
        Story,
        Tip,
        ExtraImage,
        City,
        Place
    }
    public enum status
    {
        success = 0,
        invalidInput = 1,
        ivalidCordinates = 2,
        invalidFileFormat = 3,
        unknownError = 4,
        dbError = 5,
        invalidId = 6,
        forignKeyError = 7,
        removeOnlinePlace = 8
    }
    public class Respond
    {
        public Respond(string content = "", status status = status.success)
        {
            this.status = status;
            this.content = content;
        }
        public status status { get; set; }
        public string content { get; set; }
    }

    public class userpackage
    {
        public int uuid { get; set; }
        public List<CityVM> usercities { get; set; }
    }
}