using System;

namespace HeroServer
{
    public class UpdateAccountRequest
    {
        public long WebSysUserId { get; set; } = -1;
        public long PhoneCountryId { get; set; } = -1;
        public String Phone { get; set; } = null;
        public String Email { get; set; } = null;

        public UpdateAccountRequest()
        {
        }

        public UpdateAccountRequest(long webSysUserId, long phoneCountryId, String phone, String email)
        {
            WebSysUserId = webSysUserId;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
        }
    }
}
