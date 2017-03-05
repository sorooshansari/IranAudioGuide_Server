using System.IO;

namespace IranAudioGuide_MainServer.Services
{
    public class WriteFile
    {
        public static void WriteSQL(string data)
        {
            string path = @"c:\SQLtrace.txt";
            File.AppendAllText(path, data);
        }
    }
}