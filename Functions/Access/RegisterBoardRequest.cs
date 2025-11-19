using System;

namespace HeroServer
{
    public class RegisterBoardRequest
    {
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String Roles { get; set; } = "BD";
        public String Email { get; set; }
        public String Password { get; set; }
        public long PhoneCountryId { get; set; }
        public String Phone { get; set; }


        public RegisterBoardRequest()
        {
        }

        public String GetCompleteName() => FirstName1 + (String.IsNullOrEmpty(FirstName2) ? "" : " " + FirstName2) + " " + LastName1 + (String.IsNullOrEmpty(LastName2) ? "" : " " + LastName2);
    }
}
