using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IranAudioGuide_MainServer.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [RequiredTranclate("ErrorRequire")]

        [DisplayNameTranclate("Email")]
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
        [RequiredTranclate("ErrorRequire")]
        public string Provider { get; set; }

        [RequiredTranclate("ErrorRequire")]
        [DisplayNameTranclate("Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [DisplayNameTranclate("RememberBrowser")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [RequiredTranclate("ErrorRequire")]
        [DisplayNameTranclate("Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [RequiredTranclate("ErrorRequiredEmail")]
        [DisplayNameTranclate("Email")]
        [EmailAddress(ErrorMessageResourceName = "ErrorEmail", ErrorMessageResourceType = typeof(Resources.Global))]
        public string Email { get; set; }

        //[Required(ErrorMessageResourceName = "ErrorRequiredPassword", ErrorMessageResourceType = typeof(Resources.Global)),]
        [RequiredTranclate("ErrorRequiredPassword")]
        [DataType(DataType.Password)]
        [DisplayNameTranclate("PlaceholderPassword")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        //public bool IsTheFirstLogin { get; set; }
    }

    public class RegisterViewModel
    {
        [RequiredTranclate("ErrorRequiredRegisterName")]
        //[RequiredTranclate("ErrorRequired")]

        //[StringLengthTranclate(30, "ErrorStringLengthMessage", MinimumLength = 10)]
        [StringLengthTranclate(30, "ErrorStringLengthMessage", MinimumLength = 3)]
        [DisplayNameTranclate("PlaceholderName")]
        public string Name { get; set; }

        [RequiredTranclate("ErrorRequiredEmail")]
        [EmailAddress(ErrorMessageResourceName = "ErrorEmail", ErrorMessageResourceType = typeof(Resources.Global) )]
        [DisplayNameTranclate("PlaceholderEmail")]
        public string Email { get; set; }

        [RequiredTranclate("ErrorRequiredPassword")]
        [StringLengthTranclate(100, "ErrorStringLengthMessage", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayNameTranclate("PlaceholderPassword")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayNameTranclate("PlaceholderConfirmPassword")]
        [Compare("Password", ErrorMessageResourceName = "ErrorComparePassword" ,ErrorMessageResourceType = typeof(Resources.Global))]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        [RequiredTranclate("ErrorRequire")]

        //[StringLengthTranclate(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [StringLengthTranclate(30, "ErrorStringLengthMessage", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [DisplayNameTranclate("PlaceholderPassword")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayNameTranclate("PlaceholderConfirmPassword")]
        [Compare("Password", ErrorMessageResourceName = "ErrorComparePassword", ErrorMessageResourceType = typeof(Resources.Global))]

        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string userId { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [RequiredTranclate("ErrorRequire")]

        [EmailAddress(ErrorMessageResourceName = "ErrorEmail", ErrorMessageResourceType = typeof(Resources.Global))]
        [DisplayNameTranclate("Email")]
        public string Email { get; set; }
    }
}
