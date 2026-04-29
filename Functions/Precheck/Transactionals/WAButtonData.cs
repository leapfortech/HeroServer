using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WAButtonData
    {
        [JsonPropertyName("type")]
        public String Type { get; set; }

        [JsonPropertyName("otp_type")]
        public String OtpType { get; set; }

        [JsonPropertyName("text")]
        public String Text { get; set; }
    }
}