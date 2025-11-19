using System;

namespace HeroServer
{
    public class AppUser
    {
        public long Id { get; set; }
        public long WebSysUserId { get; set; }
        public String Alias { get; set; }
        public String CSToken { get; set; }
        public long Options { get; set; } = 0;
        public long ReferrerAppUserId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Today;
        public DateTime UpdateDateTime { get; set; } = DateTime.Today;
        public int AppUserStatusId { get; set; }

        public AppUser()
        {
        }

        public AppUser(long id, long webSysUserId, String alias, String csToken, long options, long referrerAppUserId, DateTime createDateTime, DateTime updateDateTime, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            CSToken = csToken;
            Options = options;
            ReferrerAppUserId = referrerAppUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(long id, long webSysUserId, String alias, String csToken, long options, long referrerAppUserId, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            CSToken = csToken;
            Options = options;
            ReferrerAppUserId = referrerAppUserId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(long id, long webSysUserId, String alias, String csToken, long options, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            CSToken = csToken;
            Options = options;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(long id, long webSysUserId, long referrerAppUserId, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            CSToken = null;
            ReferrerAppUserId = referrerAppUserId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }
    }
}
