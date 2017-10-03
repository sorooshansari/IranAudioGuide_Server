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

      
        private ActionResult PaymentFailed(string Message, string Description, bool isFromWeb)
        {
            ViewBag.SaleReferenceId = "**************";
            ViewBag.Message = Message;
            ViewBag.ErrDesc = Description;
            ViewBag.Succeeded = false;
            if (isFromWeb)
                return View("ReturnToWebPage");
            else
                return View("Return");
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