using System;

namespace HeroServer
{
    public class LocalityFull
    {
        public long LocalityTypeId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }


        public LocalityFull()
        {
        }

        public LocalityFull(long localityTypeId, long countryId, long stateId, long cityId)
        {
            LocalityTypeId = localityTypeId;
            CountryId = countryId;
            StateId = stateId;
            CityId = cityId;
        }
    }
}
