using Elmah;
using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IranAudioGuide_MainServer.Controllers
{
    public class AppManagerV2Controller : ApiController
    {

        dbTools dbTools = new dbTools();
        private AccountTools _acTools;
        public AccountTools acTools
        {
            get
            {
                return _acTools ?? new AccountTools();
            }
            private set
            {
                _acTools = value;
            }
        }
        [HttpPost]
        // POST: api/AppManager/GetUpdates/5
        public GetUpdateVM GetUpdates(int LastUpdateNumber, string uuid)
        {
            GetUpdateVM res;
            try
            {
                res = dbTools.GetUpdate(LastUpdateNumber, uuid);
            }
            catch (Exception ex)
            {
                res = new GetUpdateVM(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return res;
        }
        // POST: api/AppManager/GetAll
        [HttpPost]
        public GetAllVM GetAll(string uuid)
        {
            GetAllVM res;
            try
            {
                res = dbTools.GetAllEntries(uuid);
            }
            catch (Exception ex)
            {
                res = new GetAllVM(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return res;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_acTools != null)
                {
                    _acTools.Dispose();
                    _acTools = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
