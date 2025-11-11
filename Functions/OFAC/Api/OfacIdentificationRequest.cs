using System;

namespace HeroServer
{
    public class OfacIdentificationRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public String type { get; set; }
        public String idNumber { get; set; }
        public String country { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacIdentificationRequest()
        {
        }

        public OfacIdentificationRequest(String type, String idNumber, String country)
        {
            this.type = type;
            this.idNumber = idNumber;
            this.country = country;
        }

        public OfacIdentificationRequest(OfacIdentification identification)
        {
            type = identification.Type;
            idNumber = identification.IdNumber;
            country = identification.Country;
        }
    }
}
