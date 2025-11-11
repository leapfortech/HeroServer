using System;

namespace HeroServer
{
    public class Income
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int IncomeTypeId { get; set; }
        public String Detail { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Income()
        {
        }

        public Income(int id, int investmentId, int incomeTypeId, String detail, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            InvestmentId = investmentId;
            IncomeTypeId = incomeTypeId;
            Detail = detail;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
