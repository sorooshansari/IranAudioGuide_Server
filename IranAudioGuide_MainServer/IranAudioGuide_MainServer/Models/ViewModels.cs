using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IranAudioGuide_MainServer.App_GlobalResources;

namespace IranAudioGuide_MainServer.Models
{
    public class ViewModels
    {
    }
    public class GetVersoinVm
    {
        public string version { get; set; }
        public bool isIOS { get; set; }

    }
    public enum getUserStatus
    {
        confirmed = 1,
        notUser = 2,
        uuidMissMatch = 3,
        notConfirmed = 4,
        unknownError = 5
    }
    public class SelectLangVM
    {
        public string action { get; set; }
        public string controller { get; set; }
        public string currentLang { get; set; }
        public List<LangVM> langs { get; set; }
    }
    public class vmessageVM
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        //public bool  IsShowUrl  { get; set; }
    }

    public class PaymentResult
    {
        #region نمایش پیغام های نتیجه پرداخت زرین پال
        /// <summary>
        /// این متد یک ورودی گرفته و نتیجه پیغام را بر می گرداند
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        public static string ZarinPal(string resultId)
        {
            switch (resultId)
            {
                case "NOK":
                    return Global.PaymentUnsuccessfully;
                case "-100":
                    return Global.ZarinpalPaymentMsg8;
                case "-1":
                    return Global.ZarinpalPaymentMsg9;
                case "-2":
                    return Global.ZarinpalPaymentMsg10;
                case "-3":
                    return Global.ZarinpalPaymentMsg11;
                case "-4":
                    return Global.ZarinpalPaymentMsg12;
                case "-11":
                    return Global.ZarinpalPaymentMsg13;
                case "-12":
                    return Global.ZarinpalPaymentMsg14;
                case "-21":
                    return Global.ZarinpalPaymentMsg15;
                case "-22":
                    return Global.ZarinpalPaymentMsg16;
                case "-33":
                    return Global.ZarinpalPaymentMsg17;
                case "34":
                    return Global.ZarinpalPaymentMsg18;
                case "40":
                    return Global.ZarinpalPaymentMsg19;
                case "41":
                    return Global.ZarinpalPaymentMsg20;
                case "42":
                    return Global.ZarinpalPaymentMsg21;
                case "54":
                    return Global.ZarinpalPaymentMsg22;
                case "100":
                    return Global.ZarinpalPaymentMsg23;
                case "101":
                    return Global.ZarinpalPaymentMsg24;
                default:
                    return Global.ServerErrorUnknown;
            }
        }

        #endregion
    }
    public class ContactEmailVM
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string message { get; set; }
        [Required]
        public string name { get; set; }
        public string subject { get; set; }
    }
    public class FormatedEmailVM
    {
        public string destination { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public string subject { get; set; }
    }
    public class NewPackage
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Package name")]
        public string Name { get; set; }
        public string PackageDesc { get; set; }
        [Required]
        [Display(Name = "Package price Toman")]
        public long PackagePrice { get; set; }
        [Required]
        [Display(Name = "Package price $")]
        public float PackagePrice_Dollar { get; set; }
        [Required]
        [Display(Name = "Package Cities")]
        public IList<int> Cities { get; set; }
    }
    public class IPData
    {
        public IPData()
        {
            countryCode = "FErr";

        }
        public string status { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string timezone { get; set; }
        public string isp { get; set; }
        public string org { get; set; }
        public string @as { get; set; }
        public string query { get; set; }
    }
    public class PackageVM
    {
        public int Index { get; internal set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public long PackagePrice { get; set; }
        public float PackagePriceDollar { get; set; }
        public List<CityVM> PackageCities { get; set; }
        public bool isPackagesPurchased { get; set; }

    }
    public class CityPymentVM
    {
        public int CityID { get; set; }
        //public string _imageUrl { get; set; }
        //public string CityImageUrl
        //{
        //    get { return _imageUrl; }
        //    set { _imageUrl = GlobalPath.FullPathImageCity + value; }
        //}
        [Display(Name = "Name")]
        public string CityName { get; set; }
        [Display(Name = "Description")]
        public string CityDesc { get; internal set; }
        //    public int Order { get; set; }
        public int TrackCount { get; internal set; }
        public int PlaceCount { get; internal set; }
        //   public int PlaceCount { get; set; }
    }

    public class PackagePymentVM
    {
        public PackagePymentVM()
        {
            PackageCities = new List<CityPymentVM>();
        }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public string PackagePrice { get; set; }
        public string PackagePriceDollar { get; set; }
        public List<CityPymentVM> PackageCities { get; set; }
    }

    public class GetPackageVM
    {
        public GetPackageVM()
        {
            PackageCities = new List<CityUserVM>();
        }

        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public string PackagePrice { get; set; }
        public string PackagePriceDollar { get; set; }
        public bool isPackagesPurchased { get; set; }
        public List<CityUserVM> PackageCities { get; set; }

    }
    public class PackageUserVM
    {
        public PackageUserVM()
        {
            PackageCities = new List<CityUserVM>();
        }
        public int Index { get; internal set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public string PackagePrice { get; set; }
        public string PackagePriceDollar { get; set; }
        public bool isPackagesPurchased { get; set; }
        public int PackageOrder { get; internal set; }

        public List<CityUserVM> PackageCities { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string CityOrder { get; set; }
        public string CityImageUrl { get; internal set; }
        public string CityDescription { get; internal set; }
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
    public class GetUrlVm
    {
        public string Url { get; set; }
        public string Lang { get; set; }
        public bool isAccess { get; set; }
    }
    public class GetAudioUrlVM
    {
        public string email { get; set; }
        public string uuid { get; set; }
        public Guid trackId { get; set; }
        public bool isAudio { get; set; }
    }

    public enum GetAudioUrlStatus
    {
        //
        // Summary:
        //     Geting url successful
        success = 0,
        //
        // Summary:
        //     this is not one of our users
        notUser = 1,
        //
        // Summary:
        //     This audio is not available for this user
        unauthorizedUser = 2,
        //
        // Summary:
        //     geting url with incorrect uuid
        uuidMissMatch = 3,
        //
        // Summary:
        //     unknow error happen
        unknownError = 4
    }
    public class ForgotPassUser
    {
        public ForgotPassUser()
        {
            lang = (int)EnumLang.en;
        }
        public string email { get; set; }
        public string uuid { get; set; }
        public int lang { get; set; }
    }
    public class AppUser
    {
        public string fullName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string uuid { get; set; }
        public int lang { get; set; }
    }
    public class GoogleUserInfo
    {
        public string name { get; set; }
        public string email { get; set; }
        public string google_id { get; set; }
        public string picture { get; set; }
        public string uuid { get; set; }
    }
    public enum CreatingUserResult
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
        uuidMissMatch = 3,
        //
        // Summary:
        //     This is a google user, just update its password
        googleUser = 4
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
        uuidMissMatch = 4,
        error = 5
    }
    public enum RecoverPassResults
    {
        //
        // Summary:
        //     Sign in was successful
        Success = 0,
        //
        // Summary:
        //     not user of the app
        NotUser = 1,
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
        UserIndex,
        vmessage,
        Seller
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
        public bool IsEmailConfirmed { get; set; }
        public bool IsSetuuid { get; set; }
        public bool IsAccessChangeUuid { get; set; }
        public DateTime? TimeSetUuid { get; set; }
        public bool IsForeign { get; set; }
    }
    public class UserProfile
    {
        public UserProfile()
        {
            IsAutintication = false;
            RolesName = "AppUser";
        }
        public bool IsAutintication { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string imgUrl { get; set; }
        public string RolesName { get; set; }

    }
    public class NewCity
    {
        [Required]
        [Display(Name = "City name")]
        public string CityName { get; set; }
        [Display(Name = "City description")]
        public string CityDesc { get; set; }
        [Required]
        [Display(Name = "City Image")]
        public HttpPostedFileBase CityImage { get; set; }
        public string lang { get; set; }
    }
    public class CityUserVM
    {
        public int CityID { get; set; }
        public string _imageUrl { get; set; }
        public string CityImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = GlobalPath.FullPathImageCity + value; }
        }
        [Display(Name = "Name")]
        public string CityName { get; set; }
        [Display(Name = "Description")]

        public IList<PlaceUserVM> Places { get; set; }
        public int TotalTrackCount { get; internal set; }
        public int TotalCountPlace { get; internal set; }
        public string CityDesc { get; internal set; }
        public bool IsloadImage { get; internal set; }
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
        [Display(Name = "City Image")]
        public string _imageUrl { get; set; }
        public string CityImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = GlobalPath.FullPathImageCity + value; }
        }
        public IList<PlaceVM> Places { get; set; }
    }
    public class EditCityVM
    {
        [Required]
        [Display(Name = "Id")]
        public int CityID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string CityName { get; set; }
        [Required]
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

        [Required(ErrorMessageResourceType = typeof(Global),
             ErrorMessageResourceName = "testError1")]
        [StringLength(50, ErrorMessageResourceType = typeof(Global),
                     ErrorMessageResourceName = "testError2")]
        public string PlaceName { get; set; }
        public string PlaceDesc { get; set; }
        public string PlaceAddress { get; set; }
        public string PlaceCordinates { get; set; }
        [Required]
        public int PlaceCityId { get; set; }
        [Required]
        public HttpPostedFileBase Image { get; set; }
    }
    public class ImagRemoveVm
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
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


        public long Price { get; set; }
        public float PriceDollar { get; set; }
    }
    public class PlaceUserVM
    {
        public Guid PlaceId { get; set; }
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
        public int AudiosCount { get; internal set; }
        public int StoriesCount { get; internal set; }
        public int Cit_Id { get; internal set; }
        public int OrderItem { get; internal set; }
    }
    public class PlaceVM
    {
        public Guid PlaceId { get; set; }
        public int Index { get; set; }
        public string PlaceName { get; set; }
        public string PlaceDesc { get; set; }
        public string CityName { get; set; }
        public int PlaceCityId { get; set; }


        public long Price { get; set; }
        public float PriceDollar { get; set; }


        public string _imageUrl { get; set; }
        public string ImgUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = GlobalPath.FullPathImagePlace + value; }
        }
        public string _tumbImgUrl { get; set; }
        public string TumbImgUrl
        {
            get { return _tumbImgUrl; }
            set { _tumbImgUrl = GlobalPath.FullPathImageTumbnail + value; }
        }
        public string PlaceAddress { get; set; }
        public string PlaceCordinates { get; set; }
        public bool isOnline { get; set; }
        public bool isPrimary { get; set; }
    }

    public class GetImagesByPlaceIdVm
    {
        public Guid PlaceId { get; set; }
        public int Type { get; set; }
    }

    public class ImageSaveOrderVM
    {
        [Required]
        public int Type { get; set; }
        [Required]
        public Guid ImageId { get; set; }
        [Required]

        public string PlaceId { get; set; }
        [Required]

        public int Index { get; set; }
    }
    public class ImageVM
    {
        public string _imageUrl { get; set; }

        public int Type { get; set; }
        public string ImageName
        {
            get { return _imageUrl; }
            set
            {
                if (Type == (int)EnumImageType.Extra)
                    _imageUrl = GlobalPath.FullPathImageExtras + value;
                else
                    _imageUrl = GlobalPath.FullPathImageGallery + value;
            }
        }
        public Guid ImageId { get; set; }
        public int Index { get; set; }
        public string ImageDesc { get; set; }
        public Guid PlaceId { get; internal set; }
    }
    //public class ImageGalleryVM
    //{
    //    public string _imageUrl { get; set; }
    //    public string ImageName
    //    {
    //        get { return _imageUrl; }
    //        set { _imageUrl = GlobalPath.FullPathImageGallery + value; }
    //    }
    //    public Guid ImageId { get; set; }
    //    public int Index { get; set; }
    //    public string ImageDesc { get; set; }

    //}
    public class EditEIDescVM
    {
        [Required]
        public Guid ImageId { get; set; }
        public string ImageDesc { get; set; }
        public string Name { get; set; }
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
    public class ChangeCityImageVM
    {
        public int CityId { get; set; }
        public HttpPostedFileBase NewImage { get; set; }
    }
    public class StoryViewVM
    {
        public StoryViewVM()
        {
            respond = new Respond();
        }
        public List<StoryVM> Storys { get; set; }
        public string _imageUrl { get; set; }
        public string PlaceImage
        {
            get { return _imageUrl; }
            set { _imageUrl = GlobalPath.FullPathImagePlace + value; }
        }
        public Respond respond { get; set; }
    }
    public class StoryVM
    {
        public string _imageUrl { get; set; }
        //public string Url
        //{
        //    get { return _imageUrl; }
        //    set { _imageUrl = GlobalPath.FullPathStory + value; }
        //}
        public string Url { get; set; }
        public int Index { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public string Lang { get; set; }
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
        public string _imageUrl { get; set; }
        public string PlaceImage
        {
            get { return _imageUrl; }
            set { _imageUrl = GlobalPath.FullPathImagePlace + value; }
        }
        public Respond respond { get; set; }
    }
    public class AudioVM
    {
        public int Index { get; set; }
        public Guid Aud_Id { get; set; }
        public string Aud_Name { get; set; }
        public string _imageUrl { get; set; }
        //public string Aud_Url
        //{
        //    get { return _imageUrl; }
        //    set { _imageUrl = GlobalPath.FullPathAudios + value; }
        //}
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
        Place,
        TCity,
        TPlace,
        TImage,

    }
    public class CommentVm
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string uuid { get; set; }
        public string email { get; set; }
    }

    public class RequestForAppVM
    {
        public string Email { get; set; }
        public string NameDevice { get; set; }

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
        removeOnlinePlace = 8,
        InvalidUsre = 9,

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
    //public class Respond2
    //{
    //    public Respond2(string content = "", string status = "")
    //    {
    //        this.status = status;
    //        this.content = content;
    //    }

    //    public string status { get; set; }
    //    public string content { get; set; }
    //}
}