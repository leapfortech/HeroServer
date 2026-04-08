using System;

namespace HeroServer
{
    public class IdentityPersonal
    {
        public long AppUserId { get; set; }
        public long IdentityId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public DateTime BirthDate { get; set; }
        public long GenderId { get; set; }

        public IdentityPersonal()
        {
        }

        public IdentityPersonal(long appUserId, long identityId,
                                String firstName1, String firstName2,
                                String lastName1, String lastName2,
                                DateTime birthDate, long genderId)
        {
            AppUserId = appUserId;
            IdentityId = identityId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            BirthDate = birthDate;
            GenderId = genderId;
        }
    }
}