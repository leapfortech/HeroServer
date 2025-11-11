using System;

namespace HeroServer
{
    public class OfacCase
    {
        public String Name { get; set; }
        public String Dob { get; set; }
        public String Gender { get; set; }
        public String PhoneNumber { get; set; }
        public String EmailAddress { get; set; }
        public OfacIdentification[] Identification { get; set; }

        public OfacCase()
        {
        }

        public OfacCase(String name, String dob, String gender, String phoneNumber, String emailAddress, OfacIdentification[] identification)
        {
            Name = name;
            Dob = dob;
            Gender = gender;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
            Identification = identification;
        }

        public OfacCase(String name, String dob)
        {
            Name = name;
            Dob = dob;
            Gender = null;
            PhoneNumber = null;
            EmailAddress = null;
            Identification = null;
        }
    }
}
