using System;
using System.ComponentModel.DataAnnotations;

namespace IranAudioGuide_MainServer.Models
{
    public class WebPaymentReqVM
    {

        public Guid packageId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsChooesZarinpal { get; set; }
    }
    public class AppPaymentReqVM

    {
        public AppPaymentReqVM()
        {
            IsChooesZarinpal = true;
        }
        public string email { get; set; }
        public string uuid { get; set; }
        public Guid packageId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsChooesZarinpal { get; set; }
    }

    public class AppPaymentReqVM_v3
    {
        public AppPaymentReqVM_v3()
        {
            IsChooesIranianBC = true;
        }
        [Required]
        public string email { get; set; }

        [Required]
        public string uuid { get; set; }

        [Required]
        public Guid Id { get; set; }

        public string ErrorMessage { get; set; }

        [Required]
        public bool IsChooesIranianBC { get; set; }

        [Required]
        public bool IsPlace { get; set; }

        [Required]
        public int LangId { get; set; }
    }


    public class paymentInfoVM
    {
        public string userEmail { get; set; }
        public PackageVM package { get; set; }
    }


}
