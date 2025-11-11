using System;

namespace HeroServer
{
    public class AppUserNamed
    {
        public int Id { get; set; }
        public String Code { get; set; }
        public int WebSysUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String LastNameMarried { get; set; }
        public String Email { get; set; }
        public int PhoneCountryId { get; set; } = -1;
        public String Phone { get; set; }
        public String CSToken { get; set; }
        public int Options { get; set; } = 11;
        public int ReferrerAppUserId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Today;
        public DateTime UpdateDateTime { get; set; } = DateTime.Today;
        public int AppUserStatusId { get; set; }

        public AppUserNamed()
        {
        }

        public AppUserNamed(int id, String code, int webSysUserId, String firstName1, String firstName2, String lastName1, String lastName2,
                            String lastNameMarried, String email, int phoneCountryId, String phone, String csToken, int options, int referrerAppUserId,
                            DateTime createDateTime, DateTime updateDateTime, int appUserStatusId)
        {
            Id = id;
            Code = code;
            WebSysUserId = webSysUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            LastNameMarried = lastNameMarried;
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
