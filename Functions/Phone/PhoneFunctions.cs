using System;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Twilio;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Rest.Lookups.V1;


namespace HeroServer
{
    public static class PhoneFunctions
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

        public static async Task<VerificationResource> SendOTP(String phoneNumber)
        {
            TwilioClient.Init(twilioAccountSid, twilioAuthToken);

            return await VerificationResource.CreateAsync(new CreateVerificationOptions(twilioVerifySid, phoneNumber, "sms"));
        }

        public static async Task<VerificationCheckResource> VerifyOTP(String phoneNumber, String code)
        {
            TwilioClient.Init(twilioAccountSid, twilioAuthToken);

            CreateVerificationCheckOptions verificationCheckOptions = new CreateVerificationCheckOptions(twilioVerifySid)
            {
                Code = code,
                To = phoneNumber
            };

            return await VerificationCheckResource.CreateAsync(verificationCheckOptions);
        }

        public static async Task<String> RegisterPhone(long phoneCountryId, String phoneNumber, bool checkInfo = false)
        {
            await new PhoneDB().UpdateStatusByPhone(phoneCountryId, phoneNumber, 1, 3);
            await new PhoneDB().UpdateStatusByPhone(phoneCountryId, phoneNumber, 2, 4);

            String phoneComplete = await GenValuesFunctions.GetStringById("K-Country", phoneCountryId, "PhonePrefix") + phoneNumber;

            int result = 1;
            if (checkInfo)
            {
                PhoneInfo phoneInfo = await GetPhoneInfo(phoneComplete);

                if (phoneInfo.CountryCode != "US" && phoneInfo.CountryCode != "GT")
                    result = -101;
                else if (phoneInfo.Carrier != null && phoneInfo.Carrier.Type != "mobile")
                    result = -102;

                await new PhoneDB().Add(new Phone(-1, phoneCountryId, phoneNumber, phoneInfo.CountryCode, phoneInfo.Caller?.Name,
                                                                phoneInfo.Carrier?.MobileCountryCode, phoneInfo.Carrier?.MobileNetworkCode,
                                                                phoneInfo.Carrier?.Name, phoneInfo.Carrier?.Type, DateTime.Now, DateTime.Now, result));

                if (result == -101)
                    return "COUNTRY";

                if (result == -102)
                    return "MOBILE";
            }
            else
                await new PhoneDB().Add(new Phone(-1, phoneCountryId, phoneNumber, null, null, null, null, null, null, DateTime.Now, DateTime.Now, 1));

            await SendOTP(phoneComplete);
            return "OK";
        }

        public static async Task<String> ValidateCode(PhoneCodeRequest phoneCodeRequest)
        {
            Phone phone = await new PhoneDB().GetByPhoneNumber(phoneCodeRequest.PhoneCountryId, phoneCodeRequest.PhoneNumber, 1);

            if (phone == null)
                return "NOT_FOUND";

            if ((DateTime.Now - phone.CreateDateTime).TotalMinutes >= 3.0)
                return "EXPIRED";

            String phoneComplete = await GenValuesFunctions.GetStringById("K-Country", phoneCodeRequest.PhoneCountryId, "PhonePrefix") + phoneCodeRequest.PhoneNumber;

            VerificationCheckResource otpResponse = await VerifyOTP(phoneComplete, phoneCodeRequest.Code);
            if (!otpResponse.Valid.HasValue || !otpResponse.Valid.Value)
                return "BAD_CODE";

            await new PhoneDB().UpdateStatus(phone.Id, 2);
            return "OK";
        }

        public static async Task<PhoneInfo> GetPhoneInfo(String phoneNumber, bool carrier = true, bool callerName = true)
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

        public static async Task<PhoneInfo> GetPhoneInfoWeb(String phoneNumber, bool carrier, bool callerName)
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
    }
}
