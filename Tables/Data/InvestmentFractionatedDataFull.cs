using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentFractionatedDataFull
    {
        public List<InvestmentFractionatedFull> InvestmentFractionatedFulls { get; set; }
        public List<InvestmentPayment> InvestmentPayments { get; set; }
        public List<BankTransaction> BankTransactions { get; set; }
        public List<InvestmentInstallment> InvestmentInstallments { get; set; }
        public List<InvestmentInstallmentPayment> InvestmentInstallmentPayments { get; set; }

        public InvestmentFractionatedDataFull()
        {
        }

        public InvestmentFractionatedDataFull(List<InvestmentFractionatedFull> investmentFractionatedFulls,
                                              List<InvestmentPayment> investmentPayments,
                                              List<BankTransaction> bankTransactions,
                                              List<InvestmentInstallment> investmentInstallments,
                                              List<InvestmentInstallmentPayment> investmentInstallmentPayments)
        {
            InvestmentFractionatedFulls = investmentFractionatedFulls;
            InvestmentPayments = investmentPayments;
            BankTransactions = bankTransactions;
            InvestmentInstallments = investmentInstallments;
            InvestmentInstallmentPayments = investmentInstallmentPayments;
        }
    }
}
