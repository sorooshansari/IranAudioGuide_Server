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
    public enum skippedUserStatus
    {
        uuidExist = 1,
        uuidAdded = 2,
        unknownError = 3,
        uuidExistInPraimaryUsers = 4
    }
    public enum getUserStatus
    {
        confirmed = 1,
        notUser = 2,
        uuidMissMatch = 3,
        notConfirmed = 4,
        unknownError = 5
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
        string result = "";
        string resultfa = "";
        switch (resultId)
        {
            case "NOK":
                result = "Payment was unsuccessfull.";
                resultfa = "پرداخت ناموفق بود";
                break;
            case "-100":
                result = "Payment was canceld";
                resultfa = "پرداخت کنسل شده";
                break;
            case "-1":
                result = "Submitted information is incomplete";
                resultfa = "اطلاعات ارسال شده ناقص است";
                break;
            case "-2":
                result = "IP or Merchant Code is not correct";
                resultfa = "و يا مرچنت كد پذيرنده صحيح نيست IP";
                break;
            case "-3":
                result = "Due to the limitations of Shaparak system, opportunity to pay the amount requested is not possible";
                resultfa = "با توجه به محدوديت هاي شاپرك امكان پرداخت با رقم درخواست شده ميسر نمي باشد";
                break;
            case "-4":
                result = "سطح تاييد پذيرنده پايين تر از سطح نقره اي است.";
                resultfa = "سطح تاييد پذيرنده پايين تر از سطح نقره اي است.";
                break;
            case "-11":
                result = "The desired request was not found.";
                resultfa = "درخواست مورد نظر يافت نشد.";
                break;
            case "-12":
                result = "Editing request is not possible.";
                resultfa = "امكان ويرايش درخواست ميسر نمي باشد.";
                break;
            case "-21":
                result = "Any type of financial operations for this transaction was not found.";
                resultfa = "هيچ نوع عمليات مالي براي اين تراكنش يافت نشد";
                break;
            case "-22":
                result = "Transaction is not successful.";
                resultfa = "تراكنش نا موفق ميباشد.";
                break;
            case "-33":
                result = "The transaction does not match the amount paid.";
                resultfa = "رقم تراكنش با رقم پرداخت شده مطابقت ندارد.";
                break;
            case "34":
                result = "Transaction divison limit is passed by number or amount";
                resultfa = "سقف تقسيم تراكنش از لحاظ تعداد يا رقم عبور نموده است";
                break;
            case "40":
                result = "The related method is not accessible.";
                resultfa = "اجازه دسترسي به متد مربوطه وجود ندارد.";
                break;
            case "41":
                result = "Sent data related to AdditionalData is invalid";
                resultfa = "غيرمعتبر ميباشد. AdditionalData اطلاعات ارسال شده مربوط به";
                break;
            case "42":
                result = "Valid payment index lifetime period is between 30 minutes to 45 days.";
                resultfa = "مدت زمان معتبر طول عمر شناسه پرداخت بايد بين 30 دقيقه تا 45 روز مي باشد.";
                break;
            case "54":
                result = "The desired request is archived";
                resultfa = "درخواست مورد نظر آرشيو شده است.";
                break;
            case "100":
                result = "Operation is successfully done.";
                resultfa = "عمليات با موفقيت انجام گرديده است.";
                break;
            case "101":
                result = "Payment process has been successful and PaymentVerification has been done.";
                resultfa = "تراكنش انجام شده است. PaymentVerification عمليات پرداخت موفق بوده و قبلا";
                break;

            default:
                result = "Unknown Error";
                resultfa = "خطای نا مشخص";
                break;

        }
        return result;
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
    public class NewPackage
    {
        [Required]
        [Display(Name = "Package name")]
        public string PackageName { get; set; }

        [Display(Name = "Package description")]
        public string PackageDesc { get; set; }

        [Required]
        [Display(Name = "Package price")]
        public int PackagePrice { get; set; }

        [Required]
        [Display(Name = "Package Cities")]
        public List<int> Cities { get; set; }
    }
    public class IPData
    {
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
        public List<CityVM> PackageCities { get; set; }
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
    public class GetAudioUrlRes
    {
        public GetAudioUrlRes(string url)
        {
            this.url = url;
            status = GetAudioUrlStatus.success;
        }
        public GetAudioUrlRes(GetAudioUrlStatus status, string error = "")
        {
            this.status = status;
            this.errorMessage = error;
        }
        public GetAudioUrlStatus status { get; set; }
        public string url { get; set; }
        public string errorMessage { get; set; }
    }
    public class GetAutorizedCitiesVM
    {
        public string username { get; set; }
        public string uuid { get; set; }
    }
    public class ForgotPassUser
    {
        public string email { get; set; }
        public string uuid { get; set; }
    }
    public class AppUser
    {
        public string fullName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string uuid { get; set; }
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
        uuidMissMatch = 4
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
        vmessage
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
        public bool IsAccessChangeUuid { get;  set; }
        public DateTime? TimeSetUuid { get;  set; }
    }
    public class UserProfile
    {

        public string FullName { get; set; }
        public string Email { get; set; }
        public string imgUrl { get; set; }
        public IList<string> RolesName;
       
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
        public string CityImageUrl { get; set; }
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
    public class CommentVm {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string uuid { get; set; }
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
}