using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class AddressAppUser
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int AddressId { get; set; }
        public int HouseholdBillCount { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public AddressAppUser()
        {
        }

        public AddressAppUser(int id, int appUserId, int addressId, int householdBillCount, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            AddressId = addressId;
            HouseholdBillCount = householdBillCount;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
