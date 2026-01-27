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

        public static async Task<List<IdentityFull>> GetFullsByStatus(int status)
        {
            return await new IdentityDB().GetFullsByStatus(status);
        }

        public static async Task<Identity> GetById(long id)
        {
            return await new IdentityDB().GetById(id);
        }

        public static async Task<Identity> GetByAppUserId(long appUserId, int status)
        {
            long identityId = await new IdentityAppUserDB().GetIdentityIdByAppUserId(appUserId, status);

            return await new IdentityDB().GetById(identityId);
        }

        public static async Task<Identity> GetByBoardUserId(long boardUserId, int status)
        {
            long identityId = await new IdentityBoardUserDB().GetIdentityIdByBoardUserId(boardUserId, status);

            return await new IdentityDB().GetById(identityId);
        }

        public static async Task<String> GetPortraitByAppUserId(long appUserId)
        {
            String portrait = null;
            
            byte[] portraitImg = await StorageFunctions.ReadFile($"user{appUserId:D08}", $"prt{appUserId:D08}", "jpg");

            if (portraitImg != null)
                portrait = Convert.ToBase64String(portraitImg);

            return portrait;
        }

        public static async Task<IdentityFull> GetFullByAppUserId(long appUserId, int status)
        {
            return await new IdentityDB().GetFullByAppUserId(appUserId, status);
        }

        public static async Task<List<Identity>> GetAllByAppUserId(long appUserId, int status)
        {
            return await new IdentityDB().GetAllByAppUserId(appUserId, status);
        }

        // REGISTER
        public static async Task<long> RegisterByAppUser(long appUserId, IdentityRegister identityRegister)
        {
            long identityId;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                IdentityAppUser identityAppUser = await new IdentityAppUserDB().GetByAppUserId(appUserId);
                if (identityAppUser != null)
                {
                    await new IdentityAppUserDB().UpdateStatus(identityAppUser.Id, 1, 0);
                    await new IdentityDB().UpdateStatus(identityAppUser.AppUserId, 1, 0);
                }

                identityRegister.Identity.Status = 1;
                identityId = await new IdentityDB().Add(identityRegister.Identity);

                identityAppUser = new IdentityAppUser(-1, appUserId, identityId, DateTime.Now, DateTime.Now, 1);
                identityAppUser.Id = await new IdentityAppUserDB().Add(identityAppUser);

                // Portrait
                String containerName = "user" + appUserId.ToString("D08");
                await StorageFunctions.CreateContainer(containerName);

                if (identityRegister.Portrait.Length > 0)
                    await StorageFunctions.UpdateFile(containerName, "prt" + appUserId, "jpg", Convert.FromBase64String(identityRegister.Portrait));

                scope.Complete();
            }

            return identityId;
        }

        public static async Task<long> RegisterByBoardUser(long boardUserId, RegisterBoardRequest registerBoardRequest)
        {
            long identityId;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                IdentityBoardUser identityBoardUser = await new IdentityBoardUserDB().GetByBoardUserId(boardUserId);
                if (identityBoardUser != null)
                {
                    await new IdentityBoardUserDB().UpdateStatus(identityBoardUser.Id, 1, 0);
                    await new IdentityDB().UpdateStatus(identityBoardUser.BoardUserId, 1, 0);
                }

                identityId = await IdentityFunctions.Add(new Identity(-1, registerBoardRequest.FirstName1,
                                                                      registerBoardRequest.FirstName2,
                                                                      registerBoardRequest.LastName1,
                                                                      registerBoardRequest.LastName2,
                                                                      -1, registerBoardRequest.BirthDate,
                                                                      -1, -1, -1, null, null, 1));

                identityBoardUser = new IdentityBoardUser(-1, boardUserId, identityId, DateTime.Now, DateTime.Now, 1);
                identityBoardUser.Id = await new IdentityBoardUserDB().Add(identityBoardUser);

                scope.Complete();
            }

            return identityId;
        }

        // ADD
        public static async Task<long> Add(Identity identity)
        {
            return await new IdentityDB().Add(identity);
        }

        // UPDATE
        public static async Task<long> Update(long appUserId, Identity identity)
        {
            long id = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                (long identityAppUserId, long identityId) = await new IdentityAppUserDB().GetIdsByAppUserId(appUserId, 1);

                await new IdentityDB().UpdateStatus(identityId, 0);
                await new IdentityAppUserDB().UpdateStatus(identityAppUserId, 0);

                identity.Status = 1;
                id = await new IdentityDB().Add(identity);
                await new IdentityAppUserDB().Add(new IdentityAppUser(-1, appUserId, id, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return id;
        }

        public static async Task UpdatePortrait(long appUserId, String portrait)
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

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new IdentityDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new IdentityDB().DeleteById(id);
        }

        public static async Task DeleteByAppUserId(long appUserId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<long> identityIds = await new IdentityAppUserDB().GetIdentityIdsByAppUserId(appUserId);

                for (int i = 0; i < identityIds.Count; i++)
                    await new IdentityDB().DeleteById(identityIds[i]);

                await new IdentityAppUserDB().DeleteByAppUserId(appUserId);

                scope.Complete();
            }
        }
    }
}