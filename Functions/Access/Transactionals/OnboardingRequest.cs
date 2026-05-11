using System;

namespace HeroServer
{
    public class OnboardingRequest
    {
        public long AppUserId { get; set; }
        public Identity Identity { get; set; }
        public Address Address { get; set; }
        public String Portrait { get; set; }

        public OnboardingRequest()
        {
        }

        public OnboardingRequest(long appUserId, Identity identity, Address address, String portrait)
        {
            AppUserId = appUserId;
            Identity = identity;
            Address = address;
            Portrait = portrait;
        }
    }
}
