using System;

namespace HeroServer
{
    public class ReferredFull
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public String FirstNames { get; set; }
        public String LastNames { get; set; }
        public String PhonePrefix { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ReferrerFull Referrer { get; set; }


        public ReferredFull()
        {
        }

        public ReferredFull(long id, long appUserId, String firstNames, String lastNames,
                            String phonePrefix, String phone, String email, DateTime createDateTime, ReferrerFull referrer)
        {
            Id = id;
            AppUserId = appUserId;
            FirstNames = firstNames;
            LastNames = lastNames;
            PhonePrefix = phonePrefix;
            Phone = phone;
            Email = email;
            CreateDateTime = createDateTime;
            Referrer = referrer;
        }
    }
}
