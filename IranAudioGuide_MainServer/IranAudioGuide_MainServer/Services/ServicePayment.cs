
using IranAudioGuide_MainServer.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace IranAudioGuide_MainServer.Services
{
    public static class ServicePayment
    {

        public static bool IsDuplicatePayment(Guid id, string UserName, bool IsPlace)
        {

            using (var db = new ApplicationDbContext())
            {
                var getItem = db.Procurements.Include(x => x.Pro_User)
                    .Where(x => x.Pro_User.UserName == UserName && x.Pro_PaymentFinished); ;
                if (IsPlace)
                    getItem = getItem.Where(x => x.Pro_TrcPlaceId == id);
                else
                    getItem = getItem.Where(x => x.Pac_Id == id);
                return getItem.Count() != 0;

            }
        }
        public static Payment Insert(string UserName, Guid packageId, EnumBankName bankName, ApplicationDbContext db)
        {


            var user = db.Users.FirstOrDefault(x => x.UserName == UserName);
            var package = db.Packages.FirstOrDefault(x => x.Pac_Id == packageId);
            var price = package.Pac_Price;

            var Payment = new Payment()
            {
                Pay_Amount = price,
                Pay_BankName = (bankName == EnumBankName.Mellat) ? EnumBankName.Mellat.ToString() : EnumBankName.Zarinpal.ToString(),
                Pay_Procurement = new Procurement()
                {
                    Pro_User = user,
                    Pro_Package = package
                }
            };
            db.Payments.Add(Payment);
            db.SaveChanges();
            return Payment;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="refId"> token</param>
        /// <param name="StatusPayment">کد وضعیت خرید</param>
        /// <param name="StatusRequest"></param>
        /// <returns></returns>
        public static bool UpdatePayment(int paymentId,
            string refId,
            string StatusPayment,
            string StatusRequest,
            string saleReferenceId = null)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    var getPayment = db.Payments.FirstOrDefault(x => x.Pay_Id == paymentId);
                    if (getPayment == null)
                    {
                        var errorMsg = "Not found Payment::: paymentId=" + paymentId +
                            "& refId=" + refId
                            + "& StatusPayment=" + StatusPayment
                            + "& StatusRequest=" + StatusRequest;
                        Exception ex = new Exception(errorMsg);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        return false;

                    }
                    getPayment.Pay_RefId = refId;
                    getPayment.Pay_StatusPayment = StatusPayment;
                    getPayment.Pay_StatusRequest = StatusRequest;
                    if (!string.IsNullOrEmpty(saleReferenceId))
                        getPayment.Pay_SaleReferenceId = saleReferenceId;
                    db.Entry(getPayment).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    // اطلاعاتی از دیتابیس پیدا نشد
                    return false;
                }


            }
        }

        public static void Update(Payment item, ApplicationDbContext db = null)
        {
            var dispose = false;
            if (db == null)
            {
                db = new ApplicationDbContext();
                dispose = true;
            }

            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            if (dispose)
                db.Dispose();

        }

        internal static Payment GetById(int paymentId)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Payments.Include("Pay_Procurement.Pro_Package.Pac_Cities")
                                   .FirstOrDefault(c => c.Pay_Id == paymentId);

            }

        }

        internal static Payment FinshPyment(int paymentId, string Status)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var getItem = db.Payments
                        //.Include("Pay_Procurement")
                        ////.Include("Pay_Procurement.Pro_Package")
                        .Include(x=>x.Pay_Procurement.Pro_Package.Pac_Cities)
                        .Include(x=> x.Pay_Procurement.Pro_TrcPlace)
                        .FirstOrDefault(c => c.Pay_Id == paymentId);

                    getItem.Pay_Procurement.Pro_PaymentFinished = true;
                    getItem.Pay_StatusPayment = Status;
                    getItem.Pay_StatusRequest = getItem.Pay_StatusRequest + "---finish";
                    db.Entry(getItem).State = EntityState.Modified;
                    db.SaveChanges();
                    return getItem;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }
}