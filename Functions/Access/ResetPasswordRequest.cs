using System;

namespace HeroServer
{
    public class ResetPasswordRequest
    {
        public int Method { get; set; } = -1;
        public int Channel { get; set; } = -1;
        public long PhoneCountryId { get; set; } = -1;
        public String Phone { get; set; } = null;
        public String Email { get; set; } = null;

        public ResetPasswordRequest()
        {
        }

        public ResetPasswordRequest(int method, int channel, long phoneCountryId, String phone, String email)
        {
            Method = method;
            Channel = channel;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
        }
    }
}
