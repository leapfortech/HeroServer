using System;

namespace HeroServer
{
    public class AliasRequest
    {
        public String Alias { get; set; }

        public AliasRequest()
        {
        }

        public AliasRequest(String alias)
        {
            Alias = alias;
        }
    }
}
