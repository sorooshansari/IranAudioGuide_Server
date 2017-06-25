using System;
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
using IranAudioGuide_MainServer.Services;
using IranAudioGuide_MainServer.Models_v2;
using IranAudioGuide_MainServer.App_GlobalResources;

namespace IranAudioGuide_MainServer
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendEmail(SendEmailForPaymentVM Message)
        {

            var body = Message.Body;
           


            if (Message.Lang.Contains("fa"))
            {

                var pCalendar = new System.Globalization.PersianCalendar();
                DateTime a = DateTime.Now;
                int year = pCalendar.GetYear(Message.Date);
                int month = pCalendar.GetMonth(Message.Date);
                int day = pCalendar.GetDayOfMonth(Message.Date);

                body = body.Replace("#Date#", year + "/" + month + "/" + day);
                body = body.Replace("#langStyle#", $"style=\"font-family: Tahoma;  direction:rtl\"");
            }
            else
            {
                body = body.Replace("#Date#", Message.Date.ToString());
                body = body.Replace("#langStyle#", $"style=\"font-family:'Open Sans'\"");
            }


            var listCity = "";
            foreach (var item in Message.Cities)
            {
                listCity = listCity + string.Format("<p style='font-size: 21px;background-color:#15437f;color:#ffffff;margin:3% auto;padding:4px 12px;box-shadow: 1px 1px 1px #15437f;font-weight:bold;display: table;min-width:201px;text-shadow: 1px 1px 2px #171514'>{0}<p>", item);
            }
            body = body.Replace("#CityItem#", listCity);

            var msg = new IdentityMessage() { Body = body, Destination = Message.Destination, Subject = Message.Subject };
            try
            {
                // Plug in your email service here to send an email.
                var credentialUserName = GlobalPath.CredentialUserName;
                var sentFrom = GlobalPath.EmailAddress;
                var pwd = GlobalPath.EmailAddressPassword;

                // Configure the client:
                SmtpClient client = new SmtpClient(GlobalPath.ServerEmail);

                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // Creatte the credentials:
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
                client.EnableSsl = false;
                client.Credentials = credentials;

                // Create the message:
                var mail = new MailMessage(sentFrom, Message.Destination);
                mail.Subject = Message.Subject;
                mail.Body = Message.Body;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
            catch
            {
                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                // Plug in your email service here to send an email.
                var credentialUserName = GlobalPath.CredentialUserName;
                var sentFrom = GlobalPath.EmailAddress;
                var pwd = GlobalPath.EmailAddressPassword;

                // Configure the client:
                SmtpClient client = new SmtpClient(GlobalPath.ServerEmail);

                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
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
            catch 
            {
                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }
        public Task SendAsync2(IdentityMessage message)
        {
            try
            {
                // Plug in your email service here to send an email.
                var credentialUserName = GlobalPath.CredentialUserName;
                var sentFrom = GlobalPath.EmailAddress;
                var pwd = GlobalPath.EmailAddressPassword;

                // Configure the client:
                SmtpClient client = new SmtpClient(GlobalPath.ServerEmail);

                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // Creatte the credentials:
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
                client.EnableSsl = false;
                client.Credentials = credentials;


                // Create the message:
                var mail = new MailMessage();
                mail.From = new MailAddress(sentFrom, "Iran Audio Guide");
                mail.To.Add(new MailAddress(message.Destination));
                mail.Subject = "[IranAudioGuide] " + message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch
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
                var credentialUserName = GlobalPath.CredentialUserName;
                var sentFrom = GlobalPath.EmailAddress;
                var pwd = GlobalPath.EmailAddressPassword;

                // Configure the client:
                SmtpClient client = new SmtpClient(GlobalPath.ServerEmail);

                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // Creatte the credentials:
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
                client.EnableSsl = false;
                client.Credentials = credentials;

                // Create the message:
                var mail = new MailMessage(sentFrom, message.Destination);
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
            catch { 
                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }
        public Task SendEmailTemplateAsync(IdentityMessage message)
        {
            try
            {
                var credentialUserName = GlobalPath.CredentialUserName;
                var sentFrom = GlobalPath.EmailAddress;
                var pwd = GlobalPath.EmailAddressPassword;

                // Configure the client:
                SmtpClient client = new SmtpClient(GlobalPath.ServerEmail);

                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // Creatte the credentials:
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
                client.EnableSsl = false;
                client.Credentials = credentials;


                // Create the message:
                var mail = new MailMessage();
                mail.From = new MailAddress(sentFrom, "Iran Audio Guide");
                //mail.Sender = new MailAddress(sentFrom, "Iran Audio Guide");
                mail.To.Add(new MailAddress(message.Destination));
                mail.Subject = "[IranAudioGuide] " + message.Subject;
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
