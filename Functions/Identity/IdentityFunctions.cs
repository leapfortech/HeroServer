using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class IdentityFunctions
    {
        // GET
        public static async Task<List<Identity>> GetAll(int status)
        {
            return await new IdentityDB().GetAll(status);
        }

        public static async Task<List<IdentityFull>> GetFullAll(int status)
        {
            return await new IdentityDB().GetFullAll(status);
        }

        public static async Task<Identity> GetById(int id)
        {
            return await new IdentityDB().GetById(id);
        }

        public static async Task<Identity> GetByAppUserId(int appUserId, int status)
        {
            return await new IdentityDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<IdentityFull> GetFullByAppUserId(int appUserId, int status)
        {
            return await new IdentityDB().GetFullByAppUserId(appUserId, status);
        }

        public static async Task<int> GetIdByAppUserId(int appUserId, int status = 1)
        {
            return await new IdentityDB().GetIdByAppUserId(appUserId, status);
        }

        public static async Task<String> GetPortraitByAppUserId(int appUserId)
        {
            String portrait = null;
            
            byte[] portraitImg = await StorageFunctions.ReadFile($"user{appUserId:D08}", $"prt{appUserId:D08}", "jpg");

            if (portraitImg != null)
                portrait = Convert.ToBase64String(portraitImg);

            return portrait;
        }

        public static async Task<List<Identity>> GetAllByAppUserId(int appUserId, int status)
        {
            return await new IdentityDB().GetAllByAppUserId(appUserId, status);
        }

        // REGISTER
        public static async Task<int> Register(IdentityRegister identityRegister)
        {
            int identityId;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new IdentityDB().UpdateStatusByAppUserId(identityRegister.Identity.AppUserId, 1, 0);

                identityRegister.Identity.Status = 1;
                identityId = await new IdentityDB().Add(identityRegister.Identity);
                identityRegister.Identity.Id = identityId;

                // Portrait
                String appUserId = identityRegister.Identity.AppUserId.ToString("D08");
                String containerName = "user" + appUserId;
                await StorageFunctions.CreateContainer(containerName);

                if (identityRegister.Portrait.Length > 0)
                    await StorageFunctions.UpdateFile(containerName, "prt" + appUserId, "jpg", Convert.FromBase64String(identityRegister.Portrait));

                // AppUser Status
                await AppUserFunctions.UpdateStatus(identityRegister.Identity.AppUserId, 2);

                scope.Complete();
            }

            return identityRegister.Identity.Id;
        }

        public static async Task<int> RegisterInvestment(Identity identity)
        {
            int identityId;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                identity.Status = 1;
                identityId = await new IdentityDB().Add(identity);
                identity.Id = identityId;

                scope.Complete();
            }

            return identity.Id;
        }

        // ADD
        public static async Task<int> Add(Identity identity)
        {
            return await new IdentityDB().Add(identity);
        }

        public static async Task<int> Copy(int id, int status = -1)
        {
            Identity identity = await new IdentityDB().GetById(id);
            if (status != -1)
                identity.Status = status;
            return await new IdentityDB().Add(identity);
        }

        public static async Task<int> CopyByAppUserId(int appUserId, int status = -1)
        {
            Identity identity = await new IdentityDB().GetByAppUserId(appUserId);
            if (status != -1)
                identity.Status = status;
            return await new IdentityDB().Add(identity);
        }

        // UPDATE
        public static async Task<int> Update(Identity identity)
        {
            int identityId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (identity.Status == 1)
                {
                    await new IdentityDB().UpdateStatusByAppUserId(identity.AppUserId, 1, 0);

                    identityId = await new IdentityDB().Add(identity);
                }
                else if (identity.Status == 2)
                {
                    identityId = identity.Id;

                    await new IdentityDB().Update(identity);
                }

                scope.Complete();
            }

            return identityId;
        }

        public static async Task UpdatePortrait(int appUserId, String portrait)
        {
            if (String.IsNullOrEmpty(portrait))
                throw new ArgumentException("No Data to Update.");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                String id = appUserId.ToString("D08");
                String containerName = "user" + id;

                await StorageFunctions.UpdateCFile(containerName, "prt" + id, "jpg", Convert.FromBase64String(portrait));

                scope.Complete();
            }
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new IdentityDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByAppUserId(int appUserId, int curStatus, int newStatus)
        {
            return await new IdentityDB().UpdateStatusByAppUserId(appUserId, curStatus, newStatus);
        }

        // DELETE

        public static async Task Delete(int id)
        {
            await new IdentityDB().DeleteById(id);
        }

        public static async Task DeleteByAppUserId(int appUserId)
        {
            await new IdentityDB().DeleteByAppUserId(appUserId);
        }
    }
}