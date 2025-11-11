using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public static class InvestmentIdentityFunctions
    {
        // GET
        public static async Task<InvestmentIdentity> GetById(int id)
        {
            return await new InvestmentIdentityDB().GetById(id);
        }

        public static async Task<List<InvestmentIdentity>> GetByInvestmentId(int investmentIdentityId, int status = 1)
        {
            return await new InvestmentIdentityDB().GetByInvestmentId(investmentIdentityId, status);
        }

        public static async Task<List<int>> GetIdentityIdsByInvestmentId(int investmentIdentityId, int status = 1)
        {
            return await new InvestmentIdentityDB().GetIdentityIdsByInvestmentId(investmentIdentityId, status);
        }

        // Register
        public static async Task<List<int>> Register(InvestmentIdentityRequest[] investmentIdentityRequests)
        {
            List<int> investmentIdentityIds = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int investmentIdentityId;
                for (int i = 0; i < investmentIdentityRequests.Length; i++)
                {
                    int identityId = await IdentityFunctions.RegisterInvestment(investmentIdentityRequests[i].Identity);

                    investmentIdentityRequests[i].InvestmentIdentity.IdentityId = identityId;
                    investmentIdentityRequests[i].InvestmentIdentity.Status = 1;
                    investmentIdentityId = await new InvestmentIdentityDB().Add(investmentIdentityRequests[i].InvestmentIdentity);
                    investmentIdentityIds.Add(investmentIdentityId);

                    await AddressFunctions.RegisterByInvestmentIdentity(investmentIdentityId, investmentIdentityRequests[i].Address);

                    if (investmentIdentityRequests[i].Identity.IsPep == 1)
                        await PepFunctions.RegisterByInvestmentIdentity(investmentIdentityId, investmentIdentityRequests[i].Pep);

                    if(investmentIdentityRequests[i].Identity.HasPepIdentity == 1)
                        await PepIdentityFunctions.RegisterByInvestmentIdentity(investmentIdentityId, investmentIdentityRequests[i].PepIdentityRequests);

                    if (investmentIdentityRequests[i].Identity.IsCpe == 1)
                        await CpeFunctions.RegisterByInvestmentIdentity(investmentIdentityId, investmentIdentityRequests[i].Cpe);
                }
                scope.Complete();
            }

            return investmentIdentityIds;
        }

        // ADD
        public static async Task<int> Add(InvestmentIdentity investmentIdentity)
        {
            return await new InvestmentIdentityDB().Add(investmentIdentity);
        }

        // UPDATE
        public static async Task<bool> Update(InvestmentIdentity investmentIdentity)
        {
            return await new InvestmentIdentityDB().Update(investmentIdentity);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentIdentityDB().UpdateStatus(id, status);
        }

        //DELETE
        public static async Task DeleteByInvestmentId(int investmentId)
        {
            List<int> identityIds = await GetIdentityIdsByInvestmentId(investmentId);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                for (int i = 0; i < identityIds.Count; i++)
                    await IdentityFunctions.Delete(identityIds[i]);
                await new InvestmentIdentityDB().DeleteByInvestmentId(investmentId);

                scope.Complete();
            }
        }
    }
}
