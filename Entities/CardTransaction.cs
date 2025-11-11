using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class CardTransaction
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public String Reference { get; set; }
        public String TransactionCode { get; set; }
        public double Amount { get; set; }
        public String ApprovalCode { get; set; }
        public String DeclinedCode { get; set; }
        public String DeclinedMotive { get; set; }
        public String CancellationReference { get; set; }
        public String CancellationTransactionCode { get; set; }
        public String CancellationStatus { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public CardTransaction()
        {
        }

        public CardTransaction(int id, int cardId, String reference, String transactionCode, double amount, String approvalCode, String declinedCode, String declinedMotive,
                               String cancellationReference, String cancellationTransactionCode, String cancellationStatus, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            CardId = cardId;
            Reference = reference;
            TransactionCode = transactionCode;
            Amount = amount;
            ApprovalCode = approvalCode;
            DeclinedCode = declinedCode;
            DeclinedMotive = declinedMotive;
            CancellationReference = cancellationReference;
            CancellationTransactionCode = cancellationTransactionCode;
            CancellationStatus = cancellationStatus;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public CardTransaction(InvestmentCardPayment payment, int status)
        {
            Id = -1;
            CardId = payment.CardTransaction.CardId;
            Reference = payment.CardTransaction.Reference;
            TransactionCode = payment.CardTransaction.TransactionCode;
            Amount = payment.CardTransaction.Amount;
            ApprovalCode = payment.CardTransaction.ApprovalCode;
            DeclinedCode = null;
            DeclinedMotive = null;
            CancellationReference = null;
            CancellationTransactionCode = null;
            CancellationStatus = null;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = status;
        }
    }
}
