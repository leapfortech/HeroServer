using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class AddressAppUser
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long AddressId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public AddressAppUser()
        {
        }

        public AddressAppUser(long id, long appUserId, long addressId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            AddressId = addressId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
