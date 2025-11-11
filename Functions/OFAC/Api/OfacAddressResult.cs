using System;

namespace HeroServer
{
    public class OfacAddressResult
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String address1 { get; set; }
        public String address2 { get; set; }
        public String address3 { get; set; }
        public String fullAddress { get; set; }
        public String city { get; set; }
        public String stateOrProvince { get; set; }
        public String postalCode { get; set; }
        public String country { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacAddressResult()
        {
        }

        public OfacAddressResult(String id, String address1, String address2, String address3, String fullAddress, String city, String stateOrProvince, String postalCode, String country)
        {
            this.id = id;
            this.address1 = address1;
            this.address2 = address2;
            this.address3 = address3;
            this.fullAddress = fullAddress;
            this.city = city;
            this.stateOrProvince = stateOrProvince;
            this.postalCode = postalCode;
            this.country = country;
        }
    }
}
