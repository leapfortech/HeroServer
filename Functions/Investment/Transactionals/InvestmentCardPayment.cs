using System;

namespace HeroServer
{
    public class InvestmentCardPayment
    {
        public InvestmentPayment InvestmentPayment { get; set; }
        public CardTransaction CardTransaction { get; set; }


        public InvestmentCardPayment()
        {
        }

        public InvestmentCardPayment(InvestmentPayment investmentPayment, CardTransaction cardTransaction)
        {
            InvestmentPayment = investmentPayment;
            CardTransaction = cardTransaction;
        }
    }
}