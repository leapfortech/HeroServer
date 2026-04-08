using System;

namespace HeroServer
{
    public class Locality
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public int LocalityType { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Locality() { }

        public Locality(long id, long appUserId, int localityType, long countryId,
                        long stateId, long cityId, DateTime createDateTime, DateTime updateDateTime,
                        int status)
        {
            Id = id;
            AppUserId = appUserId;
            LocalityType = localityType;
            CountryId = countryId;
            StateId = stateId;
            CityId = cityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
