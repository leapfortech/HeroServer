
namespace HeroServer
{
    public class OfacMatchSummary
    {
#pragma warning disable IDE1006 // Naming Styles
        public OfacMatchField[] matchFields { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacMatchSummary()
        {
        }

        public OfacMatchSummary(OfacMatchField[] matchFields)
        {
            this.matchFields = matchFields;
        }
    }
}
