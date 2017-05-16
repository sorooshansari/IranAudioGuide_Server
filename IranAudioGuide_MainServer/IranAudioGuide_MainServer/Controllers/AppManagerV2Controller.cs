using Elmah;
using IranAudioGuide_MainServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IranAudioGuide_MainServer.Services;


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
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpPost]
        //public BuyWithBarcodeStatus BuyWithBarcode(Guid packId, string email, string uuid, string barcode)
        public BuyWithBarcodeStatus BuyWithBarcode(BuyWithBarcodeVM model)
        {
            try
            {
                var user = acTools.getUser(model.email);
                var status =
                    (user == null) ? BuyWithBarcodeStatus.notUser :
                    (user.uuid != model.uuid) ? BuyWithBarcodeStatus.uuidMissMatch :
                    (!user.EmailConfirmed) ? BuyWithBarcodeStatus.notConfirmed :
                    BuyWithBarcodeStatus.confirmed;
                if (status != BuyWithBarcodeStatus.confirmed)
                {
                    return status;
                }
                ConvertBarcodetoStringVM cbs = new ConvertBarcodetoStringVM();
                using (BarcodeServices brs = new BarcodeServices())
                {
                    try
                    {
                        cbs = brs.ConvertBarcodetoString(model.barcode);
                    }
                    catch (Exception)
                    {
                        var ex = new Exception(string.Format("invalid baicode-->{0}", model.barcode));
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        return BuyWithBarcodeStatus.invalidBarcode;
                    }
                    long packPrice;
                    BarcodeVM bav = new BarcodeVM();
                    packPrice = brs.Getpackage(model.packId);
                    bav = brs.GetBarcodes(cbs.CBS_id_bar);
                    if (cbs.CBS_price_pri != bav.price)
                    {
                        return BuyWithBarcodeStatus.invalidprice;
                    }
                    if (cbs.CBS_sellername != bav.sellerName)
                    {
                        return BuyWithBarcodeStatus.invalidSellerName;
                    }
                    try
                    {
                        if (bav.price != packPrice)
                        {
                            return BuyWithBarcodeStatus.invalidpackprice;
                        }
                        if (bav.isUsed == true)
                        {
                            return BuyWithBarcodeStatus.isused_true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        return BuyWithBarcodeStatus.unknownError;
                    }
                    brs.saved(cbs.CBS_id_bar,user.Id, model.packId);
                    return BuyWithBarcodeStatus.success;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BuyWithBarcodeStatus.unknownError;
            }
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
