
namespace HeroServer
{
    public class OfacRequest
    {
        public OfacCase Case { get; set; }
        public OfacCase[] Cases { get; set; }

        public OfacRequest()
        {
        }

        public OfacRequest(OfacCase ofacCase)
        {
            Case = ofacCase;
            Cases = null;
        }

        public OfacRequest(OfacCase[] ofacCases)
        {
            Case = null;
            Cases = ofacCases;
        }

        public OfacRequest(OfacCase ofacCase, OfacCase[] ofacCases)
        {
            Case = ofacCase;
            Cases = ofacCases;
        }
    }
}
