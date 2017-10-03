using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Data.Entity;
using IranAudioGuide_MainServer.Services;
using System.Data.SqlClient;
using System.Data;
using IranAudioGuide_MainServer.App_GlobalResources;
using IranAudioGuide_MainServer.Models_v2;
using System.Collections.Generic;
using BankMellatLibrary;
using Elmah;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class PaymentController : BaseController
    {
        public static string packname;
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private TranslatePlace getItem;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //  public Payment Payment { get; private set; }

        private PackagePymentVM getPackageById(Guid pacId)
        {
            try
            {
                var lang = ServiceCulture.GeLangFromCookie();
                using (SqlConnection sqlConnection = new SqlConnection(GlobalPath.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetPackageById_v2", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@langId", lang));
                    cmd.Parameters.Add(new SqlParameter("@PackageId", pacId));
                    sqlConnection.Open();
                    var reader = cmd.ExecuteReader();
                    var dt1 = new DataTable();
                    dt1.Load(reader);
                    var Packege = dt1.AsEnumerable().Select(p => new PackagePymentVM()
                    {
                        PackageId = p["PackageId"].ConvertToGuid(),
                        PackageName = p["PackageName"].ConvertToString(),
                        PackagePrice = p["PackagePrice"].ConvertToString(),
                        PackagePriceDollar = p["PackagePriceDollar"].ConvertToString(),
                        PackageCities = dt1.AsEnumerable().Select(c => new CityPymentVM()
                        {
                            CityID = c["CityId"].ConvertToInt(),
                            CityName = c["CityName"].ConvertToString(),
                            CityDesc = c["CityDescription"].ConvertToString(),
                            TrackCount = c["TrackCount"].ConvertToInt(),
                            PlaceCount = c["PlaceCount"].ConvertToInt()
                        }).ToList()
                    }).FirstOrDefault();
                    return Packege;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        // GET: Payment
        [AllowAnonymous]
        public async Task<ActionResult> Index(AppPaymentReqVM info)
        {
            try
            {
                //if (info.IsChooesZarinpal && ExtensionMethods.IsForeign)
                //    ViewBag.IsChooesZarinpal = false;
                //else
                ViewBag.IsChooesZarinpal = info.IsChooesZarinpal;

                ApplicationUser user = await UserManager.FindByEmailAsync(info.email);
                if (user == default(ApplicationUser))
                {
                    ViewBag.Error = Global.UnauthorizedDevice;
                    return View("customError");
                }
                if (user.uuid != info.uuid)
                {
                    ViewBag.Error = Global.UnauthorizedDevice;
                    return View("customError");
                }
                if (!user.EmailConfirmed)
                {
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = Global.ErrorEmailNotConfirmed,
                        Message = Global.ErrorEmailNotConfirmedMessageForMobile
                    });
                }
                Task t = SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                var package = getPackageById(info.packageId);
                if (package == null)
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = Global.Error,
                        Message = Global.ErrorNotFoundPackage,
                    });
                await t;
                ViewBag.Error = info.ErrorMessage;
                return View(package);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View("vmessage", new vmessageVM()
                {
                    Subject = Global.Error,
                    Message = Global.PleaseTryAgain,
                });
            }
        }

        [Authorize(Roles = "AppUser")]
        private string MelatPayment(Payment resultPayment)
        {
            var ServiceBankMellat = new BankMellatIService();
            using (var db = new ApplicationDbContext())
            {
                using (var dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var resultBank = ServiceBankMellat.BpPayRequest(resultPayment.Pay_Id, resultPayment.Pay_Amount, "test");

                        Response.Write(resultBank);
                        string[] StatusSendRequest = resultBank.Split(',');
                        Payment item = new Payment();
                        item.Pay_Id = resultPayment.Pay_Id;
                        item.Pay_StatusPayment = StatusSendRequest[0];

                        if (StatusSendRequest.Length >= 2)
                            item.Pay_SaleReferenceId = StatusSendRequest[1].ConvertToLong();

                        ServicePayment.Update(item);

                        //در صورتی که مقدار خانه اول این آریه برابر صفر در نتیجه می بایست کاربر را به صفحه پرداخت هدایت کنیم
                        if (StatusSendRequest[0].StoI() != (int)ReturnCodeForBpPayRequest.Success)
                        {
                            dbTran.Commit();
                            return StatusSendRequest[0].ConvertToEnum<ReturnCodeForBpPayRequest>().GetDisplayName();
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
                        return null;
                    }


                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        // "Problem occurred in payment process. ";
                        //"Sorry, please try again in a few minutes.";
                        return Global.ZarinpalPaymentMsg7;

                    }
                }
            }
        }



        [Authorize(Roles = "AppUser")]
        public string WebMoneyPayment(PackagePymentVM res, bool isFromApp = false)
        {
            var wmService = new WebmoneyServices();

            //if (res.PackageId != Guid.Empty)
            //{

            var url = "https://merchant.wmtransfer.com/lmi/payment.asp?at=authtype_2";

            Response.Clear();
            var sb = new System.Text.StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat("<body onload='document.forms[0].submit()'>");
            sb.AppendFormat("<form action='{0}' method='post'>", url);
            sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_NO' value='{0}'>", res.PackageId);
            sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_AMOUNT' value='{0}'>", res.PackagePrice);
            sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_DESC' value='{0}'>", res.PackageName);
            sb.AppendFormat("<input type='hidden' name='LMI_PAYEE_PURSE' value='{0}'>", WebmoneyPurse.WMZ);
            //sb.AppendFormat("<input type='hidden' name='LMI_SIM_MODE' value='{0}'>", 0);
            sb.AppendFormat("<input type='hidden' name='isFromeApp' value='{0}'>", isFromApp);
            sb.AppendFormat("<input type='hidden' name='Lang' value='{0}'>", Global.Lang);
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            Response.Write(sb.ToString());
            Response.End();

            return null;
            //if (isFromApp)
            //    return View("Index", new WebPaymentReqVM() { packageId = res.PackageId });
            //else
            //    return View("PaymentWeb", new WebPaymentReqVM() { packageId = res.PackageId, IsChooesZarinpal = false, ErrorMessage = Global.PleaseTryAgain });




            //}
            //else
            //{
            //    var ex = new Exception(string.Format("PaymentId-->{0}, packageId-->{1}, UserName-->{2}", res.PaymentId, id, User.Identity.Name));
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            //    ViewBag.SaleReferenceId = "**************";
            //    //"Sorry, Your payment was unsuccessful!";
            //    //"Your payment process is not completed."
            //    ViewBag.Message = Global.WebmoneyPaymentMsg1;
            //    ViewBag.ErrDesc = Global.PaymentNotCompleted;

            //    if (isFromApp)
            //        return View("Return");
            //    else
            //        return View("ReturnToWebPage");
            //}
        }

        //soroosh goft 
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VMResult(WMReturnModel model)
        {
            var wmService = new WebmoneyServices();
            wmService.ReturnModel = model;
            var res = wmService.ProccessResult();
            ViewBag.Message = res.Message;
            ViewBag.ErrDesc = res.ErrDesc;
            ViewBag.SaleReferenceId = res.refId;
            ViewBag.Image = res.Image;
            ViewBag.Packname = (res.PackName == "") ? packname : res.PackName;
            return View();// View("ReturnToWebPage");
        }
        //todo
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VMSuccess(WMReturnModel model)
        {
            ViewBag.BankName = Global.WebmoneyGateway;
            var wmService = new WebmoneyServices();
            wmService.ReturnModel = model;
            var res = wmService.Succeeded();
            ViewBag.Message = res.Message;
            ViewBag.ErrDesc = res.ErrDesc;
            ViewBag.SaleReferenceId = res.refId;
            ViewBag.Image = res.Image;
            ViewBag.Packname = (res.PackName == "") ? packname : res.PackName;
            ViewBag.Succeeded = res.Succeeded;
            ServiceCulture.SetCulture(model.Lang);
            if (res.Succeeded == true)
                SendEmailForPurcument(res.EmailInfo);
            if (model.isFromeApp)
                return View("Return");
            else
                return View("ReturnToWebPage");
        }
        //todo
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VMFailed(WMReturnModel model)
        {
            ViewBag.BankName = Global.WebmoneyGateway;
            var wmService = new WebmoneyServices();
            wmService.ReturnModel = model;
            var res = wmService.Failed();
            ViewBag.Message = res.Message;
            ViewBag.ErrDesc = res.ErrDesc;
            ViewBag.SaleReferenceId = res.refId;
            ViewBag.Image = res.Image;
            ViewBag.Packname = (res.PackName == "") ? packname : res.PackName;
            ViewBag.Succeeded = false;
            if (model.isFromeApp)
                return View("Return");
            else
                return View("ReturnToWebPage");
        }


        public ActionResult PaymentFailed(string Message, string Description, bool isFromWeb = false)
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

        [Authorize(Roles = "AppUser")]
        public ActionResult Purchase(Guid packageId, int bankName, bool IsPlace, bool isFromWeb )
        {
            try
            {
                string UserName = User.Identity.Name;
                var count = ServicePayment.IsDuplicatePayment(packageId, User.Identity.Name, IsPlace);
                if (count)
                    return PaymentFailed(Global.PaymentMsg, Global.PaymentMsg1, isFromWeb);

                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                // string baseUrl = "localhost:8462";
                string redirectPage = baseUrl + "/" + Global.Lang + "/Payment/Return";

                if (isFromWeb)
                    redirectPage = baseUrl + "/" + Global.Lang + "/Payment/ReturnToWebPage";

                PackagePymentVM ItemInfo = new PackagePymentVM();

                var user = db.Users.FirstOrDefault(x => x.UserName == UserName);



                if (IsPlace)
                {
                    var langId = ServiceCulture.GeLangFromCookie();
                    getItem = db.TranslatePlaces
                        .Where(x => x.Pla_Id == packageId)
                        .FirstOrDefault(x=> x.langId == langId);
                    if (getItem == null)
                        return PaymentFailed(Global.PaymentMsg, Global.ErrorInvalidRequest, isFromWeb);

                    ItemInfo.PackagePrice = getItem.TrP_Price.ConvertToString();
                    ItemInfo.PackageName = getItem.TrP_Name;
                    ItemInfo.PackageId = getItem.TrP_Id;
                }
                else
                {
                    var package = db.Packages.FirstOrDefault(x => x.Pac_Id == packageId);
                    if (package == null)
                        return PaymentFailed(Global.PaymentMsg, Global.ErrorInvalidRequest, isFromWeb);
                    ItemInfo.PackagePrice = package.Pac_Price.ConvertToString();
                    ItemInfo.PackageName = package.Pac_Name;
                    ItemInfo.PackageId = package.Pac_Id;
                }
                var newProcurement = new Procurement()
                {
                    Pro_User = user
                };

                if (IsPlace)
                    newProcurement.Pro_TrcPlace = getItem;

                else
                    newProcurement.Pac_Id = packageId;

                if (bankName == (int)EnumBankName.Webmoney)
                    newProcurement.Pro_WMPayment = new WMPayment();
                else
                {
                    newProcurement.Pro_Payment = new Payment()
                    {
                        Pay_SaleReferenceId = 0,
                        Pay_BankName = bankName.ConvertToEnum<EnumBankName>().ToString(),
                        Pay_Amount = ItemInfo.PackagePrice.ConvertToLong()
                    };
                }

                db.Procurements.Add(newProcurement);
                db.SaveChanges();
                //int paymentId = Payment.Pay_Id;

                string error = string.Empty;
                if (bankName == (int)EnumBankName.Webmoney)
                {
                    error = WebMoneyPayment(ItemInfo, !isFromWeb);
                }
                else if (bankName == (int)EnumBankName.Zarinpal)
                    error = ZarinpalPayment(newProcurement.Pro_Payment.Pay_Amount, redirectPage, newProcurement.Pro_Payment.Pay_Id, ItemInfo.PackageName);
                else
                    error = MelatPayment(newProcurement.Pro_Payment);

                if (error.Length > 0)
                {
                    var ex = new Exception(string.Format("ZarinpalError-->{0}", error));
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return PaymentFailed(Global.PaymentMsg, error, isFromWeb);

                }
                else
                {
                    ViewBag.Packname = packname;
                    if (isFromWeb)
                        return View("PaymentWeb", new WebPaymentReqVM() { packageId = packageId, IsChooesZarinpal = true, ErrorMessage = "Please try again." });

                    else
                        return View("Index", new WebPaymentReqVM() { packageId = packageId });

                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                // "Problem occurred in payment process. ";
                //"Sorry, please try again in a few minutes.";
                return PaymentFailed(Global.ZarinpalPaymentMsg6, Global.ZarinpalPaymentMsg7, isFromWeb);
            }
        }
        [AllowAnonymous]
        public ActionResult Return(string paymentId)
        {
            ZarinpalReturn();
            return View();
        }

        /****  for website 
         **************************************************************/

        [AllowAnonymous]
        public ActionResult ReturnToWebPage(string paymentId)
        {
            ViewBag.Packname = packname;
            try
            {

                ZarinpalReturn();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.BankName = "Your payment gateway is not specified";
                ViewBag.Message = "No response from payment gateway";
                ViewBag.ErrDesc = "There is no response from your payment gateway!";
                ViewBag.SaleReferenceId = "**************";
                ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                ViewBag.Succeeded = false;
            }
            return View();
        }
        /********************************************/
        //Payment/PaymentWeb
        [Authorize(Roles = "AppUser")]
        public async Task<ActionResult> PaymentWeb(Guid pacId, int ChooesBank, bool IsPlace)
        {

            try
            {
                ViewBag.IsPlace = IsPlace;
                ViewBag.ChooesBank = ChooesBank.ConvertToEnum<EnumBankName>();
                ApplicationUser user = await UserManager.FindByEmailAsync(User.Identity.Name);
                if (!user.EmailConfirmed)
                    return View("ErrorPageProfile", new vmessageVM()
                    {
                        Subject = Global.ErrorEmailNotConfirmed,
                        Message = Global.ErrorEmailNotConfirmedMessage,
                    }); ;

                Task t = SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                var langId = ServiceCulture.GeLangFromCookie();
                var package = IsPlace ? GetTranclatePlaceById(pacId, langId) : getPackageById(pacId);
                if (package == null)
                    return View("ErrorPageProfile", new vmessageVM()
                    {
                        Subject = Global.Error,
                        Message = Global.ErrorNotFoundPackage,
                    });
                packname = package.PackageName;
                await t;
                return View(package);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View("ErrorPageProfile", new vmessageVM()
                {
                    Subject = Global.Error,
                    Message = Global.PleaseTryAgain,
                });
            }


        }

        private PackagePymentVM GetTranclatePlaceById(Guid itemId, int langId)
        {
            using (var db = new ApplicationDbContext())
            {
                var getPlace = db.Places
                    .Include(t => t.Pla_Stories)
                    .Include(t => t.Pla_Audios)
                    .Include(t => t.TranslatePlaces)
                    .Where(x => x.TranslatePlaces.Any(t => t.langId == langId))
                    .FirstOrDefault(f => f.Pla_Id == itemId);
                if (getPlace == null)
                    return null;
                var tranclate = getPlace.TranslatePlaces.FirstOrDefault();

                return new PackagePymentVM()
                {
                    PackageId = getPlace.Pla_Id,
                    PackageName = getPlace.Pla_Name,
                    PackagePrice = tranclate.TrP_Price.ConvertToString(),
                    PackagePriceDollar = tranclate.TrP_PriceDollar.ConvertToString(),
                    PackageCities = new List<CityPymentVM>() {
                                    new CityPymentVM(){
                                        CityDesc= tranclate.TrP_Description,
                                        CityName = tranclate.TrP_Name,
                                        TrackCount = getCountForPlace(getPlace)}
                    }
                };
            }
        }
        private int getCountForPlace(Place getPlace)
        {
            var count = 0;
            if (getPlace.Pla_Stories != null)
                count += getPlace.Pla_Stories.Count;
            if (getPlace.Pla_Audios != null)
                count += getPlace.Pla_Audios.Count;
            return count;
        }


        #region zarinpal tools
        private void UpdatePayment(int paymentId, string vresult, long saleReferenceId, string refId, bool paymentFinished = false)
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
                {
                    procurement.Pro_Payment.Pay_ReferenceNumber = refId;
                }

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

        //private void SendEmail(PaymentInfoForEmailVM returnResults)
        //{

        //    System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/Views/Shared/UserEmailTemplatePayment.html"));
        //    string body = sr.ReadToEnd();
        //    sr.Close();
        //    body = body.Replace("#ReturnPaymentReferenceID#", Global.ReturnPaymentReferenceID);
        //    body = body.Replace("#PaymentPage27#", Global.PaymentPage27);
        //    body = body.Replace("#PriceTite#", Global.Price);
        //    body = body.Replace("#Cities#", Global.Cities);
        //    body = body.Replace("#ReceiptForInvoice#", Global.ReceiptForInvoice);
        //    body = body.Replace("#ThankYouForYourPurchase#", Global.ThankYouForYourPurchase);
        //    body = body.Replace("#PaymentMsg#", Global.Paymentsuccessfully + Global.PaymentMsg5);
        //    body = body.Replace("#DateTite#", Global.DateTite);
        //    body = body.Replace("#ReferenceID#", "9382387437");
        //    body = body.Replace("#PackageName#", returnResults.Name);
        //    body = body.Replace("#Price#", returnResults.Price.ToString());
        //    body = body.Replace("#PrivacyPolicy#", Global.PrivacyPolicy);
        //    body = body.Replace("#TermsConditions#", Global.TermsConditions);
        //    body = body.Replace("#lang#", Global.Lang);


        //    if (Global.Lang.Contains("fa"))
        //    {

        //        var pCalendar = new System.Globalization.PersianCalendar();
        //        DateTime a = DateTime.Now;
        //        int year = pCalendar.GetYear(returnResults.Date);
        //        int month = pCalendar.GetMonth(returnResults.Date);
        //        int day = pCalendar.GetDayOfMonth(returnResults.Date);

        //        body = body.Replace("#Date#", year + "/" + month + "/" + day);
        //        body = body.Replace("#langStyle#", $"style=\"font-family: Tahoma;  direction:rtl\"");
        //    }
        //    else
        //    {
        //        body = body.Replace("#Date#", returnResults.Date.ToString());
        //        body = body.Replace("#langStyle#", $"style=\"font-family:'Open Sans'\"");
        //    }


        //    var listCity = "";
        //    foreach (var item in returnResults.Cities)
        //    {
        //        listCity = listCity + string.Format("<p style='font-size: 21px;background-color:#15437f;color:#ffffff;margin:3% auto;padding:4px 12px;box-shadow: 1px 1px 1px #15437f;font-weight:bold;display: table;min-width:201px;text-shadow: 1px 1px 2px #171514'>{0}<p>", item);
        //    }
        //    body = body.Replace("#CityItem#", listCity);

        //    var msg = new Microsoft.AspNet.Identity.IdentityMessage() { Body = body, Destination = dest, Subject = Global.ReceiptForInvoice };
        //    EmailService es = new EmailService();
        //    es.SendAsync2(msg);
        //}

        private string ZarinpalPayment(long price, string redirectPage, int paymentId, string packageName)
        {
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;

                var zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();

                string authority;

                string merchantCode = "2beca824-a5a6-11e6-8157-005056a205be";

                int status = zp.PaymentRequest(merchantCode, (int)price, packageName, "monaakhlaghi@gmail.com", "", redirectPage + "?PaymentId=" + paymentId.ToString(), out authority);

                // بررسی وضعیت
                if (status == 100)
                {
                    UpdatePayment(paymentId, "-100", 0, authority, false);

                    // اتصال به درگاه
                    Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + authority);
                    return "";
                }
                else
                {
                    // فرا خوانی متد آپدیت پی منت برای ویرایش اطلاعات ارسالی از درگاه در صورت عدم اتصال
                    UpdatePayment(paymentId, status.ToString(), 0, null, false);
                    // نمایش خطا به کاربر
                    return PaymentResult.ZarinPal(status.ToString());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Global.PaymentMsg2;
            }
        }
        private void ZarinpalReturn()
        {
            try
            {
                ViewBag.SaleReferenceId = "**************";
                //No response from bank gateway.
                // "If the payment is deducted from your bank account, The amount will be automatically returned.";
                ViewBag.Message = Global.ZarinpalPaymentMsg;
                ViewBag.ErrDesc = Global.PaymentAutomaticallyReturnMoney;
                ViewBag.Succeeded = false;

                int paymentId = Request.QueryString["PaymentId"].ConvertToInt();
                if (paymentId == 0)
                    return;
                var Authority = Request.QueryString["Authority"].ConvertToString();
                if (string.IsNullOrEmpty(Authority))
                    return;

                var Status = Request.QueryString["Status"].ConvertToString();
                if (!Status.Equals("OK"))
                {
                    UpdatePayment(paymentId, Status, 0, Authority, false);
                    if (!string.IsNullOrEmpty(Authority))
                        ViewBag.SaleReferenceId = Authority;
                }
                else
                {
                    var returnResults = db.Payments.Include("Pay_Procurement.Pro_Package.Pac_Cities")
                                    .Where(c => c.Pay_Id == paymentId)
                                    .Select(c => new SendEmailForPaymentVM
                                    {
                                        Price = c.Pay_Amount.ToString(),
                                        Pac_Name = c.Pay_Procurement.Pro_Package.Pac_Name,
                                        Cities = c.Pay_Procurement.Pro_Package.Pac_Cities.Select(x => x.Cit_Name).ToList(),
                                        Date = c.Pay_Procurement.Pro_InsertDatetime
                                    }).FirstOrDefault();


                    if (returnResults == null)
                    {
                        //"Sorry, Your payment was unsuccessful!";
                        // "Your payment process is not completed.";
                        ViewBag.Message = Global.PaymentUnsuccessfully;
                        ViewBag.ErrDesc = Global.PaymentNotCompleted;
                        return;
                    }

                    ViewBag.Packname = returnResults.Pac_Name;
                    ViewBag.Price = returnResults.Price;
                    long refId = 0;

                    System.Net.ServicePointManager.Expect100Continue = false;
                    var zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
                    string merchantCode = "2beca824-a5a6-11e6-8157-005056a205be";
                    int status = zp.PaymentVerification(merchantCode, Authority, returnResults.Price.ConvertToInt(), out refId);
                    if (status == 100 || status == 101)
                    {
                        UpdatePayment(paymentId, status.ConvertToString(), refId, Authority, true);
                        ViewBag.SaleReferenceId = refId;
                        returnResults.ReferenceID = refId.ToString();
                        //"Payment completed successfully.";
                        //"You have access to the package below. Thank you for your purchase!";
                        ViewBag.Message = Global.Paymentsuccessfully;
                        ViewBag.ErrDesc = Global.ZarinpalPaymentMsg5;
                        ViewBag.Succeeded = true;
                        SendEmailForPurcument(returnResults);
                    }
                    else
                    {
                        UpdatePayment(paymentId, status.ConvertToString(), 0, Authority, false);
                        ViewBag.Message = PaymentResult.ZarinPal(Convert.ToString(status));
                        if (refId > 0)
                            ViewBag.SaleReferenceId = refId;
                        ViewBag.ErrDesc = Global.PaymentNotCompleted;
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                // "Problem occurred in payment process. ";
                //"Sorry, please try again in a few minutes.";
                ViewBag.Message = Global.ZarinpalPaymentMsg6;
                ViewBag.ErrDesc = Global.ZarinpalPaymentMsg7;
            }
        }


        #endregion
        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}