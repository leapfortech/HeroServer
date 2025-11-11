using System;

namespace HeroServer
{
    public class InvestmentFractionated
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int ProductFractionatedId { get; set; }
        public double Amount { get; set; }
        public int InstallmentCount { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public InvestmentFractionated()
        {
        }

        public InvestmentFractionated(int id, int investmentId, int productFractionatedId, double amount, int installmentCount,
                                      DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentId = investmentId;
            ProductFractionatedId = productFractionatedId;
            Amount = amount;
            InstallmentCount = installmentCount;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public InvestmentFractionated(int investmentId, int productFractionatedId)
        {
            Id = -1;
            InvestmentId = investmentId;
            ProductFractionatedId = productFractionatedId;
            Amount = 0d;
            InstallmentCount = 0;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = 0;
        }
    }
}
