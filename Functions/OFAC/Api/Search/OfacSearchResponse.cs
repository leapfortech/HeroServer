using System;

namespace HeroServer
{
    public class OfacSearchResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public bool error { get; set; }
        public String errorMessage { get; set; }
        public OfacSource[] sources { get; set; }
        public OfacSearchResult[] results { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacSearchResponse()
        {
        }

        public OfacSearchResponse(bool error, String errorMessage, OfacSource[] sources, OfacSearchResult[] results)
        {
            this.error = error;
            this.errorMessage = errorMessage;
            this.sources = sources;
            this.results = results;
        }
    }
}
