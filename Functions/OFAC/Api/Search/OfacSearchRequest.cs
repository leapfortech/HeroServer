using System;

namespace HeroServer
{
    public class OfacSearchRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public String apiKey { get; set; }
        public String[] sources { get; set; }
        public String[] types { get; set; }
        public OfacSearchCase[] cases { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacSearchRequest()
        {
        }

        public OfacSearchRequest(String apiKey, String[] sources, String[] types, OfacSearchCase[] cases)
        {
            this.apiKey = apiKey;
            this.sources = sources;
            this.types = types;
            this.cases = cases;
        }

        public OfacSearchRequest(OfacRequest request, String apiKey)
        {
            this.apiKey = apiKey;
            sources = [ "SDN", "NONSDN", "UN" ];
            types = [ "person" ];
            if (request.Case != null)
            {
                cases = [new OfacSearchCase(request.Case)];
            }
            else if (request.Cases != null)
            {
                cases = new OfacSearchCase[request.Cases.Length];
                for (int i = 0; i < request.Cases.Length; i++)
                    cases[i] = new OfacSearchCase(request.Cases[i]);
            }
            else
                cases = null;
        }
    }
}
