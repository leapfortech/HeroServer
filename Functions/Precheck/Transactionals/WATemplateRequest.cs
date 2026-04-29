using System;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WATemplateRequest
    {
        [JsonPropertyName("messaging_product")]
        public String MessagingProduct { get; set; }

        [JsonPropertyName("to")]
        public String To { get; set; }

        [JsonPropertyName("type")]
        public String Type { get; set; }

        [JsonPropertyName("template")]
        public WATemplateData Template { get; set; }
    }
}