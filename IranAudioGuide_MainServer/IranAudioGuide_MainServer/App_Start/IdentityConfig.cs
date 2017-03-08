﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using IranAudioGuide_MainServer.Models;
using System.Net.Mail;

namespace IranAudioGuide_MainServer
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                // Plug in your email service here to send an email.
                var credentialUserName = "info@iranaudioguide.com";
                var sentFrom = "info@iranaudioguide.com";
                var pwd = "QQwwee11@@";

                // Configure the client:
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("mail.iranaudioguide.com");

                client.Port = 25;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // Creatte the credentials:
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
                client.EnableSsl = false;
                client.Credentials = credentials;

                System.IO.StreamReader sr = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/UserEmailTemplate.html"));

                string body = sr.ReadToEnd();
                sr.Close();
                body = body.Replace("#message#", message.Body);
                body = body.Replace("#Subject#", message.Subject);
                body = body.Replace("#Date#", DateTime.Now.ToString());

                // Create the message:
                var mail = new MailMessage();
                mail.From = new MailAddress(sentFrom, "Iran Audio Guide");
                //mail.Sender = new MailAddress(sentFrom, "Iran Audio Guide");
                mail.To.Add(new MailAddress(message.Destination));
                mail.Subject = "[IranAudioGuide] " + message.Subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
            catch (Exception ex)
            {
                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }
        public Task SendWithoutTemplateAsync(IdentityMessage message)
        {

            try
            {
                // Plug in your email service here to send an email.
                var credentialUserName = "info@iranaudioguide.com";
                var sentFrom = "info@iranaudioguide.com";
                var pwd = "QQwwee11@@";

                // Configure the client:
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("mail.iranaudioguide.com");

                client.Port = 25;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // Creatte the credentials:
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
                client.EnableSsl = false;
                client.Credentials = credentials;

                // Create the message:
                var mail = new System.Net.Mail.MailMessage(sentFrom, message.Destination);
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
            catch (Exception ex)
            {
                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
