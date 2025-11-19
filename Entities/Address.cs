using System;

namespace HeroServer
{
    public class Address
    {
        public long Id { get; set; } = -1;
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Zone { get; set; }
        public String ZipCode { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Address()
        {
        }

        public Address(long id, long countryId, long stateId, long cityId, String address1, String address2, String zone,
                       String zipCode, float? latitude, float? longitude, DateTime createDateTime, DateTime updateDateTime,
                       int status)
        {
            Id = id;
            CountryId = countryId;
            StateId = stateId;
            CityId = cityId;
            Address1 = address1;
            Address2 = address2;
            Zone = zone;
            ZipCode = zipCode;
            Latitude = latitude;
            Longitude = longitude;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
