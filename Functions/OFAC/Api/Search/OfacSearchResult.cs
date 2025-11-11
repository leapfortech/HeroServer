using System;

namespace HeroServer
{
    public class OfacSearchResult
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String name { get; set; }
        public int matchCount { get; set; }
        public OfacSearchMatch[] matches { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacSearchResult()
        {
        }

        public OfacSearchResult(String id, String name, int matchCount, OfacSearchMatch[] matches)
        {
            this.id = id;
            this.name = name;
            this.matchCount = matchCount;
            this.matches = matches;
        }
    }
}
