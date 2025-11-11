using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentIdentity
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int IdentityId { get; set; }
        public int InvestmentIdentityTypeId { get; set; }
        public String Relationship { get; set; }
        public double Pourcentage { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public InvestmentIdentity()
        {
        }

        public InvestmentIdentity(int id, int investmentId, int identityId, int investmentIdentityTypeId, String relationship, double pourcentage,
                                  DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentId = investmentId;
            IdentityId = identityId;
            InvestmentIdentityTypeId = investmentIdentityTypeId;
            Relationship = relationship;
            Pourcentage = pourcentage;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
