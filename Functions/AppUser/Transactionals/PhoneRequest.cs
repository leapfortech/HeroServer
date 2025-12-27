using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PhoneRequest
    {
        public long Id { get; set; }
        public long PhoneCountryId { get; set; }
        public String Phone { get; set; }


        public PhoneRequest()
        {
        }

        public PhoneRequest(long id, long phoneCountryId, String phone)
        {
            Id = id;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
        }
    }
}
