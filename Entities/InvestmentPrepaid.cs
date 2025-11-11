using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentPrepaid
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int ProductPrepaidId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public InvestmentPrepaid()
        {
        }

        public InvestmentPrepaid(int id, int investmentId, int productPrepaidId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentId = investmentId;
            ProductPrepaidId = productPrepaidId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public InvestmentPrepaid(int investmentId, int productPrepaidId)
        {
            Id = -1;
            InvestmentId = investmentId;
            ProductPrepaidId = productPrepaidId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = 0;
        }
    }
}
