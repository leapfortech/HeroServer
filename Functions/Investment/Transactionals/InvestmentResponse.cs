using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentResponse
    {
        public List<InvestmentFractionatedFull> InvestmentFractionatedFulls { get; set; }
        public List<InvestmentFinancedFull> InvestmentFinancedFulls { get; set; }
        public List<InvestmentPrepaidFull> InvestmentPrepaidFulls { get; set; }


        public InvestmentResponse()
        {
        }

        public InvestmentResponse(List<InvestmentFractionatedFull> investmentFractionatedFulls, List<InvestmentFinancedFull> investmentFinancedFulls,
                                  List<InvestmentPrepaidFull> investmentPrepaidFulls)
        {
            InvestmentFractionatedFulls = investmentFractionatedFulls;
            InvestmentFinancedFulls = investmentFinancedFulls;
            InvestmentPrepaidFulls = investmentPrepaidFulls;
        }
    }
}
