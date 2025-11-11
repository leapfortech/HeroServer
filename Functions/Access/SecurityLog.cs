using System;

namespace HeroServer
{
    public class SecurityLog
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public String Type { get; set; }
        public String Context { get; set; }
        public String AuthEmail { get; set; }
        public String AuthUserId { get; set; }
        public int AuthAppUserId { get; set; }
        public int AppUserId { get; set; }
        public String AppUserEmail { get; set; }

        public SecurityLog()
        {
        }

        public SecurityLog(int id, DateTime dateTime, String type, String context, String authEmail,
                                   String authUserId, int authAppUserId, int appUserId, String appUserEmail)
        {
            Id = id;
            DateTime = dateTime;
            Type = type;
            Context = context;
            AuthEmail = authEmail;
            AuthUserId = authUserId;
            AuthAppUserId = authAppUserId;
            AppUserId = appUserId;
            AppUserEmail = appUserEmail;
        }
    }
}
