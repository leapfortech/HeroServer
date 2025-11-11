using System;

namespace HeroServer
{
    public class InvestmentPaymentBankFull
    {
        public int Id { get; set; }

        public int InvestmentId { get; set; }
        public String ProjectName { get; set; }
        public String ProductName { get; set; }

        public int AppUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }

        public String InvestmentPaymentType { get; set; }
        public String AccountHpb { get; set; }
        public int TransactionId { get; set; }
        public int TransactionTypeId { get; set; }
        public String CurrencySymbol { get; set; }
        public double Amount { get; set; }
        public String Number { get; set; }
        public DateTime SendDateTime { get; set; }

        public String Receipt { get; set; }
        public int Status { get; set; }


        public InvestmentPaymentBankFull()
        {
        }

        public InvestmentPaymentBankFull(int id, int investmentId, String projectName, String productName, int appUserId, String firstName1, String firstName2, String lastName1, String lastName2,
                                         String investmentPaymentType, String accountHpb, int transactionId, int transactionTypeId, String currencySymbol, double amount, String number, DateTime sendDateTime, String receipt, int status)
        {
            Id = id;

            InvestmentId = investmentId;
            ProjectName = projectName;
            ProductName = productName;

            AppUserId = appUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;

            InvestmentPaymentType = investmentPaymentType;
            AccountHpb = accountHpb;
            TransactionId = transactionId;
            TransactionTypeId = transactionTypeId;
            CurrencySymbol = currencySymbol;
            Amount = amount;
            Number = number;
            SendDateTime = sendDateTime;

            Receipt = receipt;
            Status = status;
        }
    }
}
