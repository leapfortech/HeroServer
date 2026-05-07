using System;

namespace HeroServer
{
    public class ServiceWishUser
    {
        public long PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }

        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public long GenderId { get; set; }
        public DateTime BirthDate { get; set; }
        public long BirthCountryId { get; set; }
        public long BirthStateId { get; set; }
        public long BirthCityId { get; set; }

        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }

        public ServiceWishUser()
        {
        }

        public ServiceWishUser(ServiceWish serviceWish, long phoneCountryId, String phone, String email, String firstName1,
                               String firstName2, String lastName1, String lastName2, long genderId, DateTime birthDate,
                               long birthCountryId, long birthStateId, long birthCityId, long countryId, long stateId, long cityId)
        {
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;

            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;

            GenderId = genderId;
            BirthDate = birthDate;

            BirthCountryId = birthCountryId;
            BirthStateId = birthStateId;
            BirthCityId = birthCityId;

            CountryId = countryId;
            StateId = stateId;
            CityId = cityId;
        }
    }
}