using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class CpeInvestmentIdentity
    {
        public int Id { get; set; }
        public int InvestmentIdentityId { get; set; }
        public int CpeId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public CpeInvestmentIdentity()
        {
        }

        public CpeInvestmentIdentity(int id, int investmentIdentityId, int cpeId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentIdentityId = investmentIdentityId;
            CpeId = cpeId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
