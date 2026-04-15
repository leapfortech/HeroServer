using System;

namespace HeroServer
{
    public class AccountRequest
    {
        public long WebSysUserId { get; set; } = -1;
        public int Method { get; set; } = -1;
        public int PhoneChannel { get; set; } = -1;
        public long PhoneCountryId { get; set; } = -1;
        public String Phone { get; set; } = null;
        public String Email { get; set; } = null;

        public AccountRequest()
        {
        }

        public AccountRequest(long webSysUserId, int method, int phoneChannel, long phoneCountryId, String phone, String email)
        {
            WebSysUserId = webSysUserId;
            Method = method;
            PhoneChannel = phoneChannel;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
        }
    }
}
