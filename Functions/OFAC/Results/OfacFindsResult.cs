
namespace HeroServer
{
    public class OfacFindsResult : LeapResult
    {
        public OfacSearchResult[] Cases { get; set; }

        public OfacFindsResult()
        {
            Cases = null;
        }

        public OfacFindsResult(OfacSearchResponse ofacSeachResponse)
        {
            Cases = ofacSeachResponse.results;
        }
    }
}