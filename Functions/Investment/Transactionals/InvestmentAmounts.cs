using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentAmounts
    {
        public double InstallmentAmount { get; set; }
        public int FullTerm { get; set; }
        public double ReserveDiscount { get; set; }
        public double LastAmount { get; set; }
        public double LastDiscount { get; set; }

        public InvestmentAmounts()
        {
        }
    }
}
