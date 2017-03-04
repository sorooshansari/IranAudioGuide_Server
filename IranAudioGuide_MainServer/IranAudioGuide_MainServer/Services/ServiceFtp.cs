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
        private int bufferSize = 2048;
        public ServiceFtp()
        {
            host = GlobalPath.hostFtp;
            user = GlobalPath.UsernameFtp;
            pass = GlobalPath.PasswordFtp;
        }

        public bool Upload(System.Web.HttpPostedFileBase postedFile, string nameFile = null, string path = null)
        {
            try
            {
                var fullPath = host;
                if (!string.IsNullOrEmpty(path))
                    fullPath = fullPath + "/" + path;

                if (string.IsNullOrEmpty(nameFile))
                    fullPath = fullPath + "/" + postedFile.FileName;
                else
                    fullPath = fullPath + "/" + nameFile;


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
                        return false;
                    //Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                }

                return true;
            }
            catch (WebException ex)
            {
                //Console.WriteLine(ex.ToString());
                return false;
            }

        }
        public bool Upload(Stream sourceStream, string fullPathSource)
        {
            try
            {

                //    FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(new Uri(fullPathSource));
                //    uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
                //    uploadRequest.Credentials = new NetworkCredential(user, pass);
                //    uploadRequest.UseBinary = true;
                //    uploadRequest.KeepAlive = true;

                //    var s = getFileSize(fullPathSource);
                //    uploadRequest.ContentLength = int.Parse(s);

                //    Stream requestStream = uploadRequest.GetRequestStream();
                //    requestStream.Write(fileContents, 0, fileContents.Length);
                //    requestStream.Close();
                //    FtpWebResponse response = (FtpWebResponse)uploadRequest.GetResponse();
                //    //Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                //    response.Close();
                //    sourceStream.Close();
                return true;
            }
            catch (WebException ex)
            {
                //Console.WriteLine(ex.ToString());
                return false;
            }

        }

        public bool Copy(string pathSource, string pathDestination)
        {
            try
            {
                var fullPathSource = GlobalPath.hostFtp + "/" + pathSource;
                var fullPathDestination = GlobalPath.hostFtp + "/" + pathDestination;

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
                FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(new Uri(fullPathDestination));
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
                var fullPath = host;
                if (!string.IsNullOrEmpty(path))
                    fullPath = fullPath + "/" + path;

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
        public bool delete(string path)
        {
            try
            {
                var fullPath = host + "/" + path;

                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(fullPath);
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
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + currentFileNameAndPath);
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

        /* Create a New Directory on the FTP Server */
        public void createDirectory(string newDirectory)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + newDirectory);
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

        /* Get the Date/Time a File was Created */
        public string getFileCreatedDateTime(string fileName)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
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

        /* Get the Size of a File */
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

        /* List Directory Contents File/Folder Name Only */
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

        /* List Directory Contents in Detail (Name, Size, Created, etc.) */
        public string[] directoryListDetailed(string directory)
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
        public bool IsDirectoryExist(string directory, string path)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + path);
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
                return false;

                Console.WriteLine(ex.ToString());
            }
        }


        ///* Upload File */
        //public void Upload(string filePath, string filename)
        //{
        //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(host + "/" + filename));
        //    request.Method = WebRequestMethods.Ftp.UploadFile;

        //    // This example assumes the FTP site uses anonymous logon.
        //    request.Credentials = new NetworkCredential(user, pass);
        //    request.UseBinary = true;
        //    request.KeepAlive = true;

        //    // Copy the contents of the file to the request stream.
        //    StreamReader sourceStream = new StreamReader(filePath);
        //    byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //    sourceStream.Close();
        //    request.ContentLength = fileContents.Length;

        //    Stream requestStream = request.GetRequestStream();
        //    requestStream.Write(fileContents, 0, fileContents.Length);
        //    requestStream.Close();

        //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        //    Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

        //    response.Close();


        //}
        ///* Upload File */

        ///* Upload File */
        //public void upload(string remoteFile, string localFile)
        //{
        //    try
        //    {
        //        /* Create an FTP Request */
        //        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
        //        /* Log in to the FTP Server with the User Name and Password Provided */
        //        ftpRequest.Credentials = new NetworkCredential(user, pass);
        //        /* When in doubt, use these options */
        //        ftpRequest.UseBinary = true;
        //        ftpRequest.UsePassive = true;
        //        ftpRequest.KeepAlive = true;
        //        /* Specify the Type of FTP Request */
        //        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
        //        /* Establish Return Communication with the FTP Server */
        //        ftpStream = ftpRequest.GetRequestStream();
        //        /* Open a File Stream to Read the File for Upload */
        //        FileStream localFileStream = new FileStream(localFile, FileMode.Create);
        //        /* Buffer for the Downloaded Data */
        //        byte[] byteBuffer = new byte[bufferSize];
        //        int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
        //        /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
        //        try
        //        {
        //            while (bytesSent != 0)
        //            {
        //                ftpStream.Write(byteBuffer, 0, bytesSent);
        //                bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
        //            }
        //        }
        //        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        //        /* Resource Cleanup */
        //        localFileStream.Close();
        //        ftpStream.Close();
        //        ftpRequest = null;
        //    }
        //    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        //    return;
        //}

        ///* Download File */
        //public void download(string remoteFile, string localFile)
        //{
        //    try
        //    {
        //        /* Create an FTP Request */
        //        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
        //        /* Log in to the FTP Server with the User Name and Password Provided */
        //        ftpRequest.Credentials = new NetworkCredential(user, pass);
        //        /* When in doubt, use these options */
        //        ftpRequest.UseBinary = true;
        //        ftpRequest.UsePassive = true;
        //        ftpRequest.KeepAlive = true;
        //        /* Specify the Type of FTP Request */
        //        ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
        //        /* Establish Return Communication with the FTP Server */
        //        ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
        //        /* Get the FTP Server's Response Stream */
        //        ftpStream = ftpResponse.GetResponseStream();
        //        /* Open a File Stream to Write the Downloaded File */
        //        FileStream localFileStream = new FileStream(localFile, FileMode.Create);
        //        /* Buffer for the Downloaded Data */
        //        byte[] byteBuffer = new byte[bufferSize];
        //        int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
        //        /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
        //        try
        //        {
        //            while (bytesRead > 0)
        //            {
        //                localFileStream.Write(byteBuffer, 0, bytesRead);
        //                bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
        //            }
        //        }
        //        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        //        /* Resource Cleanup */
        //        localFileStream.Close();
        //        ftpStream.Close();
        //        ftpResponse.Close();
        //        ftpRequest = null;
        //    }
        //    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        //    return;
        //}



    }
}