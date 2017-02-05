using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace IranAudioGuide_MainServer.Controllers
{

    public class RepaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
  
    public static class ServiceRecaptcha
    {

        private static string _secret= "6LffxRMUAAAAAFe57_yXZf8bFU-NjvNnsGPFpQQK";
        public static string    ErrorMessage { get; set; }

        public static bool IsValid(string response)
        {
         
            //secret that was generated in key value pair
            var client = new WebClient();
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={_secret}&response={response}";
            var reply = client.DownloadString(url);

            var captchaResponse = JsonConvert.DeserializeObject<RepaptchaResponse>(reply);

            // when response is false check for the error message
            if (!captchaResponse.Success)
            {
                if (captchaResponse.ErrorCodes == null || captchaResponse.ErrorCodes.Count <= 0)
                {
                    ErrorMessage = "Error occured. Please try again";
                    return false;
                }

                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                       ErrorMessage = "The secret parameter is missing.";
                        break;
                    case ("invalid-input-secret"):
                        ErrorMessage = "The secret parameter is invalid or malformed.";
                        break;

                    case ("missing-input-response"):
                        ErrorMessage = "The response parameter is missing.";
                        break;
                    case ("invalid-input-response"):
                        ErrorMessage = "The response parameter is invalid or malformed.";
                        break;

                    default:
                        ErrorMessage = "Error occured. Please try again";
                        break;
                }
                return false;
            }
            // Captcha is valid
            return true;
        }
    }

}