using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace IranAudioGuide_Server.Models
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
    public class Place
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid Pla_Id { get; set; }
        public string Pla_Name { get; set; }
        public string Pla_ImgUrl { get; set; }
        public string Pla_Discription { get; set; }
        public List<Audio> Pla_Audios { get; set; }
        public city Pla_city { get; set; }
        public double Pla_cordinate_X { get; set; }
        public double Pla_cordinate_Y { get; set; }
        public string Pla_Address { get; set; }
    }
    public class city
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Cit_Id { get; set; }
        public string Cit_Name { get; set; }
        public string Cit_Description { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IranAudioGuide", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Audio> Audios { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<city> Cities { get; set; }
        //public System.Data.Entity.DbSet<IranAudioGuide_Server.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}