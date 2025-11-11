using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PepIdentityInvestmentIdentity
    {
        public int Id { get; set; }
        public int InvestmentIdentityId { get; set; }
        public int PepIdentityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public PepIdentityInvestmentIdentity()
        {
        }

        public PepIdentityInvestmentIdentity(int id, int investmentIdentityId, int pepIdentityId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentIdentityId = investmentIdentityId;
            PepIdentityId = pepIdentityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public PepIdentityInvestmentIdentity(int id, int investmentIdentityId, int pepIdentityId, int status)
        {
            Id = id;
            InvestmentIdentityId = investmentIdentityId;
            PepIdentityId = pepIdentityId;
            Status = status;
        }
    }
}
