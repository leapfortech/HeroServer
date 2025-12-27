using System;
using Newtonsoft.Json;

namespace HeroServer
{
    public class PhoneCaller
    {
        [JsonProperty("caller_name")]
        public String Name { get; set; }
        [JsonProperty("caller_type")]
        public String Type { get; set; }
        [JsonProperty("error_code")]
        public String ErrorCode { get; set; }
    }
}
