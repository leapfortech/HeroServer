using System;

namespace HeroServer
{
    public class OfacAddressRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public String address1 { get; set; }
        public String address2 { get; set; }
        public String city { get; set; }
        public String stateOrProvince { get; set; }
        public String postalCode { get; set; }
        public String country { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacAddressRequest()
        {
        }

        public OfacAddressRequest(String address1, String address2, String city, String stateOrProvince, String postalCode, String country)
        {
            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.stateOrProvince = stateOrProvince;
            this.postalCode = postalCode;
            this.country = country;
        }
    }
}
