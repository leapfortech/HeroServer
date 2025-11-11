using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentPayment
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int AppUserId { get; set; }
        public int InvestmentPaymentTypeId { get; set; }
        public int TransactionTypeId { get; set; }
        public int TransactionId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public InvestmentPayment()
        {
        }

        public InvestmentPayment(int id, int investmentId, int appUserId, int investmentPaymentTypeId, int transactionTypeId, int transactionId, 
                                 DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentId = investmentId;
            AppUserId = appUserId;
            InvestmentPaymentTypeId = investmentPaymentTypeId;
            TransactionTypeId = transactionTypeId;
            TransactionId = transactionId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public InvestmentPayment(int id, int investmentId, int appUserId, int investmentPaymentTypeId, int transactionTypeId, int transactionId,
                                 int status)
        {
            Id = id;
            InvestmentId = investmentId;
            AppUserId = appUserId;
            InvestmentPaymentTypeId = investmentPaymentTypeId;
            TransactionTypeId = transactionTypeId;
            TransactionId = transactionId;
            Status = status;
        }
    }
}
