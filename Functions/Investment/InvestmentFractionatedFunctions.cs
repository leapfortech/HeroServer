using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public static class InvestmentFractionatedFunctions
    {
        // GET
        public static async Task<InvestmentFractionated> GetById(int id)
        {
            return await new InvestmentFractionatedDB().GetById(id);
        }

        public static async Task<InvestmentFractionated> GetByInvestmentId(int investmentId, int status = 1)
        {
            return await new InvestmentFractionatedDB().GetByInvestmentId(investmentId, status);
        }

        public static async Task<InvestmentFractionated> GetByProductFractionatedId(int productFractionatedId, int status = 1)
        {
            return await new InvestmentFractionatedDB().GetByProductFractionatedId(productFractionatedId, status);
        }

        // FULL
        public static async Task<InvestmentFractionatedFull> GetFullById(int id)
        {
            return await new InvestmentFractionatedDB().GetFullById(id);
        }

        public static async Task<InvestmentFractionatedFull> GetFullByInvestmentId(int investmentId)
        {
            return await new InvestmentFractionatedDB().GetFullByInvestmentId(investmentId);
        }

        public static async Task<List<InvestmentFractionatedFull>> GetFullsByStatus(int status)
        {
            InvestmentFractionatedDataFull investmentFractionatedDataFull = await new InvestmentFractionatedDB().GetDataFullByStatus(status);

            return GetFulls(investmentFractionatedDataFull);
        }

        public static async Task<List<InvestmentFractionatedFull>> GetFullsByAppUserId(int appUserId, int status = -1)
        {
            InvestmentFractionatedDataFull investmentFractionatedDataFull = await new InvestmentFractionatedDB().GetDataFullByAppUserId(appUserId, status);

            return GetFulls(investmentFractionatedDataFull);
        }

        public static List<InvestmentFractionatedFull> GetFulls(InvestmentFractionatedDataFull investmentFractionatedDataFull)
        {
            // InvestmentPayment
            Dictionary<int, List<InvestmentPayment>> investmentPaymentsDict = [];
            foreach (InvestmentPayment investmentPayment in investmentFractionatedDataFull.InvestmentPayments)
            {
                if (investmentPaymentsDict.TryGetValue(investmentPayment.InvestmentId, out List<InvestmentPayment> value))
                    value.Add(investmentPayment);
                else
                    investmentPaymentsDict[investmentPayment.InvestmentId] = [investmentPayment];
            }

            // BankTransaction
            Dictionary<int, BankTransaction> bankTransactionsDict = [];
            foreach (BankTransaction bankTransaction in investmentFractionatedDataFull.BankTransactions)
            {
                if (bankTransactionsDict.TryGetValue(bankTransaction.Id, out BankTransaction value))
                    throw new Exception($"Duplicate BankTransactionId {bankTransaction.Id}");

                bankTransactionsDict[bankTransaction.Id] = bankTransaction;
            }

            // InvestmentInstallment
            Dictionary<int, List<InvestmentInstallment>> investmentInstallmentsDict = [];
            foreach (InvestmentInstallment investmentInstallment in investmentFractionatedDataFull.InvestmentInstallments)
            {
                if (investmentInstallmentsDict.TryGetValue(investmentInstallment.InvestmentId, out List<InvestmentInstallment> value))
                    value.Add(investmentInstallment);
                else
                    investmentInstallmentsDict[investmentInstallment.InvestmentId] = [investmentInstallment];
            }

            // InvestmentInstallmentPayment
            Dictionary<int, List<InvestmentInstallmentPayment>> investmentInstallmentPaymentsDict = [];
            foreach (InvestmentInstallmentPayment investmentInstallmentPayment in investmentFractionatedDataFull.InvestmentInstallmentPayments)
            {
                if (investmentInstallmentPaymentsDict.TryGetValue(investmentInstallmentPayment.InvestmentInstallmentId, out List<InvestmentInstallmentPayment> value))
                    value.Add(investmentInstallmentPayment);
                else
                    investmentInstallmentPaymentsDict[investmentInstallmentPayment.InvestmentInstallmentId] = [investmentInstallmentPayment];
            }

            // InvestmentFractionatedFull
            List<InvestmentFractionatedFull> investmentFractionatedFulls = [];
            foreach (InvestmentFractionatedFull investFractionatedFull in investmentFractionatedDataFull.InvestmentFractionatedFulls)
            {
                if (!investmentPaymentsDict.TryGetValue(investFractionatedFull.InvestmentId, out List<InvestmentPayment> payments))
                    payments = [];

                investFractionatedFull.InvestmentBankPayments = [];
                foreach (InvestmentPayment payment in payments)
                {
                    if (!bankTransactionsDict.TryGetValue(payment.TransactionId, out BankTransaction bankTransaction))
                        throw new Exception($"Transaction #{payment.TransactionId} not Found for InvestmentPayment #{payment.Id}");
                    investFractionatedFull.InvestmentBankPayments.Add(new InvestmentBankPayment(payment, bankTransaction, null));
                }

                if (!investmentInstallmentsDict.TryGetValue(investFractionatedFull.InvestmentId, out List<InvestmentInstallment> installments))
                    installments = [];

                investFractionatedFull.InvestmentInstallmentInfos = [];
                foreach (InvestmentInstallment installment in installments)
                {
                    if (!investmentInstallmentPaymentsDict.TryGetValue(installment.Id, out List<InvestmentInstallmentPayment> installmentPayments))
                        installmentPayments = [];
                    investFractionatedFull.InvestmentInstallmentInfos.Add(new InvestmentInstallmentInfo(installment, installmentPayments));
                }

                investmentFractionatedFulls.Add(investFractionatedFull);
            }

            return investmentFractionatedFulls;
        }

        // ADD
        public static async Task<InvestmentFractionatedFull> Register(Investment investment)
        {
            if (investment.ProductTypeId != 1)
                throw new Exception("Error de Tipo de Producto");

            int productFractionatedId = await ProductFractionatedFunctions.GetIdByProjectId(investment.ProjectId);
            if (productFractionatedId == -1)
                throw new Exception("No se encontró el Producto");

            (investment, InvestmentAmounts amounts) = await RefreshAmounts(investment, false);

            int id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                investment.BoardUserId = -1;
                investment.InvestmentStatusId = 0;
                investment.Id = await InvestmentFunctions.Add(investment);

                InvestmentFractionated investmentFractionated = new InvestmentFractionated(investment.Id, productFractionatedId);
                id = await Add(investmentFractionated);

                await InvestmentInstallmentFunctions.RegisterReserve(investment, amounts.ReserveDiscount);

                scope.Complete();
            }

            return await GetFullById(id);
        }

        public static async Task<(Investment, InvestmentAmounts)> RefreshAmounts(Investment investment, bool reservePaid)
        {
            InvestmentAmounts amounts = new InvestmentAmounts();
            double fractionatedReserveRate = await ProductFractionatedFunctions.GetReserveRateByProjectId(investment.ProjectId);

            (DateTime startDate, int projectDevTerm) = await ProjectFunctions.GetStartDateTerm(investment.ProjectId);
            DateTime effectiveDate = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(1);
            investment.EffectiveDate = effectiveDate < startDate ? startDate : effectiveDate;
            DateTime endDate = startDate.AddMonths(projectDevTerm).AddDays(-1);
            investment.DevelopmentTerm = ProjectFunctions.CalculateInvestmentTerm(investment.EffectiveDate, endDate);

            investment.DiscountRate = await ProjectFunctions.GetDiscountRate(investment.ProjectId, investment.ProductTypeId, investment.CpiCount);
            double discountMonthRate = investment.DiscountRate / 12d;
            double installmentAmount = (investment.TotalAmount - investment.ReserveAmount) / (investment.DevelopmentTerm - 1);
            amounts.InstallmentAmount = installmentAmount.RoundAmount();

            investment.ReserveAmount = (investment.TotalAmount * fractionatedReserveRate).RoundAmount();
            amounts.ReserveDiscount = investment.ReserveAmount * discountMonthRate * investment.DevelopmentTerm;

            double baseTotalDiscount = amounts.ReserveDiscount + installmentAmount * (investment.DevelopmentTerm - 2) * discountMonthRate * (investment.DevelopmentTerm + 1) / 2d;
            double baseDueAmount = investment.TotalAmount - baseTotalDiscount;

            amounts.FullTerm = (int)Math.Floor((baseDueAmount - investment.ReserveAmount) / amounts.InstallmentAmount);
            double fullAmount = (investment.ReserveAmount + amounts.InstallmentAmount * amounts.FullTerm).RoundAmount();
            double fullDiscount = (amounts.ReserveDiscount + amounts.InstallmentAmount * discountMonthRate * (amounts.FullTerm * (2d * (investment.DevelopmentTerm - 1) - amounts.FullTerm + 1) / 2d)).RoundAmount();

            double lastTerm = investment.DevelopmentTerm - 1 - amounts.FullTerm;
            amounts.LastAmount = ((investment.TotalAmount - fullAmount - fullDiscount) / (1 + discountMonthRate * lastTerm)).RoundAmount();
            amounts.LastDiscount = investment.TotalAmount - fullAmount - fullDiscount - amounts.LastAmount; //amounts.LastAmount * discountMonthRate * lastTerm;

            investment.DueAmount = fullAmount + amounts.LastAmount;
            investment.DiscountAmount = investment.TotalAmount - investment.DueAmount;
            investment.Balance = reservePaid ? investment.DueAmount - investment.ReserveAmount : investment.DueAmount;

            return (investment, amounts);
        }

        public static async Task<int> Add(InvestmentFractionated investmentFractionated)
        {
            return await new InvestmentFractionatedDB().Add(investmentFractionated);
        }

        // UPDATE
        public static async Task<bool> Update(InvestmentFractionated investmentFractionated)
        {
            return await new InvestmentFractionatedDB().Update(investmentFractionated);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentFractionatedDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByInvestmentId(int investmentId, int status)
        {
            return await new InvestmentFractionatedDB().UpdateStatusByInvestmentId(investmentId, status);
        }

        // DELETE
        public static async Task DeleteByInvestmentId(int investmentId, int projectId, int cpiCount)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (await new InvestmentFractionatedDB().DeleteByInvestmentId(investmentId))
                {
                    int projectCpiCount = await ProjectFunctions.GetCpiCount(projectId) - cpiCount;
                    await ProjectFunctions.UpdateCpiCount(projectId, projectCpiCount);
                }

                scope.Complete();
            }
        }
    }
}
