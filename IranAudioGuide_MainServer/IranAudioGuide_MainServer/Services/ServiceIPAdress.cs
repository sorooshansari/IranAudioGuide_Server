using IranAudioGuide_MainServer.Models;
using System.Web;
using System.Linq;

namespace IranAudioGuide_MainServer.Services
{
    public class ServiceIpAdress
    {
        public string UserIPAddress
        {
            get { return HttpContext.Current.Request.UserHostAddress; }
        }
        public bool IsIpadressFailuers()
        {
            using (var db = new ApplicationDbContext())
            {
                var failer = db.LogUserFailures.Where(x => x.IpAddress == UserIPAddress).FirstOrDefault();
                if (failer == null)
                    return true;
            }
            return false;
        }
        public void RemoveIpadressFailuers()
        {
            using (var db = new ApplicationDbContext())
            {
                var logIP = db.LogUserFailures.Where(x => x.IpAddress == UserIPAddress).ToList();
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
                var logIP = db.LogUserFailures.Where(x => x.IpAddress == UserIPAddress).ToList();
                if (logIP.Count == 0)
                {
                    db.LogUserFailures.Add(new LogUserFailure() { IpAddress = UserIPAddress});
                    db.SaveChanges();
                }
            }
        }
    }
}