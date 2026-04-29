using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WAResponse
    {
        [JsonPropertyName("messages")]
        public List<WAMessage> Messages { get; set; }
    }
}