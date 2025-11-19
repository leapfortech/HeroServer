using System;

namespace HeroServer
{
    public class WebSysToken
    {
        public long Id { get; set; }
        public long WebSysUserId { get; set; }
        public String Token { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public WebSysToken()
        {

        }

        public WebSysToken(long id, long webSysUserId, String token, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Token = token;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public WebSysToken(long id, long webSysUserId, String token, int status)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Token = token;
            Status = status;
        }
    }
}
