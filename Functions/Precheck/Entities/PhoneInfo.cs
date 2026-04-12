using System;
using Newtonsoft.Json;

namespace HeroServer
{
    public class PhoneInfo
    {
        [JsonProperty("caller_name")]
        public PhoneCaller Caller { get; set; }
        [JsonProperty("country_code")]
        public String CountryCode { get; set; }
        [JsonProperty("phone_number")]
        public String PhoneNumber { get; set; }
        [JsonProperty("national_format")]
        public String NationalFormat { get; set; }
        [JsonProperty("carrier")]
        public PhoneCarrier Carrier { get; set; }
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}
