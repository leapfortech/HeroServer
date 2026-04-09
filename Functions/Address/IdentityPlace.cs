using System;

namespace HeroServer
{
    public class IdentityPlace
    {
        public long AppUserId { get; set; }
        public long IdentityId { get; set; }
        public long BirthCountryId { get; set; }
        public long BirthStateId { get; set; }
        public long BirthCityId { get; set; }

        public IdentityPlace()
        {
        }

        public IdentityPlace(long appUserId, long identityId,
                              long birthCountryId, long birthStateId, long birthCityId)
        {
            AppUserId = appUserId;
            IdentityId = identityId;
            BirthCountryId = birthCountryId;
            BirthStateId = birthStateId;
            BirthCityId = birthCityId;
        }
    }
}