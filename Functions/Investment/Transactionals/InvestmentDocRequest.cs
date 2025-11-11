using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentDocRequest
    {
        public int InvestmentId { get; set; }
        public List<String> Docs { get; set; }

        public InvestmentDocRequest()
        {
        }

        public InvestmentDocRequest(int investmentId, List<String> docs)
        {
            InvestmentId = investmentId;
            Docs = docs;
        }
    }
}