using System;

namespace HeroServer
{
    public class ReferredFull
    {
        public long Id { get; set; }
        public String Code { get; set; }
        public long AppUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String PhonePrefix { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ReferrerFull Referrer { get; set; }


        public ReferredFull()
        {
        }

        public ReferredFull(long id, String code, long appUserId, String firstName1, String firstName2, String lastName1, String lastName2,
                            String phonePrefix, String phone, String email, DateTime createDateTime,
                            ReferrerFull referrer)
        {
            Id = id;
            Code = code;
            AppUserId = appUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            PhonePrefix = phonePrefix;
            Phone = phone;
            Email = email;
            CreateDateTime = createDateTime;
            Referrer = referrer;
        }
    }
}
