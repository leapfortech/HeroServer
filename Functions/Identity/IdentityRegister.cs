using System;

namespace HeroServer
{
    public class IdentityRegister
    {
        public IdentityInfo IdentityInfo { get; set; }
        public Pep Pep { get; set; }
        public PepIdentityRequest[] PepIdentityRequests { get; set; }
        public Cpe Cpe { get; set; }
        public String Portrait { get; set; }


        public IdentityRegister()
        {
        }

        public IdentityRegister(IdentityInfo identityInfo, Pep pep, PepIdentityRequest[] pepIdentityRequests, Cpe cpe, String portrait)
        {
            IdentityInfo = identityInfo;
            Pep = pep;
            PepIdentityRequests = pepIdentityRequests;
            Cpe = cpe;
            Portrait = portrait;
        }
    }
}
