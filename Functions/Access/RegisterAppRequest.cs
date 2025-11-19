using System;

namespace HeroServer
{
    public class RegisterAppRequest
    {
        public String Roles { get; set; } = "AC";
        public String Email { get; set; }
        public String Password { get; set; }
        public long PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public long ReferredId { get; set; }


        public RegisterAppRequest()
        {
        }
    }
}
