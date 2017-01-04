﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace IranAudioGuide_MainServer.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
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
    }
    public class Payment
    {
        public Payment()
        {
            InsertDatetime = DateTime.Now;
        }
        [Key]
        [Display(Name = "Payment Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PaymentId { get; set; }

        [Display(Name = "Reference Number")]
        [MaxLength(100)]
        public string ReferenceNumber { get; set; }

        [Display(Name = "Sale Reference Id")]
        public long SaleReferenceId { get; set; }

        [Display(Name = "Status Payment")]
        [MaxLength(100)]
        public string StatusPayment { get; set; }

        // فقط در صورتی که این فید ترو باشد پرداخت موفق بوده است
        [Display(Name = "Payment Finished")]
        public bool PaymentFinished { get; set; }

        [Display(Name = "Amount")]
        public long Amount { get; set; }

        [Display(Name = "Bank Name")]
        [MaxLength(50)]
        public string BankName { get; set; }

        [Display(Name = "User")]
        public ApplicationUser User { get; set; }

        [Display(Name = "Package")]
        public Package Package { get; set; }

        [Display(Name = "Insert Datetime")]
        public DateTime InsertDatetime { get; set; }
    }
    public class UpdateLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UpL_Id { get; set; }
        public System.Guid? Aud_Id { get; set; }
        public System.Guid? Pla_ID { get; set; }
        public int? Cit_ID { get; set; }
        public System.Guid? Img_Id { get; set; }
        public System.Guid? Sto_Id { get; set; }
        public System.Guid? Tip_Id { get; set; }
        public System.Guid? Ima_Id { get; set; }
        public bool isRemoved { get; set; }
    }
    public class Audio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Aud_Id { get; set; }
        public string Aud_Name { get; set; }
        public string Aud_Url { get; set; }
        public string Aud_Discription { get; set; }
        public virtual Place Pla_Id { get; set; }
    }
    public class Story
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Sto_Id { get; set; }
        public string Sto_Name { get; set; }
        public string Sto_Url { get; set; }
        public string Sto_Discription { get; set; }
        public virtual Place Pla_Id { get; set; }
    }
    public class Place
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Pla_Id { get; set; }
        public string Pla_Name { get; set; }
        public string Pla_ImgUrl { get; set; }
        public string Pla_TumbImgUrl { get; set; }
        public string Pla_Discription { get; set; }
        public List<Audio> Pla_Audios { get; set; }
        public List<Story> Pla_Stories { get; set; }
        public List<Image> Pla_ExtraImages { get; set; }
        public List<Tip> Pla_Tips { get; set; }
        public city Pla_city { get; set; }
        public double Pla_cordinate_X { get; set; }
        public double Pla_cordinate_Y { get; set; }
        public string Pla_Address { get; set; }
        public bool Pla_Deactive { get; set; }
        public bool Pla_isOnline { get; set; }
        public bool Pla_isPrimary { get; set; }
    }

    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Img_Id { get; set; }
        public string Img_Name { get; set; }
        public virtual Place Pla_Id { get; set; }
        public string Img_Description { get; set; }
    }
    public class Tip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Tip_Id { get; set; }
        public TipCategory Tip_Category { get; set; }
        public string Tip_Content { get; set; }
        public virtual Place Pla_Id { get; set; }

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
    public class city
    {
        public city()
        {
            Cit_Packages = new HashSet<Package>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Cit_Id { get; set; }
        public string Cit_Name { get; set; }
        public string Cit_Description { get; set; }
        public virtual ICollection<Package> Cit_Packages { get; set; }
    }
    public class Package
    {
        public Package()
        {
            Pac_Cities = new HashSet<city>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Pac_Id { get; set; }
        public string Pac_Name { get; set; }
        public long Pac_Price { get; set; }
        public virtual ICollection<city> Pac_Cities { get; set; }
        public object pac_Cities { get; internal set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(ConnectionString.connString
                  , throwIfV1Schema: false)
        {
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
        //public DbSet<OnlinePlace> OnlinePlaces { get; set; }
        //public System.Data.Entity.DbSet<IranAudioGuide_MainServer.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}