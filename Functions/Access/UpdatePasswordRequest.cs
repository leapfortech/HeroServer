using System;

namespace HeroServer
{
    public class UpdatePasswordRequest
    {
        public long WebSysUserId { get; set; } = -1;
        public String Password { get; set; } = null;

        public UpdatePasswordRequest()
        {
        }

        public UpdatePasswordRequest(long webSysUserId, String password)
        {
            WebSysUserId = webSysUserId;
            Password = password;
        }
    }
}
