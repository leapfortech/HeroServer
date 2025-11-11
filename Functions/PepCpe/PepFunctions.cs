using System;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class PepFunctions
    {
        // GET
        public static async Task<Pep> GetById(int id)
        {
            return await new PepDB().GetById(id);
        }

        public static async Task<Pep> GetByAppUserId(int appUserId, int status = 1)
        {
            int pepId = await GetIdByAppUserId(appUserId, status);

            return await new PepDB().GetById(pepId);
        }

        public static async Task<Pep> GetByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            int pepId = await GetIdByInvestmentIdentityId(investmentIdentityId, status);

            return await new PepDB().GetById(pepId);
        }

        public static async Task<int> GetIdByAppUserId(int appUserId, int status = 1)
        {
            return await new PepAppUserDB().GetIdByAppUserId(appUserId, status);
        }


        public static async Task<int> GetIdByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            return await new PepInvestmentIdentityDB().GetIdByInvestmentIdentityId(investmentIdentityId, status);
        }

        // ADD
        public static async Task<int> Add(Pep pep)
        {
            return await new PepDB().Add(pep);
        }

        public static async Task<int> RegisterByAppUser(int appUserId, Pep pep)
        {
            int pepId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new PepAppUserDB().UpdateStatusByAppUserId(appUserId, 1, 0);

                pepId = await new PepDB().Add(pep);

                await new PepAppUserDB().Add(new PepAppUser(-1, appUserId, pepId, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return pepId;
        }

        public static async Task<int> RegisterByInvestmentIdentity(int investmentIdentityId, Pep pep)
        {
            int pepId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new PepInvestmentIdentityDB().UpdateStatusByInvestmentIdentityId(investmentIdentityId, 1, 0);

                pepId = await new PepDB().Add(pep);

                await new PepInvestmentIdentityDB().Add(new PepInvestmentIdentity(-1, investmentIdentityId, pepId, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return pepId;
        }

        // UPDATE
    }
}