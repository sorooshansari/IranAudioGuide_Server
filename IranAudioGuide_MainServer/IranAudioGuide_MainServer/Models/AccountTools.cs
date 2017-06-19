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
using Elmah;

namespace IranAudioGuide_MainServer.Models
{
    public class AccountTools : IDisposable
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
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
        // todo correct
        public async Task<int> SendEmailConfirmedAgain(string email, string BaseUrl)
        {
            try
            {
                var applicationUser = await UserManager.FindByNameAsync(email);
                if (applicationUser != default(ApplicationUser))
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(applicationUser.Id);
                    var callbackUrl = string.Format("{0}/Account/ConfirmEmail?userId={1}&code={2}", BaseUrl, applicationUser.Id, code);
                    await UserManager.SendEmailAsync(applicationUser.Id, App_GlobalResources.Global.ServerConfirmTitel, string.Format(App_GlobalResources.Global.ServerConfirmMessage, callbackUrl));
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return 2;
            }
        }
          

        public ApplicationUser getUser(string username)
        {
            return UserManager.FindByName(username);
        }
        public async Task<AuthorizedUser> AutorizeAppUser(string username, string password, string uuid)
        {
            var user = await UserManager.FindByNameAsync(username);
            var result = await SignInManager.PasswordSignInAsync(username, password, false, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    if (user.uuid == null)
                        await updateUserUUID(user, uuid);
                    else if (user.uuid != uuid)
                        return new AuthorizedUser() { Result = SignInResults.uuidMissMatch };
                    if (!user.EmailConfirmed)
                        return new AuthorizedUser()
                        {
                            GoogleId = user.GoogleId,
                            Email = user.Email,
                            FullName = user.FullName,
                            Picture = user.Picture,
                            Result = SignInResults.RequiresVerification
                        };
                    return new AuthorizedUser()
                    {
                        GoogleId = user.GoogleId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Picture = user.Picture,
                        Result = SignInResults.Success
                    };
                case SignInStatus.LockedOut:
                    return new AuthorizedUser() { Result = SignInResults.LockedOut };
                case SignInStatus.RequiresVerification:
                    return new AuthorizedUser()
                    {
                        GoogleId = user.GoogleId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Picture = user.Picture,
                        Result = SignInResults.RequiresVerification
                    };
                case SignInStatus.Failure:
                    return new AuthorizedUser() { Result = SignInResults.Failure };
                default:
                    return new AuthorizedUser() { Result = SignInResults.Failure };
            }
        }
        //todo correct
        public async Task<CreatingUserResult> CreateAppUser(string fullName, string Email, string password, string uuid, string baseUrl)
        {
            
            var appUser = await UserManager.FindByNameAsync(Email);
            if (appUser != null)
            {
                if (appUser.uuid != null && appUser.uuid != uuid)
                    return CreatingUserResult.uuidMissMatch;
                if (appUser.GoogleId != null && appUser.GoogleId.Length > 0)
                    return CreatingUserResult.googleUser;
                return CreatingUserResult.userExists;
            }
            var user = new ApplicationUser() { UserName = Email, Email = Email, uuid = uuid, FullName = fullName };
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
                    return CreatingUserResult.success;
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    UserManager.Delete(user);
                }
            }
            return CreatingUserResult.fail;
        }
        
        //todo correct
        public async Task<RecoverPassResults> ForgotPassword(string email, string uuid, string baseUrl)
        {
            try
            {
                var appUser = await UserManager.FindByNameAsync(email);
                if (appUser == null)
                    return RecoverPassResults.NotUser;
                if (!appUser.EmailConfirmed)
                    return RecoverPassResults.RequiresVerification;
                if (appUser.uuid != null && appUser.uuid != uuid)
                    return RecoverPassResults.uuidMissMatch;
                string code = await UserManager.GeneratePasswordResetTokenAsync(appUser.Id);
               // code = HttpUtility.UrlEncode(code);

                var callbackUrl = string.Format("{0}/Account/ResetPassword?userId={1}&code={2}", baseUrl, appUser.Id, code);
                await UserManager.SendEmailAsync(appUser.Id, App_GlobalResources.Global.ServerResetPassword, string.Format(App_GlobalResources.Global.ServerConfirmMessage, callbackUrl));
                return RecoverPassResults.Success;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RecoverPassResults.Failure;
            }
        }

        public async Task<CreatingUserResult> CreateGoogleUser(ApplicationUser userInfo)
        {
            var appUser = await UserManager.FindByNameAsync(userInfo.Email);
            if (appUser == null)
            {
                var res = await UserManager.CreateAsync(userInfo);
                if (res.Succeeded)
                {
                    await UserManager.AddToRoleAsync(userInfo.Id, "AppUser");
                    return CreatingUserResult.success;
                }
            }
            else
            {
                if (appUser.uuid == null || appUser.uuid == userInfo.uuid)
                {
                    return (await updateUserInfo(appUser, userInfo)) ? CreatingUserResult.success : CreatingUserResult.fail;
                }
                return CreatingUserResult.uuidMissMatch;
            }
            return CreatingUserResult.fail;
        }
        private async Task<bool> updateUserInfo(ApplicationUser user, ApplicationUser NewUserInfo)
        {
            user.GoogleId = NewUserInfo.GoogleId;
            user.Picture = NewUserInfo.Picture;
            user.FullName = NewUserInfo.FullName;
            user.gender = NewUserInfo.gender;
            user.EmailConfirmed = NewUserInfo.EmailConfirmed;
            if (user.uuid == null)
                user.uuid = NewUserInfo.uuid;
            var res = await UserManager.UpdateAsync(user);
            return res.Succeeded;
        }
        private async Task<bool> updateUserUUID(ApplicationUser user, string uuid)
        {
            if (user.uuid == null)
            {
                user.uuid = uuid;
                var res = await UserManager.UpdateAsync(user);
                return res.Succeeded;
            }
            return false;
        }



        public ApplicationUser GetUserByName(string userName)
        {
            try
            {
                return UserManager.FindByName(userName);

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }

        public IList<string> GetUserRoles(ApplicationUser user)
        {
            try
            {
                return UserManager.GetRoles(user.Id);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }
        internal void loginUser(string username)
        {
            var user = UserManager.FindByName(username);
            if (user != null)
                SignInManager.SignIn(user, true, true);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_userManager != null)
                    {
                        _userManager.Dispose();
                        _userManager = null;
                    }

                    if (_signInManager != null)
                    {
                        _signInManager.Dispose();
                        _signInManager = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~AccountTools()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}