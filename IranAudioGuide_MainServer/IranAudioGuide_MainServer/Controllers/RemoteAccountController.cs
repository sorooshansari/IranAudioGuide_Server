using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IranAudioGuide_MainServer.Models;

namespace IranAudioGuide_MainServer.Controllers
{
    public class RemoteAccountController : ApiController
    {
        AccountTools AccountTools = new AccountTools();
        [HttpGet]
        public string LogIn(LoginViewModel model)
        {
            return AccountTools.logIn(model.Email, model.Password);
        }
    }
}
