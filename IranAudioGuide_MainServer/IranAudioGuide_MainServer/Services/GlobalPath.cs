﻿
namespace IranAudioGuide_MainServer
{
    public static class GlobalPath
    {
        //pourmand
        public static readonly string ConnectionString = @"Data Source=DESKTOP-KUDE22P\POURMANDDB;Initial Catalog=iranaud1_admin;Integrated Security=True";
        //  public static readonly string ConnectionString = @"Password = Law!z323; Persist Security Info=True;User ID = iranaud1_test; Initial Catalog = iranaud1_test; Data Source =88.99.137.107\MSSQLSERVER2014";

        //sina Connection
        //public static readonly string ConnectionString = @"Data Source = (localdb)\MSSQLLocalDB;Initial Catalog = IranAudTest; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //Danial Connection
        //public static readonly string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True";
        //Soroosh Connection
        //  public static readonly string ConnectionString = @"Data Source=DESKTOP-PA8TBNK\SOROOSH;Initial Catalog=iranaudi_test2;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static readonly string ConnectionStringElmah = @"Password = Zcvd14?3; Persist Security Info=True;User ID = iranaudi_Elmah; Initial Catalog = iranaudi_Elmah; Data Source = 185.55.224.3";
        // public static readonly string ConnectionString = @"Password = l9*Eav61i783!0o; Persist Security Info=True;User ID = iranaud1_admin; Initial Catalog = iranaud1_db; Data Source = 88.99.137.107\MSSQLSERVER2012";
        public static readonly string hostFtp = "ftp://iranaudioguide.net/test"; // "ftp://iranaudioguide.com/test.iranaudioguide.com";
        public static readonly string host = "https://iranaudioguide.net/test";// "http://test.iranaudioguide.com";
        public static readonly string UsernameFtp = "dlftp@iranaudioguide.net";
        public static readonly string PasswordFtp = "TD[kKD~q=rGr";

        public static readonly string FullPathImageCity = host + "/Files/Cities/";
        public static readonly string FullPathImagePlace = host + "/Files/Places/";
        public static readonly string FullPathImageExtras = host + "/Files/Places/Extras/";
        public static readonly string FullPathImageTumbnail = host + "/Files/Places/TumbnailImages/";
        public static readonly string FullPathImageGallery = host + "/Files/Places/Gallery/";


        public static readonly string FtpPathImageCity = hostFtp + "/Files/Cities/";
        public static readonly string FtpPathImagePlace = hostFtp + "/Files/Places/";
        public static readonly string FtpPathImageExtras = hostFtp + "/Files/Places/Extras/";
        public static readonly string FtpPathImageTumbnail = hostFtp + "/Files/Places/TumbnailImages/";
        public static readonly string FtpPathImageGallery = hostFtp + "/Files/Places/Gallery/";



        public static readonly string FtpPrimaryPathStory = hostFtp + "/Files/Stories/";
        public static readonly string FtpPrimaryPathAudios = hostFtp + "/Files/Audios/";

        public static readonly string PrimaryPathStory = "/Files/Stories/";
        public static readonly string PrimaryPathAudios = "/Files/Audios/";

        public static readonly string DownloadPathStory = "/Download/Stories/";
        public static readonly string DownloadPathAudios = "/Download/Audios/";
        public static readonly string ImagePath = host + "/Images";

        public static readonly string CryptographyKey = "2017SorooshDeveloperTeamKey;)";

        public static readonly string EmailAddress = "info@iranaudioguide.com";
        public static readonly string CredentialUserName = "info@iranaudioguide.com";
        public static readonly string EmailAddressPassword = "QQwwee11@@";
        public static readonly string ServerEmail = "mail.iranaudioguide.com";
        public static readonly string FtpPathQRCode = hostFtp + "/Images/Barcodes/";
        public static readonly string PathQRCode = host + "/Images/Barcodes/";
        //public static readonly string  = "";
        //public static readonly string  = "";
        //public static readonly string  = "";

    }
}