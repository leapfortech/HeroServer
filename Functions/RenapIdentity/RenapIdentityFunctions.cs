using System.Threading.Tasks;

namespace HeroServer
{
    public class RenapIdentityFunctions
    {
        // GET
        public static async Task<RenapIdentity> GetById(int id)
        {
            return await new RenapIdentityDB().GetById(id);
        }

        public static async Task<RenapIdentity> GetByAppUserId(int appUserId)
        {
            return await new RenapIdentityDB().GetByAppUserId(appUserId);
        }

        // ADD
        public static async Task<int> Add(RenapIdentity renapIdentity)
        {
            return await new RenapIdentityDB().Add(renapIdentity);
        }

        // UPDATE
        public static async Task<bool> Update(RenapIdentity renapIdentity)
        {
            return await new RenapIdentityDB().Update(renapIdentity);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new RenapIdentityDB().UpdateStatus(id, status);
        }

        // DELETE
        public static async Task DeleteByAppUserId(int appUserId)
        {
            await new RenapIdentityDB().DeleteByAppUserId(appUserId);
        }
    }
}
