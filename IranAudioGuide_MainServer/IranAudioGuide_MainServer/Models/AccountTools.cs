using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using IranAudioGuide_MainServer.Models;
using System.Threading.Tasks;

namespace IranAudioGuide_MainServer.Models
{
    public class AccountTools
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public SignInStatus AutorizeAppUser(string username, string password)
        {
            return SignInManager.PasswordSignIn(username, password, false, true);
        }
        public async Task<CreateingUserResult> CreateAppUser(string Email, string password, string baseUrl)
        {
            var appUser = await UserManager.FindByNameAsync(Email);
            if (appUser != null)
                return CreateingUserResult.userExists;
            var user = new ApplicationUser() { UserName = Email, Email = Email };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                try
                {
                    string code = HttpUtility.UrlEncode(UserManager.GenerateEmailConfirmationToken(user.Id));
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    await UserManager.AddToRoleAsync(user.Id, "AppUser");
                    var callbackUrl = string.Format("{0}/Account/ConfirmEmail?userId={1}&code={2}", baseUrl, user.Id, code);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    return CreateingUserResult.success;
                }
                catch (Exception ex)
                {
                    UserManager.Delete(user);
                }
            }
            return CreateingUserResult.fail;
        }
        public bool CreateGoogleUser(ApplicationUser userInfo)
        {
            var appUser = UserManager.FindByNameAsync(userInfo.Email).Result;
            if (appUser == null)
            {
                if (UserManager.CreateAsync(userInfo).Result.Succeeded)
                {
                    UserManager.AddToRoleAsync(userInfo.Id, "AppUser");
                    return true;
                }
            }
            else
                return updateUserInfo(appUser, userInfo);
            return false;
        }
        private bool updateUserInfo(ApplicationUser user, ApplicationUser NewUserInfo)
        {
            user.GoogleId = NewUserInfo.GoogleId;
            user.Picture = NewUserInfo.Picture;
            user.FullName = NewUserInfo.FullName;
            user.gender = NewUserInfo.gender;
            return UserManager.UpdateAsync(user).Result.Succeeded;
        }
        public string logIn(string email, string pass)
        {
            var result = SignInManager.PasswordSignIn(email, pass, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return "Success";
                case SignInStatus.LockedOut:
                    return "LockedOut";
                case SignInStatus.RequiresVerification:
                    return "RequiresVerification";
                case SignInStatus.Failure:
                    return "Failure";
                default:
                    return "Invalid login attempt.";
            }
        }
    }
}