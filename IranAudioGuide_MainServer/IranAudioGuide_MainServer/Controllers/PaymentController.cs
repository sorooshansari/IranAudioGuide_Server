using System;
using System.Collections.Generic;
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

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class PaymentController : BaseController
    {
        public static string packname;
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
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
                        PackageId = p["PackageId"].convertToGuid(),
                        PackageName = p["PackageName"].convertToString(),
                        PackagePrice = p["PackagePrice"].convertToString(),
                        PackagePriceDollar = p["PackagePriceDollar"].convertToString(),
                        PackageCities = dt1.AsEnumerable().Select(c => new CityPymentVM()
                        {
                            CityID = c["CityId"].convertToInt(),
                            CityName = c["CityName"].convertToString(),
                            CityDesc = c["CityDescription"].convertToString(),
                            TrackCount = c["TrackCount"].convertToInt(),
                            PlaceCount = c["PlaceCount"].convertToInt()
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
                packname = package.PackageName;
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
        public ActionResult WMPurchase(Guid packageId, bool isFromApp = false)
        {
            var wmService = new WebmoneyServices();
            var res = wmService.CreatePayment(User.Identity.Name, packageId);
            if (res.isDuplicate)
            {
                //"Sorry, Your payment was unsuccessful!"
                ViewBag.Message = Global.WebmoneyPaymentMsg1; 
                ViewBag.SaleReferenceId = "**************";
                //"You are already purchased this package.";
                ViewBag.ErrDesc = Global.PaymentMsg1;

                ViewBag.Succeeded = false;
                if (isFromApp)
                    return View("Return");
                else
                    return View("ReturnToWebPage");
            }
            if (res.PaymentId != 0)
            {
                var url = "https://merchant.wmtransfer.com/lmi/payment.asp?at=authtype_2";

                Response.Clear();
                var sb = new System.Text.StringBuilder();
                sb.Append("<html>");
                sb.AppendFormat("<body onload='document.forms[0].submit()'>");
                sb.AppendFormat("<form action='{0}' method='post'>", url);
                sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_NO' value='{0}'>", res.PaymentId);
                sb.AppendFormat("<input type='hidden' name='LMI_PAYMENT_AMOUNT' value='{0}'>", res.PackageAmount);
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
                if (isFromApp)
                    return View("Index", new WebPaymentReqVM() { packageId = packageId });
                else
                    return View("PaymentWeb", new WebPaymentReqVM() { packageId = packageId, IsChooesZarinpal = false, ErrorMessage =Global.PleaseTryAgain });
            }
            else
            {
                var ex = new Exception(string.Format("PaymentId-->{0}, packageId-->{1}, UserName-->{2}", res.PaymentId, packageId, User.Identity.Name));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.SaleReferenceId = "**************";
                //"Sorry, Your payment was unsuccessful!";
                //"Your payment process is not completed."
                ViewBag.Message = Global.WebmoneyPaymentMsg1;
                ViewBag.ErrDesc = Global.PaymentNotCompleted;

                if (isFromApp)
                    return View("Return");
                else
                    return View("ReturnToWebPage");
            }
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


        [Authorize(Roles = "AppUser")]
        public ActionResult Purchase(Guid packageId, string bankName, bool isFromWeb = false)
        {
            try
            {
                string UserName = User.Identity.Name;
                var count = db.Procurements.Include(x => x.Pro_User)
                        .Count(x => x.Pro_User.UserName == UserName && x.Pac_Id == packageId && x.Pro_PaymentFinished);
                if (count > 0)
                {
                    ViewBag.SaleReferenceId = "**************";
                    ViewBag.Message = Global.PaymentMsg;
                    ViewBag.ErrDesc = Global.PaymentMsg1;
                    ViewBag.Succeeded = false;
                    if (isFromWeb)
                        return View("ReturnToWebPage");
                    else
                        return View("Return");
                }
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string redirectPage = baseUrl + "/" + Global.Lang + "/Payment/Return";
                if (isFromWeb)
                    redirectPage = baseUrl + "/" + Global.Lang + "/Payment/ReturnToWebPage";



                var user = db.Users.FirstOrDefault(x => x.UserName == UserName);
                var package = db.Packages.FirstOrDefault(x => x.Pac_Id == packageId);
                var price = package.Pac_Price;

                var Payment = new Payment()
                {
                    Pay_Amount = price,
                    Pay_SaleReferenceId = 0,
                    Pay_BankName = bankName,
                    Pay_Procurement = new Procurement()
                    {
                        Pro_User = user,
                        Pro_Package = package
                    }
                };
                db.Payments.Add(Payment);
                db.SaveChanges();
                int paymentId = Payment.Pay_Id;

                string error = ZarinpalPayment(package.Pac_Price, redirectPage, paymentId, package.Pac_Name);
                if (error.Length > 0)
                {
                    var ex = new Exception(string.Format("ZarinpalError-->{0}", error));
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    ViewBag.Message = error;
                    ViewBag.SaleReferenceId = "**************";
                    ViewBag.Succeeded = false;
                    if (isFromWeb)
                        return View("ReturnToWebPage");
                    else
                        return View("Return");
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
                ViewBag.Message = Global.ZarinpalPaymentMsg6;
                ViewBag.ErrDesc = Global.ZarinpalPaymentMsg7; 

                ViewBag.Succeeded = false;
                if (isFromWeb)
                    return View("ReturnToWebPage");
                else
                    return View("Return");
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
                //  ViewBag.BankName = "ZarinPal Payment Gateway";
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
        public async Task<ActionResult> PaymentWeb(Guid pacId, bool IsZarinpal)
        {

            try
            {

                if (IsZarinpal && ExtensionMethods.IsForeign)
                    ViewBag.IsChooesZarinpal = false;
                else
                    ViewBag.IsChooesZarinpal = IsZarinpal;

                var info = new AppPaymentReqVM()
                {
                    packageId = pacId,
                    email = User.Identity.Name
                };
                ApplicationUser user = await UserManager.FindByEmailAsync(info.email);
                if (!user.EmailConfirmed)
                    return View("ErrorPageProfile", new vmessageVM()
                    {
                        Subject = App_GlobalResources.Global.ErrorEmailNotConfirmed,
                        Message = App_GlobalResources.Global.ErrorEmailNotConfirmedMessage,
                    });

                PackageVM package = (from p in db.Packages
                                     where p.Pac_Id == info.packageId
                                     select new PackageVM()
                                     {
                                         PackageId = p.Pac_Id,
                                         PackageName = p.Pac_Name,
                                         PackagePrice = p.Pac_Price,
                                         PackagePriceDollar = p.Pac_Price_Dollar,
                                         PackageCities = (from c in db.Cities
                                                          where (from pc in p.Pac_Cities select pc.Cit_Id).Contains(c.Cit_Id)
                                                          select new CityVM()
                                                          {
                                                              CityDesc = c.Cit_Description,
                                                              CityID = c.Cit_Id,
                                                              CityName = c.Cit_Name
                                                          }).ToList()
                                     }).FirstOrDefault();
                packname = package.PackageName;
                ViewBag.Error = info.ErrorMessage;
                return View(package);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View("ErrorPageProfile", new vmessageVM()
                {
                    Subject = App_GlobalResources.Global.Error,
                    Message = App_GlobalResources.Global.PleaseTryAgain,
                });
            }
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

                int paymentId = Request.QueryString["PaymentId"].convertToInt();
                if (paymentId == 0)
                    return;
                var Authority = Request.QueryString["Authority"].convertToString();
                if (string.IsNullOrEmpty(Authority))
                    return;

                var Status = Request.QueryString["Status"].convertToString();
                if (!Status.Equals("OK"))
                {
                    UpdatePayment(paymentId, Status, 0, Authority, false);
                    if (!string.IsNullOrEmpty(Authority))
                        ViewBag.SaleReferenceId = Authority;
                }
                else
                {
                    var returnResults = db.Payments.Include("Pay_Procurement.Pro_Package")
                    .Where(c => c.Pay_Id == paymentId)
                    .Select(c => new
                    {
                        price = c.Pay_Amount,
                        name = c.Pay_Procurement.Pro_Package.Pac_Name
                    }).FirstOrDefault();

                    if (returnResults == null)
                    {
                        //"Sorry, Your payment was unsuccessful!";
                        // "Your payment process is not completed.";
                        ViewBag.Message = Global.PaymentUnsuccessful;
                        ViewBag.ErrDesc = Global.PaymentNotCompleted;
                        return;
                    }

                    ViewBag.Packname = returnResults.name;
                    ViewBag.Price = returnResults.price.convertToInt();
                    long refId = 0;

                    System.Net.ServicePointManager.Expect100Continue = false;
                    var zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
                    string merchantCode = "2beca824-a5a6-11e6-8157-005056a205be";
                    int status = zp.PaymentVerification(merchantCode, Authority, ViewBag.Price, out refId);
                    if (status == 100)
                    {
                        UpdatePayment(paymentId, status.convertToString(), refId, Authority, true);
                        ViewBag.SaleReferenceId = refId;
                        //"Payment completed successfully.";
                        //"You have access to the package below. Thank you for your purchase!";
                        ViewBag.Message = Global.Paymentsuccessfully;
                        ViewBag.ErrDesc = Global.ZarinpalPaymentMsg5;
                        ViewBag.Succeeded = true;
                    }
                    else
                    {
                        UpdatePayment(paymentId, status.convertToString(), 0, Authority, false);
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