using System;
using System.Text.Json;

namespace HeroServer
{
    public class LeapResultJsonConverter : PolymorphicJsonConverter<LeapResult>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(LeapResult).IsAssignableFrom(typeToConvert);
        }

        public override LeapResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("Score", out _))
                    return JsonSerializer.Deserialize<LeapScoreResult>(root);
                if (root.TryGetProperty("Scores", out _))
                    return JsonSerializer.Deserialize<LeapScoresResult>(root);
                if (root.TryGetProperty("IsAlive", out _))
                    return JsonSerializer.Deserialize<RenapDataResult>(root);
                if (root.TryGetProperty("Persons", out _))
                    return JsonSerializer.Deserialize<OfacCheckResult>(root);
                if (root.TryGetProperty("Cases", out _))
                    return JsonSerializer.Deserialize<OfacChecksResult>(root);

                if (root.TryGetProperty("Value", out JsonElement valueElement))
                {
                    if (valueElement.ValueKind == JsonValueKind.String)
                        return JsonSerializer.Deserialize<LeapValueResult<System.String>>(root);
                    else if (valueElement.ValueKind == JsonValueKind.Number)
                        return JsonSerializer.Deserialize<LeapValueResult<int>>(root);
                }

                if (root.TryGetProperty("Values", out _))
                    return JsonSerializer.Deserialize<LeapValuesResult<System.String>>(root);

                if (root.TryGetProperty("Points", out _))
                    return JsonSerializer.Deserialize<VisionFeatureResult>(root);
                if (root.TryGetProperty("Features", out _))
                    return JsonSerializer.Deserialize<VisionFeaturesResult>(root);
                if (root.TryGetProperty("Cui", out _))
                    return JsonSerializer.Deserialize<VisionDpiFrontResult>(root);
                if (root.TryGetProperty("BirthState", out _))
                    return JsonSerializer.Deserialize<VisionDpiBackResult>(root);
                if (root.TryGetProperty("LivenessStatus", out _))
                    return JsonSerializer.Deserialize<VisionLiveness3dResult>(root);

                return null;
            }
        } 
    }
}
