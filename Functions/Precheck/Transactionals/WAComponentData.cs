using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class WAComponentData
    {
        [JsonPropertyName("type")]
        public String Type { get; set; }

        [JsonPropertyName("sub_type")]
        public String SubType { get; set; }

        [JsonPropertyName("index")]
        public String Index { get; set; }

        [JsonPropertyName("parameters")]
        public List<WAParameterData> Parameters { get; set; }
    }
}