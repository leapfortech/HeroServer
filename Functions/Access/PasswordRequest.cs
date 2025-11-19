using System;

namespace HeroServer
{
    public class PasswordRequest
    {
        public long AppUserId { get; set; }
        public String AuthUserId { get; set; }
        public String NewPassword { get; set; }

        public PasswordRequest()
        {
        }

        public PasswordRequest(long appUserId, String authUserId, String newPassword)
        {
            AppUserId = appUserId;
            AuthUserId = authUserId;
            NewPassword = newPassword;
        }
    }
}
