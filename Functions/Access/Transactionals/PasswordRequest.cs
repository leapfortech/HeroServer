using System;

namespace HeroServer
{
    public class PasswordRequest
    {
        public int Method { get; set; } = -1;
        public int PhoneChannel { get; set; } = -1;
        public long PhoneCountryId { get; set; } = -1;
        public String Phone { get; set; } = null;
        public String Email { get; set; } = null;

        public PasswordRequest()
        {
        }

        public PasswordRequest(int method, int phoneChannel, long phoneCountryId, String phone, String email)
        {
            Method = method;
            PhoneChannel = phoneChannel;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
        }
    }
}
