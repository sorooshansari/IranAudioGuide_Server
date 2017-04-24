using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IranAudioGuide_MainServer.Controllers
{
    public class AdminApiController : ApiController
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddPackage(NewPackage model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Check input fields");
            }
            return BadRequest();
           
        }
    }
}
