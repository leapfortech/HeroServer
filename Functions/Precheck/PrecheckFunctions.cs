using System;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Twilio;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Rest.Lookups.V1;


namespace HeroServer
{
    public static class PrecheckFunctions
    {
        private static String twilioAccountSid = "";
        private static String twilioAuthToken = "";
        private static String twilioVerifySid = "";

        public static async void Initialize()
        {
            twilioAccountSid = await new SystemParamDB().GetValue("TwilioAccountSid");
            twilioAuthToken = await new SystemParamDB().GetValue("TwilioAuthToken");
            twilioVerifySid = await new SystemParamDB().GetValue("TwilioVerifySid");
        }

        // SMS
        public static async Task<VerificationResource> SendOTPSms(String phoneNumber)
        {
            TwilioClient.Init(twilioAccountSid, twilioAuthToken);

            return await VerificationResource.CreateAsync(new CreateVerificationOptions(twilioVerifySid, phoneNumber, "sms"));
        }

        public static async Task<VerificationCheckResource> VerifyOTPSms(String phoneNumber, String code)
        {
            TwilioClient.Init(twilioAccountSid, twilioAuthToken);

            CreateVerificationCheckOptions verificationCheckOptions = new CreateVerificationCheckOptions(twilioVerifySid)
            {
                Code = code,
                To = phoneNumber
            };

            return await VerificationCheckResource.CreateAsync(verificationCheckOptions);
        }

        public static async Task<String> RegisterPhoneSms(long phoneCountryId, String phoneNumber, bool checkInfo = false)
        {
            await new PrecheckPhoneDB().UpdateStatusByPhone(phoneCountryId, phoneNumber, 21, 23);
            await new PrecheckPhoneDB().UpdateStatusByPhone(phoneCountryId, phoneNumber, 22, 24);

            String phoneComplete = await GenValuesFunctions.GetStringById("K-Country", phoneCountryId, "PhonePrefix") + phoneNumber;

            int result = 21;
            if (checkInfo)
            {
                PhoneInfo phoneInfo = await GetPhoneInfoTwilio(phoneComplete);

                if (phoneInfo.CountryCode != "US" && phoneInfo.CountryCode != "GT")
                    result = -101;
                else if (phoneInfo.Carrier != null && phoneInfo.Carrier.Type != "mobile")
                    result = -102;

                await new PrecheckPhoneDB().Add(new PrecheckPhone(-1, phoneCountryId, phoneNumber, null, phoneInfo.CountryCode, phoneInfo.Caller?.Name,
                                                                  phoneInfo.Carrier?.MobileCountryCode, phoneInfo.Carrier?.MobileNetworkCode,
                                                                  phoneInfo.Carrier?.Name, phoneInfo.Carrier?.Type, DateTime.Now, DateTime.Now, result));

                if (result == -101)
                    return "COUNTRY";

                if (result == -102)
                    return "MOBILE";
            }
            else
                await new PrecheckPhoneDB().Add(new PrecheckPhone(-1, phoneCountryId, phoneNumber, null, null, null, null, null, null, null, DateTime.Now, DateTime.Now, 21));

            await SendOTPSms(phoneComplete);
            return "OK";
        }

        public static async Task<String> ValidatePhoneSmsCode(PhoneCodeRequest phoneCodeRequest)
        {
            PrecheckPhone precheckPhone = await new PrecheckPhoneDB().GetByPhoneNumber(phoneCodeRequest.PhoneCountryId, 
                                                                                       phoneCodeRequest.PhoneNumber,
                                                                                       21);

            if (precheckPhone == null)
                return "NOT_FOUND";

            if ((DateTime.Now - precheckPhone.CreateDateTime).TotalMinutes >= 3.0)
                return "EXPIRED";

            String phoneComplete = await GenValuesFunctions.GetStringById("K-Country", phoneCodeRequest.PhoneCountryId, "PhonePrefix") + phoneCodeRequest.PhoneNumber;

            VerificationCheckResource otpResponse = await VerifyOTPSms(phoneComplete, phoneCodeRequest.Code);
            if (!otpResponse.Valid.HasValue || !otpResponse.Valid.Value)
                return "BAD_CODE";

            await new PrecheckPhoneDB().UpdateStatus(precheckPhone.Id, 22);
            return "OK";
        }

        public static async Task<PhoneInfo> GetPhoneInfoTwilio(String phoneNumber, bool carrier = true, bool callerName = true)
        {
            TwilioClient.Init(twilioAccountSid, twilioAuthToken);

            FetchPhoneNumberOptions options = new FetchPhoneNumberOptions(phoneNumber)
            {
                Type = []
            };

            if (carrier)
                options.Type.Add("carrier");
            if (callerName)
                options.Type.Add("caller-name");

            PhoneNumberResource phoneNumberResource = await PhoneNumberResource.FetchAsync(options);

            PhoneInfo phoneInfo = new PhoneInfo()
            {
                Caller = (PhoneCaller)phoneNumberResource.CallerName,
                CountryCode = phoneNumberResource.CountryCode,
                PhoneNumber = phoneNumberResource.PhoneNumber?.ToString(),
                NationalFormat = phoneNumberResource.NationalFormat,
                Carrier = (PhoneCarrier)phoneNumberResource.Carrier,
                Url = phoneNumberResource.Url
            };

            return phoneInfo;
        }

