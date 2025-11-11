using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentFinanced
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int ProductFinancedId { get; set; }
        public double AdvAmount { get; set; }
        public int AdvInstallmentTotal { get; set; }
        public double LoanInterestRate { get; set; }
        public int LoanTerm { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public InvestmentFinanced()
        {
        }

        public InvestmentFinanced(int id, int investmentId, int productFinancedId, double advAmount, int advInstallmentTotal,
                                  double loanInterestRate, int loanTerm, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentId = investmentId;
            ProductFinancedId = productFinancedId; 
            AdvAmount = advAmount;
            AdvInstallmentTotal = advInstallmentTotal;
            LoanInterestRate = loanInterestRate;
            LoanTerm = loanTerm;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public InvestmentFinanced(int investmentId, int productFinancedId)
        {
            Id = -1;
            InvestmentId = investmentId;
            ProductFinancedId = productFinancedId;
            AdvAmount = 0d;
            AdvInstallmentTotal = 0;
            LoanInterestRate = 0d;
            LoanTerm = 0;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = 0;
        }
    }
}
