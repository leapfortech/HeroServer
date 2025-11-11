using System;

namespace HeroServer
{
    public class OfacScreenResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public bool error { get; set; }
        public String errorMessage { get; set; }
        public OfacSource[] sources { get; set; }
        public OfacScreenResult[] results { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacScreenResponse()
        {
        }

        public OfacScreenResponse(bool error, String errorMessage, OfacSource[] sources, OfacScreenResult[] results)
        {
            this.error = error;
            this.errorMessage = errorMessage;
            this.sources = sources;
            this.results = results;
        }
    }
}
