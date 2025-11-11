using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentDocInfo
    {
        public Investment Investment { get; set; }
        public List<String> DocRtus { get; set; }
        public EconomicsInfo EconomicsInfo { get; set; }
        public List<String> DocBanks { get; set; }

        public InvestmentDocInfo()
        {
        }

        public InvestmentDocInfo(Investment investment, List<String> docRtus, EconomicsInfo economicsInfo, List<String> docBanks)
        {
            Investment = investment;
            DocRtus = docRtus;
            EconomicsInfo = economicsInfo;
            DocBanks = docBanks;
        }
    }
}