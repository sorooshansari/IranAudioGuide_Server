
namespace IranAudioGuide_MainServer.Services
{
    public static class GlobalPath
    {
        //sina Connection
        public static readonly string ConnectionString = @"Data Source = (localdb)\MSSQLLocalDB;Initial Catalog = IranAudTest; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //Danial Connection
        //public static readonly string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True";
        //Soroosh Connection
        //public static readonly string ConnectionString = @"Data Source=DESKTOP-PA8TBNK\SOROOSH;Initial Catalog=iranaudi_test2;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static readonly string ConnectionStringElmah = "Password = Zcvd14?3; Persist Security Info=True;User ID = iranaudi_Elmah; Initial Catalog = iranaudi_Elmah; Data Source = 185.55.224.3";
      // public static readonly string ConnectionString = "Password = 1Kr?g4e7; Persist Security Info=True;User ID = iranaud1_admin; Initial Catalog = iranaud1_db; Data Source = 164.138.23.164";
        public static readonly string hostFtp = "ftp://lnx1.morvahost.com"; // "ftp://iranaudioguide.com/test.iranaudioguide.com";
        public static readonly string host = "http://iranaudioguide.net";// "http://test.iranaudioguide.com";
        public static readonly string UsernameFtp = "admin@iranaudioguide.net";//"pourmand";
        public static readonly string PasswordFtp = "QQwwee11@@"; // "QQwwee11@@";

        public static readonly string FullPathImageCity = host + "/Files/Cities/";
        public static readonly string FullPathImagePlace = host + "/Files/Places/";
        public static readonly string FullPathImageExtras = host + "/Files/Places/Extras/";
        public static readonly string FullPathImageTumbnail = host + "/Files/Places/TumbnailImages/";

        
        public static readonly string FtpPathImageCity = hostFtp + "/Files/Cities/";
        public static readonly string FtpPathImagePlace = hostFtp + "/Files/Places/";
        public static readonly string FtpPathImageExtras = hostFtp + "/Files/Places/Extras/";
        public static readonly string FtpPathImageTumbnail = hostFtp + "/Files/Places/TumbnailImages/";



        public static readonly string FtpPrimaryPathStory = hostFtp + "/Files/Stories/";
        public static readonly string FtpPrimaryPathAudios = hostFtp + "/Files/Audios/";

        public static readonly string PrimaryPathStory = "/Files/Stories/";
        public static readonly string PrimaryPathAudios = "/Files/Audios/";

        public static readonly string DownloadPathStory = "/Download/Stories/";
        public static readonly string DownloadPathAudios = "/Download/Audios/";
        
    }
}