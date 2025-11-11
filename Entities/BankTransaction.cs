using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class BankTransaction
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int AccountHpbId { get; set; }
        public int TransactionTypeId { get; set; }
        public double Amount { get; set; }
        public String Number { get; set; }
        public DateTime SendDateTime { get; set; }
        public String ApprovalCode { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public BankTransaction()
        {
        }

        public BankTransaction(int id, int appUserId, int accountHpbId, int transactionTypeId, double amount, String number, DateTime sendDateTime, String approvalCode, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            AccountHpbId = accountHpbId;
            TransactionTypeId = transactionTypeId;
            Amount = amount;
            Number = number;
            SendDateTime = sendDateTime;
            ApprovalCode = approvalCode;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public BankTransaction(InvestmentBankPayment payment, int status)
        {
            Id = -1;
            AppUserId = payment.InvestmentPayment.AppUserId;
            AccountHpbId = payment.BankTransaction.AccountHpbId;
            TransactionTypeId = payment.InvestmentPayment.TransactionTypeId;
            Amount = payment.BankTransaction.Amount;
            Number = payment.BankTransaction.Number;
            SendDateTime = payment.BankTransaction.SendDateTime;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = status;
        }
    }
}
