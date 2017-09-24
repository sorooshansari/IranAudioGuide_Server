using BankMellatLibrary.MellatWebService;
using System;

namespace BankMellatLibrary
{
   public class BankMellatIService
    {

        #region Base Variable Definition

        public static readonly string PgwSite = "https://bpm.shaparak.ir/pgwchannel/startpay.mellat";
        static readonly string callBackUrl = "https://iranaudioguide.com/Payment/BankMelatCallback";
        //ﺷﻤﺎره ﭘﺎﻳﺎﻧﻪ ﭘﺬﻳﺮﻧﺪه 
        static readonly long terminalId = 2789695;
        static readonly string userName = "ira49";
        static readonly string password = "65056090";

        string localDate = string.Empty;
        string localTime = string.Empty;
        #endregion

        public BankMellatIService()
        {
            try
            {
                localDate = DateTime.Now.ToString("yyyyMMdd");
                localTime = DateTime.Now.ToString("HHMMSS");
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
        }
        /// <summary>
        /// به درگاه پرداخت متصل شده و مبلغی را که تعیین کرده اید از حساب فرد کسر خواهد کرد
        /// </summary>
        /// <param name="orderId">ﺷﻤﺎره درﺧﻮاﺳﺖ-- حتما باید یکتا باشد</param>
        /// <param name="priceAmount">ﻣﺒﻠﻎ ﺧﺮﻳﺪ</param>
        /// <param name="additionalText">اﻃﻼﻋﺎت ﺗﻮﺿﻴﺤﻲ ﻛﻪ ﭘﺬﻳﺮﻧﺪه ﻣﺎﻳﻞ ﺑﻪ ﺣﻔﻆ آﻧﻬﺎ ﺑﺮای ﻫﺮ ﺗﺮاﻛﻨﺶ ﻣﻲ ﺑﺎﺷﺪ</param>
        /// <returns> ReturnCodeForBpPayRequest,RefID(شماره پیگیری تراکنش شما) </returns>
        public string BpPayRequest(long orderId, long priceAmount, string additionalText)
        {
            try
            {
                var WebService = new PaymentGatewayClient();
     
                //if result == 0,RefID --> عملایت موفقیت آمیز بوده است 
                var result = WebService.bpPayRequest(terminalId, userName, password, orderId, priceAmount, localDate, localTime,
                                 additionalText, callBackUrl, 0);

                return result;
                // result.SelectMany(n => n.Split(',')).ToList();
            }
            catch (Exception error)
            {
                throw new Exception(error.Message); ;
            }
        }

        /// <summary>
        /// با استفاده از این متد می توانید وضعیت یک تراکنش را بر اساس شماره پیگری که در اختیار دارید، بررسی کنید
        ///ﻣﺘﺪ ﺗﺎﻳﻴد تراکنش ﺧﺮید 
        /// </summary>
        /// <param name="orderId">ﺷﻤﺎره درﺧﻮاﺳﺖ-- حتما باید یکتا باشد -- id خرید</param>
        /// <param name="saleOrderId">ﺷﻤﺎره درﺧﻮاﺳﺖ ﭘﺮداﺧﺖ</param>
        /// <param name="saleReferenceId">ﻛﺪ ﻣﺮﺟﻊ ﺗﺮاﻛﻨﺶ ﺧﺮﻳﺪ</param>
        /// <returns></returns>

        public string VerifyRequest(long orderId, long saleOrderId, long saleReferenceId)
        {
            try
            {
                var WebService = new PaymentGatewayClient(); 
                return WebService.bpVerifyRequest(terminalId, userName, password, orderId, saleOrderId, saleReferenceId);

            }
            catch (Exception Error)
            {
                throw new Exception(Error.Message);
            }
        }
        /// <summary>
        /// ﺑﺎﻧﻚ ﺗﺮاﻛﻨﺸﻬﺎی ﺗﺎﻳﻴﺪ ﺷﺪه  ﺗﻮﺳﻂ اﻳﻦ ﻣﺘﺪ را ﻃﺒﻖ ﻗﺮارداد ﺑﻪ ﺣﺴﺎب شما وارﻳﺰ می کند. 
        /// ﻣﻘﺪار ﺑﺮﮔﺸﺘﻲ "0" ﺑﻪ ﻣﻌﻨﺎی ﻣﻮﻓﻖ درﺧﻮاﺳﺖ وارﻳﺰ به حساب شما می ﺑﺎﺷﺪ.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="saleOrderId"></param>
        /// <param name="saleReferenceId"></param>
        /// <returns></returns>
        public string SettleRequest(long orderId, long saleOrderId, long saleReferenceId)
        {
            try
            {
                var WebService = new PaymentGatewayClient();
                return WebService.bpSettleRequest(terminalId, userName, password, orderId, saleOrderId, saleReferenceId);

            }
            catch (Exception Error)
            {
                throw new Exception(Error.Message);
            }
        }
        /// <summary>
        /// اﻳﻦ ﻣﺘﺪ زﻣﺎﻧﻲ کارﺑﺮد دارد ﻛﻪ ﺑﻪ ﻫﺮ دﻟﻴﻠﻲ از ﻧﺘﻴﺠﻪ ﻣﻘﺪار ﺑﺎزﮔﺸﺘﻲ bpVerifyRequest ﻣﻄﻠﻊ ﻧﮕﺮدید.
        /// در اﻳﻦ ﺣﺎﻟﺖ ﺑﺮای آﮔﺎﻫﻲ از نتیجه ﺗﺮاﻛﻨﺶ می ﺗﻮاﻧید در هر زﻣﺎﻧﻲ، اﻳﻦ ﻣﺘﺪ را ﻓﺮاﺧﻮاﻧﻲ ﻧﻤﺎید
        /// (اﺳﺘﻌﻼم ﺗﺮاﻛﻨﺶ)


        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="saleOrderId"></param>
        /// <param name="saleReferenceId"></param>
        /// <returns></returns>



        public string InquiryRequest(long orderId, long saleOrderId, long saleReferenceId)
        {
            try
            {
              var WebService = new PaymentGatewayClient();
                return WebService.bpInquiryRequest(terminalId, userName, password, orderId, saleOrderId, saleReferenceId);

            }
            catch (Exception Error)
            {
                throw new Exception(Error.Message);
            }
        }
        /// <summary>
        /// درخواست برگشت وجه--
        /// از بانک درخواست کنید تا اگر این کاربر مبلغی را به حساب شما واریز کرده است، آن را به حسابش بازگرداند
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="saleOrderId"></param>
        /// <param name="saleReferenceId"></param>
        /// <returns></returns>
        public string BpReversalRequest(long orderId, long saleOrderId, long saleReferenceId)
        {
            try
            {
                var WebService = new PaymentGatewayClient();
                return WebService.bpReversalRequest(terminalId, userName, password, orderId, saleOrderId, saleReferenceId);

            }
            catch (Exception error)
            {
                throw new Exception(error.Message); ;
            }
        }
        public int GetNumberForStatusCode(ReturnCodeForBpPayRequest msg) {
            return (int)msg;
        }
        public String GetMessageForStatusCode(string msg)
        {
            var statusCode = 0;
            try
            {
                statusCode = Convert.ToInt32(msg);
            }
            catch
            {
                return "";
            }
            switch (statusCode)
            {
                case 0:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_ﺑﺎ_ﻣﻮﻓﻘﻴﺖ_اﻧﺠﺎم_ﺷﺪ.ToString();
                case 11:
                    return ReturnCodeForBpPayRequest.ﺷﻤﺎره_ﻛﺎرت_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 12:
                    return ReturnCodeForBpPayRequest.ﻣﻮﺟﻮدی_ﻛﺎﻓﻲ_ﻧﻴﺴﺖ.ToString();
                case 13:
                    return ReturnCodeForBpPayRequest.رﻣﺰ_ﻧﺎدرﺳﺖ_اﺳﺖ.ToString();
                case 14:
                    return ReturnCodeForBpPayRequest.ﺗﻌﺪاد_دﻓﻌﺎت_وارد_ﻛﺮدن_رﻣﺰ_ﺑﻴﺶ_از_ﺣﺪ_ﻣﺠﺎز_اﺳﺖ.ToString();
                case 15:
                    return ReturnCodeForBpPayRequest.ﻛﺎرت_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 16:
                    return ReturnCodeForBpPayRequest.دﻓﻌﺎت_ﺑﺮداﺷﺖ_وﺟﻪ_ﺑﻴﺶ_از_ﺣﺪ_ﻣﺠﺎز_اﺳﺖ.ToString();
                case 17:
                    return ReturnCodeForBpPayRequest.ﻛﺎرﺑﺮ_از_اﻧﺠﺎم_ﺗﺮاﻛﻨﺶ_ﻣﻨﺼﺮف_ﺷﺪه_اﺳﺖ.ToString();
                case 18:
                    return ReturnCodeForBpPayRequest.ﺗﺎرﻳﺦ_اﻧﻘﻀﺎی_ﻛﺎرت_ﮔﺬﺷﺘﻪ_اﺳﺖ.ToString();
                case 19:
                    return ReturnCodeForBpPayRequest.ﻣﺒﻠﻎ_ﺑﺮداﺷﺖ_وﺟﻪ_ﺑﻴﺶ_از_ﺣﺪ_ﻣﺠﺎز_اﺳﺖ.ToString();
                case 111:
                    return ReturnCodeForBpPayRequest.ﺻﺎدر_ﻛﻨﻨﺪه_ﻛﺎرت_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 112:
                    return ReturnCodeForBpPayRequest.ﺧﻄﺎی_ﺳﻮﻳﻴﭻ_ﺻﺎدر_ﻛﻨﻨﺪه_ﻛﺎرت.ToString();
                case 113:
                    return ReturnCodeForBpPayRequest.ﭘﺎﺳﺨﻲ_از_ﺻﺎدر_ﻛﻨﻨﺪه_ﻛﺎرت_درﻳﺎﻓﺖ_ﻧﺸﺪ.ToString();
                case 114:
                    return ReturnCodeForBpPayRequest.دارﻧﺪه_ﻛﺎرت_ﻣﺠﺎز_ﺑﻪ_اﻧﺠﺎم_اﻳﻦ_ﺗﺮاﻛﻨﺶ_ﻧﻴﺴﺖ.ToString();
                case 21:
                    return ReturnCodeForBpPayRequest.ﭘﺬﻳﺮﻧﺪه_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 23:
                    return ReturnCodeForBpPayRequest.ﺧﻄﺎی_اﻣﻨﻴﺘﻲ_رخ_داده_اﺳﺖ.ToString();
                case 24:
                    return ReturnCodeForBpPayRequest.اﻃﻼﻋﺎت_ﻛﺎرﺑﺮی_ﭘﺬﻳﺮﻧﺪه_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 25:
                    return ReturnCodeForBpPayRequest.ﻣﺒﻠﻎ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 31:
                    return ReturnCodeForBpPayRequest.ﭘﺎﺳﺦ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 32:
                    return ReturnCodeForBpPayRequest.ﻓﺮﻣﺖ_اﻃﻼﻋﺎت_وارد_ﺷﺪه_ﺻﺤﻴﺢ_ﻧﻤﻲ_ﺑﺎﺷﺪ.ToString();
                case 33:
                    return ReturnCodeForBpPayRequest.ﺣﺴﺎب_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 34:
                    return ReturnCodeForBpPayRequest.ﺧﻄﺎی_ﺳﻴﺴﺘﻤﻲ.ToString();
                case 35:
                    return ReturnCodeForBpPayRequest.ﺗﺎرﻳﺦ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 41:
                    return ReturnCodeForBpPayRequest.ﺷﻤﺎره_درﺧﻮاﺳﺖ_ﺗﻜﺮاری_اﺳﺖ.ToString();
                case 42:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_Sale_یافت_نشد_.ToString();
                case 43:
                    return ReturnCodeForBpPayRequest.ﻗﺒﻼ_Verify_درﺧﻮاﺳﺖ_داده_ﺷﺪه_اﺳﺖ.ToString();



                case 44:
                    return ReturnCodeForBpPayRequest.درخواست_verify_یافت_نشد.ToString();
                case 45:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_Settle_ﺷﺪه_اﺳﺖ.ToString();
                case 46:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_Settle_نشده_اﺳﺖ.ToString();

                case 47:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_Settle_یافت_نشد.ToString();
                case 48:
                    return ReturnCodeForBpPayRequest.تراکنش_Reverse_شده_است.ToString();
                case 49:
                    return ReturnCodeForBpPayRequest.تراکنش_Refund_یافت_نشد.ToString();
                case 412:
                    return ReturnCodeForBpPayRequest.شناسه_قبض_نادرست_است.ToString();
                case 413:
                    return ReturnCodeForBpPayRequest.ﺷﻨﺎﺳﻪ_ﭘﺮداﺧﺖ_ﻧﺎدرﺳﺖ_اﺳﺖ.ToString();
                case 414:
                    return ReturnCodeForBpPayRequest.سازﻣﺎن_ﺻﺎدر_ﻛﻨﻨﺪه_ﻗﺒﺾ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 415:
                    return ReturnCodeForBpPayRequest.زﻣﺎن_ﺟﻠﺴﻪ_ﻛﺎری_ﺑﻪ_ﭘﺎﻳﺎن_رسیده_است.ToString();
                case 416:
                    return ReturnCodeForBpPayRequest.ﺧﻄﺎ_در_ﺛﺒﺖ_اﻃﻼﻋﺎت.ToString();
                case 417:
                    return ReturnCodeForBpPayRequest.ﺷﻨﺎﺳﻪ_ﭘﺮداﺧﺖ_ﻛﻨﻨﺪه_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 418:
                    return ReturnCodeForBpPayRequest.اﺷﻜﺎل_در_ﺗﻌﺮﻳﻒ_اﻃﻼﻋﺎت_ﻣﺸﺘﺮی.ToString();
                case 419:
                    return ReturnCodeForBpPayRequest.ﺗﻌﺪاد_دﻓﻌﺎت_ورود_اﻃﻼﻋﺎت_از_ﺣﺪ_ﻣﺠﺎز_ﮔﺬﺷﺘﻪ_اﺳﺖ.ToString();
                case 421:
                    return ReturnCodeForBpPayRequest.IP_نامعتبر_است.ToString();

                case 51:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_ﺗﻜﺮاری_اﺳﺖ.ToString();
                case 54:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_ﻣﺮﺟﻊ_ﻣﻮﺟﻮد_ﻧﻴﺴﺖ.ToString();
                case 55:
                    return ReturnCodeForBpPayRequest.ﺗﺮاﻛﻨﺶ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ.ToString();
                case 61:
                    return ReturnCodeForBpPayRequest.ﺧﻄﺎ_در_واریز.ToString();

            }
            return "";
        }
    }
}