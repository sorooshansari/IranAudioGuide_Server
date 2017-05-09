using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer.Models
{
    public class CreatBarCodeVM
    {
        [Required]
        public int PriceId { get; set; }
        [Required]
        public int quantity { get; set; }
        public List<SelectListItem> prices { get; set; }
    }
    public class PriceVM
    {
        public int id { get; set; }
        public double value { get; set; }
    }


}