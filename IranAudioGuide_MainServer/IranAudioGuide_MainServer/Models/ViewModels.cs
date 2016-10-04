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
    public class GoogleUserInfo
    {
        public string name { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string google_id { get; set; }
        public string picture { get; set; }
        public string profile { get; set; }
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
        AdminIndex
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
        public string PlaceAddress { get; set; }
        public string PlaceCordinates { get; set; }
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
        public Guid PlaceId { get; set; }
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
        public string ImageName { get; set; }
        public HttpPostedFileBase NewImage { get; set; }
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
    public class Respond
    {
        public Respond(string content = "", int status = 0)
        {
            this.status = status;
            this.content = content;
        }
        public int status { get; set; }
        public string content { get; set; }
    }
}