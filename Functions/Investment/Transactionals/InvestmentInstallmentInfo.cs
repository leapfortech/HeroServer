using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentInstallmentInfo
    {
        public InvestmentInstallment InvestmentInstallment { get; set; }
        public List<InvestmentInstallmentPayment> InvestmentInstallmentPayments { get; set; }


        public InvestmentInstallmentInfo()
        {
        }

        public InvestmentInstallmentInfo(InvestmentInstallment investmentInstallment, List<InvestmentInstallmentPayment> investmentInstallmentPayments)
        {
            InvestmentInstallment = investmentInstallment;
            InvestmentInstallmentPayments = investmentInstallmentPayments;
        }
    }
}
