using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class LeapValuesResult<T> : LeapResult
    {
        public List<T> Values { get; set; }

        [JsonIgnore]
        public T Value => Values[0];

        public LeapValuesResult()
        {
            Values = [];
        }

        public LeapValuesResult(List<T> values)
        {
            Values = values;
        }

        public LeapValuesResult(T value)
        {
            Values = [ value ];
        }
    }
}