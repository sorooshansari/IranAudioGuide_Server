using IranAudioGuide_MainServer.Models;
using System.Web;
using System.Linq;

namespace IranAudioGuide_MainServer.Services
{
    public static class ExtensionMethods {
        public static string UserIPAddress
        {

            get { return HttpContext.Current.Request.UserHostAddress; }
        }
        public  static IPData GetInfoIPAddress(){
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                string response = client.DownloadString("http://ip-api.com/json/" + UserIPAddress);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IPData>(response);
            }
            catch {
                return null; 
            }
        }

     
      //  public static bool IsIran { get { return (GetInfoIPAddress().countryCode == "IR") ? true : false; } }
        public static bool IsForeign { get { return (GetInfoIPAddress().countryCode == "IR") ? false : true; } }

    }
    public class ServiceIpAdress
    {
        //public string UserIPAddress
        //{
        //    get { return HttpContext.Current.Request.UserHostAddress; }
        //}
        public bool IsTheFirstLogin()
        {
            using (var db = new ApplicationDbContext())
            {
                var failer = db.LogUserFailures.Where(x => x.IpAddress == ExtensionMethods.UserIPAddress).FirstOrDefault();
                if (failer == null)
                    return true;
            }
            return false;
        }
        public void RemoveIpadressFailuers()
        {
            using (var db = new ApplicationDbContext())
            {
                var logIP = db.LogUserFailures.Where(x => x.IpAddress == ExtensionMethods.UserIPAddress).ToList();
                if (logIP.Count > 0)
                {
                    db.LogUserFailures.RemoveRange(logIP);
                    db.SaveChanges();
                }
            }
        }
        public void SaveIpadressFailuers()
        {
            using (var db = new ApplicationDbContext())
            {
                var logIP = db.LogUserFailures.Where(x => x.IpAddress == ExtensionMethods.UserIPAddress).ToList();
                if (logIP.Count == 0)
                {
                    db.LogUserFailures.Add(new LogUserFailure() { IpAddress = ExtensionMethods.UserIPAddress });
                    db.SaveChanges();
                }
            }
        }
    }
}