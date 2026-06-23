using System;

namespace HeroServer
{
    public class AppUser
    {
        public long Id { get; set; }
        public long WebSysUserId { get; set; }
        public String Alias { get; set; }
        public String ReferringCode { get; set; }
        public long ReferrerAppUserId { get; set; }
        public String CSToken { get; set; }
        public long Options { get; set; } = 0;
        public DateTime CreateDateTime { get; set; } = DateTime.Today;
        public DateTime UpdateDateTime { get; set; } = DateTime.Today;
        public int AppUserStatusId { get; set; }

        public AppUser()
        {
        }

        public AppUser(long id, long webSysUserId, String alias, String referringCode, long referrerAppUserId, String csToken, long options, DateTime createDateTime, DateTime updateDateTime, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            ReferringCode = referringCode;
            ReferrerAppUserId = referrerAppUserId;
            CSToken = csToken;
            Options = options;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(long id, long webSysUserId, String alias, String referringCode, long referrerAppUserId, String csToken, long options, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            ReferringCode = referringCode;
            ReferrerAppUserId = referrerAppUserId;
            CSToken = csToken;
            Options = options;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(long id, long webSysUserId, String alias, String referringCode, String csToken, long options, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            ReferringCode = referringCode;
            CSToken = csToken;
            Options = options;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(long id, long webSysUserId, String alias, String referringCode, long referrerAppUserId, int appUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            ReferringCode = referringCode;
            ReferrerAppUserId = referrerAppUserId;
            CSToken = null;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }
    }
}
