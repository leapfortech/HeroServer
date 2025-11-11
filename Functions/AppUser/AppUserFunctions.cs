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

        public static async Task<AppUser> GetById(int id)
        {
            return await new AppUserDB().GetById(id);
        }

        public static async Task<AppUser> GetByIdStatus(int id, int status)
        {
            return await new AppUserDB().GetByIdStatus(id, status);
        }

        public static async Task<int> GetIdByAuthUserId(String authUserId)
        {
            return await new AppUserDB().GetIdByAuthUserId(authUserId);
        }

        public static async Task<AppUser> GetByWebSysUserId(int webSysUserId)
        {
            return await new AppUserDB().GetByWebSysUserId(webSysUserId);
        }

        public static async Task<int> GetWebSysUserId(int id)
        {
            return await new AppUserDB().GetWebSysUserId(id);
        }

        public static async Task<int> GetIdByWebSysUserId(int webSysUserId)
        {
            return await new AppUserDB().GetIdByWebSysUserId(webSysUserId);
        }

        public static async Task<int> GetIdByEmail(String eMail)
        {
            return await new AppUserDB().GetIdByEmail(eMail);
        }

        public static async Task<int> GetOptions(int appUserId)
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

        // ADD
        public static async Task<int> Add(AppUser appUser)
        {
            return await new AppUserDB().Add(appUser);
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

        public static async Task<bool> UpdateOptions(int id, int options)
        {
            return await new AppUserDB().UpdateOptions(id, options);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new AppUserDB().UpdateStatus(id, status);
        }

        public static async Task<int> UpdateReferred(int id, String referredCode)
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

        // DELETE
        public static async Task DeleteById(int id, bool delAuthUser = true, bool delRenap = false)
        {
            int webSysUserId = await new AppUserDB().GetWebSysUserId(id);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new AppUserDB().DeleteById(id);

                await IdentityFunctions.DeleteByAppUserId(id);
                await AppointmentFunctions.DeleteByAppUserId(id);
                await BankTransactionFunctions.DeleteByAppUserId(id);
                await CardFunctions.DeleteByAppUserId(id);
                await InvestmentFunctions.DeleteByAppUserId(id);
                await OnboardingFunctions.DeleteByAppUserId(id);
                await ReferredFunctions.DeleteByAppUserId(id);
                if (delRenap)
                    await RenapIdentityFunctions.DeleteByAppUserId(id);

                scope.Complete();
            }

            await DeleteImages(id);

            if (webSysUserId == -1)
                return;

            int boardUserId = await BoardUserFunctions.GetIdByWebSysUserId(webSysUserId);
            if (boardUserId != -1)
                return;

            await NotificationFunctions.DeleteByWebSysUserId(webSysUserId);
            await WebSysUserFunctions.DeleteById(webSysUserId, delAuthUser);
        }

        public static async Task DeleteByEmail(String eMail, bool delAuthUser = true, bool delRenap = false)
        {
            int appUserId = await GetIdByEmail(eMail);
            if (appUserId == -1)
                throw new Exception("Email NOT Found");
            await DeleteById(appUserId, delAuthUser, delRenap);
        }

        public static async Task DeleteImages(int id, bool delRenap = false)
        {
            if (delRenap)
                await StorageFunctions.DeleteContainer($"user{id:D08}");
            await StorageFunctions.DeleteContainer($"invest{id:D08}");
        }
    }
}