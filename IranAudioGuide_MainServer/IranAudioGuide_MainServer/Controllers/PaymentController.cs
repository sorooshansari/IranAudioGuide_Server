using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IranAudioGuide_MainServer.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IranAudioGuide_MainServer.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
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
                ApplicationUser user = await UserManager.FindByEmailAsync(info.email);
                if (user.uuid != info.uuid || !user.EmailConfirmed)
                    return View("Error");
                Task t = SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                PackageVM package = (from p in db.Packages
                                     where p.Pac_Id == info.packageId
                                     select new PackageVM()
                                     {
                                         PackageId = p.Pac_Id,
                                         PackageName = p.Pac_Name,
                                         PackagePrice = p.Pac_Price,
                                         PackageCities = (from c in db.Cities
                                                          where (from pc in p.Pac_Cities select pc.Cit_Id).Contains(c.Cit_Id)
                                                          select new CityVM()
                                                          {
                                                              CityDesc = c.Cit_Description,
                                                              CityID = c.Cit_Id,
                                                              CityName = c.Cit_Name
                                                          }).ToList()
                                     }).First();
                await t;
                ViewBag.Error = info.ErrorMessage;
                return View(package);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
        [HttpPost]
        [Authorize(Roles = "AppUser")]
        public async Task<ActionResult> Index(WebPaymentReqVM info)
        {
            try
            {
                ApplicationUser user = await UserManager.FindByEmailAsync(User.Identity.Name);
                if (!user.EmailConfirmed)
                    return View("Error");
                PackageVM package = (from p in db.Packages
                                     where p.Pac_Id == info.packageId
                                     select new PackageVM()
                                     {
                                         PackageId = p.Pac_Id,
                                         PackageName = p.Pac_Name,
                                         PackagePrice = p.Pac_Price,
                                         PackageCities = (from c in db.Cities
                                                          where (from pc in p.Pac_Cities select pc.Cit_Id).Contains(c.Cit_Id)
                                                          select new CityVM()
                                                          {
                                                              CityDesc = c.Cit_Description,
                                                              CityID = c.Cit_Id,
                                                              CityName = c.Cit_Name
                                                          }).ToList()
                                     }).First();
                ViewBag.Error = info.ErrorMessage;
                return View(package);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
        [Authorize(Roles = "AppUser")]
        public ActionResult Purchase(Guid packageId, string bankName)
        {
            try
            {
                var user = db.Users.Where(x=>x.UserName == User.Identity.Name).First();
                var package = db.Packages.Find(packageId);
                if (package == null)
                    return RedirectToAction("Index", new WebPaymentReqVM()
                    {
                        packageId = packageId,
                        ErrorMessage = "Error in finding package"
                    });
                string redirectPage = "http://iranaudioguide.com/Payment/Return";
                var payment = new Payment()
                {
                    Amount = package.Pac_Price,
                    SaleReferenceId = 0,
                    BankName = bankName,
                    PaymentFinished = false,
                    User = user,
                    Package = package
                };

                db.Payments.Add(payment);
                db.SaveChanges();

                Guid paymentId = payment.PaymentId;
                string error = ZarinpalPayment(package.Pac_Price, redirectPage, paymentId);
                if (error.Length > 0)
                    return RedirectToAction("Index", new WebPaymentReqVM()
                    {
                        packageId = packageId,
                        ErrorMessage = error
                    });
                else
                    return RedirectToAction("Index", new WebPaymentReqVM() { packageId = packageId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new WebPaymentReqVM()
                {
                    packageId = packageId,
                    ErrorMessage = ex.Message
                });
            }
        }
        public ActionResult Return(string paymentId)
        {
            try
            {
                ViewBag.BankName = "درگاه واسط زرین پال";
                ZarinpalReturn();
            }
            catch
            {
                ViewBag.BankName = "درگاه پرداخت نامشخص است";
                ViewBag.Message = "پاسخی از درگاه پرداخت دریافت نشده";
                ViewBag.SaleReferenceId = "**************";
                ViewBag.Image = "~/images/notaccept.png";
            }
            return View();
        }
        /********************************************/
        #region zarinpal tools



        private void UpdatePayment(Guid paymentId, string vresult, long saleReferenceId, string refId, bool paymentFinished = false)
        {

            var payment = db.Payments.Find(paymentId);

            if (payment != null)
            {
                payment.StatusPayment = vresult;
                payment.SaleReferenceId = saleReferenceId;
                payment.PaymentFinished = paymentFinished;

                if (refId != null)
                {
                    payment.ReferenceNumber = refId;
                }

                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                // اطلاعاتی از دیتابیس پیدا نشد
            }
        }

        private string ZarinpalPayment(long price, string redirectPage, Guid paymentId)
        {
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;

                var zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();

                string authority;

                string merchantCode = "2beca824-a5a6-11e6-8157-005056a205be";

                // نکته:» مبلغ به صورت تومان به درگاه زرین پال ارسال می شود

                // 1- کد پذیرنده
                // 2- مبلغ به تومان
                // 3- توضیح
                // 4- ایمیل
                // 5- موبایل
                // 6- آدرس برگشت از درگاه

                // ارسال اطلاعات به درگاه
                int status = zp.PaymentRequest(merchantCode, (int)price, "Iran Audio Guide", "monaakhlaghi@gmail.com", "", redirectPage + "?PaymentId=" + paymentId.ToString(), out authority);

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
                return "در حال حاظر امکان اتصال به این درگاه وجود ندارد " + ex.Message;
            }
        }

        private void ZarinpalReturn()
        {
            try
            {
                if (Request.QueryString["PaymentId"] != "" && Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
                {
                    Guid paymentId = Guid.Parse(Request.QueryString["PaymentId"]);
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

                            ViewBag.Message = "پرداخت با موفقیت انجام شد.";
                            ViewBag.SaleReferenceId = refId;
                            ViewBag.Image = "../images/accept.png";

                        }
                        else
                        {
                            UpdatePayment(paymentId, status.ToString(), 0, Request.QueryString["Authority"].ToString(), false);

                            ViewBag.Message = PaymentResult.ZarinPal(Convert.ToString(status));
                            ViewBag.SaleReferenceId = "**************";
                            ViewBag.Image = "../images/notaccept.png";
                        }

                    }
                    else
                    {
                        UpdatePayment(paymentId, Request.QueryString["Status"].ToString(), 0, Request.QueryString["Authority"].ToString(), false);

                        ViewBag.Message = "پرداخت ناموفق بود";
                        ViewBag.SaleReferenceId = "**************";
                        ViewBag.Image = "../images/notaccept.png";
                    }
                }
                else
                {
                    ViewBag.SaleReferenceId = "**************";
                    ViewBag.Image = "../images/notaccept.png";
                    ViewBag.Message = "پاسخی از درگاه بانکی دریافت نشد";
                }
            }
            catch (Exception ex)
            {
                ViewBag.SaleReferenceId = "**************";
                ViewBag.Image = "../images/notaccept.png";
                ViewBag.Message = "مشکلی در پرداخت به وجود آمده است ، در صورتیکه وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد";
            }
        }
        private long FindAmountPayment(Guid paymentId)
        {
            long amount = db.Payments.Where(c => c.PaymentId == paymentId).Select(c => c.Amount).FirstOrDefault();

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