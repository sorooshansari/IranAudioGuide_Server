using System.Collections.Generic;

namespace IranAudioGuide_MainServer
{
    public class LangVM
    {
       
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string FullTitle { get; set; }
    }
    public class langsVM
    {
        public langsVM()
        {
            Get = new List<LangVM>() {
                new LangVM() { Id = "en" , Icon =GlobalPath.ImagePath+ "/flag-uk-min.png" , Title="En", FullTitle ="English"},
                new LangVM() { Id = "fa" , Icon =GlobalPath.ImagePath+ "/flag-fa-min.png" , Title="فا" , FullTitle= "فارسی"}
            };

        }
        public List<LangVM> Get{ get; set; }
    }

}