using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IranAudioGuide_MainServer.Models
{
    public class UserViewModel
    {

    }
    public class WebPaymentReqVM
    {
      
        public Guid packageId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsChooesZarinpal { get; set; }
    }
    public class AppPaymentReqVM
    {
        public string email { get; set; }
        public string uuid { get; set; }
        public Guid packageId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsChooesZarinpal { get; set; }
    }
    public class paymentInfoVM
    {
        public string userEmail { get; set; }
        public PackageVM package { get; set; }
    }


}
