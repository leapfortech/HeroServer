using System;

namespace HeroServer
{
    public class OfacScreenMatch
    {
#pragma warning disable IDE1006 // Naming Styles
        public int score { get; set; }
        public OfacMatchSummary matchSummary { get; set; }
        public OfacSanction sanction { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacScreenMatch()
        {
        }
    }
}
