using System;

namespace HeroServer
{
    public class OfacIdentification
    {
        public String Type { get; set; }
        public String IdNumber { get; set; }
        public String Country { get; set; }

        public OfacIdentification()
        {
        }

        public OfacIdentification(String type, String idNumber, String country)
        {
            Type = type;
            IdNumber = idNumber;
            Country = country;
        }
    }
}
