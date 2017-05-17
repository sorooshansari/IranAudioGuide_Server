using IranAudioGuide_MainServer.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IranAudioGuide_MainServer
{
    public class LangVM
    {
       
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
    }
    public class langsVM
    {
        public langsVM()
        {
            Get = new List<LangVM>() {
                new LangVM() { Id = "en" , Icon =Global.ImagePath+ "/flag-uk.png" , Title="En"},
                new LangVM() { Id = "fa" , Icon =Global.ImagePath+ "/flag-fa.png" , Title="فا"}
            };

        }
        public List<LangVM> Get{ get; set; }
    }

}