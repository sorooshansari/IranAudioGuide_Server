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
                new LangVM() { Id = "en" , Icon ="/Content/images/flag-uk.png" , Title="En"},
                new LangVM() { Id = "fa" , Icon ="/Content/images/flag-fa.png" , Title="فا"}
            };

        }
        public List<LangVM> Get{ get; set; }
    }

}