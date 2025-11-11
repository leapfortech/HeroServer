using System;

namespace HeroServer
{
    public class Investment
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProductTypeId { get; set; }
        public int AppUserId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int DevelopmentTerm { get; set; }
        public int CpiCount { get; set; }
        public double TotalAmount { get; set; }
        public double ReserveAmount { get; set; }
        public double DueAmount { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountAmount { get; set; }
        public double Balance { get; set; }
        public DateTime? CompletionDate { get; set; }
        public String DocuSignReference { get; set; }
        public int BoardUserId { get; set; }
        public int InvestmentMotiveId { get; set; }
        public String BoardComment { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int InvestmentStatusId { get; set; }


        public Investment()
        {
        }

        public Investment(int id, int projectId, int productTypeId, int appUserId, DateTime effectiveDate, int developmentTerm, int cpiCount, double totalAmount,
                          double reserveAmount, double dueAmount, double discountRate, double discountAmount, double balance, DateTime? completionDate, String docuSignReference,
                          int boardUserId, int investmentMotiveId, String boardComment, DateTime createDateTime, DateTime updateDateTime, int investmentStatusId)
        {
            Id = id;
            ProjectId = projectId;
            ProductTypeId = productTypeId;
            AppUserId = appUserId;
            EffectiveDate = effectiveDate;
            DevelopmentTerm = developmentTerm;
            CpiCount = cpiCount;
            TotalAmount = totalAmount;
            ReserveAmount = reserveAmount;
            DueAmount = dueAmount;
            DiscountRate = discountRate;
            DiscountAmount = discountAmount;
            Balance = balance;
            CompletionDate = completionDate;
            DocuSignReference = docuSignReference;
            BoardUserId = boardUserId;
            InvestmentMotiveId = investmentMotiveId;
            BoardComment = boardComment;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            InvestmentStatusId = investmentStatusId;
        }
    }
}
