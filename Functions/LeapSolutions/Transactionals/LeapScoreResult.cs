
namespace HeroServer
{
    public class LeapScoreResult : LeapResult
    {
        public float Score { get; set; }

        public LeapScoreResult()
        {
        }

        public LeapScoreResult(float score)
        {
            Score = score;
        }
    }
}