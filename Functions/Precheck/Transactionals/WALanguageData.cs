using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WALanguageData
    {
        [JsonPropertyName("code")]
        public String Code { get; set; }
    }
}