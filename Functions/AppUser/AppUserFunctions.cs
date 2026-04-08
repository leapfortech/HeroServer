using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class AppUserFunctions
    {
        // GET
        public static async Task<List<AppUserNamed>> GetNamed(int count, int page)
        {
            return await new AppUserDB().GetNamed(count, page);
        }

        public static async Task<List<AppUserNamed>> GetNamedByStatus(int status, int count, int page)
        {
            return await new AppUserDB().GetNamedByStatus(status, count, page);
        }

        public static async Task<List<AppUserFull>> GetFullByStatus(int status)
        {
            return await new AppUserDB().GetFullByStatus(status);
        }

        public static async Task<AppUser> GetById(long id)
        {
            return await new AppUserDB().GetById(id);
        }

        public static async Task<AppUser> GetByIdStatus(long id, int status)
        {
            return await new AppUserDB().GetByIdStatus(id, status);
        }

        public static async Task<long> GetIdByAuthUserId(String authUserId)
        {
            return await new AppUserDB().GetIdByAuthUserId(authUserId);
        }

        public static async Task<AppUser> GetByWebSysUserId(long webSysUserId)
        {
            return await new AppUserDB().GetByWebSysUserId(webSysUserId);
        }

        public static async Task<long> GetWebSysUserId(long id)
        {
            return await new AppUserDB().GetWebSysUserId(id);
        }

        public static async Task<long> GetIdByWebSysUserId(long webSysUserId)
        {
            return await new AppUserDB().GetIdByWebSysUserId(webSysUserId);
        }

        public static async Task<long> GetIdByEmail(String eMail)
        {
            return await new AppUserDB().GetIdByEmail(eMail);
        }

        public static async Task<long> GetOptions(long appUserId)
        {
            return await new AppUserDB().GetOptions(appUserId);
        }

        public static async Task<int> GetCountAll()
        {
            return await new AppUserDB().GetCountAll();
        }

        public static async Task<int> GetCountByStatus(int status)
        {
            return await new AppUserDB().GetCountByStatus(status);
        }

        public static async Task<AliasResponse> ValidateAlias(AliasRequest aliasRequest)
        {
            return new AliasResponse(await new AppUserDB().GetMailByAlias(aliasRequest.Alias, 1));
        }

        public static async Task<String> GetPortrait(long appUserId)
        {
            String portrait = null;

            byte[] portraitImg = await StorageFunctions.ReadFile($"user{appUserId:D08}", $"prt{appUserId:D08}", "jpg");

            if (portraitImg != null)
                portrait = Convert.ToBase64String(portraitImg);

            return portrait;
        }

        // ADD
        public static async Task<long> Add(AppUser appUser)
        {
            return await new AppUserDB().Add(appUser);
        }

        public static async Task RegisterPortrait(long appUserId, String portrait)
        {
            String containerName = "user" + appUserId.ToString("D08");
            await StorageFunctions.CreateContainer(containerName);

            if (portrait != null && portrait.Length > 0)
                await StorageFunctions.UpdateFile(containerName, "prt" + appUserId, "jpg", Convert.FromBase64String(portrait));
        }

        // UPDATE
        public static async Task Update(AppUser appUser)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new AppUserDB().UpdateStatusByWebSysUserId(appUser.WebSysUserId, 0);
                
                appUser.AppUserStatusId = 1;
                if (!await new AppUserDB().Update(appUser))
                    throw new Exception("UAU¶Cannot Update AppUser");

                scope.Complete();
            }
        }

        public static async Task UpdatePhone(PhoneRequest phoneRequest)
        {
            phoneRequest.Id = await AppUserFunctions.GetWebSysUserId(phoneRequest.Id);
            await WebSysUserFunctions.UpdatePhone(phoneRequest);
        }

        public static async Task<bool> UpdateOptions(long id, long options)
        {
            return await new AppUserDB().UpdateOptions(id, options);
        }

        public static async Task<long> UpdateOption(long id, int index, int newStatus)
        {
            AppUser appUser = await new AppUserDB().GetById(id);

            long options = appUser.Options;

            long power = (long)Math.Pow(10, index);
            long currentStatus = (options / power) % 10;

            long updatedOptions = options + (newStatus - currentStatus) * power;

            await new AppUserDB().UpdateOptions(id, updatedOptions);

            return updatedOptions;
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new AppUserDB().UpdateStatus(id, status);
        }

        public static async Task<long> UpdateReferred(long id, String referredCode)
        {
            Referred referred = null;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (referredCode != null && referredCode.Length > 0)
                    referred = await ReferredFunctions.GetByCode(referredCode);

                await new AppUserDB().UpdateReferredAppUserId(id, referred != null ? referred.AppUserId : -1);

                scope.Complete();
            }

            return referred != null ? referred.AppUserId : -1;
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

        // DELETE
        public static async Task DeleteById(long id, bool delAuthUser = true)
        {
            long webSysUserId = await new AppUserDB().GetWebSysUserId(id);
            bool committed = false;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await ReferredFunctions.DeleteByAppUserId(id);
                await IdentityFunctions.DeleteByAppUserId(id);
                await AddressFunctions.DeleteByAppUserId(id);
                await CardFunctions.DeleteByAppUserId(id);

                await new AppUserDB().DeleteById(id);

                scope.Complete();
                committed = true;
            }

            if (committed)
            {
                String containerName = "user" + id.ToString("D08");
                await StorageFunctions.DeleteContainer(containerName);
            }

            if (webSysUserId == -1)
                return;

            long boardUserId = await BoardUserFunctions.GetIdByWebSysUserId(webSysUserId);
            if (boardUserId != -1)
                return;

            await NotificationFunctions.DeleteByWebSysUserId(webSysUserId);
            await WebSysUserFunctions.DeleteById(webSysUserId, delAuthUser);
        }

        public static async Task DeleteByEmail(String eMail, bool delAuthUser = true)
        {
            long appUserId = await GetIdByEmail(eMail);
            if (appUserId == -1)
                throw new Exception("Email NOT Found");
            await DeleteById(appUserId, delAuthUser);
        }
    }
}