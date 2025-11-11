using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public static class InvestmentFinancedFunctions
    {
        // GET
        public static async Task<InvestmentFinanced> GetById(int id)
        {
            return await new InvestmentFinancedDB().GetById(id);
        }

        public static async Task<InvestmentFinanced> GetByInvestmentId(int investmentId, int status = 1)
        {
            return await new InvestmentFinancedDB().GetByInvestmentId(investmentId, status);
        }

        public static async Task<InvestmentFinanced> GetByProductFinancedId(int productFinancedId, int status = 1)
        {
            return await new InvestmentFinancedDB().GetByProductFinancedId(productFinancedId, status);
        }

        // FULL
        public static async Task<InvestmentFinancedFull> GetFullById(int id)
        {
            return await new InvestmentFinancedDB().GetFullById(id);
        }

        public static async Task<InvestmentFinancedFull> GetFullByInvestmentId(int investmentId)
        {
            return await new InvestmentFinancedDB().GetFullByInvestmentId(investmentId);
        }

        public static async Task<List<InvestmentFinancedFull>> GetFullsByStatus(int status)
        {
            InvestmentFinancedDataFull investmentFinancedDataFull = await new InvestmentFinancedDB().GetDataFullByStatus(status);

            return GetFulls(investmentFinancedDataFull);
        }

        public static async Task<List<InvestmentFinancedFull>> GetFullsByAppUserId(int appUserId, int status = -1)
        {
            InvestmentFinancedDataFull investmentFinancedDataFull = await new InvestmentFinancedDB().GetDataFullByAppUserId(appUserId, status);

            return GetFulls(investmentFinancedDataFull);
        }

        public static List<InvestmentFinancedFull> GetFulls(InvestmentFinancedDataFull investmentFinancedDataFull)
        {
            // InvestmentPayment
            Dictionary<int, List<InvestmentPayment>> investmentPaymentsDict = [];
            foreach (InvestmentPayment investmentPayment in investmentFinancedDataFull.InvestmentPayments)
            {
                if (investmentPaymentsDict.TryGetValue(investmentPayment.InvestmentId, out List<InvestmentPayment> value))
                    value.Add(investmentPayment);
                else
                    investmentPaymentsDict[investmentPayment.InvestmentId] = [investmentPayment];
            }

            // BankTransaction
            Dictionary<int, BankTransaction> bankTransactionsDict = [];
            foreach (BankTransaction bankTransaction in investmentFinancedDataFull.BankTransactions)
            {
                if (bankTransactionsDict.TryGetValue(bankTransaction.Id, out BankTransaction value))
                    throw new Exception($"Duplicate BankTransactionId {bankTransaction.Id}");

                bankTransactionsDict[bankTransaction.Id] = bankTransaction;
            }

            // InvestmentInstallment
            Dictionary<int, List<InvestmentInstallment>> investmentInstallmentsDict = [];
            foreach (InvestmentInstallment investmentInstallment in investmentFinancedDataFull.InvestmentInstallments)
            {
                if (investmentInstallmentsDict.TryGetValue(investmentInstallment.InvestmentId, out List<InvestmentInstallment> value))
                    value.Add(investmentInstallment);
                else
                    investmentInstallmentsDict[investmentInstallment.InvestmentId] = [investmentInstallment];
            }

            // InvestmentInstallmentPayment
            Dictionary<int, List<InvestmentInstallmentPayment>> investmentInstallmentPaymentsDict = [];
            foreach (InvestmentInstallmentPayment investmentInstallmentPayment in investmentFinancedDataFull.InvestmentInstallmentPayments)
            {
                if (investmentInstallmentPaymentsDict.TryGetValue(investmentInstallmentPayment.InvestmentInstallmentId, out List<InvestmentInstallmentPayment> value))
                    value.Add(investmentInstallmentPayment);
                else
                    investmentInstallmentPaymentsDict[investmentInstallmentPayment.InvestmentInstallmentId] = [investmentInstallmentPayment];
            }

            // InvestmentFinancedFull
            List<InvestmentFinancedFull> investmentFinancedFulls = [];
            foreach (InvestmentFinancedFull investFinancedFull in investmentFinancedDataFull.InvestmentFinancedFulls)
            {
                if (!investmentPaymentsDict.TryGetValue(investFinancedFull.InvestmentId, out List<InvestmentPayment> payments))
                    payments = [];

                investFinancedFull.InvestmentBankPayments = [];
                foreach (InvestmentPayment payment in payments)
                {
                    if (!bankTransactionsDict.TryGetValue(payment.TransactionId, out BankTransaction bankTransaction))
                        throw new Exception($"Transaction #{payment.TransactionId} not Found for InvestmentPayment #{payment.Id}");
                    investFinancedFull.InvestmentBankPayments.Add(new InvestmentBankPayment(payment, bankTransaction, null));
                }

                if (!investmentInstallmentsDict.TryGetValue(investFinancedFull.InvestmentId, out List<InvestmentInstallment> installments))
                    installments = [];

                investFinancedFull.InvestmentInstallmentInfos = [];
                foreach (InvestmentInstallment installment in installments)
                {
                    if (!investmentInstallmentPaymentsDict.TryGetValue(installment.Id, out List<InvestmentInstallmentPayment> installmentPayments))
                        installmentPayments = [];
                    investFinancedFull.InvestmentInstallmentInfos.Add(new InvestmentInstallmentInfo(installment, installmentPayments));
                }

                investmentFinancedFulls.Add(investFinancedFull);
            }

            return investmentFinancedFulls;
        }

        // ADD
        public static async Task<InvestmentFinancedFull> Register(Investment investment)
        {
            if (investment.ProductTypeId != 2)
                throw new Exception("Error de Tipo de Producto");

            int productFinancedId = await ProductFinancedFunctions.GetIdByProjectId(investment.ProjectId);
            if (productFinancedId == -1)
                throw new Exception("No se encontró el Producto");

            investment = await RefreshAmounts(investment, false);

            int id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                investment.BoardUserId = -1;
                investment.InvestmentStatusId = 0;
                investment.Id = await InvestmentFunctions.Add(investment);

                InvestmentFinanced investmentFinanced = new InvestmentFinanced(investment.Id, productFinancedId);
                id = await Add(investmentFinanced);

                await InvestmentInstallmentFunctions.RegisterReserve(investment, 0d);

                scope.Complete();
            }

            return await GetFullById(id);
        }

        public static async Task<Investment> RefreshAmounts(Investment investment, bool reservePaid)
        {
            (double financedReserveRate, double financedAdvRate) = await ProductFinancedFunctions.GetRatesByProjectId(investment.ProjectId);

            DateTime startDate = await ProjectFunctions.GetStartDate(investment.ProjectId);
            DateTime effectiveDate = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(1);
            investment.EffectiveDate = effectiveDate < startDate ? startDate : effectiveDate;

            investment.DiscountRate = 0d;
            investment.DueAmount = (investment.TotalAmount * financedAdvRate).RoundAmount();
            investment.DiscountAmount = 0d;
            investment.ReserveAmount = (investment.TotalAmount * financedReserveRate).RoundAmount();
            investment.Balance = reservePaid ? investment.DueAmount - investment.ReserveAmount : investment.DueAmount;

            return investment;
        }

        public static async Task<int> Register(InvestmentFinanced investmentFinanced)
        {
            return await Add(investmentFinanced);
        }

        public static async Task<int> Add(InvestmentFinanced investmentFinanced)
        {
            return await new InvestmentFinancedDB().Add(investmentFinanced);
        }

        // UPDATE
        public static async Task<bool> Update(InvestmentFinanced investmentFinanced)
        {
            return await new InvestmentFinancedDB().Update(investmentFinanced);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentFinancedDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByInvestmentId(int investmentId, int status)
        {
            return await new InvestmentFinancedDB().UpdateStatusByInvestmentId(investmentId, status);
        }

        // DELETE
        public static async Task DeleteByInvestmentId(int investmentId, int projectId, int cpiCount)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (await new InvestmentFinancedDB().DeleteByInvestmentId(investmentId))
                {
                    int projectCpiCount = await ProjectFunctions.GetCpiCount(projectId) - cpiCount;
                    await ProjectFunctions.UpdateCpiCount(projectId, projectCpiCount);
                }

                scope.Complete();
            }
        }
    }
}
