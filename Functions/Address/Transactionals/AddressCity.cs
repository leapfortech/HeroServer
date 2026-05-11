
namespace HeroServer
{
    public class AddressCity
    {
        public long AppUserId { get; set; }
        public long AddressId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }

        public AddressCity()
        {
        }

        public AddressCity(long appUserId, long addressId, long countryId, long stateId, long cityId)
        {
            AppUserId = appUserId;
            AddressId = addressId;
            CountryId = countryId;
            StateId = stateId;
            CityId = cityId;
        }
    }
}