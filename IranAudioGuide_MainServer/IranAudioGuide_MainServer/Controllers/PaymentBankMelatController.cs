using BankMellatLibrary;
using IranAudioGuide_MainServer.App_GlobalResources;
using IranAudioGuide_MainServer.Models;
using System;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using IranAudioGuide_MainServer.Services;
using Elmah;
using IranAudioGuide_MainServer.Models_v2;

namespace IranAudioGuide_MainServer.Controllers
{
    public class PaymentBankMelatController : BaseController
    {

        private string GetStringEnum(ReturnCodeForBpPayRequest msg)
        {
            return msg.GetDisplayName();
        }

        public ActionResult Purchase(Guid packageId, bool isFromWeb = false)
        {
            var ServiceBankMellat = new BankMellatIService();
            using (var db = new ApplicationDbContext())
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        string UserName = User.Identity.Name;
                        var count = db.Procurements.Include(x => x.Pro_User)
                                .Count(x => x.Pro_User.UserName == UserName && x.Pac_Id == packageId && x.Pro_PaymentFinished);
                        if (count > 0)
                            return PaymentFailed(Global.PaymentMsg, Global.PaymentMsg1, isFromWeb);


                        var resultPayment = ServicePayment.Insert(UserName, packageId, EnumBankName.Mellat, db);

                        var resultBank = ServiceBankMellat.BpPayRequest(resultPayment.Pay_Id, resultPayment.Pay_Amount, "test");

                        string[] StatusSendRequest = resultBank.Split(',');

                        resultPayment.Pay_StatusPayment = StatusSendRequest[0];

                        if (StatusSendRequest.Length >= 2)
                            resultPayment.Pay_SaleReferenceId = StatusSendRequest[1].ConvertToLong();

                        ServicePayment.Update(resultPayment, db);

                        //در صورتی که مقدار خانه اول این آریه برابر صفر در نتیجه می بایست کاربر را به صفحه پرداخت هدایت کنیم
                        if (StatusSendRequest[0].StoI() != (int)ReturnCodeForBpPayRequest.Success)
                        {
                            dbTran.Commit();
                            var msg = GetStringEnum(StatusSendRequest[0].ConvertToEnum<ReturnCodeForBpPayRequest>());
                            return PaymentFailed(Global.PaymentMsg, msg, isFromWeb);

                        }
                        Response.Clear();
                        var sb = new System.Text.StringBuilder();
                        sb.Append("<html>");
                        sb.AppendFormat("<body onload='document.forms[0].submit()'>");
                        sb.AppendFormat("<form action='{0}' method='post'>", ServiceBankMellat.PgwSite);
                        sb.AppendFormat("<input type='hidden' name='RefId' value='{0}'>", resultPayment.Pay_SaleReferenceId);
                        sb.Append("</form>");
                        sb.Append("</body>");
                        sb.Append("</html>");
                        Response.Write(sb.ToString());
                        Response.End();
                        return View();
                    }


                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        // "Problem occurred in payment process. ";
                        //"Sorry, please try again in a few minutes.";
                        return PaymentFailed(Global.ZarinpalPaymentMsg6, Global.ZarinpalPaymentMsg7, isFromWeb);

                    }
                }
            }
        }

        private ActionResult PaymentFailed(string Msg, string Description, bool isFromWeb)
        {
            ViewBag.Message = Msg;
            ViewBag.ErrDesc = Description;
            ViewBag.Succeeded = false;
            //if (isFromWeb)
            return View("ReturnToWebPage");
            //else
            //    return View("Return");
        }

        public ActionResult ReturnToWebPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BankMelatCallback()
        {
            bool Run_bpReversalRequest = true;

            //ﻛﺪ ﻣﺮﺟﻊ ﺗﺮاﻛﻨﺶ ﺧﺮﻳﺪ ﻛﻪ از ﺳﺎﻳﺖ ﺑﺎﻧﻚ ﺑﻪ ﭘﺬﻳﺮﻧﺪه داده ﻣﻲ ﺷﻮد
            long saleReferenceId = -999;

            //ﺷﻤﺎره درﺧﻮاﺳﺖ ﭘﺮداﺧﺖ
            long saleOrderId = -999;

            var bankMellatImplement = new BankMellatIService();

            try
            {

                saleReferenceId = Request.Params["SaleReferenceId"].ToString().ConvertToLong();
                saleOrderId = Request.Params["SaleOrderId"].ToString().ConvertToLong();
                ////وضیعت خرید
                string ResCode = Request.Params["ResCode"].ToString();

                ReturnCodeForBpPayRequest ResCodeEnum = ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>();
                string msg = GetStringEnum(ResCodeEnum);

                if (ResCodeEnum != ReturnCodeForBpPayRequest.Success)
                    return PaymentFailed(Global.PaymentMsg, GetStringEnum(ResCodeEnum), true);


                // orderId می تونیم همان saleOrderId استفاده کنیم 
                ResCode = bankMellatImplement.VerifyRequest(saleOrderId, saleOrderId, saleReferenceId);

                if (ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>() != ReturnCodeForBpPayRequest.Success)
                {
                    ResCode = bankMellatImplement.InquiryRequest(saleOrderId, saleOrderId, saleReferenceId);
                    ResCodeEnum = ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>();
                    msg = GetStringEnum(ResCodeEnum);

                    if (ResCodeEnum != ReturnCodeForBpPayRequest.Success)
                        return PaymentFailed(Global.PaymentMsg, msg, true);
                }

                ResCode = bankMellatImplement.SettleRequest(saleOrderId, saleOrderId, saleReferenceId);
                ResCodeEnum = ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>();
                msg = GetStringEnum(ResCodeEnum);

                if (ResCodeEnum == ReturnCodeForBpPayRequest.Success ||
                    ResCodeEnum == ReturnCodeForBpPayRequest.TransactionSettled)
                {
                    ViewBag.SaleReferenceId = saleReferenceId;
                    //"Payment completed successfully.";
                    //"You have access to the package below. Thank you for your purchase!";
                    ViewBag.Message = Global.Paymentsuccessfully;
                    ViewBag.ErrDesc = Global.ZarinpalPaymentMsg5;
                    ViewBag.Succeeded = true;


                    var result = ServicePayment.GetById(saleOrderId.ConvertToInt());
                    result.Pay_ReferenceNumber = saleOrderId.ToString();
                    result.Pay_Procurement.Pro_PaymentFinished = true;
                    result.Pay_StatusPayment = ResCode;
                    result.Pay_SaleReferenceId = saleReferenceId;                  
                    ServicePayment.Update(result);

                    Run_bpReversalRequest = true;

                    var emailModel = new SendEmailForPaymentVM()
                    {
                        Price = result.Pay_Amount.ToString(),
                        Pac_Name = result.Pay_Procurement.Pro_Package.Pac_Name,
                        Cities = result.Pay_Procurement.Pro_Package.Pac_Cities.Select(x => x.Cit_Name).ToList(),
                        Date = result.Pay_Procurement.Pro_InsertDatetime
                    };
                    SendEmailForPurcument(emailModel);
                    return View("ReturnToWebPage");

                }
                else
                {
                    return PaymentFailed(Global.PaymentMsg, msg, true);

                }

            }
            catch (Exception ex)
            {
                Run_bpReversalRequest = true;
                ErrorSignal.FromCurrentContext().Raise(ex);

                // "Problem occurred in payment process. ";
                //"Sorry, please try again in a few minutes.";
                return PaymentFailed(Global.ZarinpalPaymentMsg6, Global.ZarinpalPaymentMsg7, true);
            }
            finally
            {
                if (Run_bpReversalRequest) //ReversalRequest
                {
                    if (saleOrderId != -999 && saleReferenceId != -999)
                        bankMellatImplement.BpReversalRequest(saleOrderId, saleOrderId, saleReferenceId);
                    // Save information to Database...
                }
            }

        }
    }
}