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

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class PaymentController : Controller
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


        // GET: Payment
        [AllowAnonymous]
        public async Task<ActionResult> Index(AppPaymentReqVM info)
        {
            try
            {
                //if (info.IsChooesZarinpal && ExtensionMethods.IsForeign)
                //    ViewBag.IsChooesZarinpal = false;
                //else
                //    ViewBag.IsChooesZarinpal = info.IsChooesZarinpal;
                ViewBag.IsChooesZarinpal = info.IsChooesZarinpal;

                ApplicationUser user = await UserManager.FindByEmailAsync(info.email);
                if (user.uuid != info.uuid)
                {
                    ViewBag.Error = "Unauthorized device!";
                    return View("customError");
                }
                if (!user.EmailConfirmed)
                {
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = "Email not confirmed yet!",
                        Message = @"For purchasing, first need to confirm your email. Please click <a id='loginlink' href='/Account/Login'>here</a> to Login",
                    });
                }
                Task t = SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
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
                                     }).First();
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
                    Subject = "Error!",
                    Message = "Please try again",
                });
            }
        }
        //Payment/PaymentWeb
        [Authorize(Roles = "AppUser")]
        public async Task<ActionResult> PaymentWeb(WebPaymentReqVM pak)
        {
            try
            {

                //if (pak.IsChooesZarinpal && ExtensionMethods.IsForeign)
                //    ViewBag.IsChooesZarinpal = false;
                //else
                //    ViewBag.IsChooesZarinpal = pak.IsChooesZarinpal;

                ViewBag.IsChooesZarinpal = pak.IsChooesZarinpal;

                var info = new AppPaymentReqVM()
                {
                    packageId = pak.packageId,
                    email = User.Identity.Name
                };
                ApplicationUser user = await UserManager.FindByEmailAsync(info.email);
                if (!user.EmailConfirmed)
                    return View("vmessage", new vmessageVM()
                    {
                        Subject = "Email not confirmed yet!",
                        Message = @"For purchasing, first need to confirm your email. Please click <a id='loginlink' href='/user'>here</a> to Login",
                    });
                Task t = SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
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
                                     }).First();
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
                    Subject = "Error!",
                    Message = "Please try again",
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
                ViewBag.Message = "Sorry, Your payment was unsuccessful!";
                ViewBag.SaleReferenceId = "**************";
                ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                ViewBag.ErrDesc = "You are already purchased this package.";
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
                sb.Append("</form>");
                sb.Append("</body>");
                sb.Append("</html>");
                Response.Write(sb.ToString());
                Response.End();
                if (isFromApp)
                    return RedirectToAction("Index", new WebPaymentReqVM() { packageId = packageId });
                else
                    return RedirectToAction("PaymentWeb", new WebPaymentReqVM() { packageId = packageId, IsChooesZarinpal = false, ErrorMessage = "Please try again." });
            }
            else
            {
                var ex = new Exception(string.Format("PaymentId-->{0}, packageId-->{1}, UserName-->{2}", res.PaymentId, packageId, User.Identity.Name));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Message = "Sorry, Your payment was unsuccessful!";
                ViewBag.SaleReferenceId = "**************";
                ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                ViewBag.ErrDesc = "Your payment process is not completed.";
                if (isFromApp)
                    return View("Return");
                else
                    return View("ReturnToWebPage");
            }
            //if (isFromApp)
            //    return RedirectToAction("PaymentWeb", new WebPaymentReqVM() { packageId = packageId, IsChooesZarinpal = false, ErrorMessage = "Please try again." });
            //else
            //    return RedirectToAction("PaymentWeb", packageId);

            //string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            //string success = baseUrl + "/Payment/VMSuccess";
            //string failed = baseUrl + "/Payment/VMFailed";
            //using (var client = new HttpClient())
            //{
            //    var values = new Dictionary<string, string>
            //        {
            //           { "LMI_PAYEE_PURSE", "Z945718891756" },
            //           { "LMI_PAYMENT_AMOUNT", "5" },
            //           { "LMI_PAYMENT_NO", "1" },
            //           { "LMI_PAYMENT_DESC", "helo world" },
            //           { "LMI_SIM_MODE", "0" },
            //           { "LMI_SUCCESS_URL", success },
            //           { "LMI_SUCCESS_METHOD", "1" },
            //           { "LMI_FAIL_URL", failed },
            //           { "LMI_FAIL_METHOD", "1" },
            //           { "myId", "52005" }
            //        };

            //    var content = new FormUrlEncodedContent(values);

            //    var response = await client.PostAsync("https://merchant.wmtransfer.com/lmi/payment.asp", content);

            //    var responseString = await response.Content.ReadAsStringAsync();
            //    ViewBag.Error = responseString;
            //}
            //RedirectToAction("PaymentWeb", packageId);
        }

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
            return View("ReturnToWebPage");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VMSuccess(WMReturnModel model)
        {
            ViewBag.BankName = "Webmoney Gateway";
            var wmService = new WebmoneyServices();
            wmService.ReturnModel = model;
            var res = wmService.Succeeded();
            ViewBag.Message = res.Message;
            ViewBag.ErrDesc = res.ErrDesc;
            ViewBag.SaleReferenceId = res.refId;
            ViewBag.Image = res.Image;
            ViewBag.Packname = (res.PackName == "") ? packname : res.PackName;
            ViewBag.Succeeded = res.Succeeded;
            if (model.isFromeApp)
                return View("Return");
            else
                return View("ReturnToWebPage");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult VMFailed(WMReturnModel model)
        {
            ViewBag.BankName = "Webmoney Gateway";
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
                    ViewBag.Message = "Sorry, Your payment was unsuccessful!";
                    ViewBag.SaleReferenceId = "**************";
                    ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                    ViewBag.ErrDesc = "You are already purchased this package.";
                    ViewBag.Succeeded = false;
                    if (isFromWeb)
                        return View("ReturnToWebPage");
                    else
                        return View("Return");
                }

                //var user = db.Users.Where(x => x.UserName == UserName).First();
                //var package = db.Packages.Find(packageId);
                //if (package == null)
                //{
                //    ViewBag.Message = "Sorry, Your payment was unsuccessful!";
                //    ViewBag.SaleReferenceId = "**************";
                //    ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                //    ViewBag.ErrDesc = "Your payment process is not completed.";
                //    ViewBag.Succeeded = false;
                //    if (isFromWeb)
                //        return View("ReturnToWebPage");
                //    else
                //        return View("Return");
                //}
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string redirectPage = baseUrl + "/Payment/Return";
                if (isFromWeb)
                    redirectPage = baseUrl + "/Payment/ReturnToWebPage";
                //Todo I've changed command


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
                    ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
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
                        return RedirectToAction("PaymentWeb", new WebPaymentReqVM() { packageId = packageId, IsChooesZarinpal = true, ErrorMessage = "Please try again." });

                    else
                        return RedirectToAction("Index", new WebPaymentReqVM() { packageId = packageId });

                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Message = "Sorry, Your payment was unsuccessful!";
                ViewBag.SaleReferenceId = "**************";
                ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                ViewBag.ErrDesc = "Your payment process is not completed.";
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
            ViewBag.Packname = packname;
            try
            {
                ViewBag.BankName = "ZarinPal Payment Gateway";
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
        [AllowAnonymous]
        public ActionResult ReturnToWebPage(string paymentId)
        {
            ViewBag.Packname = packname;
            try
            {
                ViewBag.BankName = "ZarinPal Payment Gateway";
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




        /***********************************************************/
        #region zarinpal tools



        private void UpdatePayment(int paymentId, string vresult, long saleReferenceId, string refId, bool paymentFinished = false)
        {
            //Todo I've changed command
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
                //Todo I've changed command
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
                return "At the moment there is no possibility of connecting to this port.";
            }
        }
        private void ZarinpalReturn()
        {
            try
            {
                if (Request.QueryString["PaymentId"] != "" && Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
                {
                    int paymentId = Convert.ToInt32(Request.QueryString["PaymentId"]);
                    if (Request.QueryString["Status"].ToString().Equals("OK"))
                    {
                        int amount = (int)FindAmountPayment(paymentId);

                        long refId = 0;

                        System.Net.ServicePointManager.Expect100Continue = false;

                        var zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();

                        string merchantCode = "2beca824-a5a6-11e6-8157-005056a205be";

                        int status = zp.PaymentVerification(merchantCode, Request.QueryString["Authority"].ToString(), amount, out refId);

                        if (status == 100)
                        {
                            UpdatePayment(paymentId, status.ToString(), refId, Request.QueryString["Authority"].ToString(), true);

                            ViewBag.Message = "Payment completed successfully.";
                            ViewBag.SaleReferenceId = refId;
                            ViewBag.Image = "<i class=\"fa fa-check\" style=\"color: lightgreen; font-size:35px; vertical-align:sub; \"></i>";
                            ViewBag.ErrDesc = "You have access to the package below. Thank you for your purchase! <br />";
                            ViewBag.Succeeded = true;
                        }
                        else
                        {
                            UpdatePayment(paymentId, status.ToString(), 0, Request.QueryString["Authority"].ToString(), false);

                            ViewBag.Message = PaymentResult.ZarinPal(Convert.ToString(status));
                            if (refId > 0)
                                ViewBag.SaleReferenceId = refId;
                            else
                                ViewBag.SaleReferenceId = "**************";
                            ViewBag.Image = "<i class=\"fa fa-exclamation-triangle\" style=\"color: Yellow; font-size:35px; vertical-align:sub; \"></i>";
                            ViewBag.ErrDesc = "Your payment process is not completed.";
                            ViewBag.Succeeded = false;
                        }

                    }
                    else
                    {
                        UpdatePayment(paymentId, Request.QueryString["Status"].ToString(), 0, Request.QueryString["Authority"].ToString(), false);

                        ViewBag.Message = "Sorry, Your payment was unsuccessful!";
                        ViewBag.SaleReferenceId = "**************";
                        try
                        {
                            ViewBag.SaleReferenceId = Convert.ToInt64(Request.QueryString["Authority"]);
                        }
                        catch (Exception)
                        {
                            ViewBag.SaleReferenceId = "**************";
                        }
                        ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                        ViewBag.ErrDesc = "Your payment process is not completed.";
                        ViewBag.Succeeded = false;
                    }
                }
                else
                {
                    ViewBag.Message = "No response from bank gateway.";
                    ViewBag.SaleReferenceId = "**************";
                    ViewBag.Image = "<i class=\"fa fa-exclamation-triangle\" style=\"color: Yellow; font-size:35px; vertical-align:sub; \"></i>";
                    ViewBag.ErrDesc = "Sorry, please try again in a few minutes.";
                    ViewBag.Succeeded = false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.SaleReferenceId = "**************";
                ViewBag.Image = "<i class=\"fa fa-close\" style=\"color: red; font-size:35px; vertical-align:sub; \"></i>";
                ViewBag.Message = "Problem occurred in payment process. ";
                ViewBag.ErrDesc = "If the payment is deducted from your bank account, The amount will be automatically returned.";
                ViewBag.Succeeded = false;
            }
        }
        private long FindAmountPayment(int paymentId)
        {
            //Todo Is it correct 
            long amount = db.Payments.Where(c => c.Pay_Id == paymentId).Select(c => c.Pay_Amount).FirstOrDefault();

            return amount;
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