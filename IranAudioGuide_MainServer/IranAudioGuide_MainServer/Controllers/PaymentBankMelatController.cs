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
using System.ComponentModel.DataAnnotations;

namespace IranAudioGuide_MainServer.Controllers
{
    public class PaymentBankMelatController : BaseController
    {
        //private int GetNumberForStatusCode(ReturnCodeForBpPayRequest msg)
        //{
        //    return (int)msg;
        //}
        private string GetStringEnum(ReturnCodeForBpPayRequest msg)
        {
            return msg.GetDisplayName();
            //  return msg.GetDisplay();
        }
        //private ReturnCodeForBpPayRequest GetMessageForStatusCode(string statusCode)
        //{

        //    var id = 413;
        //    try
        //    {
        //        id = int.Parse(statusCode);
        //    }
        //    catch
        //    {
        //        id = 413;
        //    }
        //   return (ReturnCodeForBpPayRequest)id;
        //}
        public ActionResult Purchase(Guid packageId, bool isFromWeb = false)
        {
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
                        var bankMellatService = new BankMellatIService();
                        var resultBank = bankMellatService.BpPayRequest(resultPayment.Pay_Id, resultPayment.Pay_Amount, "test");

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
                        sb.AppendFormat("<form action='{0}' method='post'>", BankMellatLibrary.BankMellatIService.PgwSite);
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
        //public ActionResult Index()
        //{

        //    long orderID = 0; //شماره تراکنش که باید منحصر به فرد باشد
        //    long priceAmount = 20000; // هزینه ایی که کاربر در صفحه پرداخت باید آن را بپردازد
        //    string additionalText = "خرید یک محصول "; // توضیحات شما برای این تراکنش
        //    var bankMellatService = new BankMellatIService();
        //    string resultRequest = bankMellatService.BpPayRequest(orderID, priceAmount, additionalText);
        //    string[] StatusSendRequest = resultRequest.Split(',');
        //    //در صورتی که مقدار خانه اول این آریه برابر صفر در نتیجه می بایست کاربر را به صفحه پرداخت هدایت کنیم
        //    if (StatusSendRequest[0] == "0")
        //    {
        //        return RedirectToAction("PayWithMellat", "Payment", new { id = StatusSendRequest[1] });
        //    }

        //    TempData["Message"] = GetMessageForStatusCode(StatusSendRequest[0]);
        //    return RedirectToAction("ShowError", "Payment");


        //}
        //public ActionResult PayWithMellat(string id)
        //{
        //    try
        //    {
        //        if (id == null)
        //        {
        //            TempData["Message"] = "هیچ شماره پیگیری برای پرداخت از سمت بانک ارسال نشده است!";

        //            return RedirectToAction("ShowError", "Payment");
        //        }
        //        else
        //        {
        //            ViewBag.id = id;
        //            return View();
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        TempData["Message"] = error + "متاسفانه خطایی رخ داده است، لطفا مجددا عملیات خود را انجام دهید در صورت تکرار این مشکل را به بخش پشتیبانی اطلاع دهید";

        //        return RedirectToAction("ShowError", "Payment");
        //    }

        //}

        [HttpPost]
        public ActionResult BankMelatCallback()
        {
            //bool Run_bpReversalRequest = false;

            //ﻛﺪ ﻣﺮﺟﻊ ﺗﺮاﻛﻨﺶ ﺧﺮﻳﺪ ﻛﻪ از ﺳﺎﻳﺖ ﺑﺎﻧﻚ ﺑﻪ ﭘﺬﻳﺮﻧﺪه داده ﻣﻲ ﺷﻮد
            long saleReferenceId = -999;

            //ﺷﻤﺎره درﺧﻮاﺳﺖ ﭘﺮداﺧﺖ
            long saleOrderId = -999;


            try
            {
                //string ResCode_bpPayRequest
                // ﻛﺪ ﻣﺮﺟﻊ درﺧﻮاﺳﺖ ﭘﺮداﺧﺖ ﻛﻪ ﻫﻤﺮاه ﺑﺎ درﺧﻮاﺳﺖ bpPayRequest ﺗﻮﻟﻴد ﺷﺪه اﺳﺖ 
                //و به پذیرنده اختصاص یافته است.
                // var refId = Request.Params["RefId"].ToString();
                var bankMellatImplement = new BankMellatIService();
                saleReferenceId = Request.Params["SaleReferenceId"].ToString().ConvertToLong();
                saleOrderId = Request.Params["SaleOrderId"].ToString().ConvertToLong();
                ////وضیعت خرید
                string ResCode = Request.Params["ResCode"].ToString();

                ReturnCodeForBpPayRequest ResCodeEnum = ResCode.ConvertToEnum<ReturnCodeForBpPayRequest>();
                string msg = GetStringEnum(ResCodeEnum);

                if (ResCodeEnum != ReturnCodeForBpPayRequest.Success)
                    return PaymentFailed(Global.PaymentMsg, GetStringEnum(ResCodeEnum), true);

                #region Success
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



                #region SettleRequest

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

                // Save information to Database...

                #endregion

                #endregion




            }
            catch (Exception Error)
            {
                //TempData["Message"] = "متاسفانه خطایی رخ داده است، لطفا مجددا عملیات خود را انجام دهید در صورت تکرار این مشکل را به بخش پشتیبانی اطلاع دهید";
                //// Save and send Error for admin user
                //Run_bpReversalRequest = true;
                //return RedirectToAction("ShowError", "Payment");
            }
            finally
            {
                //if (Run_bpReversalRequest) //ReversalRequest
                //{
                //    if (saleOrderId != -999 && saleReferenceId != -999)
                //        bankMellatImplement.BpReversalRequest(saleOrderId, saleOrderId, saleReferenceId);
                //    // Save information to Database...
                //}
            }
            return PaymentFailed(Global.PaymentMsg, "", false);

        }
    }
}