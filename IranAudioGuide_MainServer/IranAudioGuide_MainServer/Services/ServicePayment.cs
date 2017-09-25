
using IranAudioGuide_MainServer.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace IranAudioGuide_MainServer.Services
{
    public static class ServicePayment
    {

        public static Payment Insert(string UserName, Guid packageId, EnumBankName bankName, ApplicationDbContext db)
        {


            var user = db.Users.FirstOrDefault(x => x.UserName == UserName);
            var package = db.Packages.FirstOrDefault(x => x.Pac_Id == packageId);
            var price = package.Pac_Price;

            var Payment = new Payment()
            {
                Pay_Amount = price,
                Pay_SaleReferenceId = 0,
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
        public static void UpdatePayment(int paymentId, string vresult, long saleReferenceId, string refId, bool paymentFinished = false)
        {
            using (var db = new ApplicationDbContext())
            {
                var procurement = db.Procurements
              .Include(x => x.Pro_Payment)
              .FirstOrDefault(x => x.Pro_Payment.Pay_Id == paymentId);

                if (procurement != null)
                {
                    procurement.Pro_Payment.Pay_StatusPayment = vresult;
                    procurement.Pro_Payment.Pay_SaleReferenceId = saleReferenceId;
                    procurement.Pro_PaymentFinished = paymentFinished;
                    if (refId != null)
                        procurement.Pro_Payment.Pay_ReferenceNumber = refId;

                    db.Entry(procurement).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    Exception ex = new Exception("UpdatePayment:  dont find procurement for IdPayment ");
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    // اطلاعاتی از دیتابیس پیدا نشد
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

    }
}