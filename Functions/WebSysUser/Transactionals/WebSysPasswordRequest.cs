using System;

namespace HeroServer
{
    public class WebSysPasswordRequest
    {
        public long WebSysUserId { get; set; }
        public String Password { get; set; }

        public WebSysPasswordRequest()
        {
        }

        public WebSysPasswordRequest(long webSysUserId, String password)
        {
            WebSysUserId = webSysUserId;
            Password = password;
        }
    }
}
