using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Economics
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int IncomeCurrencyId { get; set; }
        public double IncomeAmount { get; set; }
        public int ExpensesCurrencyId { get; set; }
        public double ExpensesAmount { get; set; }
        public String Activity { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Economics()
        {
        }

        public Economics(int id, int investmentId, int incomeCurrencyId, double incomeAmount, int expensesCurrencyId, double expensesAmount, String activity, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentId = investmentId;
            IncomeCurrencyId = incomeCurrencyId;
            IncomeAmount = incomeAmount;
            ExpensesCurrencyId = expensesCurrencyId;
            ExpensesAmount = expensesAmount;
            Activity = activity;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
