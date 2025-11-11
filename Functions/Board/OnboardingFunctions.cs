using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;

namespace HeroServer
{
    public class OnboardingFunctions
    {

        public static async Task<Onboarding> GetByAppUserId(int appUserId)
        {
            return await new OnboardingDB().GetByAppUserId(appUserId);
        }

        public static async Task<List<Onboarding>> GetAllByAppUserId(int appUserId)
        {
            return await new OnboardingDB().GetAllByAppUserId(appUserId);
        }

        public static async Task<int> Add(Onboarding onboarding)
        {
            int onboardingId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new OnboardingDB().UpdateStatus(onboarding.Id, 0);

                onboardingId = await new OnboardingDB().Add(onboarding);

                scope.Complete();
            }

            if (onboarding.Status == 3)
                await SendRequestMessage(onboarding.Id, onboarding.AppUserId, GetStage(onboarding));

            return onboardingId;
        }

        public static int GetStage(Onboarding onboarding)
        {
            int onboardingStage = GetOnboardingStage(onboarding.GetDpiFrontResult(), 1);
            if (onboardingStage != -1)
                return onboardingStage;

            onboardingStage = GetOnboardingStage(onboarding.GetDpiBackResult(), 2);
            if (onboardingStage != -1)
                return onboardingStage;

            onboardingStage = GetOnboardingStage(onboarding.GetPortraitResult(), 3);
            if (onboardingStage != -1)
                return onboardingStage;

            onboardingStage = GetOnboardingStage(onboarding.GetAddressResult(), 4);
            if (onboardingStage != -1)
                return onboardingStage;

            return 0;
        }

        private static int GetOnboardingStage(int result, int type)
        {
            return Onboarding.GetResultType(result) == 2 ? Onboarding.SetResultType(result, type) : -1;
        }

        public static async Task<int> UpdateOnboarding(Onboarding onboarding)
        {
            if (onboarding.Id == -1 || await new OnboardingDB().GetAppUserIdById(onboarding.Id) == -1)
                return await Add(onboarding);

            if (!await new OnboardingDB().Update(onboarding))
                throw new Exception("Onboarding not found.");

            return onboarding.Id;
        }

        public static async Task UpdateDpiFront(int appUserId, String dpiPhotos)
        {
            String[] photos = dpiPhotos.Split("|");
            await OnboardingFunctions.UpdateDpiFront(Convert.ToInt32(appUserId), photos[0], photos[1]);
        }

        public static async Task UpdateDpiFront(int appUserId, String dpiFront, String dpiPortrait)
        {
            await IdentityFunctions.UpdateDpiFront(appUserId, dpiFront, dpiPortrait);
            await IdentityFunctions.UpdateVersion(appUserId, null, 2);

            await AddOnboarding(appUserId, 1);
        }

        public static async Task UpdateDpiBack(int appUserId, String dpiBack)
        {
            await IdentityFunctions.UpdateDpiBack(appUserId, dpiBack);
            await IdentityFunctions.UpdateSerie(appUserId, null, 2);

            await AddOnboarding(appUserId, 2);
        }

        public static async Task<int> UpdateIdentityInfo(IdentityInfo identityInfo)
        {
            int identityId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                identityInfo.Identity.DpiVersion = null;
                identityInfo.Identity.DpiSerie = null;
                identityId = await IdentityFunctions.UpdateInfo(identityInfo, false);

                await IdentityFunctions.UpdateStatusByAppUserId(identityInfo.Identity.AppUserId, 2, 4);
                identityInfo.Identity.Status = 2;
                int obdIdentityId = await IdentityFunctions.Add(identityInfo.Identity); // await IdentityFunctions.Copy(identityId, 2);

                await AddOnboarding(identityInfo.Identity.AppUserId, 3);

                await UpdateIdentityId(identityInfo.Identity.AppUserId, obdIdentityId);

                scope.Complete();
            }

