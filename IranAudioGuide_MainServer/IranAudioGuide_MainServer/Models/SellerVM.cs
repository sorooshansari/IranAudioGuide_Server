using IranAudioGuide_MainServer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IranAudioGuide_MainServer.Models
{
    /// <summary>
    /// creat barcod for seller
    /// </summary>
    public class CreatBarCodeVM
    {
        [Required]
        public int PriceId { get; set; }
        [Required]
        public int quantity { get; set; }
        public List<SelectListItem> prices { get; set; }
    }
    /// <summary>
    /// price for seller
    /// </summary>
    public class PriceVM
    {
        public int id { get; set; }
        public double value { get; set; }
    }
    /// <summary>
    /// barcode for buyer
    /// </summary>
    public class BarcodeVM
    {
        public double price { get; set; }
        public bool isUsed { get; set; }
        public string sellerName { get; set; }
    }
    public class BuyWithBarcodeVM
    {
        public Guid packId { get; set; }
        public string email { get; set; }
        public string uuid { get; set; }
        public string barcode { get; set; }

    }
    public enum BuyWithBarcodeStatus
    {
        confirmed = 1,
        notUser = 2,
        uuidMissMatch = 3,
        notConfirmed = 4,
        unknownError = 5,
        invalidBarcode = 6,
        invalidprice = 7,
        isused_true = 8,
        invalidSellerName = 9,
        invalidpackprice = 10,
        success = 11
    }
    /// <summary>
    /// convert barcode to string for buy with barcode
    /// </summary>
    public class ConvertBarcodetoStringVM
    {
        public int CBS_id_bar { get; set; }
        public double CBS_price_pri { get; set; }
        public string CBS_sellername { get; set; }
    }
    public class GeneratePDFModel
    {
        public string PDFContent { get; set; }
        public string PDFLogo { get; set; }
        public List<BarImageInfo> ImageInfoes { get; set; }

    }
    public class BarImageInfo
    {
        public string ImageUrl { get; set; }
        public double Price { get; set; }
    }
}