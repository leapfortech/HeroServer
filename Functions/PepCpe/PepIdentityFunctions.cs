using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class PepIdentityFunctions
    {
        // GET
        public static async Task<PepIdentity> GetById(int id)
        {
            return await new PepIdentityDB().GetById(id);
        }

        public static async Task<IEnumerable<PepIdentityAppUser>> GetByAppUserId(int appUserId, int status = 1)
        {
            return await new PepIdentityAppUserDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<IEnumerable<PepIdentityInvestmentIdentity>> GetByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            return await new PepIdentityInvestmentIdentityDB().GetByInvestmentIdentityId(investmentIdentityId, status);
        }

        // ADD
        public static async Task<int> Add(PepIdentity pepIdentity)
        {
            return await new PepIdentityDB().Add(pepIdentity);
        }

        public static async Task<List<int>> RegisterByAppUser(int appUserId, PepIdentityRequest[] pepIdentitiesRequests)
        {
            List<int> pepIdentityIds = [];
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new PepIdentityAppUserDB().UpdateStatusByAppUserId(appUserId, 1, 0);

                int pepIdentityId;
                for (int i = 0; i < pepIdentitiesRequests.Length; i++)
                {
                    pepIdentitiesRequests[i].Identity.Status = 1;
                    int identityId = await IdentityFunctions.Add(pepIdentitiesRequests[i].Identity);

                    pepIdentitiesRequests[i].PepIdentity.IdentityId = identityId;
                    pepIdentityId = await Add(pepIdentitiesRequests[i].PepIdentity);
                    await new PepIdentityAppUserDB().Add(new PepIdentityAppUser(-1, appUserId, pepIdentityId, 1));
                    pepIdentityIds.Add(pepIdentityId);
                }

                scope.Complete();
            }

            return pepIdentityIds;
        }

        public static async Task<List<int>> RegisterByInvestmentIdentity(int investmentIdentityId, PepIdentityRequest[] pepIdentitiesRequests)
        {
            List<int> pepIdentityIds = [];
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new PepIdentityInvestmentIdentityDB().UpdateStatusByInvestmentIdentityId(investmentIdentityId, 1, 0);

                int pepIdentityId;
                for (int i = 0; i < pepIdentitiesRequests.Length; i++)
                {
                    pepIdentitiesRequests[i].Identity.Status = 1;
                    int identityId = await IdentityFunctions.Add(pepIdentitiesRequests[i].Identity);

                    pepIdentitiesRequests[i].PepIdentity.IdentityId = identityId;
                    pepIdentityId = await Add(pepIdentitiesRequests[i].PepIdentity);

                    await new PepIdentityInvestmentIdentityDB().Add(new PepIdentityInvestmentIdentity(-1, investmentIdentityId, pepIdentityId, 1));
                    pepIdentityIds.Add(pepIdentityId);
                }

                scope.Complete();
            }

            return pepIdentityIds;
        }

        // UPDATE
    }
}