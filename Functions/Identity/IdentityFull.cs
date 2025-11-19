using System;

namespace HeroServer
{
    public class IdentityFull
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public String OriginCountry { get; set; }
        public String OriginState { get; set; }
        public String PhonePrefix { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int AppUserStatusId { get; set; }
        public int Status { get; set; }


        public IdentityFull()
        {
        }

        public IdentityFull(long id, long appUserId, String firstName1, String firstName2, String lastName1, String lastName2,
                            String gender, DateTime birthDate, String originCountry, String originState, 
                            String phonePrefix, String phone, String email,
                            DateTime createDateTime, DateTime updateDateTime, int appUserStatusId, int status)
        {
            Id = id;
            AppUserId = appUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            Gender = gender;
            BirthDate = birthDate;
            OriginCountry = originCountry;
            OriginState = originState;
            PhonePrefix = phonePrefix;
            Phone = phone;
            Email = email;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            AppUserStatusId = appUserStatusId;
            Status = status;
        }
    }
}
