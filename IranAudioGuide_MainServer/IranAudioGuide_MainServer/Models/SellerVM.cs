using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace IranAudioGuide_MainServer.Models
{
    public class CreatBarCodeVM
    {
        [Required]
        public double price { get; set; }
        [Required]
        public int quantity { get; set; }
    }


}