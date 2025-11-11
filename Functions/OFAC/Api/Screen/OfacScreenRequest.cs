using System;

namespace HeroServer
{
    public class OfacScreenRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public String apiKey { get; set; }
        public int minScore { get; set; }
        public String[] sources { get; set; }
        public String[] types { get; set; }
        public OfacScreenCase[] cases { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacScreenRequest()
        {
        }

        public OfacScreenRequest(String apiKey, int minScore, String[] sources, String[] types, OfacScreenCase[] cases)
        {
            this.apiKey = apiKey;
            this.minScore = minScore;
            this.sources = sources;
            this.types = types;
            this.cases = cases;
        }

        public OfacScreenRequest(OfacRequest request, String apiKey)
        {
            this.apiKey = apiKey;
            minScore = 95;
            sources = [ "SDN", "NONSDN", "UN" ];
            types = [ "person" ];
            if (request.Case != null)
            {
                cases = [ new OfacScreenCase(request.Case) ];
            }
            else if (request.Cases != null)
            {
                cases = new OfacScreenCase[request.Cases.Length];
                for (int i = 0; i < request.Cases.Length; i++)
                    cases[i] = new OfacScreenCase(request.Cases[i]);
            }
            else
                cases = null;
        }
    }
}
