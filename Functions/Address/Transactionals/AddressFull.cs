using System;

namespace HeroServer
{
    public class AddressFull
    {
        public long Id { get; set; }
        public String Country { get; set; }
        public String State { get; set; }
        public String City { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Zone { get; set; }
        public String ZipCode { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int Status { get; set; }


        public AddressFull()
        {
        }

        public AddressFull(long id, String country, String state, String city, String address1, String address2, 
                          String zone, String zipCode, float? latitude, float? longitude, int status)
        {
            Id = id;
            Country = country;
            State = state;
            City = city;
            Address1 = address1;
            Address2 = address2;
            Zone = zone;
            ZipCode = zipCode;
            Latitude = latitude;
            Longitude = longitude;
            Status = status;
        }
    }
}
