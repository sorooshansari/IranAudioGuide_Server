using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IranAudioGuide_MainServer.Controllers
{
    public class AppManagerController : ApiController
    {
        dbTools dbTools = new dbTools();
        [HttpPost]
        // POST: api/AppManager/GetUpdates/5
        public GetAllVM GetUpdates(int LastUpdateNumber)
        {
            return dbTools.GetUpdate(LastUpdateNumber);
        }
    }
}
