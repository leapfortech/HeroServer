using System;

namespace HeroServer
{
    public class AppUser
    {
        public int Id { get; set; }
        public String Code { get; set; }
        public int WebSysUserId { get; set; }
        public String CSToken { get; set; }
        public int News { get; set; }
        public int Options { get; set; } = 0;
        public int ReferrerAppUserId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Today;
        public DateTime UpdateDateTime { get; set; } = DateTime.Today;
        public int AppUserStatusId { get; set; }

        public AppUser()
        {
        }

        public AppUser(int id, String code, int webSysUserId, String csToken, int news, int options, int referrerAppUserId, DateTime createDateTime, DateTime updateDateTime, int appUserStatusId)
        {
            Id = id;
            Code = code;
            WebSysUserId = webSysUserId;
            CSToken = csToken;
            News = news;
            Options = options;
            ReferrerAppUserId = referrerAppUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(int id, String code, int webSysUserId, String csToken, int news, int options, int referrerAppUserId, int appUserStatusId)
        {
            Id = id;
            Code = code;
            WebSysUserId = webSysUserId;
            CSToken = csToken;
            News = news;
            Options = options;
            ReferrerAppUserId = referrerAppUserId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(int id, int webSysUserId, String csToken, int news, int options, int appUserStatusId)
        {
            Id = id;
            Code = null;
            WebSysUserId = webSysUserId;
            CSToken = csToken;
            News = news;
            Options = options;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }

        public AppUser(int id, int webSysUserId, int news, int referrerAppUserId, int appUserStatusId)
        {
            Id = id;
            Code = null;
            WebSysUserId = webSysUserId;
            CSToken = null;
            News = news;
            ReferrerAppUserId = referrerAppUserId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            AppUserStatusId = appUserStatusId;
        }
    }
}
