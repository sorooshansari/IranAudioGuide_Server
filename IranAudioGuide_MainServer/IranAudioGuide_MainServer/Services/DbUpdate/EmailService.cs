using IranAudioGuide_MainServer;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DbUpdate
{
    public static class EmailServiceForJob
    {
        public static Task SendEmail(string messsage)
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
                var mail = new MailMessage(sentFrom, "sorosh.ansari@gmail.com");
                mail.To.Add("pourmand2010@gmail.com");
                mail.Subject = "Error Ftp";
                mail.Body = messsage;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
            catch
            {
                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }
    }
}