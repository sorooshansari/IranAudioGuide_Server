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
        public Task<ApplicationUser> getUser(string username, string uuid)
        {
            return UserManager.FindByNameAsync(username);
        }
        public async Task<AuthorizedUser> AutorizeAppUser(string username, string password, string uuid)
        {
            var user = await UserManager.FindByNameAsync(username);
            bool res = await UserManager.CheckPasswordAsync(user, password);
            if (res)
            {
                if (user.uuid != uuid)
                    return new AuthorizedUser() { Result = SignInResults.uuidMissMatch };
                return new AuthorizedUser()
                {
                    GoogleId = user.GoogleId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Picture = user.Picture,
                    Result = SignInResults.Success
                };
            }
            return new AuthorizedUser() { Result = SignInResults.Failure };
        }
        public async Task<CreateingUserResult> CreateAppUser(string Email, string password, string uuid, string baseUrl)
        {
            var appUser = await UserManager.FindByNameAsync(Email);
            if (appUser != null)
            {
                if (appUser.uuid != uuid)
                    return CreateingUserResult.uuidMissMatch;
                if (appUser.GoogleId != null && appUser.GoogleId.Length > 0)
                    return CreateingUserResult.googleUser;
                return CreateingUserResult.userExists;
            }
            var user = new ApplicationUser() { UserName = Email, Email = Email, uuid = uuid };
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
                catch (Exception)
                {
                    UserManager.Delete(user);
                }
            }
            return CreateingUserResult.fail;
        }
        public async Task<CreateingUserResult> CreateGoogleUser(ApplicationUser userInfo)
        {
            var appUser = await UserManager.FindByNameAsync(userInfo.Email);
            if (appUser == null)
            {
                var res = await UserManager.CreateAsync(userInfo);
                if (res.Succeeded)
                {
                    await UserManager.AddToRoleAsync(userInfo.Id, "AppUser");
                    return CreateingUserResult.success;
                }
            }
            else
            {
                if (appUser.uuid == null || appUser.uuid == userInfo.uuid)
                {
                    return (await updateUserInfo(appUser, userInfo)) ? CreateingUserResult.success : CreateingUserResult.fail;
                }
                return CreateingUserResult.uuidMissMatch;
            }
            return CreateingUserResult.fail;
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


        public ApplicationUser GetUserByName(string userName)
        {
            try
            {
                return UserManager.FindByName(userName);   
               
            }
            catch
            {
                return null;
            }

        }

        public IList<string> GetUserRoles(ApplicationUser user)
        {
            try
            {
               return  UserManager.GetRoles(user.Id);             
            }
            catch
            {
                return null;
            }

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