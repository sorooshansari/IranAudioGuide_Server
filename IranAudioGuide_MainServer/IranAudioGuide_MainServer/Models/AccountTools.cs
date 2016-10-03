using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public bool CreateUser(ApplicationUser userInfo)
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
            user.UserName = NewUserInfo.UserName;
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