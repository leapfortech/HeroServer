using System;

namespace HeroServer
{
    public class InvestmentInstallmentPayment
    {
        public int Id { get; set; }
        public int InvestmentPaymentId { get; set; }
        public int InvestmentInstallmentId { get; set; }
        public double Amount { get; set; }
        public double DiscountAmount { get; set; }
        public double NewBalance { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public InvestmentInstallmentPayment()
        {
        }

        public InvestmentInstallmentPayment(int id, int investmentPaymentId, int investmentInstallmentId, double amount, double discountAmount, double newBalance, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentPaymentId = investmentPaymentId;
            InvestmentInstallmentId = investmentInstallmentId;
            Amount = amount;
            DiscountAmount = discountAmount;
            NewBalance = newBalance;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public InvestmentInstallmentPayment(int investmentPaymentId, int investmentInstallmentId, double amount, double discountAmount, double newBalance)
        {
            Id = -1;
            InvestmentPaymentId = investmentPaymentId;
            InvestmentInstallmentId = investmentInstallmentId;
            Amount = amount;
            DiscountAmount = discountAmount;
            NewBalance = newBalance;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = 1;
        }
    }
}
