

using System.Data.Entity.ModelConfiguration;

namespace IranAudioGuide_MainServer.Models
{

    public class PlaceConfig : EntityTypeConfiguration<Place>
    {
        public PlaceConfig()
        {
            // one-to-one
            this.HasMany(x => x.TranslatePlaces)
                .WithRequired(x => x.Place)
                .WillCascadeOnDelete(true);

            // one-to-one
            this.HasMany(x => x.Pla_ExtraImages)
                .WithRequired(x => x.Place)
                .WillCascadeOnDelete(true);
            
            // one-to-one
            this.HasMany(x => x.Pla_Tips)
                .WithRequired(x => x.Place)
                .WillCascadeOnDelete(true);


        }
    }

    public class ImageConfig : EntityTypeConfiguration<Image>
    {
        public ImageConfig()
        {
            // one-to-one
            this.HasMany(x => x.TranslateImages)
                .WithRequired(x => x.Image)
                .WillCascadeOnDelete(true);
            

        }
    }
    public class ProcurementConfig : EntityTypeConfiguration<Procurement>
    {
        public ProcurementConfig()
        {

            //// one-to-one
            //this.HasOptional(x => x.Payment)
            //    .WithRequired(x => x.procurement)
            //    //.Map(m => m.MapKey("PayId"))
            //    .WillCascadeOnDelete();

            //// one-to-one
            //this.HasOptional(x => x.WMPayment)
            //    .WithRequired(x => x.procurement)
            //    //.Map(m => m.MapKey("WMPaymentId"))
            //    .WillCascadeOnDelete();


            this.HasRequired(x => x.Pro_User)
                .WithMany(x => x.procurements)
                //.HasForeignKey(x=> x.UserId)
                .WillCascadeOnDelete(false);

            this.HasRequired(x => x.Pro_Package)
                .WithMany(x => x.procurements)
                //.HasForeignKey(x=> x.UserId)
                .WillCascadeOnDelete(false);

        }
    }

    public class PaymentConfig : EntityTypeConfiguration<Payment>
    {
        public PaymentConfig()
        {
            // one-to-one
            this.HasRequired(x => x.Pay_Procurement)
                .WithOptional(x => x.Pro_Payment)
                // .Map(m => m.MapKey("Pac_Id"))
                .WillCascadeOnDelete();
        }
    }
    public class WMPaymentConfig : EntityTypeConfiguration<WMPayment>
    {
        public WMPaymentConfig()
        {
            // one-to-one
            this.HasRequired(x => x.WMP_Procurement)
                .WithOptional(x => x.Pro_WMPayment)
                // .Map(m => m.MapKey("Pac_Id"))
                .WillCascadeOnDelete();
        }
    }
    //public class PackageConfig : EntityTypeConfiguration<Package>
    //{
    //    public PackageConfig()
    //    {
    //        // one-to-one
    //        this.HasOptional(x => x.procurement)
    //            .WithRequired(x => x.Package)
    //            // .Map(m => m.MapKey("Pac_Id"))
    //            .WillCascadeOnDelete();
    //    }
    //}


}