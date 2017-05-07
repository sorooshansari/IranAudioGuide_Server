using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IranAudioGuide_MainServer.Models;
using Neodynamic.SDK.Barcode;
using System.IO;


using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace IranAudioGuide_MainServer.Services
{
    public class BarcodeServices : IDisposable
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public bool Creatbarcode(double Price, int Quantity, string SellerName)
        {
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    List<Barcode> barcodes = new List<Barcode>();
                    for (int i = 0; i < Quantity; i++)
                    {
                        barcodes.Add(new Barcode() { Bar_Price = Price, Bar_IsUsed = false, Bar_SellerName = SellerName });
                    }
                    db.Barcodes.AddRange(barcodes);
                    db.SaveChanges();
                    //string logoPath = HttpContext.Current.Server.MapPath("~/Content/images/miniBWlogo.jpg");
                    //System.Drawing.Image logo = System.Drawing.Image.FromFile(logoPath);
                    foreach (var b in barcodes)
                    {
                        string barcodeString = string.Format("{0}#{1}#{2}", b.Bar_Id, Price, SellerName);
                        //byte[] barcodeArray = GetBarcodeImg(barcodeString);
                        string imgName = string.Format("{0}.jpeg", b.Bar_Id);
                        string path = string.Format("{0}/{1}", HttpContext.Current.Server.MapPath("~/Content/images/barcodes"), imgName);
                        CreateCode(barcodeString, path);
                        //var fs = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
                        //fs.Write(barcodeArray);
                        //fs.Close();
                        b.Bar_Image_Url = imgName;
                    }
                    db.SaveChanges();
                    dbTran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return false;
                }
            }
                    
        }
        #region tools
        protected void CreateCode(string value, string destPath)
        {
            QRCodeEncoder encoder = new QRCodeEncoder();

            encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H; // 30%
            encoder.QRCodeScale = 10;

            Bitmap img = encoder.Encode(value);

            //int left = (img.Width / 2) - (logo.Width / 2);
            //int top = (img.Height / 2) - (logo.Height / 2);

            //Graphics g = Graphics.FromImage(img);

            //g.DrawImage(logo, new Point(left, top));

            img.Save(destPath, ImageFormat.Jpeg);
        }
        private byte[] GetBarcodeImg(string value)
        {
            BarcodeProfessional barcode = new BarcodeProfessional();
            barcode.Symbology = Symbology.QRCode;
            barcode.Code = value;
            return barcode.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        #endregion

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
        }
}