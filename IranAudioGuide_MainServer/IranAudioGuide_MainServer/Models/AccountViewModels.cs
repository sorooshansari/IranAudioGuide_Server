using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IranAudioGuide_MainServer.App_GlobalResources;

namespace IranAudioGuide_MainServer.Models
{
    public class ExternalLoginConfirmationViewModel
    {

        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredEmail), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderEmail))]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequire), ErrorMessageResourceType = typeof(Global))]
        public string Provider { get; set; }

        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequire), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.DispalyCode), ResourceType = typeof(Global))]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = nameof(Global.RememberBrowser), ResourceType = typeof(Global))]
        public bool RememberBrowser { get; set; }

        [Display(Name = nameof(Global.LabelForRememberMe), ResourceType = typeof(Global))]
        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {

        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredEmail), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderEmail))]
        [EmailAddress(ErrorMessageResourceName = nameof(Global.ErrorEmail), ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {

        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredEmail), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderEmail))]
        [EmailAddress(ErrorMessageResourceName = nameof(Global.ErrorEmail), ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredPassword), ErrorMessageResourceType = typeof(Global))]
        [DataType(DataType.Password, ErrorMessageResourceName = nameof(Global.ErrorRequiredPassword), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderPassword), ResourceType = typeof(Global))]
        public string Password { get; set; }

        [Display(Name = nameof(Global.LabelForRememberMe), ResourceType = typeof(Global))]
        public bool RememberMe { get; set; }
        //public bool IsTheFirstLogin { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredEmail), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderEmail))]
        [EmailAddress(ErrorMessageResourceName = nameof(Global.ErrorEmail), ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredRegisterName), ErrorMessageResourceType = typeof(Global))]
        [StringLength(30, ErrorMessageResourceName = nameof(Global.ErrorStringLengthMessage), ErrorMessageResourceType = typeof(Global), MinimumLength = 3)]
        [Display(Name = nameof(Global.PlaceholderName), ResourceType = typeof(Global))]
        public string Name { get; set; }


        [StringLength(100, ErrorMessageResourceName = nameof(Global.ErrorStringLengthMessage), ErrorMessageResourceType = typeof(Global), MinimumLength = 6)]
        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredPassword), ErrorMessageResourceType = typeof(Global))]
        [DataType(DataType.Password, ErrorMessageResourceName = nameof(Global.ErrorRequiredPassword), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderPassword), ResourceType = typeof(Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = nameof(Global.PlaceholderConfirmPassword), ResourceType = typeof(Global))]
        [Compare("Password", ErrorMessageResourceName = nameof(Global.ErrorComparePassword), ErrorMessageResourceType = typeof(Global))]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {

        [StringLength(30, ErrorMessageResourceName = nameof(Global.ErrorStringLengthMessage), ErrorMessageResourceType = typeof(Global), MinimumLength = 3)]
        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredPassword), ErrorMessageResourceType = typeof(Global))]
        [DataType(DataType.Password, ErrorMessageResourceName = nameof(Global.ErrorRequiredPassword), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderPassword), ResourceType = typeof(Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = nameof(Global.PlaceholderConfirmPassword), ResourceType = typeof(Global))]
        [Compare("Password", ErrorMessageResourceName = nameof(Global.ErrorComparePassword), ErrorMessageResourceType = typeof(Global))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string userId { get; set; }
    }

    public class ForgotPasswordViewModel
    {

        [Required(ErrorMessageResourceName = nameof(Global.ErrorRequiredEmail), ErrorMessageResourceType = typeof(Global))]
        [Display(Name = nameof(Global.PlaceholderEmail))]
        [EmailAddress(ErrorMessageResourceName = nameof(Global.ErrorEmail), ErrorMessageResourceType = typeof(Global))]
        public string Email { get; set; }
      
    }
}
