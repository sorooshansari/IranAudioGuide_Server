using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer.Controllers
{
    public abstract class BaseController : Controller
    {

       
        public int GetlangFromCookie
        {
            get
            {
                return ServiceCulture.GeLangFromCookie();
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
           
            ServiceCulture.SetCulture();
            return base.BeginExecuteCore(callback, state);
        }
    }
}