using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class LeapScoresResult : LeapResult
    {
        public List<float> Scores { get; set; }

        [JsonIgnore]
        public float Score => Scores[0];

        public LeapScoresResult()
        {
            Scores = [];
        }

        public LeapScoresResult(List<float> values)
        {
            Scores = values;
        }

        public LeapScoresResult(float value)
        {
            Scores = [ value ];
        }
    }
}