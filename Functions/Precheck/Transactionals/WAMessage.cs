using System;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WAMessage
    {
        [JsonPropertyName("id")]
        public String Id { get; set; }

        [JsonPropertyName("message_status")]
        public String MessageStatus { get; set; }
    }
}