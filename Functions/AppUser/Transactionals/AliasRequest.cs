using System;

namespace HeroServer
{
    public class AliasRequest
    {
        public long AppUserId { get; set; }
        public String Alias { get; set; }

        public AliasRequest()
        {
        }

        public AliasRequest(long appUserId, String alias)
        {
            Alias = alias;
        }
    }
}
