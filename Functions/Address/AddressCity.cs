using System;

namespace HeroServer
{
    public class IdentityOrigin
    {
        public long AppUserId { get; set; }
        public long IdentityId { get; set; }
        public long OriginCountryId { get; set; }
        public long OriginStateId { get; set; }
        public long OriginCityId { get; set; }

        public IdentityOrigin()
        {
        }

        public IdentityOrigin(long appUserId, long identityId,
                              long originCountryId, long originStateId, long originCityId)
        {
            AppUserId = appUserId;
            IdentityId = identityId;
            OriginCountryId = originCountryId;
            OriginStateId = originStateId;
            OriginCityId = originCityId;
        }
    }
}