        public static async Task<PhoneInfo> GetPhoneInfoWebTwilio(String phoneNumber, bool carrier, bool callerName)
        {
            using (HttpClient client = new HttpClient())
            {
                String authenticationString = $"{twilioAccountSid}:{twilioAuthToken}";
                String base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                String link = "https://lookups.twilio.com/v1/PhoneNumbers/" + phoneNumber + "?CountryCode=GT";

                if (carrier)
                    link += "&Type=carrier";
                if (callerName)
                    link += "&Type=caller-name";

                HttpResponseMessage result = await client.GetAsync(link);

                if (result.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Failed to POST data: ({result.StatusCode}): {await result.Content.ReadAsStringAsync()}");

                return await JsonSerializer.DeserializeAsync<PhoneInfo>(await result.Content.ReadAsStreamAsync());
                //return Newtonsoft.Json.JsonConvert.DeserializeObject<PhoneInfo>(await result.Content.ReadAsStringAsync());
            }
        }

        // WHATSAPP
        //public static async Task SendOTPWA(String phoneNumber, String code)
        //{
        //    // WA CALL
        //}

        public static async Task<String> RegisterPhoneWA(long phoneCountryId, String phoneNumber)
        {
            await new PrecheckPhoneDB().UpdateStatusByPhone(phoneCountryId, phoneNumber, 11, 13);
            await new PrecheckPhoneDB().UpdateStatusByPhone(phoneCountryId, phoneNumber, 12, 14);

            String phoneComplete = await GenValuesFunctions.GetStringById("K-Country", phoneCountryId, "PhonePrefix") + phoneNumber;
            
            String code = GenerateCode();

            await new PrecheckPhoneDB().Add(new PrecheckPhone(-1, phoneCountryId, phoneNumber, code, null, null, null, null, null, null, DateTime.Now, DateTime.Now, 11));

            //await SendOTPWA(phoneComplete, code);
            return "OK";
        }

        public static async Task<String> ValidatePhoneWACode(PhoneCodeRequest phoneCodeRequest)
        {
            PrecheckPhone precheckPhone = await new PrecheckPhoneDB().GetByPhoneNumber(phoneCodeRequest.PhoneCountryId,
                                                                                       phoneCodeRequest.PhoneNumber,
                                                                                       11);

            if (precheckPhone == null)
                return "NOT_FOUND";

            if ((DateTime.Now - precheckPhone.CreateDateTime).TotalMinutes >= 3.0)
                return "EXPIRED";

            if (precheckPhone.Code != phoneCodeRequest.Code)
                return "BAD_CODE";

            await new PrecheckPhoneDB().UpdateStatus(precheckPhone.Id, 12);
            return "OK";
        }

        // EMAIL
        public static async Task<int> SendOTPEmail(String email, String code)
        {
            String subject = "Código de verificación Heroes Migrantes";
            String body = $"Código de verificación de identidad: <strong>{code}</strong>";

            String message = HtmlHelper.GetConfirmResultHtml(subject, body, "#666666");

            if (message == null)
                return 3;

            try
            {
                await MailHelper.SendMail(email, "Heroes Migrantes", subject, message, true);
            }
            catch
            {
                return 2;
            }

            return 1;
        }

        public static async Task<String> RegisterEmail(String email)
        {
            await new PrecheckEmailDB().UpdateStatusByEmail(email, 1, 3);
            await new PrecheckEmailDB().UpdateStatusByEmail(email, 2, 4);

            String code = GenerateCode();

            await new PrecheckEmailDB().Add(new PrecheckEmail(-1, email, code, DateTime.Now, DateTime.Now, 1));

            await SendOTPEmail(email, code);
            return "OK";
        }

        public static async Task<String> ValidateEmailCode(EmailCodeRequest emailCodeRequest)
        {
            PrecheckEmail precheckEmail = await new PrecheckEmailDB().GetByEmail(emailCodeRequest.Email, 1);

            if (precheckEmail == null)
                return "NOT_FOUND";

            if ((DateTime.Now - precheckEmail.CreateDateTime).TotalMinutes >= 3.0)
                return "EXPIRED";

            if (precheckEmail.Code != emailCodeRequest.Code)
                return "BAD_CODE";

            await new PrecheckEmailDB().UpdateStatus(precheckEmail.Id, 2);
            return "OK";
        }


        // OTP

        public static String GenerateCode()
        {
            return Convert.ToString(RandomNumberGenerator.GetInt32(100000, 1000000));
        }
    }
}
