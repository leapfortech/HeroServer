
namespace HeroServer
{
    public class OfacFindResult : LeapResult
    {
        public OfacSearchMatch[] Persons { get; set; }

        public OfacFindResult()
        {
            Persons = null;
        }

        public OfacFindResult(OfacSearchResponse ofacSeachResponse)
        {
            Persons = ofacSeachResponse.results[0].matches;
        }
    }
}