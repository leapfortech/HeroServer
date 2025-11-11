
namespace HeroServer
{
    public class OfacChecksResult : LeapResult
    {
        public OfacCheckCase[] Cases { get; set; }

        public OfacChecksResult()
        {
            Cases = null;
        }

        public OfacChecksResult(OfacScreenResponse ofacScreenResponse)
        {
            Cases = new OfacCheckCase[ofacScreenResponse.results.Length];

            for (int i = 0; i < Cases.Length; i++)
                Cases[i] = new OfacCheckCase(ofacScreenResponse.results[i]);
        }
    }
}