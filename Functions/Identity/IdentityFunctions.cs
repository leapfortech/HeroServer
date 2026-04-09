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

        public static async Task<IdentityFull> GetFullByAppUserId(long appUserId, int status)
        {
            return await new IdentityDB().GetFullByAppUserId(appUserId, status);
        }

        public static async Task<List<Identity>> GetAllByAppUserId(long appUserId, int status)
        {
            return await new IdentityDB().GetAllByAppUserId(appUserId, status);
        }

        // REGISTER
        public static async Task<long> RegisterByAppUser(long appUserId, Identity identity)
        {
            long identityId;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                IdentityAppUser identityAppUser = await new IdentityAppUserDB().GetByAppUserId(appUserId);
                if (identityAppUser != null)
                {
                    await new IdentityAppUserDB().UpdateStatus(identityAppUser.Id, 1, 0);
                    await new IdentityDB().UpdateStatus(identityAppUser.IdentityId, 1, 0);
                }

                identity.Status = 1;
                identityId = await new IdentityDB().Add(identity);

                identityAppUser = new IdentityAppUser(-1, appUserId, identityId, DateTime.Now, DateTime.Now, 1);
                identityAppUser.Id = await new IdentityAppUserDB().Add(identityAppUser);

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
                                                                      -1, -1, -1, -1, null, null, 1));

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
        public static async Task<long> Update(Identity identity)
        {
            long identityId = -1;

            if (await new IdentityDB().Update(identity))
                identityId = identity.Id;

            return identityId;
        }

        public static async Task<long> UpdatePersonal(IdentityPersonal identityPersonal)
        {
            long id = -1;
            DateTime sqlMinDate = new DateTime(1753, 1, 1);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Identity identity = await new IdentityDB().GetById(identityPersonal.IdentityId);

                identity.FirstName1 = identityPersonal.FirstName1 != null ? identityPersonal.FirstName1 : identity.FirstName1;
                identity.FirstName2 = identityPersonal.FirstName2 != null ? identityPersonal.FirstName2 : identity.FirstName2;
                identity.LastName1 = identityPersonal.LastName1 != null ? identityPersonal.LastName1 : identity.LastName1;
                identity.LastName2 = identityPersonal.LastName2 != null ? identityPersonal.LastName2 : identity.LastName2;
                identity.BirthDate = identityPersonal.BirthDate != sqlMinDate ? identityPersonal.BirthDate : identity.BirthDate;
                identity.GenderId = identityPersonal.GenderId != -1 ? identityPersonal.GenderId : identity.GenderId;

                id = await RegisterByAppUser(identityPersonal.AppUserId, identity);

                scope.Complete();
            }

            return id;
        }

        public static async Task<long> UpdatePlace(IdentityPlace identityPlace)
        {
            long id = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Identity identity = await new IdentityDB().GetById(identityPlace.IdentityId);

                identity.BirthCountryId = identityPlace.BirthCountryId != -1 ? identityPlace.BirthCountryId : identity.BirthCountryId;
                identity.BirthStateId = identityPlace.BirthStateId != -1 ? identityPlace.BirthStateId : identity.BirthStateId;
                identity.BirthCityId = identityPlace.BirthCityId != -1 ? identityPlace.BirthCityId : identity.BirthCityId;

                id = await RegisterByAppUser(identityPlace.AppUserId, identity);

                scope.Complete();
            }

            return id;
        }

        public static async Task<long> UpdateByAppUserId(long appUserId, Identity identity)
        {
            long id = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                (long identityAppUserId, long identityId) = await new IdentityAppUserDB().GetIdsByAppUserId(appUserId, 1);

                if ((identityAppUserId == -1) || (identityId == -1))
                    throw new Exception("AppUser not Found");

                await new IdentityDB().UpdateStatus(identityId, 0);
                await new IdentityAppUserDB().UpdateStatus(identityAppUserId, 0);

                identity.Status = 1;
                id = await new IdentityDB().Add(identity);
                await new IdentityAppUserDB().Add(new IdentityAppUser(-1, appUserId, id, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return id;
        }

        public static async Task<long> UpdateByBoardUserId(long boardUserId, Identity identity)
        {
            long id = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                (long identityBoardUserId, long identityId) = await new IdentityBoardUserDB().GetIdsByBoardUserId(boardUserId, 1);

                if ((identityBoardUserId == -1) || (identityId == -1))
                    throw new Exception("BoardUser not Found");

                await new IdentityDB().UpdateStatus(identityId, 0);
                await new IdentityBoardUserDB().UpdateStatus(identityBoardUserId, 0);

                identity.Status = 1;
                id = await new IdentityDB().Add(identity);
                await new IdentityBoardUserDB().Add(new IdentityBoardUser(-1, boardUserId, id, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return id;
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