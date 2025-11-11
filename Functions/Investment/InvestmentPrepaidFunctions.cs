using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public static class InvestmentPrepaidFunctions
    {
        // GET
        public static async Task<InvestmentPrepaid> GetById(int id)
        {
            return await new InvestmentPrepaidDB().GetById(id);
        }

        public static async Task<InvestmentPrepaid> GetByInvestmentId(int investmentId, int status = 1)
        {
            return await new InvestmentPrepaidDB().GetByInvestmentId(investmentId, status);
        }

        public static async Task<InvestmentPrepaid> GetByProductPrepaidId(int productPrepaidId, int status = 1)
        {
            return await new InvestmentPrepaidDB().GetByProductPrepaidId(productPrepaidId, status);
        }

        // FULL
        public static async Task<InvestmentPrepaidFull> GetFullById(int id)
        {
            return await new InvestmentPrepaidDB().GetFullById(id);
        }

        public static async Task<InvestmentPrepaidFull> GetFullByInvestmentId(int investmentId)
        {
            return await new InvestmentPrepaidDB().GetFullByInvestmentId(investmentId);
        }

        public static async Task<List<InvestmentPrepaidFull>> GetFullsByStatus(int status)
        {
            InvestmentPrepaidDataFull investmentPrepaidDataFull = await new InvestmentPrepaidDB().GetFullsByStatus(status);

            return GetFulls(investmentPrepaidDataFull);
        }

        public static async Task<List<InvestmentPrepaidFull>> GetFullsByAppUserId(int appUserId, int status = -1)
        {
            InvestmentPrepaidDataFull investmentPrepaidDataFull = await new InvestmentPrepaidDB().GetFullsByAppUserId(appUserId, status);

            return GetFulls(investmentPrepaidDataFull);
        }

        public static List<InvestmentPrepaidFull> GetFulls(InvestmentPrepaidDataFull investmentPrepaidDataFull)
        {
            // InvestmentPayment
            Dictionary<int, List<InvestmentPayment>> investmentPaymentsDict = [];
            foreach (InvestmentPayment investmentPayment in investmentPrepaidDataFull.InvestmentPayments)
            {
                if (investmentPaymentsDict.TryGetValue(investmentPayment.InvestmentId, out List<InvestmentPayment> value))
                    value.Add(investmentPayment);
                else
                    investmentPaymentsDict[investmentPayment.InvestmentId] = [investmentPayment];
            }

            // BankTransaction
            Dictionary<int, BankTransaction> bankTransactionsDict = [];
            foreach (BankTransaction bankTransaction in investmentPrepaidDataFull.BankTransactions)
            {
                if (bankTransactionsDict.TryGetValue(bankTransaction.Id, out BankTransaction value))
                    throw new Exception($"Duplicate BankTransactionId {bankTransaction.Id}");

                bankTransactionsDict[bankTransaction.Id] = bankTransaction;
            }

            // InvestmentInstallment
            Dictionary<int, List<InvestmentInstallment>> investmentInstallmentsDict = [];
            foreach (InvestmentInstallment investmentInstallment in investmentPrepaidDataFull.InvestmentInstallments)
            {
                if (investmentInstallmentsDict.TryGetValue(investmentInstallment.InvestmentId, out List<InvestmentInstallment> value))
                    value.Add(investmentInstallment);
                else
                    investmentInstallmentsDict[investmentInstallment.InvestmentId] = [investmentInstallment];
            }

            // InvestmentInstallmentPayment
            Dictionary<int, List<InvestmentInstallmentPayment>> investmentInstallmentPaymentsDict = [];
            foreach (InvestmentInstallmentPayment investmentInstallmentPayment in investmentPrepaidDataFull.InvestmentInstallmentPayments)
            {
                if (investmentInstallmentPaymentsDict.TryGetValue(investmentInstallmentPayment.InvestmentInstallmentId, out List<InvestmentInstallmentPayment> value))
                    value.Add(investmentInstallmentPayment);
                else
                    investmentInstallmentPaymentsDict[investmentInstallmentPayment.InvestmentInstallmentId] = [investmentInstallmentPayment];
            }

            // InvestmentPrepaidFull
            List<InvestmentPrepaidFull> investmentPrepaidFulls = [];
            foreach (InvestmentPrepaidFull investPrepaidFull in investmentPrepaidDataFull.InvestmentPrepaidFulls)
            {
                if (!investmentPaymentsDict.TryGetValue(investPrepaidFull.InvestmentId, out List<InvestmentPayment> payments))
                    payments = [];

                investPrepaidFull.InvestmentBankPayments = [];
                foreach (InvestmentPayment payment in payments)
                {
                    if (!bankTransactionsDict.TryGetValue(payment.TransactionId, out BankTransaction bankTransaction))
                        throw new Exception($"Transaction #{payment.TransactionId} not Found for InvestmentPayment #{payment.Id}");
                    investPrepaidFull.InvestmentBankPayments.Add(new InvestmentBankPayment(payment, bankTransaction, null));
                }

                if (!investmentInstallmentsDict.TryGetValue(investPrepaidFull.InvestmentId, out List<InvestmentInstallment> installments))
                    installments = [];

                investPrepaidFull.InvestmentInstallmentInfos = [];
                foreach (InvestmentInstallment installment in installments)
                {
                    if (!investmentInstallmentPaymentsDict.TryGetValue(installment.Id, out List<InvestmentInstallmentPayment> installmentPayments))
                        installmentPayments = [];
                    investPrepaidFull.InvestmentInstallmentInfos.Add(new InvestmentInstallmentInfo(installment, installmentPayments));
                }

                investmentPrepaidFulls.Add(investPrepaidFull);
            }

            return investmentPrepaidFulls;
        }

        // ADD
        public static async Task<InvestmentPrepaidFull> Register(Investment investment)
        {
            if (investment.ProductTypeId != 3)
                throw new Exception("Error de Tipo de Producto");

            int productPrepaidId = await ProductPrepaidFunctions.GetIdByProjectId(investment.ProjectId);
            if (productPrepaidId == -1)
                throw new Exception("No se encontró el Producto");

            investment = await RefreshAmounts(investment, false);

            int id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                investment.BoardUserId = -1;
                investment.InvestmentStatusId = 0;
                investment.Id = await InvestmentFunctions.Add(investment);

                InvestmentPrepaid investmentPrepaid = new InvestmentPrepaid(investment.Id, productPrepaidId);
                id = await Add(investmentPrepaid);

                await InvestmentInstallmentFunctions.RegisterReserve(investment, 0d);

                scope.Complete();
            }

            return await GetFullById(id);
        }

        public static async Task<Investment> RefreshAmounts(Investment investment, bool reservePaid)
        {
            double prepaidReserveRate = await ProductPrepaidFunctions.GetReserveRateByProjectId(investment.ProjectId);

            DateTime startDate = await ProjectFunctions.GetStartDate(investment.ProjectId);
            DateTime effectiveDate = DateTime.Today.AddDays(1);
            investment.EffectiveDate = effectiveDate < startDate ? startDate : effectiveDate;

            investment.DiscountRate = await ProjectFunctions.GetDiscountRate(investment.ProjectId, investment.ProductTypeId, investment.CpiCount);
            investment.DueAmount = (investment.TotalAmount / (1 + investment.DiscountRate / 12.0d * investment.DevelopmentTerm)).RoundAmount();
            investment.DiscountAmount = investment.TotalAmount - investment.DueAmount;
            investment.ReserveAmount = (investment.DueAmount * prepaidReserveRate).RoundAmount();
            investment.Balance = reservePaid ? investment.DueAmount - investment.ReserveAmount : investment.DueAmount;

            return investment;
        }

        public static async Task<int> Register(InvestmentPrepaid investmentPrepaid)
        {
            return await Add(investmentPrepaid);
        }

        public static async Task<int> Add(InvestmentPrepaid investmentPrepaid)
        {
            return await new InvestmentPrepaidDB().Add(investmentPrepaid);
        }

        // UPDATE
        public static async Task<bool> Update(InvestmentPrepaid investmentPrepaid)
        {
            return await new InvestmentPrepaidDB().Update(investmentPrepaid);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentPrepaidDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByInvestmentId(int investmentId, int status)
        {
            return await new InvestmentPrepaidDB().UpdateStatusByInvestmentId(investmentId, status);
        }

        // DELETE
        public static async Task DeleteByInvestmentId(int investmentId, int projectId, int cpiCount)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (await new InvestmentPrepaidDB().DeleteByInvestmentId(investmentId))
                {
                    int projectCpiCount = await ProjectFunctions.GetCpiCount(projectId) - cpiCount;
                    await ProjectFunctions.UpdateCpiCount(projectId, projectCpiCount);
                }

                scope.Complete();
            }
        }
    }
}
