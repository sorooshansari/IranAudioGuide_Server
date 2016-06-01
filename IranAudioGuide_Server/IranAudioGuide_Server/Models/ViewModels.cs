using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IranAudioGuide_Server.Models
{
    public class ViewModels
    {
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
        public UserInfo AdminInfo { get; set; }
        public NewPlace NewPlace { get; set; }
        public List<PlaceVM> Places { get; set; }
        public NewAudioVM NewAudio { get; set; }
    }
    public class UserInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string imgUrl { get; set; }
    }
    public class NewPlace
    {
        [Required]
        [Display(Name = "Place name")]
        public string PlaceName { get; set; }
        [Required]
        [Display(Name = "Place description")]
        public string PlaceDesc { get; set; }
        [Required]
        [Display(Name = "Choose")]
        public HttpPostedFileBase Image { get; set; }
    }
    public class PlaceVM
    {
        public System.Guid PlaceId { get; set; }
        [Display(Name = "#")]
        public int Index { get; set; }
        [Display(Name = "Name")]
        public string PlaceName { get; set; }
        [Display(Name = "Description")]
        public string PlaceDesc { get; set; }
        public string ImgUrl { get; set; }
        public List<AudioVM> Audios { get; set; }
    }
    public class AudioViewVM
    {
        public List<AudioVM> audios { get; set; }
        public string PlaceImage { get; set; }
    }
    public class AudioVM
    {
        public System.Guid Aud_Id { get; set; }
        public string Aud_Name { get; set; }
        public string Aud_Url { get; set; }
        public string Aud_Discription { get; set; }
    }
    public class NewAudioVM
    {
        [Required(ErrorMessage ="Something went wrong. Failed to upload file.")]
        public System.Guid PlaceId { get; set; }
        [Required]
        [Display(Name = "Audio name")]
        public string AudioName { get; set; }
        [Required]
        [Display(Name = "Audio file")]
        public string AudioFile { get; set; }
        [Required]
        [Display(Name = "Audio description")]
        public string AudioDesc { get; set; }
    }

}