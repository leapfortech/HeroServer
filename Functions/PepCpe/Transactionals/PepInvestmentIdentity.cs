using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PepInvestmentIdentity
    {
        public int Id { get; set; }
        public int InvestmentIdentityId { get; set; }
        public int PepId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public PepInvestmentIdentity()
        {
        }

        public PepInvestmentIdentity(int id, int investmentIdentityId, int pepId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentIdentityId = investmentIdentityId;
            PepId = pepId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
