
namespace IranAudioGuide_MainServer.Services
{
    public static class GlobalPath
    {

        public static readonly string host = "ftp://lnx1.morvahost.com"; // "ftp://iranaudioguide.com/test.iranaudioguide.com";
        public static readonly string host2 = "http://iranaudioguide.net";// "http://test.iranaudioguide.com";
        public static readonly string UsernameFtp = "admin@iranaudioguide.net";//"pourmand";
        public static readonly string PasswordFtp = "QQwwee11@@"; // "QQwwee11@@";
        public static readonly string PathStory = "Files/Stories";
        public static readonly string PathAudios = "Files/Audios";
        public static readonly string PathImageCity = "Files/Cities";
        public static readonly string PathImagePlace = "Files/Places";
        public static readonly string PathImageExtras = "Files/Places/Extras";
        public static readonly string PathImageTumbnail = "Files/Places/TumbnailImages";
        public static readonly string FullPathStory = host2 + "/Files/Stories/";
        public static readonly string FullPathAudios = host2 + "/Files/Audios/";
        public static readonly string FullPathImageCity = host2 + "/Files/Cities/";
        public static readonly string FullPathImagePlace = host2 + "/Files/Places/";
        public static readonly string FullPathImageExtras = host2 + "/Files/Places/Extras/";
        public static readonly string FullPathImageTumbnail = host2 + "/Files/Places/TumbnailImages/";
    }
}