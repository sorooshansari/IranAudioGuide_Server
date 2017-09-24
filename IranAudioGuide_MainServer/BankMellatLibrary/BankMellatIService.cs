using BankMellatLibrary.MellatWebService;
using System;

namespace BankMellatLibrary
{
   public class BankMellatIService
    {

        #region Base Variable Definition

        public static readonly string PgwSite = "https://bpm.shaparak.ir/pgwchannel/startpay.mellat";
        static readonly string callBackUrl = "https://iranaudioguide.com/PaymentBankMelat/BankMelatCallback";
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
        /// درخواست انجام يک تراکنش را از بانک مي نمايد
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
                var WebService  = new PaymentGatewayImplService();
                //result = bpService.bpPayRequest

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
                var WebService = new PaymentGatewayImplService();
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
                var WebService = new PaymentGatewayImplService();
                return WebService.bpSettleRequest(terminalId, userName, password, orderId, saleOrderId, saleReferenceId);

            }
            catch (Exception Error)
            {
                throw new Exception(Error.Message);
            }
        }
        /// <summary>
       
        /// در اﻳﻦ ﺣﺎﻟﺖ ﺑﺮای آﮔﺎﻫﻲ از نتیجه ﺗﺮاﻛﻨﺶ می ﺗﻮاﻧید در هر زﻣﺎﻧﻲ، اﻳﻦ ﻣﺘﺪ را ﻓﺮاﺧﻮاﻧﻲ ﻧﻤﺎید
        /// (اﺳﺘﻌﻼم ﺗﺮاﻛﻨﺶ)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="saleOrderId"></param>
        /// <param name="saleReferenceId"></param>
        /// <returns></returns>
        public string InquiryRequest(long orderId, long saleOrderId, long saleReferenceId)
        {
            // اﻳﻦ ﻣﺘﺪ زﻣﺎﻧﻲ کارﺑﺮد دارد ﻛﻪ ﺑﻪ ﻫﺮ دﻟﻴﻠﻲ از ﻧﺘﻴﺠﻪ ﻣﻘﺪار ﺑﺎزﮔﺸﺘﻲ
            // bpVerifyRequest ﻣﻄﻠﻊ ﻧﮕﺮدید.
            try
            {
                var WebService = new PaymentGatewayImplService();
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
                var WebService = new PaymentGatewayImplService();
                return WebService.bpReversalRequest(terminalId, userName, password, orderId, saleOrderId, saleReferenceId);

            }
            catch (Exception error)
            {
                throw new Exception(error.Message); ;
            }
        }
      
    }
}