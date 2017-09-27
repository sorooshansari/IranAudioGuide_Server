using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IranAudioGuide_MainServer.Models
{
    public enum EnumBankName
    {
        Mellat = 0,
        Zarinpal = 1,
        Webmoney = 2
    };
    public enum ReturnCodeForBpPayRequest
    {
        [Display(Name = "BankMelatcode0", ResourceType = typeof(App_GlobalResources.Global))]
        Success = 0,
        [Display(Name = "BankMelatcode11", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidCardNumber = 11,
        


        [Display(Name = "BankMelatcode12", ResourceType = typeof(App_GlobalResources.Global))]
        YourCreditIsNotEnough = 12,

        [Display(Name = "BankMelatcode13", ResourceType = typeof(App_GlobalResources.Global))]
        ThePasswordIsIncorrect = 13,

        [Display(Name = "BankMelatcode14", ResourceType = typeof(App_GlobalResources.Global))]
        NumberPasswordExceeded = 14,

        //ﻛﺎرت_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 15,
        [Display(Name = "BankMelatcode15", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidCard = 15,

        //دﻓﻌﺎت_ﺑﺮداﺷﺖ_وﺟﻪ_ﺑﻴﺶ_از_ﺣﺪ_ﻣﺠﺎز_اﺳﺖ = 16,
        [Display(Name = "BankMelatcode16", ResourceType = typeof(App_GlobalResources.Global))]
        TimesExceeded = 16,

        //ﻛﺎرﺑﺮ_از_اﻧﺠﺎم_ﺗﺮاﻛﻨﺶ_ﻣﻨﺼﺮف_ﺷﺪه_اﺳﺖ = 17,
        [Display(Name = "BankMelatcode17", ResourceType = typeof(App_GlobalResources.Global))]
        TransactionCancel = 17,

        //ﺗﺎرﻳﺦ_اﻧﻘﻀﺎی_ﻛﺎرت_ﮔﺬﺷﺘﻪ_اﺳﺖ = 18,
        [Display(Name = "BankMelatcode18", ResourceType = typeof(App_GlobalResources.Global))]
        ExpirationCard = 18,

        //ﻣﺒﻠﻎ_ﺑﺮداﺷﺖ_وﺟﻪ_ﺑﻴﺶ_از_ﺣﺪ_ﻣﺠﺎز_اﺳﺖ = 19,
        [Display(Name = "BankMelatcode19", ResourceType = typeof(App_GlobalResources.Global))]
        WithdrawalAmountIsExcessive = 19,

        //ﺻﺎدر_ﻛﻨﻨﺪه_ﻛﺎرت_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 111,
        [Display(Name = "BankMelatcode111", ResourceType = typeof(App_GlobalResources.Global))]
        CardExporterIsInvalid = 111,

        //ﺧﻄﺎی_ﺳﻮﻳﻴﭻ_ﺻﺎدر_ﻛﻨﻨﺪه_ﻛﺎرت = 112,
        [Display(Name = "BankMelatcode112", ResourceType = typeof(App_GlobalResources.Global))]
        ErrorSwitchingCardIssuer = 112,

        [Display(Name = "BankMelatcode113", ResourceType = typeof(App_GlobalResources.Global))]
        NoResponseFromTheCard = 113,


        //دارﻧﺪه_ﻛﺎرت_ﻣﺠﺎز_ﺑﻪ_اﻧﺠﺎم_اﻳﻦ_ﺗﺮاﻛﻨﺶ_ﻧﻴﺴﺖ = 114,
        [Display(Name = "BankMelatcode114", ResourceType = typeof(App_GlobalResources.Global))]
        TUserNotAllowedForRransaction = 114,


        //ﭘﺬﻳﺮﻧﺪه_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 21,
        [Display(Name = "BankMelatcode21", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidReceiver = 21,

        //ﺧﻄﺎی_اﻣﻨﻴﺘﻲ_رخ_داده_اﺳﺖ = 23,
        [Display(Name = "BankMelatcode23", ResourceType = typeof(App_GlobalResources.Global))]
        SecurityErrorOccurred = 23,

        //اﻃﻼﻋﺎت_ﻛﺎرﺑﺮی_ﭘﺬﻳﺮﻧﺪه_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 24,
        [Display(Name = "BankMelatcode24", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidUserInformationIsInvalid = 24,

        //ﻣﺒﻠﻎ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 25,
        [Display(Name = "BankMelatcode25", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidAmount = 25,

        //ﭘﺎﺳﺦ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 31,
        [Display(Name = "BankMelatcode31", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidResponse = 31,

        //ﻓﺮﻣﺖ_اﻃﻼﻋﺎت_وارد_ﺷﺪه_ﺻﺤﻴﺢ_ﻧﻤﻲ_ﺑﺎﺷﺪ = 32,
        [Display(Name = "BankMelatcode32", ResourceType = typeof(App_GlobalResources.Global))]
        InformationNotCorrect = 32,

        //ﺣﺴﺎب_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 33,
        [Display(Name = "BankMelatcode33", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidAccount = 33,

        //ﺧﻄﺎی_ﺳﻴﺴﺘﻤﻲ = 34,
        [Display(Name = "BankMelatcode34", ResourceType = typeof(App_GlobalResources.Global))]
        SystemError = 34,

        //ﺗﺎرﻳﺦ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 35,
        [Display(Name = "BankMelatcode35", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidDate = 35,

        //ﺷﻤﺎره_درﺧﻮاﺳﺖ_ﺗﻜﺮاری_اﺳﺖ = 41,
        [Display(Name = "BankMelatcode41", ResourceType = typeof(App_GlobalResources.Global))]
        RequestDuplicate = 41,

        //ﺗﺮاﻛﻨﺶ_Sale_یافت_نشد_ = 42,
        [Display(Name = "BankMelatcode42", ResourceType = typeof(App_GlobalResources.Global))]
        NotFoundSaleTransaction = 42,

        //ﻗﺒﻼ_Verify_درﺧﻮاﺳﺖ_داده_ﺷﺪه_اﺳﺖ = 43,
        [Display(Name = "BankMelatcode43", ResourceType = typeof(App_GlobalResources.Global))]
        AlreadyRequested = 43,

        //درخواست_verify_یافت_نشد = 44,
        [Display(Name = "BankMelatcode44", ResourceType = typeof(App_GlobalResources.Global))]
        VerfiyApplicationWasNotFound = 44,


        [Display(Name = "BankMelatcode45", ResourceType = typeof(App_GlobalResources.Global))]
        TransactionSettled = 45,
    
        //ﺗﺮاﻛﻨﺶ_Settle_نشده_اﺳﺖ = 46,
        [Display(Name = "BankMelatcode46", ResourceType = typeof(App_GlobalResources.Global))]
        NotSettleTransaction = 46,

        //ﺗﺮاﻛﻨﺶ_Settle_یافت_نشد = ,
        [Display(Name = "BankMelatcode47", ResourceType = typeof(App_GlobalResources.Global))]
        SettleTransactionNotFound = 47,

        //تراکنش_Reverse_شده_است = ,
        [Display(Name = "BankMelatcode48", ResourceType = typeof(App_GlobalResources.Global))]
        ReverseTransaction = 48,

        //تراکنش_Refund_یافت_نشد = 49,
        [Display(Name = "BankMelatcode49", ResourceType = typeof(App_GlobalResources.Global))]
        FoundTransactionRefund = 49,



        //شناسه_قبض_نادرست_است = 412,
        [Display(Name = "BankMelatcode412", ResourceType = typeof(App_GlobalResources.Global))]
        BillingIDIsIncorrect = 412,

        [Display(Name = "BankMelatcode413", ResourceType = typeof(App_GlobalResources.Global))]
        PaymentIDIsIncorrect = 413,
        //سازﻣﺎن_ﺻﺎدر_ﻛﻨﻨﺪه_ﻗﺒﺾ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 414,
        [Display(Name = "BankMelatcode414", ResourceType = typeof(App_GlobalResources.Global))]
        OrganizationInvalid = 414,

        //زﻣﺎن_ﺟﻠﺴﻪ_ﻛﺎری_ﺑﻪ_ﭘﺎﻳﺎن_رسیده_است = 415,
        [Display(Name = "BankMelatcode415", ResourceType = typeof(App_GlobalResources.Global))]
        SessionIsOver = 415,

        //ﺧﻄﺎ_در_ﺛﺒﺖ_اﻃﻼﻋﺎت = 416,
        [Display(Name = "BankMelatcode416", ResourceType = typeof(App_GlobalResources.Global))]
        ErrorRecordingInformation = 416,

        //ﺷﻨﺎﺳﻪ_ﭘﺮداﺧﺖ_ﻛﻨﻨﺪه_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 417,
        [Display(Name = "BankMelatcode417", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidPayeeID = 417,

        //اﺷﻜﺎل_در_ﺗﻌﺮﻳﻒ_اﻃﻼﻋﺎت_ﻣﺸﺘﺮی = 418,
        [Display(Name = "BankMelatcode418", ResourceType = typeof(App_GlobalResources.Global))]
        ErrorInDefiningCustomers = 418,

        //ﺗﻌﺪاد_دﻓﻌﺎت_ورود_اﻃﻼﻋﺎت_از_ﺣﺪ_ﻣﺠﺎز_ﮔﺬﺷﺘﻪ_اﺳﺖ = 419,
        [Display(Name = "BankMelatcode419", ResourceType = typeof(App_GlobalResources.Global))]
        MoreLogin = 419,

        //IP_نامعتبر_است = 421,
        [Display(Name = "BankMelatcode421", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidIp = 421,


        //ﺗﺮاﻛﻨﺶ_ﺗﻜﺮاری_اﺳﺖ = 51,
        [Display(Name = "BankMelatcode51", ResourceType = typeof(App_GlobalResources.Global))]
        TransactionIsDuplicate = 51,

        //ﺗﺮاﻛﻨﺶ_ﻣﺮﺟﻊ_ﻣﻮﺟﻮد_ﻧﻴﺴﺖ = 54,
        [Display(Name = "BankMelatcode54", ResourceType = typeof(App_GlobalResources.Global))]
        ReferencetransactionIsNotAvailable = 54,

        //ﺗﺮاﻛﻨﺶ_ﻧﺎﻣﻌﺘﺒﺮ_اﺳﺖ = 55,
        [Display(Name = "BankMelatcode55", ResourceType = typeof(App_GlobalResources.Global))]
        InvalidTransaction = 55,

        //ﺧﻄﺎ_در_واریز = 61
        [Display(Name = "BankMelatcode61", ResourceType = typeof(App_GlobalResources.Global))]
        ErrorInDeposit = 61,

        
    }

}