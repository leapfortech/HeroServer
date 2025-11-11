using System;

namespace HeroServer
{
    public class OfacPassport
    {
#pragma warning disable IDE1006 // Naming Styles
        public String passport { get; set; }
        public String passportCountry { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacPassport()
        {
        }

        public OfacPassport(String passport, String passportCountry)
        {
            this.passport = passport;
            this.passportCountry = passportCountry;
        }
    }
}
