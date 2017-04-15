
namespace IranAudioGuide_MainServer.Services
{
    public static class GlobalPath
    {
        // public static readonly string ConnectionString = @"Data Source=DESKTOP-UG1254U;Initial Catalog=iranaudi_test1;Integrated Security=True";
        public static readonly string ConnectionStringElmah = "Password = Zcvd14?3; Persist Security Info=True;User ID = iranaudi_Elmah; Initial Catalog = iranaudi_Elmah; Data Source = 185.55.224.3";
       public static readonly string ConnectionString = "Password = 1Kr?g4e7; Persist Security Info=True;User ID = iranaud1_admin; Initial Catalog = iranaud1_db; Data Source = 164.138.23.164";
        public static readonly string hostFtp = "ftp://lnx1.morvahost.com"; // "ftp://iranaudioguide.com/test.iranaudioguide.com";
        public static readonly string host = "http://iranaudioguide.net";// "http://test.iranaudioguide.com";
        public static readonly string UsernameFtp = "admin@iranaudioguide.net";//"pourmand";
        public static readonly string PasswordFtp = "QQwwee11@@"; // "QQwwee11@@";

        public static readonly string FullPathImageCity = host + "/Files/Cities/";
        public static readonly string FullPathImagePlace = host + "/Files/Places/";
        public static readonly string FullPathImageExtras = host + "/Files/Places/Extras/";
        public static readonly string FullPathImageTumbnail = host + "/Files/Places/TumbnailImages/";

        

        public static readonly string FtpPrimaryPathStory = hostFtp + "/Files/Stories/";
        public static readonly string FtpPrimaryPathAudios = hostFtp + "/Files/Audios/";

        public static readonly string PrimaryPathStory = "/Files/Stories/";
        public static readonly string PrimaryPathAudios = "/Files/Audios/";

        public static readonly string DownloadPathStory = "/Download/Stories/";
        public static readonly string DownloadPathAudios = "/Download/Audios/";
        
    }
}