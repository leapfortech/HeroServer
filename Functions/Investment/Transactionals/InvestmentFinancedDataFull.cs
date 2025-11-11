using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentFinancedDataFull
    {
        public List<InvestmentFinancedFull> InvestmentFinancedFulls { get; set; }
        public List<InvestmentPayment> InvestmentPayments { get; set; }
        public List<BankTransaction> BankTransactions { get; set; }
        public List<InvestmentInstallment> InvestmentInstallments { get; set; }
        public List<InvestmentInstallmentPayment> InvestmentInstallmentPayments { get; set; }

        public InvestmentFinancedDataFull()
        {
        }

        public InvestmentFinancedDataFull(List<InvestmentFinancedFull> investmentFinancedFulls,
                                          List<InvestmentPayment> investmentPayments,
                                          List<BankTransaction> bankTransactions,
                                          List<InvestmentInstallment> investmentInstallments,
                                          List<InvestmentInstallmentPayment> investmentInstallmentPayments)
        {
            InvestmentFinancedFulls = investmentFinancedFulls;
            InvestmentPayments = investmentPayments;
            BankTransactions = bankTransactions;
            InvestmentInstallments = investmentInstallments;
            InvestmentInstallmentPayments = investmentInstallmentPayments;
        }
    }
}
