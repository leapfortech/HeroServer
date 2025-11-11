using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class CpiRange
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProductTypeId { get; set; }
        public int AmountMin { get; set; }
        public int AmountMax { get; set; }
        public double DiscountRate { get; set; }
        public double ProfitablityRate { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public CpiRange()
        {
        }

        public CpiRange(int id, int projectId, int productTypeId, int amountMin, int amountMax, double discountRate, double profitablityRate,
                        DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            ProjectId = projectId;
            ProductTypeId = productTypeId;
            AmountMin = amountMin;
            AmountMax = amountMax;
            DiscountRate = discountRate;
            ProfitablityRate = profitablityRate;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
