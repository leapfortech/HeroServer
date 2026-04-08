using System;

namespace HeroServer
{
    public class LocalityResponse
    {
        public long InterestLocalityId { get; set; }
        public long CurrentLocalityId { get; set; }
      
        public LocalityResponse()
        {
        }

        public LocalityResponse(long interestLocalityId, long currentLocalityId)
        {
            InterestLocalityId = interestLocalityId;
            CurrentLocalityId = currentLocalityId;
        }
    }
}