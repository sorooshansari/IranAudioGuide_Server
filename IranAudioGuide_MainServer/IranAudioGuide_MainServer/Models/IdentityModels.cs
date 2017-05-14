using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
using IranAudioGuide_MainServer.Services;

namespace IranAudioGuide_MainServer.Models
{// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            TimeSetUuid = null;
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public string FullName { get; set; }
        public string ImgUrl { get; set; }
        public string GoogleId { get; set; }
        public gender gender { get; set; }
        public string Picture { get; set; }
        public string uuid { get; set; }
        public DateTime? TimeSetUuid { get; set; }
        public IList<Procurement> procurements { get; set; }

    }
   
    public class UserLog
    {
        [Key]
        [Display(Name = "Temporary User Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsL_Id { get; set; }
        [Display(Name = "Unified Unique Identifier")]
        public string UsL_UUId { get; set; }
        [Display(Name = "date time")]
        public DateTime UsL_DateTime { get; set; }
    }
    public class LogUserFailure
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
    }
    public class Payment
    {
        public Payment()
        {
            //InsertDatetime = DateTime.Now;
        }
        [Key]
        //[Display(Name = "Payment Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pay_Id { get; set; }
        public Procurement Pay_Procurement { get; set; }

        [Display(Name = "Reference Number")]
        [MaxLength(100)]
        public string Pay_ReferenceNumber { get; set; }

        [Display(Name = "Sale Reference Id")]
        public long Pay_SaleReferenceId { get; set; }

        [Display(Name = "Status Payment")]
        [MaxLength(100)]
        public string Pay_StatusPayment { get; set; }

        //// فقط در صورتی که این فید ترو باشد پرداخت موفق بوده است
        //[Display(Name = "Payment Finished")]
        //public bool Pay_PaymentFinished { get; set; }

        [Display(Name = "Amount")]
        public long Pay_Amount { get; set; }

        [Display(Name = "Bank Name")]
        [MaxLength(50)]
        public string Pay_BankName { get; set; }

    }
    public class UpdateLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UpL_Id { get; set; }
        public Guid? Aud_Id { get; set; }
        public Guid? Pla_ID { get; set; }
        public int? Cit_ID { get; set; }
        public Guid? Img_Id { get; set; }
        public Guid? Sto_Id { get; set; }
        public Guid? Tip_Id { get; set; }
        public Guid? Ima_Id { get; set; }
        public bool isRemoved { get; set; }
    }

    public class Audio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Aud_Id { get; set; }
        public string Aud_Name { get; set; }
        public string Aud_Url { get; set; }
        public string Aud_Discription { get; set; }
        public int Aud_Order { get; set; }

        #region Relation property
        public Guid PlaceId { get; set; }
        public Place Place { get; set; }
        public int langId { get; set; }
      
        #endregion
    }

    public class Story
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Sto_Id { get; set; }
        public string Sto_Name { get; set; }
        public string Sto_Url { get; set; }
        public string Sto_Discription { get; set; }
        public int Sto_Order { get; set; }

        #region Relation property
        public Guid PlaceId { get; set; }
        public Place Place { get; set; }
        public int langId { get; set; }
      
        #endregion
    }


    public class TranslatePlace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TrP_Id { get; set; }

        public string TrP_Name { get; set; }
        public string TrP_Description { get; set; }
        public string TrP_Address { get; set; }

        public Guid Pla_Id { get; set; }
        public Place Place { get; set; }

        public int langId { get; set; }
      

    }
    public class Place
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Pla_Id { get; set; }
        public string Pla_Name { get; set; }
        public string Pla_ImgUrl { get; set; }
        public string Pla_TumbImgUrl { get; set; }
        public string Pla_Discription { get; set; }
        public city Pla_city { get; set; }
        public double Pla_cordinate_X { get; set; }
        public double Pla_cordinate_Y { get; set; }
        public string Pla_Address { get; set; }
        public bool Pla_Deactive { get; set; }
        public int Pla_Order { get; set; }

        public bool Pla_isOnline { get; set; }
        public bool Pla_isPrimary { get; set; }
        #region Relation property
        public List<Audio> Pla_Audios { get; set; }
        public List<Story> Pla_Stories { get; set; }
        public List<Image> Pla_ExtraImages { get; set; }
        public List<Tip> Pla_Tips { get; set; }
        public List<TranslatePlace> TranslatePlaces { get; set; }

        #endregion
    }

    public class TranslateImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TrI_Id { get; set; }

        public string TrI_Name { get; set; }
        public string TrI_Description { get; set; }

        public Guid Img_Id { get; set; }
        public Image Image { get; set; }


        public int langId { get; set; }
      

    }
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Img_Id { get; set; }
        public string Img_Name { get; set; }
        public string Img_Description { get; set; }
        public int Tmg_Order { get; set; }
        #region Relation property
        public Place Place { get; set; }
        public List<TranslateImage> TranslateImages { get; set; }
        #endregion
    }


    public class Tip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Tip_Id { get; set; }
        public TipCategory Tip_Category { get; set; }
        public string Tip_Content { get; set; }
        public int Tip_Order { get; set; }

        #region Relation property
        public Place Place { get; set; }
        public int langId { get; set; }
      

        #endregion

    }
    public class TipCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid TiC_Id { get; set; }
        public string TiC_Class { get; set; }
        public string TiC_Unicode { get; set; }
        public string TiC_Name { get; set; }
        public int TiC_Priority { get; set; }
    }

    public class TranslateCity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TrC_Id { get; set; }

        public string TrC_Name { get; set; }
        public string TrC_Description { get; set; }

        #region Relation property
        public int Cit_Id { get; set; }
        public city city { get; set; }
        public int langId { get; set; }
      

        #endregion

    }
    public class city
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Cit_Id { get; set; }
        public string Cit_Name { get; set; }
        public string Cit_Description { get; set; }
        public string Cit_ImageUrl { get; set; }
        public int Cit_Order { get; set; }

        #region Relation property
        public List<TranslateCity> TranslateCities { get; set; }
        public IList<Package> Cit_Packages { get; set; }
        #endregion
    }

    public class TranslatePackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TrP_Id { get; set; }

        public string TrP_Name { get; set; }
        #region Relation property

        public Guid Pac_Id { get; set; }

        public Package Package { get; set; }


        public int langId { get; set; }
      

        #endregion

    }
    public class Package
    {
        public Package()
        {
            //Pac_Cities = new HashSet<city>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Pac_Id { get; set; }
        public string Pac_Name { get; set; }
        public long Pac_Price { get; set; }
        public float Pac_Price_Dollar { get; set; }
        public int Pac_Order { get; set; }

        //public object pac_Cities { get; internal set; }
        //public IList<ApplicationUser> Pac_User { get; set; }

        #region Relation property
        public IList<city> Pac_Cities { get; set; }
        public IList<Procurement> procurements { get; set; }
        public List<TranslatePackage> TranslatePackages { get; set; }

        #endregion
    }
    public class Procurement
    {
        public Procurement()
        {
            Pro_PaymentFinished = false;
            Pro_InsertDatetime = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Pro_Id { get; set; }

        // This is true only if the payment was successful
        [Display(Name = "Payment Finished")]
        public bool Pro_PaymentFinished { get; set; }
        [Display(Name = "Insert Datetime")]
        public DateTime Pro_InsertDatetime { get; set; }

        #region Relation property


        [MaxLength(128)]
        public string Id { get; set; }
        public ApplicationUser Pro_User { get; set; }


        public Guid Pac_Id { get; set; }
        public Package Pro_Package { get; set; }


        //public Guid? PaymentId { get; set; }
        public Payment Pro_Payment { get; set; }

        //public int? WMPaymentId { get; set; }
        public WMPayment Pro_WMPayment { get; set; }

        public int? Bar_Id { get; set; }
        public Barcode Pro_bar { get; set; }

        #endregion

    }
    public class RequestForApp
    {
        public RequestForApp()
        {
            CreateRequest = DateTime.Now;
            IsSend = false;
        }
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreateRequest { get; set; }
        public bool IsSend { get; set; }
        public string NameDevice { get; set; }
    }

    public class DownloadLink
    {
        public DownloadLink()
        {
            IsDisable = false;
            //Path = "test";
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Dow_Id { get; set; }
        public string FileName { get; set; }

        public string Path { get; set; }
        public DateTime TimeToVisit { get; set; }
        public bool IsDisable { get; internal set; }
        public bool IsAudio { get; set; }
    }
    public class Comment
    {
        [Key]
        public int Comment_Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsRead { get; set; }
        public string uuid { get; set; }
        public string Email { get; internal set; }
    }



    public class WMPayment
    {
        public WMPayment()
        {
            var date = DateTime.Now;
            WMP_SYS_TRANS_DATE = date;
            WMP_SYS_TRANS_DATE_Result = date;
        }



        [Key]
        //[Display(Name = "Payment Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WMP_Id { get; set; }
        public Procurement WMP_Procurement { get; set; }


        //[Display(Name = "User")]
        //public ApplicationUser User { get; set; }
        //[Display(Name = "Package")]
        //public Package Package { get; set; }



        //// This is true only if the payment was successful
        //[Display(Name = "Payment Finished")]
        //public bool WMP_PaymentFinished { get; set; }


        [Display(Name = "Data Integrity")]
        public bool WMP_DataIntegrity { get; set; }
        public string WMP_PAYEE_PURSE { get; set; }
        public string WMP_PAYMENT_AMOUNT { get; set; }
        public string WMP_HOLD { get; set; }
        public string WMP_PAYMENT_NO { get; set; }
        public string WMP_MODE { get; set; }
        public string WMP_SYS_INVS_NO { get; set; }
        public string WMP_SYS_INVS_NO_Result { get; set; }
        public string WMP_SYS_TRANS_NO { get; set; }
        public string WMP_SYS_TRANS_NO_Result { get; set; }
        public DateTime WMP_SYS_TRANS_DATE { get; set; }
        public DateTime WMP_SYS_TRANS_DATE_Result { get; set; }
        public string WMP_PAYER_PURSE { get; set; }
        public string WMP_PAYER_WM { get; set; }

        public string WMP_CAPITALLER_WMID { get; set; }
        public string WMP_PAYMER_NUMBER { get; set; }
        public string WMP_PAYMER_EMAIL { get; set; }
        public string WMP_PAYMENT_CREDITDAYS { get; set; }

        public string WMP_SDP_TYPE { get; set; }
        public string WMP_PAYER_COUNTRYID { get; set; }
        public string WMP_PAYER_PCOUNTRYID { get; set; }
        public string WMP_PAYER_IP { get; set; }

    }
    public class Barcode
    {
        public Barcode()
        {
            Bar_DateTime = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Bar_Id { get; set; }
        public int Pri_Id { get; set; }
        public Price Bar_Price { get; set; }
        public string Bar_SellerName { get; set; }
        public bool Bar_IsUsed { get; set; }
        public string Bar_Image_Url { get; set; }
        public DateTime Bar_DateTime { get; set; }
    }
    public class Price
    {
        public Price()
        {
           
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pri_Id { get; set; }
        public double Pri_Value { get; set; }
        public string Pri_Description { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(GlobalPath.ConnectionString
                  , throwIfV1Schema: false)
        {
            // Database.Log = WriteFile.WriteSQL; 
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Audio> Audios { get; set; }
        public DbSet<Story> Storys { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<city> Cities { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UpdateLog> UpdateLogs { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<TipCategory> TipCategories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<LogUserFailure> LogUserFailures { get; set; }
        //public DbSet<OnlinePlace> OnlinePlaces { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RequestForApp> RequestForApps { get; set; }
        public DbSet<WMPayment> WMPayment { get; set; }
        public DbSet<DownloadLink> DownloadLinks { get; set; }
        public DbSet<Procurement> Procurements { get; set; }

        //Culture
        public DbSet<TranslateCity> TranslateCities { get; set; }
        public DbSet<TranslateImage> TranslateImages { get; set; }
        public DbSet<TranslatePackage> TranslatePackages { get; set; }
        public DbSet<TranslatePlace> TranslatePlaces { get; set; }

        //seller
        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<Price> Prices { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProcurementConfig());
            modelBuilder.Configurations.Add(new PaymentConfig());
            modelBuilder.Configurations.Add(new WMPaymentConfig());
            //modelBuilder.Configurations.Add(new PackageConfig());
            base.OnModelCreating(modelBuilder);
        }


    }
}