using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IranAudioGuide_MainServer.Models
{
    public class WBViewModel
    {
    }
    public class SuccessVM
    {
        public string LMI_PAYEE_PURSE { get; set; }
        public string LMI_PAYMENT_AMOUNT { get; set; }
        public string LMI_PAYMENT_NO { get; set; }
        public string LMI_MODE { get; set; }

        public string LMI_SYS_INVS_NO { get; set; }
        public string LMI_SYS_TRANS_NO { get; set; }

        public string LMI_PAYER_PURSE { get; set; }
        public string LMI_PAYER_WM { get; set; }
        public string LMI_CAPITALLER_WMID { get; set; }
        public string LMI_PAYMER_NUMBER { get; set; }
        public string LMI_PAYMER_EMAIL { get; set; }
        public string LMI_EURONOTE_NUMBER { get; set; }
        public string LMI_EURONOTE_EMAIL { get; set; }
        public string LMI_WMCHECK_NUMBER { get; set; }
        public string LMI_TELEPAT_PHONENUMBER { get; set; }
        public string LMI_TELEPAT_ORDERID { get; set; }
        public string LMI_PAYMENT_CREDITDAYS { get; set; }

        public string LMI_HOLD { get; set; }

        public string LMI_HASH { get; set; }
        public string LMI_HASH2 { get; set; }
        public string LMI_SYS_TRANS_DATE { get; set; }
        public string LMI_SECRET_KEY { get; set; }

        public string LMI_SDP_TYPE { get; set; }
        public string LMI_PAYER_COUNTRYID { get; set; }
        public string LMI_PAYER_PCOUNTRYID { get; set; }
        public string LMI_PAYER_IP { get; set; }
        public string UserName { get; set; }
    }
}