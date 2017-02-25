using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IranAudioGuide_MainServer.Models;

namespace IranAudioGuide_MainServer.Controllers
{
    public class WebmoneyResultController : ApiController
    {
        // POST: api/WebmoneyResult
        [HttpPost]
        public void Index(WMReturnModel model)
        {
            var wmService = new WebmoneyServices();
            wmService.ReturnModel = model;
            wmService.ProccessResult();
            return;
        }
    }
}
