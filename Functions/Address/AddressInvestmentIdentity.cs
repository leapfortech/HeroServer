using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class AddressInvestmentIdentity
    {
        public int Id { get; set; }
        public int InvestmentIdentityId { get; set; }
        public int AddressId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public AddressInvestmentIdentity()
        {
        }

        public AddressInvestmentIdentity(int id, int investmentIdentityId, int addressId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentIdentityId = investmentIdentityId;
            AddressId = addressId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
