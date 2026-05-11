using System;

namespace HeroServer
{
    public class OnboardingResponse
    {
        public long IdentityId { get; set; }
        public long AddressId { get; set; }

        public OnboardingResponse()
        {
        }

        public OnboardingResponse(long identityId, long addressId)
        {
            IdentityId = identityId;
            AddressId = addressId;
        }
    }
}
