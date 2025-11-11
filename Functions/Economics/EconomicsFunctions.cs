using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class EconomicsFunctions
    {
        // GET
        public static async Task<Economics> GetByInvestmentId(int investmentId)
        {
            return await new EconomicsDB().GetByInvestmentId(investmentId);
        }

        public static async Task<EconomicsInfo> GetInfoByInvestmentId(int investmentId)
        {
            Economics economics = await GetByInvestmentId(investmentId);
            List<Income> incomes = await new IncomeDB().GetByInvestmentId(investmentId, 1);
            List<String> docIncomes = await InvestmentFunctions.GetDocs(investmentId, "docincome");

            return new EconomicsInfo(economics, incomes, docIncomes);
        }

        // REGISTER
        public static async Task<List<int>> Register(EconomicsInfo economicsInfo)
        {
            List<int> incomeIds = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Economics
                await UpdateStatusByInvestmentId(economicsInfo.Incomes[0].InvestmentId, 1, 0);
                await IncomeFunctions.UpdateStatusByInvestmentId(economicsInfo.Incomes[0].InvestmentId, 1, 0);

                economicsInfo.Economics.Status = 1;

                int economicsId = await Add(economicsInfo.Economics);

                // Incomes
                int incomeId;
                for (int i = 0; i < economicsInfo.Incomes.Count; i++)
                {
                    economicsInfo.Incomes[i].Status = 1;
                    incomeId = await IncomeFunctions.Add(economicsInfo.Incomes[i]);
                    incomeIds.Add(incomeId);
                }

                // DocIncomes
                await InvestmentFunctions.RegisterDocs(economicsInfo.Economics.InvestmentId, "docincome", economicsInfo.DocIncomes);

                // Investment Status
                await new InvestmentDB().UpdateStatus(economicsInfo.Economics.InvestmentId, 5);

                scope.Complete();
            }

            return incomeIds;
        }

        // ADD
        public static async Task<int> Add(Economics economics)
        {
            return await new EconomicsDB().Add(economics);
        }

        // UPDATE
        public static async Task<bool> Update(Economics economics)
        {
            return await new EconomicsDB().Update(economics);
        }

        public static async Task<List<int>> Update(EconomicsInfo economicsInfo)
        {
            List<int> incomeIds = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Economics
                await UpdateStatusByInvestmentId(economicsInfo.Economics.InvestmentId, 1, 0);

                economicsInfo.Economics.Status = 1;
                int economicsId = await Add(economicsInfo.Economics);

                // Incomes
                await IncomeFunctions.UpdateStatusByInvestmentId(economicsInfo.Incomes[0].InvestmentId, 1, 0);

                int incomeId;
                for (int i = 0; i < economicsInfo.Incomes.Count; i++)
                {
                    economicsInfo.Incomes[i].Status = 1;
                    incomeId = await IncomeFunctions.Add(economicsInfo.Incomes[i]);
                    incomeIds.Add(incomeId);
                }

                // DocIncomes
                await InvestmentFunctions.RegisterDocs(economicsInfo.Economics.InvestmentId, "docincome", economicsInfo.DocIncomes);

                // Investment Status
                await new InvestmentDB().UpdateMotive(economicsInfo.Economics.InvestmentId, -1, 0, null);

                scope.Complete();
            }

            await InvestmentFunctions.SendUpdateMessage(economicsInfo.Economics.InvestmentId);

            return incomeIds;
        }

        public static async Task<bool> UpdateStatusByInvestmentId(int investmentId, int curStatus, int newStatus)
        {
            return await new EconomicsDB().UpdateStatusByInvestmentId(investmentId, curStatus, newStatus);
        }

        //DELETE
        public static async Task DeleteByInvestmentId(int investmentId)
        {
            await new EconomicsDB().DeleteByInvestmentId(investmentId);
        }
    }
}
