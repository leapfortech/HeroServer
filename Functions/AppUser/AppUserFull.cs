using System;

namespace HeroServer
{
    public class AppUserFull
    {
        public long Id { get; set; }
        public String AuthUserId { get; set; }
        public String Email { get; set; }
        public String PhonePrefix { get; set; }
        public String Phone { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int AppUserStatusId { get; set; }

        public AppUserFull()
        {

        }

        public AppUserFull(long id, String authUserId, String email, String phonePrefix, String phone,
                            DateTime createDateTime, DateTime updateDateTime, int appUserStatusId)
        {
            Id = id;
            AuthUserId = authUserId;
            Email = email;
            PhonePrefix = phonePrefix;
            Phone = phone;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            AppUserStatusId = appUserStatusId;
        }
    }
}
