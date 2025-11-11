using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class ReferredCount
    {
        public int Count { get; set; }
        public int InvestmentCount { get; set; }

        public ReferredCount()
        {
        }

        public ReferredCount(int count, int investmentCount)
        {
            Count = count;
            InvestmentCount = investmentCount;
        }
    }
}
