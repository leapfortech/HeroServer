using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentFractionatedFull : InvestmentFull
    {
        public int Id { get; set; }
        public int ProductFractionatedId { get; set; }
        public double Amount { get; set; }
        public int InstallmentCount { get; set; }
        public int Status { get; set; }

        public InvestmentFractionatedFull()
        {
        }

        public InvestmentFractionatedFull(int id, int investmentId, int productFractionatedId,
                                          int projectId, int productTypeId, int appUserId, DateTime effectiveDate, int developmentTerm, int cpiCount, double totalAmount, double reserveAmount, double dueAmount,
                                          double discountRate, double discountAmount, double balance, DateTime? completionDate, String docuSignReference, int boardUserId, int investmentMotiveId, String boardComment, int investmentStatusId,
                                          double amount, int installmentCount, int status, List<InvestmentBankPayment> investmentBankPayments,
                                          List<InvestmentInstallmentInfo> investmentInstallmentInfos)
                                          : base(investmentId, projectId, productTypeId, appUserId, effectiveDate, developmentTerm, cpiCount, totalAmount, reserveAmount, dueAmount, discountRate, discountAmount,
                                                 balance, completionDate, docuSignReference, boardUserId, investmentMotiveId, boardComment, investmentStatusId, investmentBankPayments, investmentInstallmentInfos)
        {
            Id = id;
            ProductFractionatedId = productFractionatedId;
            Amount = amount;
            InstallmentCount = installmentCount;
            Status = status;
        }

        public InvestmentFractionatedFull(InvestmentFractionatedFull iff)
                                          : base(iff.InvestmentId, iff.ProjectId, iff.ProductTypeId, iff.AppUserId, iff.EffectiveDate, iff.DevelopmentTerm, iff.CpiCount, iff.TotalAmount, iff.ReserveAmount, iff.DueAmount, iff.DiscountRate,
                                                 iff.DiscountAmount, iff.Balance, iff.CompletionDate, iff.DocuSignReference, iff.BoardUserId, iff.InvestmentMotiveId, iff.BoardComment, iff.InvestmentStatusId, iff.InvestmentBankPayments, iff.InvestmentInstallmentInfos)
        {
            Id = iff.Id;
            ProductFractionatedId = iff.ProductFractionatedId;
            Amount = iff.Amount;
            InstallmentCount = iff.InstallmentCount;
            Status = iff.Status;
        }
    }
}