            return identityId;
        }

        public static async Task UpdatePortrait(int appUserId, String portrait)
        {
            await IdentityFunctions.UpdatePortrait(appUserId, portrait);

            await AddOnboarding(appUserId, 4);
        }

        public static async Task UpdateHouseholdBills(int appUserId, String[] householdBills)
        {
            await AddressFunctions.UpdateHouseholdBills(appUserId, householdBills, 2);

            await AddOnboarding(appUserId, 5);
        }

        // Onboarding
        private static async Task AddOnboarding(int appUserId, int stage)
        {
            Onboarding onboarding = await GetByAppUserId(appUserId);
            await new OnboardingDB().UpdateStatus(onboarding.Id, 0);

            if (stage == 1)
            {
                onboarding.DpiFront = 0;
                onboarding.Portrait = 16383;
                onboarding.Renap = 0;
            }
            else if (stage == 2)
            {
                onboarding.DpiBack = 0;
                onboarding.Renap = 0;
            }
            else if (stage == 3)
            {
                onboarding.DpiFront = 0;
                onboarding.DpiBack = 0;
                onboarding.Portrait = 16383;
                onboarding.Renap = 0;
            }
            else if (stage == 4)
                onboarding.Portrait = 16383;
            else if (stage == 5)
                onboarding.Address = 0;

            onboarding.Status = 1;
            await new OnboardingDB().Add(onboarding);

            await SendUpdateMessage(onboarding);
        }

        private static async Task UpdateIdentityId(int appUserId, int identityId)
        {
            await new OnboardingDB().UpdateIdentityId(appUserId, identityId);
        }

        public static async Task Authorize(int onboardingId, int appUserId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new OnboardingDB().UpdateStatus(onboardingId, 2);

                await AppUserFunctions.UpdateStatus(appUserId, 5);
                await IdentityFunctions.UpdateStatusByAppUserId(appUserId, 1, 3);
                await IdentityFunctions.UpdateStatusByAppUserId(appUserId, 2, 1);
                await AddressFunctions.UpdateStatusByAppUserId(appUserId, 1, 3);
                await AddressFunctions.UpdateStatusByAppUserId(appUserId, 2, 1);

                await new LeapUsageDB().UpdateStatusByProductId(appUserId, 4, 1, 2);
                await new LeapUsageDB().UpdateStatusByProductId(appUserId, 5, 1, 2);

                scope.Complete();
            }

            await SendOnboardingMessage(onboardingId, appUserId, 5);
        }

        public static async Task Reject(int onboardingId, int appUserId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new OnboardingDB().UpdateStatus(onboardingId, 4);

                await AppUserFunctions.UpdateStatus(appUserId, 4);

                await new LeapUsageDB().UpdateStatusByProductId(appUserId, 4, 1, 2);
                await new LeapUsageDB().UpdateStatusByProductId(appUserId, 5, 1, 2);

                scope.Complete();
            }

            await SendOnboardingMessage(onboardingId, appUserId, 4);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new OnboardingDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByAppUserId(int appUserId, int curStatus, int newStatus)
        {
            return await new OnboardingDB().UpdateStatusByAppUserId(appUserId, curStatus, newStatus);
        }

        // DELETE
        public static async Task DeleteByAppUserId(int appUserId)
        {
            await new OnboardingDB().DeleteByAppUserId(appUserId);
        }


        // Messages

        public static async Task<int> SendRequestMessage(int onboardingId, int appUserId, int onboardingStage, ILogger logger = null)
        {
            String body = ", tu solicitud requiere actualizar algunos datos.";

            String parameter = onboardingStage.ToString();

            return await FirebaseHelper.SendMessage(appUserId, "Onboarding", onboardingId, "Solicitud", body, "Onboarding", "Request", parameter, 1, logger);
        }

        public static async Task<int> SendUpdateMessage(Onboarding onboarding, ILogger logger = null)
        {
            (String firstName1, String _1, String lastName1, String _2) = await new IdentityDB().GetFullNameByAppUserId(onboarding.AppUserId);
            String body = $", {firstName1} {lastName1} ha actualizado sus datos.";

            String parameter = onboarding.AppUserId.ToString();

            return await FirebaseHelper.SendMessage(onboarding.BoardUserId, "Onboarding", onboarding.Id, "Respuesta", body, "Onboarding", "Update", parameter, 0, logger);
        }

        public static async Task<int> SendOnboardingMessage(int onboardingId, int appUserId, int appUserStatusId, ILogger logger = null)
        {
            String body;
            if (appUserStatusId == 5)
            {
                body = ", ¡felicidades! tu solicitud fue aceptada.";
            }
            else if (appUserStatusId == 4)
            {
                body = ", tu solicitud fue rechazada.";
            }
            else
                return 3;

            String parameter = appUserStatusId.ToString();

            return await FirebaseHelper.SendMessage(appUserId, "Onboarding", onboardingId, "Solicitud", body, "Onboarding", "Finalize", parameter, 1, logger);
        }
    }
}