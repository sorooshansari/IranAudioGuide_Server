using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IranAudioGuide_MainServer.Models;
using System.IO;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using IranAudioGuide_MainServer.Services;

namespace IranAudioGuide_MainServer.Services
{
    public class BarcodeServices : IDisposable
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public List<SelectListItem> getPrices()
        {
            try
            {
                //var res = (from p in db.Prices
                //           select new PriceVM()
                //           {
                //               id = p.Pri_Id,
                //               value = p.Pri_Value
                //           }).ToList();
                var res = db.Prices.OrderBy(x => x.Pri_Value).Select(x => new SelectListItem()
                {
                    Value = x.Pri_Id.ToString(),
                    Text = x.Pri_Value.ToString()
                }).ToList();
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public bool Creatbarcode(int PriceId, int Quantity, string SellerName)
        {
            using (var dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    var price = db.Prices.Where(x => x.Pri_Id == PriceId).FirstOrDefault();
                    List<Barcode> barcodes = new List<Barcode>();
                    for (int i = 0; i < Quantity; i++)
                    {
                        barcodes.Add(new Barcode() { Bar_Price = price, Bar_IsUsed = false, Bar_SellerName = SellerName });
                    }
                    db.Barcodes.AddRange(barcodes);
                    db.SaveChanges();
                    //string logoPath = HttpContext.Current.Server.MapPath("~/Content/images/miniBWlogo.jpg");
                    //System.Drawing.Image logo = System.Drawing.Image.FromFile(logoPath);
                    foreach (var b in barcodes)
                    {
                        string barcodeString = CryptographyService.Encrypt(string.Format("{0};{1};{2}", b.Bar_Id, price.Pri_Value.ToString(), SellerName), true);
                        //byte[] barcodeArray = GetBarcodeImg(barcodeString);
                        string imgName = string.Format("{0}.jpeg", b.Bar_Id);
                        string path = string.Format("{0}/{1}", HttpContext.Current.Server.MapPath("~/Content/images/barcodes"), imgName);
                        //string pdfpath = string.Format("{0}/{1}", HttpContext.Current.Server.MapPath("~/Content/images/pdf"), imgName);
                        CreateQRCode(barcodeString, path);
                        //ImagesToPdf(path, pdfpath);
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
        protected void CreateQRCode(string value, string destPath)
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
        //private byte[] GetBarcodeImg(string value)
        //{
        //    BarcodeProfessional barcode = new BarcodeProfessional();
        //    barcode.Symbology = Symbology.QRCode;
        //    barcode.Code = value;
        //    return barcode.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Jpeg);
        //}
        //public void ImagesToPdf(string imagepaths, string pdfpath)
        //{
        //    iTextSharp.text.Rectangle pageSize = null;

        //    using (var srcImage = new Bitmap(imagepaths.ToString()))
        //    {
        //        pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
        //    }

        //    using (var ms = new MemoryStream())
        //    {
        //        var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
        //        iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
        //        document.Open();
        //        var image = iTextSharp.text.Image.GetInstance(imagepaths.ToString());
        //        document.Add(image);
        //        document.Close();

        //        File.WriteAllBytes(pdfpath + "guide.pdf", ms.ToArray());
        //    }
        //}
        public long Getpackage(Guid id)
        {
            try
            {
                var res = (from p in db.Packages
                           where p.Pac_Id == id
                           select p.Pac_Price).FirstOrDefault();
                return (res == default(long)) ? 0 : res;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return 0;
            }
        }
        /// <summary>
        /// use Getbarcode for BuyWithBarcode
        /// </summary>
        /// <param name="id">barcode id</param>
        /// <returns>is used,price</returns>
        public BarcodeVM GetBarcodes(int id)
        {
            var res = (from b in db.Barcodes
                       where b.Bar_Id == id
                       select new BarcodeVM()
                       {
                           isUsed = b.Bar_IsUsed,
                           price = b.Bar_Price.Pri_Value,
                           sellerName = b.Bar_SellerName
                       }).FirstOrDefault();
            return res;
        }
        public ConvertBarcodetoStringVM ConvertBarcodetoString(string barcode)
        {
            string decryptedBarcode = CryptographyService.Decrypt(barcode, true);
            var list = decryptedBarcode.Split(';').ToList();
            ConvertBarcodetoStringVM cbs = new ConvertBarcodetoStringVM()
            {
                CBS_id_bar = int.Parse(list[0]),
                CBS_price_pri = double.Parse(list[1]),
                CBS_sellername = list[2]
            };
            return cbs;
        }
        /// <summary>
        ///  new row  Procurement and barisused
        /// </summary>
        /// <param name="CBS_id_bar">ConvertBarcodetoString id</param>
        /// <param name="userid">userid</param>
        /// <param name="packid">packageid</param>
        public void saved(int CBS_id_bar,string userid,Guid packid)
        {           
            var bars = db.Barcodes.Where(s => s.Bar_Id == CBS_id_bar).FirstOrDefault();
            bars.Bar_IsUsed = true;          
            var t = new Procurement { Bar_Id = CBS_id_bar, Pro_PaymentFinished = true, Id = userid, Pac_Id = packid };
            db.Procurements.Add(t);
            db.SaveChanges();
        }
        public GeneratePDFModel DownloadPDF1()
        {
            try
            {
                //get the information to display in pdf from database
                //for the time
                //Hard coding values are here, these are the content to display in pdf 
                var content = "راهنمای صوتی ایران";
                 //   +
                 //"<p>Ohh This is very easy to generate pdf using Rotativa <p>";
                var logoFile = @"/Images/IAGappHeaderLOGO.png";
                string barImgPath = @"/Content/images/barcodes/";

                var userName = HttpContext.Current.User.Identity.Name;

                List<BarImageInfo> ImgInfo = db.Barcodes
                    .Where(x => x.Bar_IsUsed == false && x.Bar_SellerName == userName)
                    .Select(t => new BarImageInfo()
                    {
                        ImageUrl = barImgPath + t.Bar_Image_Url,
                        Price = t.Bar_Price.Pri_Value
                    }).ToList();
                //var img = @"/Content/images/barcodes/8/.jpeg";
                var result = new GeneratePDFModel()
                {
                    PDFContent = content,
                    PDFLogo = HttpContext.Current.Server.MapPath(logoFile),
                    ImageInfoes = ImgInfo
                };
                
                
                return (result);

                //Use ViewAsPdf Class to generate pdf using GeneratePDF.cshtml view

            }

            catch (Exception ex)
            {

                throw;
            }
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