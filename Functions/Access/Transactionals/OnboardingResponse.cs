using System;

namespace HeroServer
{
    public class OnboardingResponse
    {
        public long IdentityId { get; set; }
        public long AddressId { get; set; }
        public LocalityResponse LocalityResponse { get; set; }

        public OnboardingResponse()
        {
        }

        public OnboardingResponse(long identityId, long addressId, LocalityResponse localityResponse)
        {
            IdentityId = identityId;
            AddressId = addressId;
            LocalityResponse = localityResponse;
        }
    }
}
