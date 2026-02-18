using System;

namespace HeroServer
{
    public class WebSysPasswordRequest
    {
        public long WebSysUserId { get; set; }
        public String NewPassword { get; set; }

        public WebSysPasswordRequest()
        {
        }

        public WebSysPasswordRequest(long webSysUserId, String newPassword)
        {
            WebSysUserId = webSysUserId;
            NewPassword = newPassword;
        }
    }
}
