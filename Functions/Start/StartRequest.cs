using System;

namespace HeroServer
{
    public class StartRequest
    {
        public String PublicKey { get; set; }
        public String Version { get; set; }

        public StartRequest()
        {
        }

        public StartRequest(String publicKey, String version)
        {
            PublicKey = publicKey;
            Version = version;
        }
    }
}
