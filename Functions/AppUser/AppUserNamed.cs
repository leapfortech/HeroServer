using System;

namespace HeroServer
{
    public class AppUserNamed
    {
        public long Id { get; set; }
        public long WebSysUserId { get; set; }
        public String Alias { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String Email { get; set; }
        public long PhoneCountryId { get; set; } = -1;
        public String Phone { get; set; }
        public String CSToken { get; set; }
        public long Options { get; set; } = 11;
        public long ReferrerAppUserId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Today;
        public DateTime UpdateDateTime { get; set; } = DateTime.Today;
        public int AppUserStatusId { get; set; }

        public AppUserNamed()
        {
        }

        public AppUserNamed(long id, long webSysUserId,String alias, String firstName1, String firstName2, String lastName1, String lastName2,
                            String email, long phoneCountryId, String phone, String csToken, long options,
                            long referrerAppUserId, DateTime createDateTime, DateTime updateDateTime, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            Email = email;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            CSToken = csToken;
            Options = options;
            ReferrerAppUserId = referrerAppUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            AppUserStatusId = appUserStatusId;
        }
    }
}
