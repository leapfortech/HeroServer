using System;

namespace HeroServer
{
    public class InvestmentBankPayment
    {
        public InvestmentPayment InvestmentPayment { get; set; }
        public BankTransaction BankTransaction { get; set; }
        public String Receipt { get; set; }

        public InvestmentBankPayment()
        {
        }

        public InvestmentBankPayment(InvestmentPayment investmentPayment, BankTransaction bankTransaction, String receipt)
        {
            InvestmentPayment = investmentPayment;
            BankTransaction = bankTransaction;
            Receipt = receipt;
        }
    }
}