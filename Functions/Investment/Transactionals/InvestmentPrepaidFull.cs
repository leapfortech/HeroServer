using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentPrepaidFull : InvestmentFull
    {
        public int Id { get; set; }
        public int ProductPrepaidId { get; set; }
        public int Status { get; set; }

        public InvestmentPrepaidFull()
        {
        }

        public InvestmentPrepaidFull(int id, int investmentId, int productPrepaidId,
                                     int projectId, int productTypeId, int appUserId, DateTime effectiveDate, int developmentTerm, int cpiCount, double totalAmount, double reserveAmount, double dueAmount,
                                     double discountRate, double discountAmount, double balance, DateTime? completionDate, String docuSignReference, int boardUserId, int investmentMotiveId, String boardComment, int investmentStatusId,
                                     int status, List<InvestmentBankPayment> investmentBankPayments, List<InvestmentInstallmentInfo> investmentInstallmentInfos)
                                     : base(investmentId, projectId, productTypeId, appUserId, effectiveDate, developmentTerm, cpiCount, totalAmount, reserveAmount, dueAmount, discountRate, discountAmount,
                                            balance, completionDate, docuSignReference, boardUserId, investmentMotiveId, boardComment, investmentStatusId, investmentBankPayments, investmentInstallmentInfos)
        {
            Id = id;
            ProductPrepaidId = productPrepaidId;
            Status = status;
        }

        public InvestmentPrepaidFull(InvestmentPrepaidFull ipf)
                                     : base(ipf.InvestmentId, ipf.ProjectId, ipf.ProductTypeId, ipf.AppUserId, ipf.EffectiveDate, ipf.DevelopmentTerm, ipf.CpiCount, ipf.TotalAmount, ipf.ReserveAmount, ipf.DueAmount, ipf.DiscountRate,
                                            ipf.DiscountAmount, ipf.Balance, ipf.CompletionDate, ipf.DocuSignReference, ipf.BoardUserId, ipf.InvestmentMotiveId, ipf.BoardComment, ipf.InvestmentStatusId, ipf.InvestmentBankPayments, ipf.InvestmentInstallmentInfos)
        {
            Id = ipf.Id;
            ProductPrepaidId = ipf.ProductPrepaidId;
            Status = ipf.Status;
        }
    }
}
