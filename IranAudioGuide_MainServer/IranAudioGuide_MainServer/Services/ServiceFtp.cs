using System;
using System.IO;
using System.Net;

namespace IranAudioGuide_MainServer.Services
{
    public class ServiceFtp
    {
        /* Construct Object */
        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;

        public ServiceFtp()
        {
            //host = GlobalPath.hostFtp;
            user = GlobalPath.UsernameFtp;
            pass = GlobalPath.PasswordFtp;
        }

        public bool Upload(System.Web.HttpPostedFileBase postedFile, string fullPath)
        {
            try
            {
                if (string.IsNullOrEmpty(fullPath))
                    return false;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(fullPath));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(user, pass);
                request.UseBinary = true;
                request.KeepAlive = true;

                // Copy the contents of the file to the request stream.

                using (Stream sourceStream = postedFile.InputStream)
                {

                    var FileLen = postedFile.ContentLength;
                    byte[] fileContents = new byte[FileLen];

                    // Read the file into the byte array.
                    sourceStream.Read(fileContents, 0, FileLen);
                    sourceStream.Close();
                    request.ContentLength = fileContents.Length;

                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    if (FtpStatusCode.ClosingData == response.StatusCode)
                    {
                        response.Close();
                        return true;
                    }
                    else
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("ftp when uload  StatusCode: " + response.StatusCode));
                        return false;
                        //Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                    }
                }

                // return false;
            }
            catch (WebException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }

        }


        public bool Copy(string pathSource, string pathDestination)
        {
            try
            {
                var fullPathSource = pathSource;
                var fullPathDestination = pathDestination;
                var downloadRequest = (FtpWebRequest)FtpWebRequest.Create(fullPathSource);
                downloadRequest.Credentials = new NetworkCredential(user, pass);
                downloadRequest.UseBinary = true;
                downloadRequest.UsePassive = true;
                downloadRequest.KeepAlive = true;
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)downloadRequest.GetResponse();
                Stream sourceStream = ftpResponse.GetResponseStream();

                byte[] fileContents;
                int FileLen;
                using (var streamReader = new MemoryStream())
                {
                    sourceStream.CopyTo(streamReader);
                    fileContents = streamReader.ToArray();
                    FileLen = fileContents.Length;
                }


                //upload
                var uploadRequest = (FtpWebRequest)WebRequest.Create(new Uri(fullPathDestination));
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
                uploadRequest.Credentials = new NetworkCredential(user, pass);
                uploadRequest.UseBinary = true;
                uploadRequest.KeepAlive = true;


                Stream requestStream = uploadRequest.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)uploadRequest.GetResponse();
                var f = response.StatusCode;
                sourceStream.Close();
                sourceStream.Close();

                if (FtpStatusCode.ClosingData == response.StatusCode)
                {
                    response.Close();
                    return true;
                }
                else
                    return false;
            }
            catch (WebException e)
            {
                String status = ((FtpWebResponse)e.Response).StatusCode.ToString();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        /* Delete File */
        public bool delete(string deleteFile, string path = null)
        {
            try
            {
                var fullPath = "";
                if (!string.IsNullOrEmpty(path))
                    fullPath = fullPath + path;

                if (string.IsNullOrEmpty(deleteFile))
                    return false;

                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(fullPath + "/" + deleteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;

            }

        }
        public bool delete(string FullPath)
        {
            try
            {


                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(FullPath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;

            }

        }
        /* Rename File */
        public void rename(string currentFileNameAndPath, string newFileName)
        {
            try
            {
                //var fullPath = host + "/" + currentFileNameAndPath
                var fullPath = currentFileNameAndPath;
                /* Create an FTP Request */

                ftpRequest = (FtpWebRequest)WebRequest.Create(fullPath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                /* Rename the File */
                ftpRequest.RenameTo = newFileName;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        /// <summary> Create a New Directory on the FTP Server </summary>
        /// <param name="fullPath">  host + "/" + newDirectory</param>
        public void createDirectory(string fullpath)
        {
            try
            {

                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(fullpath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        /// <summary>Get the Date/Time a File was Created.</summary>
        /// <param name="fullPath">  host + "/" + fileName</param>
        public string getFileCreatedDateTime(string fullPath)
        {
            try
            {

                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(fullPath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string fileInfo = null;
                /* Read the Full Response Stream */
                try { fileInfo = ftpReader.ReadToEnd(); }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return File Created Date Time */
                return fileInfo;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /// <summary> Get the Size of a File </summary>
        /// <param name="fullPath">  host + "/" + fileName</param>

        public string getFileSize(string fullpath)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(fullpath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string fileInfo = null;
                /* Read the Full Response Stream */
                try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return File Size */
                return fileInfo;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /// <summary> List Directory Contents File/Folder Name Only </summary>
        /// <param name="fullPath">  host + "/" + fileName</param>

        public string[] directoryListSimple(string directory)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList; }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Return an Empty string Array if an Exception Occurs */
            return new string[] { "" };
        }

        /// <summary>List Directory Contents in Detail (Name, Size, Created, etc.) </summary>
        /// <param name="fullPath">  host + "/" + fullPathDirectory</param>
        public string[] directoryListDetailed(string fullPathDirectory)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(fullPathDirectory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList; }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Return an Empty string Array if an Exception Occurs */
            return new string[] { "" };
        }
        public bool IsDirectoryExist(string directory, string Fullpath)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(Fullpath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */

                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try
                {
                    while (ftpReader.Peek() != -1)
                    {

                        if (ftpReader.ReadLine() == directory)
                            return true;
                    }
                    return false;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); return false; }
                /* Resource Cleanup */
                finally
                {

                    ftpReader.Close();
                    ftpStream.Close();
                    ftpResponse.Close();
                    ftpRequest = null;
                }
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return false;
            }
        }


    }
}