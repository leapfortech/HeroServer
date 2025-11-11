using System;

namespace HeroServer
{
    public class ReferrerFull
    {
        public int IdentityId { get; set; }
        public String Cui { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String PhonePrefix { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }


        public ReferrerFull()
        {
        }

        public ReferrerFull(int identityId, String cui, String firstName1, String firstName2, String lastName1, String lastName2, String phonePrefix, String phone, String email)
        {
            IdentityId = identityId;
            Cui = cui;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            PhonePrefix = phonePrefix;
            Phone = phone;
            Email = email;
        }
    }
}
