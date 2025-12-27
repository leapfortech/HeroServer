using System;
using Newtonsoft.Json;

namespace HeroServer
{
    public class PhoneCarrier
    {
        [JsonProperty("mobile_country_code")]
        public String MobileCountryCode { get; set; }
        [JsonProperty("mobile_network_code")]
        public String MobileNetworkCode { get; set; }
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("type")]
        public String Type { get; set; }
        [JsonProperty("error_code")]
        public String ErrorCode { get; set; }
    }
}
