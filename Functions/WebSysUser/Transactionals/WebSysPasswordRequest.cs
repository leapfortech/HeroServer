using System;

namespace HeroServer
{
    public class WebSysPasswordRequest
    {
        public int WebSysUserId { get; set; }
        public String Password { get; set; }

        public WebSysPasswordRequest()
        {
        }

        public WebSysPasswordRequest(int webSysUserId, String password)
        {
            WebSysUserId = webSysUserId;
            Password = password;
        }
    }
}
