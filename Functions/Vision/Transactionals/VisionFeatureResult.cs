using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class VisionFeatureResult : LeapResult
    {
        public List<VisionPoint2D> Points { get; set; }
        public String Label { get; set; }
        public List<float> Scores { get; set; }

        public VisionFeatureResult()
        {
            Points = null;
            Label = null;
            Scores = null;
        }

        public VisionFeatureResult(List<VisionPoint2D> points, String text, List<float> scores)
        {
            Points = points;
            Label = text;
            Scores = scores;
        }

        public VisionFeatureResult(List<VisionPoint2D> points, List<float> scores)
        {
            Points = points;
            Label = null;
            Scores = scores;
        }

        public VisionFeatureResult(String text, List<float> scores)
        {
            Points = null;
            Label = text;
            Scores = scores;
        }

        public VisionFeatureResult(List<float> scores)
        {
            Points = null;
            Label = null;
            Scores = scores;
        }

        public VisionFeatureResult(VisionFeatureResult result)
        {
            Points = new List<VisionPoint2D>(result.Points.Count);
            for (int k = 0; k < result.Points.Count; k++)
                Points.Add(new VisionPoint2D(result.Points[k].x, result.Points[k].y));
            Label = result.Label;
            Scores = new List<float>(result.Scores.Count);
            for (int k = 0; k < Scores.Count; k++)
                Scores.Add(result.Scores[k]);
        }
    }
}