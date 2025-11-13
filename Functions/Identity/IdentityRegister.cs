using System;

namespace HeroServer
{
    public class IdentityRegister
    {
        public Identity Identity { get; set; }
        public String Portrait { get; set; }


        public IdentityRegister()
        {
        }

        public IdentityRegister(Identity identity, String portrait)
        {
            Identity = identity;
            Portrait = portrait;
        }
    }
}
