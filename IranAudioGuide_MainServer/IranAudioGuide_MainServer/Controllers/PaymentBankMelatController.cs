using Elmah;
using IranAudioGuide_MainServer.Models;
using IranAudioGuide_MainServer.Models_v2;
using System;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Services;
using System.Linq;
using IranAudioGuide_MainServer.App_GlobalResources;
using BankMellatLibrary;

namespace IranAudioGuide_MainServer.Controllers
{
    public class PaymentBankMelatController : BaseController
    {

        private string GetStringEnum(ReturnCodeForBpPayRequest msg)
        {
            return msg.GetDisplayName();
        }
        private ActionResult PaymentFailed(string Message, string Description, bool isFromWeb, string fakemsg = null)
        {
            ViewBag.SaleReferenceId = "**************";

            ViewBag.Message = Message;
            if (!string.IsNullOrEmpty(fakemsg))
                ViewBag.Msg = fakemsg;

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

        public ActionResult Return()
        {
            return View();
        }
        [ActionName("BankMelatCallback")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult BankMelatCallback()
        {


            bool Run_bpReversalRequest = true;
            //string ResCode, string RefId, string SaleOrderId, string SaleRefrenceId
            var bankMellatImplement = new BankMellatIService();

            //كد مرجع تراكنش خريد كه از سايت بانك به پذيرنده داده می شود

            var SaleReferenceId = string.IsNullOrEmpty(Request.Params["SaleReferenceId"]) ? -999 : Request.Params["SaleReferenceId"].ConvertToLong();

            //شماره درخواست پرداخت
            var SaleOrderId = string.IsNullOrEmpty(Request.Params["SaleOrderId"]) ? -999 : Request.Params["SaleOrderId"].ConvertToInt();
            //وضیعت خرید
            string ResCode = string.IsNullOrEmpty(Request.Params["ResCode"]) ? null : Request.Params["ResCode"].ToString();
            //token
            string RefId = string.IsNullOrEmpty(Request.Params["RefId"]) ? null : Request.Params["RefId"].ToString();
            string SaleRefrenceId = string.IsNullOrEmpty(Request.Params["SaleRefrenceId"]) ? null : Request.Params["SaleRefrenceId"].ToString();

            var msgPayment = "--return To webSite";
            try
            {
                ServicePayment.UpdatePayment(SaleOrderId, RefId, ResCode, msgPayment, Request.Params["SaleReferenceId"]);

                //كد مرجع تراكنش خريد كه از سايت بانك به پذيرنده داده مي شود
                //کد رهگیری
                if (string.IsNullOrEmpty(ResCode) || string.IsNullOrEmpty(RefId)
                   || SaleReferenceId == -999 || SaleOrderId == -999)
                {
                    var errorMsg = "return to  website but Not fund Payment. and Request.Params::: " + Request.Params;
                    ErrorSignal.FromCurrentContext().Raise(new Exception(errorMsg));
                    return PaymentFailed(Global.PaymentMsg, Global.ErrorInvalidRequest, true);
                }

                ReturnCodeForBpPayRequest ResCodeEnum = ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>();
                string msg = GetStringEnum(ResCodeEnum);

                if (ResCode != ((int)ReturnCodeForBpPayRequest.Success).ToString())
                {
                    return PaymentFailed(Global.PaymentMsg, msg, true);

                }

                // orderId می تونیم همان saleOrderId استفاده کنیم 
                ResCode = bankMellatImplement.VerifyRequest(SaleOrderId, SaleOrderId, SaleReferenceId);
                msgPayment += " -- " + ResCode + "--step VerifyRequest";
                ServicePayment.UpdatePayment(SaleOrderId, RefId, ResCode, msgPayment);


                if (ResCode != ((int)ReturnCodeForBpPayRequest.Success).ToString())
                {
                    ResCode = bankMellatImplement.InquiryRequest(SaleOrderId, SaleOrderId, SaleReferenceId);
                    ResCodeEnum = ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>();
                    msg = GetStringEnum(ResCodeEnum);

                    msgPayment += " -- " + ResCode + "--step InquiryRequest ";
                    ServicePayment.UpdatePayment(SaleOrderId, RefId, ResCode, msgPayment);
                    if (ResCodeEnum != ReturnCodeForBpPayRequest.Success)
                        return PaymentFailed(Global.PaymentMsg, msg, true);
                }

                ResCode = bankMellatImplement.SettleRequest(SaleOrderId, SaleOrderId, SaleReferenceId);
                ResCodeEnum = ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>();
                msg = GetStringEnum(ResCodeEnum);
                msgPayment += " -- " + ResCode + "--step SettleRequest ";
                ServicePayment.UpdatePayment(SaleOrderId, RefId, ResCode, msgPayment);

                if (ResCode == ((int)ReturnCodeForBpPayRequest.Success).ToString() ||
                    ResCode == ((int)ReturnCodeForBpPayRequest.TransactionSettled).ToString())
                {
                    ViewBag.SaleReferenceId = SaleReferenceId;
                    ViewBag.Message = Global.Paymentsuccessfully;
                    ViewBag.ErrDesc = Global.ZarinpalPaymentMsg5;
                    ViewBag.Succeeded = true;



                    var result = ServicePayment.FinshPyment(SaleOrderId, ResCode);

                    Run_bpReversalRequest = false;
                    var emailModel = new SendEmailForPaymentVM();
                    if (result != null)
                    {

                        emailModel.Price = result.Pay_Amount.ToString();
                        emailModel.Date = result.Pay_Procurement.Pro_InsertDatetime;
                        if (result.Pay_Procurement.Pro_Package != null)
                        {
                            emailModel.Pac_Name = result.Pay_Procurement.Pro_Package.Pac_Name;
                            emailModel.Cities = result.Pay_Procurement.Pro_Package.Pac_Cities == null ? null : result.Pay_Procurement.Pro_Package.Pac_Cities.Select(x => x.Cit_Name).ToList();

                        }
                        else if (result.Pay_Procurement.Pro_TrcPlace != null)
                            emailModel.Pac_Name = result.Pay_Procurement.Pro_TrcPlace.TrP_Name;


                        SendEmailForPurcument(emailModel);
                    }
                    ViewBag.Price = emailModel.Price;
                    ViewBag.Packname = emailModel.Pac_Name;

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
                return PaymentFailed(Global.ZarinpalPaymentMsg6, Global.ZarinpalPaymentMsg7, true, "error");
            }
            finally
            {
                if (Run_bpReversalRequest) //ReversalRequest
                {
                    bankMellatImplement.BpReversalRequest(SaleOrderId, SaleOrderId, SaleReferenceId);
                    msgPayment += " -- " + ResCode + "--ReversalRequest";
                    ServicePayment.UpdatePayment(SaleOrderId, RefId, ResCode, msgPayment);

                    // Save information to Database...
                }
            }

        }
    }
}