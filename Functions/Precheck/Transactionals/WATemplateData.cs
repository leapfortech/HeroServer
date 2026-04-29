using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WATemplateData
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }

        [JsonPropertyName("language")]
        public WALanguageData Language { get; set; }

        [JsonPropertyName("components")]
        public List<WAComponentData> Components { get; set; }
    }
}