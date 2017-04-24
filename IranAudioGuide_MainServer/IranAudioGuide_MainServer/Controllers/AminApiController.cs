using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IranAudioGuide_MainServer.Controllers
{
    public class AminApiController : ApiController
    {
        [HttpGet]
        public String TestApi()
        {
            return "welcome";
        }
    }
}
