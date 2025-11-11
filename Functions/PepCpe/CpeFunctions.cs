using System;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class CpeFunctions
    {
        // GET
        public static async Task<Cpe> GetById(int id)
        {
            return await new CpeDB().GetById(id);
        }

        public static async Task<Cpe> GetByAppUserId(int appUserId, int status = 1)
        {
            int cpeId = await GetIdByAppUserId(appUserId, status);

            return await new CpeDB().GetById(cpeId);
        }

        public static async Task<Cpe> GetByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            int cpeId = await GetIdByInvestmentIdentityId(investmentIdentityId, status);

            return await new CpeDB().GetById(cpeId);
        }

        public static async Task<int> GetIdByAppUserId(int appUserId, int status = 1)
        {
            return await new CpeAppUserDB().GetIdByAppUserId(appUserId, status);
        }


        public static async Task<int> GetIdByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            return await new CpeInvestmentIdentityDB().GetIdByInvestmentIdentityId(investmentIdentityId, status);
        }

        // ADD
        public static async Task<int> Add(Cpe cpe)
        {
            return await new CpeDB().Add(cpe);
        }

        public static async Task<int> RegisterByAppUser(int appUserId, Cpe cpe)
        {
            int cpeId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new CpeAppUserDB().UpdateStatusByAppUserId(appUserId, 1, 0);

                cpeId = await new CpeDB().Add(cpe);

                await new CpeAppUserDB().Add(new CpeAppUser(-1, appUserId, cpeId, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return cpeId;
        }

        public static async Task<int> RegisterByInvestmentIdentity(int investmentIdentityId, Cpe cpe)
        {
            int cpeId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new CpeInvestmentIdentityDB().UpdateStatusByInvestmentIdentityId(investmentIdentityId, 1, 0);

                cpeId = await new CpeDB().Add(cpe);

                await new CpeInvestmentIdentityDB().Add(new CpeInvestmentIdentity(-1, investmentIdentityId, cpeId, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return cpeId;
        }

        // UPDATE
    }
}