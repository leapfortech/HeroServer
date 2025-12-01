using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Contact
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public String Name { get; set; }
        public long PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Contact() { }

        public Contact(long id, long productId, String name, long phoneCountryId, String phone, String email,
                       DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            ProductId = productId;
            Name = name;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
