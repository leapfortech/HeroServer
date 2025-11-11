using System;

namespace HeroServer
{
    public class RegisterAppRequest
    {
        public String Roles { get; set; } = "AC";
        public String Email { get; set; }
        public String Password { get; set; }
        public int PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public int News { get; set; }
        public String ReferredCode { get; set; }


        public RegisterAppRequest()
        {
        }
    }
}
