using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Services;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    [Localization]

    public class AccountController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            // IsTheFirstLogin check by Angularjs 
            //var serviceIpAdress = new ServiceIpAdress();
            //ViewBag.IsTheFirstLogin = serviceIpAdress.IsTheFirstLogin();
            ViewBag.View = Views.Login;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var serviceIpAdress = new ServiceIpAdress();
            ViewBag.IsTheFirstLogin = serviceIpAdress.IsTheFirstLogin();
            if (!ViewBag.IsTheFirstLogin)
            {
                var response = Request["g-recaptcha-response"];
                if ((response == null || response == "" || !ServiceRecaptcha.IsValid(response)))
                {
                    ModelState.AddModelError("", ServiceRecaptcha.ErrorMessage);
                    return View(model);
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.View = Views.Login;

                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    serviceIpAdress.RemoveIpadressFailuers();
                    var user = UserManager.FindByName(model.Email);
                    string UserRole = UserManager.GetRoles(user.Id).FirstOrDefault();
                    if (UserRole == "Admin")
                        return RedirectToAction("Index", "Admin");
                    else if (UserRole == "AppUser")
                    {
                        if (returnUrl != null && returnUrl.Length > 0)
                            return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "User");
                    }
                    else
                        return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View();
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new
                    {
                        ReturnUrl = returnUrl,
                        RememberMe = model.RememberMe
                    });
                case SignInStatus.Failure:
                    serviceIpAdress.SaveIpadressFailuers();
                    ViewBag.IsTheFirstLogin = false;
                    ModelState.AddModelError("", Resources.Global.ServerErrorInvalidUsernameOrPassword);
                    return View(model);
                default:
                    ModelState.AddModelError("", Resources.Global.ServerInvalidLoginAttempt);
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {

            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", Resources.Global.ServerInvalidCode);
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.View = Views.Register;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.Name };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "AppUser");
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //code = HttpUtility.UrlEncode(code);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, Resources.Global.ServerConfirmTitel, string.Format(Resources.Global.ServerConfirmMessage, callbackUrl));

                    return RedirectToAction("Index", "User");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            //ViewBag.View = Views.Register;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        //   [ValidateAntiForgeryToken]
        public async Task<JsonResult> SendEmailConfirmedAgain()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Json(new Respond("", status.InvalidUsre));


                var UserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(UserId))
                    return Json(new Respond("", status.InvalidUsre));


                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
                //  code = HttpUtility.UrlEncode(code);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = UserId, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(UserId, Resources.Global.ServerConfirmTitel, string.Format(Resources.Global.ServerConfirmMessage, callbackUrl));
                // return new HttpResponseMessage(HttpStatusCode.OK);
                return Json(new Respond(Resources.Global.ServerEmailSendingSuccess, status.success));

            }
            catch (Exception ex)
            {
                return Json(new Respond("", status.unknownError));

            }
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = Resources.Global.ServerErrorConfirmSubject,
                        Message = Resources.Global.ServerErrorUnknown,
                        //IsShowUrl = true
                    });
                }
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = Resources.Global.ServerEmailConfirmation,
                        Message = Resources.Global.ServerEmailConfirmationSucceeded,
                    });
                }
                else
                {
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = Resources.Global.ServerErrorConfirmSubject,
                        Message = Resources.Global.ServerEmailConfirmationMessage2,
                        //IsShowUrl = true
                    });
                }
            }
            catch
            {
                return View("vmessage", new vmessageVM()
                {
                    Subject = Resources.Global.ServerErrorConfirmSubject,
                    Message = Resources.Global.ServerErrorUnknown,
                    //IsShowUrl = true
                });
            }
        }
        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = Resources.Global.ServerForgotPassword,
                        Message = Resources.Global.ServerForgotPasswordMessage,
                    });
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //  code = HttpUtility.UrlEncode(code);
                string baseUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                var callbackUrl = string.Format("{0}/Account/ResetPassword?userId={1}&code={2}", baseUrl, user.Id, code);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, Resources.Global.ServerResetPassword, string.Format(Resources.Global.ServerResetPasswordMessage, callbackUrl));
                string ResetLink = Url.Action("ForgotPassword", "Account", model);
                return View("vmessage", new vmessageVM()
                {
                    Subject = Resources.Global.ServerResetPassword,
                    Message = string.Format(Resources.Global.ServerResetPasswordMessage2, ResetLink, model.Email)
                });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //


        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            return (code == null || userId == null) ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(model.userId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return View("vmessage", new vmessageVM()
                {
                    Subject = Resources.Global.ServerPasswordChanged,
                    Message = Resources.Global.ServerPasswordChangedMessage
                });
            }
            //var code = model.Code.Replace(" ", "+");
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("vmessage", new vmessageVM()
                {
                    Subject = Resources.Global.ServerPasswordChanged,
                    Message = Resources.Global.ServerPasswordChangedMessage
                });
            }
            AddErrors(result);
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            var newUser = new ApplicationUser() { UserName = loginInfo.Email, Email = loginInfo.Email, EmailConfirmed = true };
            var accessToken = loginInfo.ExternalIdentity.Claims.Where(c => c.Type.Equals("urn:google:accesstoken")).Select(c => c.Value).FirstOrDefault();
            Uri apiRequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + accessToken);
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(apiRequestUri);
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                newUser.Picture = result.picture;
                newUser.FullName = result.name;
                newUser.GoogleId = result.id;
                newUser.gender = result.gender;
            }
            var appUser = await UserManager.FindByNameAsync(newUser.Email);
            if (appUser == null)
            {
                var res = await UserManager.CreateAsync(newUser);
                if (res.Succeeded)
                {
                    var d = await UserManager.AddToRoleAsync(newUser.Id, "AppUser");
                    if (!d.Succeeded)
                        return RedirectToAction("Login");
                    await SignInManager.SignInAsync(newUser, true, true);
                    return RedirectToAction("Index", "User");
                }
            }
            else
            {
                appUser.GoogleId = newUser.GoogleId;
                appUser.FullName = newUser.FullName;
                appUser.Picture = newUser.Picture;
                appUser.gender = newUser.gender;
                var d = await UserManager.UpdateAsync(appUser);
                if (!d.Succeeded)
                    return RedirectToAction("Login");
                await SignInManager.SignInAsync(appUser, true, true);
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Login");



            //// Sign in the user with this external login provider if the user already has a login
            //var result1 = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            //switch (result1)
            //{
            //    case SignInStatus.Success:

            //        return Redirect("/user/index");
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
            //    case SignInStatus.Failure:
            //        var user = new ApplicationUser { UserName = loginInfo.Email, Email = loginInfo.Email, EmailConfirmed = true };
            //        var r = await UserManager.CreateAsync(user);
            //        if (!r.Succeeded)
            //            return RedirectToAction("Login");
            //        var d = await UserManager.AddToRoleAsync(user.Id, "AppUser");
            //        if (!d.Succeeded)
            //            return RedirectToAction("Login");
            //        await SignInManager.SignInAsync(user, true, true);
            //        //if (!r.Succeeded)
            //        //    return RedirectToAction("Login");
            //        return RedirectToAction("Index", "User");

            //}
            //return RedirectToAction("Login");
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
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

            base.Dispose(disposing);
        }



        #region Danial
        public ActionResult vmessage(string subject, string Message)
        {

            return View("vmessage", new vmessageVM()
            {
                Subject = subject,
                Message = Message
            });
        }

        #endregion




        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

}


//[AllowAnonymous]
//[HttpPost]
////[ValidateAntiForgeryToken] - Whats the point? F**k security 
//public async Task<ActionResult> LoginUser(string name)
//{

//    var user = await UserManager.FindByNameAsync(name);

//    if (user != null)
//    {

//        await SignInManager.SignInAsync(user, true, true);
//    }

//    return View();
//}