using System;
using System.IO;
using System.Net;

namespace dbUpdater.Services
{
    public class ServiceFtp
    {
        /* Construct Object */
        private string host = null;
        private string user = null;
        private string pass = null;

        public ServiceFtp()
        {
            host = GlobalPath.hostFtp;
            user = GlobalPath.UsernameFtp;
            pass = GlobalPath.PasswordFtp;
        }
        public bool delete(string path)
        {
            try
            {
                var fullPath = host + "/" + path;

                /* Create an FTP Request */
                var ftpRequest = (FtpWebRequest)WebRequest.Create(fullPath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                return false;

            }

        }
    }
}