using System;

namespace HeroServer
{
    public class OfacScreenResult
    {
#pragma warning disable IDE1006 // Naming Styles
        public String name { get; set; }
        public int matchCount { get; set; }
        public OfacScreenMatch[] matches { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacScreenResult()
        {
        }

        public OfacScreenResult(String name, int matchCount, OfacScreenMatch[] matches)
        {
            this.name = name;
            this.matchCount = matchCount;
            this.matches = matches;
        }
    }
}
