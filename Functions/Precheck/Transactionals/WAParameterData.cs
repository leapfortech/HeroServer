using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WAParameterData
    {
        [JsonPropertyName("type")]
        public String Type { get; set; }

        [JsonPropertyName("text")]
        public String Text { get; set; }
    }

}