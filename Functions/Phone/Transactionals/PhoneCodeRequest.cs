using System;

namespace HeroServer
{
    public class PhoneCodeRequest
    {
        public int PhoneCountryId { get; set; }
        public String PhoneNumber { get; set; }
        public String Code { get; set; }


        public PhoneCodeRequest()
        {
        }

        public PhoneCodeRequest(int phoneCountryId, String phoneNumber, String code)
        {
            PhoneCountryId = phoneCountryId;
            PhoneNumber = phoneNumber;
            Code = code;
        }
    }
}
