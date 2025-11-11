using System;

namespace HeroServer
{
    public class OfacCryptoWallet
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String publicKey { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacCryptoWallet()
        {
        }

        public OfacCryptoWallet(String id, String publicKey)
        {
            this.id = id;
            this.publicKey = publicKey;
        }
    }
}
