using System;

namespace HeroServer
{
    public class Identity
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public long GenderId { get; set; }
        public DateTime BirthDate { get; set; }
        public long OriginCountryId { get; set; }
        public long OriginStateId { get; set; }
       
        public long PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Identity()
        {
        }

        public Identity(long id, long appUserId, String firstName1, String firstName2, String lastName1, String lastName2,
                        long genderId, DateTime birthDate, long originCountryId, long originStateId,
                        long phoneCountryId, String phone, String email,
                        DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            GenderId = genderId;
            BirthDate = birthDate;
            OriginCountryId = originCountryId;
            OriginStateId = originStateId;
          
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
           
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
