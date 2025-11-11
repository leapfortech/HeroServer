using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PhoneRequest
    {
        public int Id { get; set; }
        public int PhoneCountryId { get; set; }
        public String Phone { get; set; }


        public PhoneRequest()
        {
        }

        public PhoneRequest(int id, int phoneCountryId, String phone)
        {
            Id = id;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
        }
    }
}
