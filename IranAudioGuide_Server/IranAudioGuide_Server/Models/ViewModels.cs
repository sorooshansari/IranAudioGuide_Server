using System;
using System.Collections.Generic;
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
        Login
    }
    public class AdminIndexVM
    {
        public UserInfo AdminInfo { get; set; }
    }
    public class UserInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string imgUrl { get; set; }
    }
}