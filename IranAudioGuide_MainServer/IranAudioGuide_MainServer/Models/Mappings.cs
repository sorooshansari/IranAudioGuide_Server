

using System.Data.Entity.ModelConfiguration;

namespace IranAudioGuide_MainServer.Models
{

    public class ProcurementConfig : EntityTypeConfiguration<Procurement>
    {
        public ProcurementConfig()
        {

            // one-to-one
            this.HasOptional(x => x.Payment)
                .WithRequired(x => x.procurement)
                //.Map(m => m.MapKey("PayId"))
                .WillCascadeOnDelete();

            // one-to-one
            this.HasOptional(x => x.WMPayment)
                .WithRequired(x => x.procurement)
                //.Map(m => m.MapKey("WMPaymentId"))
                .WillCascadeOnDelete();


            this.HasRequired(x => x.User)
                .WithMany(x => x.procurements)
                //.HasForeignKey(x=> x.UserId)
                .WillCascadeOnDelete(false);

        }
    }
    public class PackageConfig : EntityTypeConfiguration<Package>
    {
        public PackageConfig()
        {
            // one-to-one
            this.HasOptional(x => x.procurement)
                .WithRequired(x => x.Package)
                // .Map(m => m.MapKey("Pac_Id"))
                .WillCascadeOnDelete();
        }
    }


}