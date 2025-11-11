using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class VisionFeaturesResult : LeapResult
    {
        public List<VisionFeatureResult> Features { get; set; }

        public VisionFeaturesResult()
        {
            Features = [];
        }

        public VisionFeaturesResult(int capacity)
        {
            Features = new List<VisionFeatureResult>(capacity);
        }

        public VisionFeaturesResult(List<VisionFeatureResult> features)
        {
            Features = features;
        }

        [JsonIgnore]
        public int Count => Features.Count;

        [JsonIgnore]
        public VisionFeatureResult this[int idx] => Features[idx];

        public void Add(VisionFeatureResult feature)
        {
            Features.Add(feature);
        }

        public void Insert(int idx, VisionFeatureResult feature)
        {
            Features.Insert(idx, feature);
        }

        public void RemoveAt(int idx)
        {
            Features.RemoveAt(idx);
        }

        public static VisionFeaturesResult operator + (VisionFeaturesResult visionAreasResult, VisionFeatureResult feature) { visionAreasResult.Add(feature); return visionAreasResult; }
        public static VisionFeaturesResult operator + (VisionFeaturesResult visionAreasResult, (int idx, VisionFeatureResult feature) param) { visionAreasResult.Insert(param.idx, param.feature); return visionAreasResult; }

        public static implicit operator VisionFeaturesResult(List<VisionFeatureResult> features) => new VisionFeaturesResult(features);
        public static explicit operator VisionFeaturesResult(VisionFeatureResult feature) => new VisionFeaturesResult([feature]);
    }
}