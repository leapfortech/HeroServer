using System;

namespace HeroServer
{
    public class InvestmentIdentityRequest
    {
        public InvestmentIdentity InvestmentIdentity { get; set; }
        public Identity Identity { get; set; }
        public Address Address { get; set; }
        public Pep Pep { get; set; }
        public PepIdentityRequest[] PepIdentityRequests { get; set; }
        public Cpe Cpe { get; set; }

        public InvestmentIdentityRequest()
        {
        }

        public InvestmentIdentityRequest(InvestmentIdentity investmentIdentity, Identity identity, Address address, Pep pep,
                                         PepIdentityRequest[] pepIdentityRequests, Cpe cpe)
        {
            InvestmentIdentity = investmentIdentity;
            Identity = identity;
            Address = address;
            Pep = pep;
            PepIdentityRequests = pepIdentityRequests;
            Cpe = cpe;
        }
    }
}