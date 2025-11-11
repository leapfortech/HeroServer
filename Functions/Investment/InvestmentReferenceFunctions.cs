using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public static class InvestmentReferenceFunctions
    {
        // GET
        public static async Task<InvestmentReference> GetById(int id)
        {
            return await new InvestmentReferenceDB().GetById(id);
        }

        public static async Task<IEnumerable<InvestmentReference>> GetByAppUserId(int appUserId, int status = 1)
        {
            return await new InvestmentReferenceDB().GetByAppUserId(appUserId, status);
        }

        // Register
        public static async Task<List<int>> Register(InvestmentReference[] investmentReferences)
        {
            List<int> investmentReferenceIds = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int investmentReferenceId;
                for (int i = 0; i < investmentReferences.Length; i++)
                {
                    investmentReferences[i].Status = 1;
                    investmentReferenceId = await new InvestmentReferenceDB().Add(investmentReferences[i]);
                    investmentReferenceIds.Add(investmentReferenceId);
                }
                scope.Complete();
            }

            return investmentReferenceIds;
        }

        // ADD
        public static async Task<int> Add(InvestmentReference investmentReference)
        {
            return await new InvestmentReferenceDB().Add(investmentReference);
        }

        // UPDATE
        public static async Task<bool> Update(InvestmentReference investmentReference)
        {
            return await new InvestmentReferenceDB().Update(investmentReference);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentReferenceDB().UpdateStatus(id, status);
        }

        //DELETE
        public static async Task DeleteByInvestmentId(int investmentId)
        {
            await new InvestmentReferenceDB().DeleteByInvestmentId(investmentId);
        }
    }
}
