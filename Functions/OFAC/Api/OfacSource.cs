using System;

namespace HeroServer
{
    public class OfacSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public String source { get; set; }
        public String name { get; set; }
        public String country { get; set; }
        public String publishDate { get; set; }
        public String downloadDate { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacSource()
        {
        }

        public OfacSource(string source, string name, string country, string publishDate, string downloadDate)
        {
            this.source = source;
            this.name = name;
            this.country = country;
            this.publishDate = publishDate;
            this.downloadDate = downloadDate;
        }
    }
}
