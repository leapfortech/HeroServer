using System;

namespace HpbServer
{
    public class ContactFull
    {
        public long Id { get; set; }
        public String Name { get; set; }
        public long PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }

        public ContactFull()
        {
        }

        public ContactFull(long id, String name, long phoneCountryId,
                           String phone, String email)
        {
            Id = id;
            Name = name;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
        }
    }
}
