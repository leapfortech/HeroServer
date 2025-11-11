using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class EconomicsInfo
    {
        public Economics Economics { get; set; }
        public List<Income> Incomes { get; set; }
        public List<String> DocIncomes { get; set; }

        public EconomicsInfo()
        {
        }

        public EconomicsInfo(Economics economics, List<Income> incomes, List<String> docIncomes)
        {
            Economics = economics;
            Incomes = incomes;
            DocIncomes = docIncomes;
        }
    }
}
