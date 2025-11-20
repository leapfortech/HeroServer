using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;

namespace HeroServer
{
    public class ReferredFunctions
    {
        // GET
        public static async Task<List<Referred>> GetAll()
        {
            return await new ReferredDB().GetAll();
        }

        public static async Task<List<ReferredFull>> GetFullAll()
        {
            return await new ReferredDB().GetFullAll();
        }

        public static async Task<Referred> GetById(long id)
        {
            return await new ReferredDB().GetById(id);
        }

        public static async Task<IEnumerable<Referred>> GetByAppUserId(long appUserId, int status = 1)
        {
            return await new ReferredDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<IEnumerable<Referred>> GetHistory(ReferredHistoryRequest referredHistoryRequest)
        {
            return await new ReferredDB().GetHistory(referredHistoryRequest.AppUserId, referredHistoryRequest.DateStart, referredHistoryRequest.DateEnd);
        }

        public static async Task<long> GetAppUserIdById(long id)
        {
            return await new ReferredDB().GetAppUserIdById(id);
        }

        public static async Task<long> Validate(long id)   // JAD : Remove
        {
            return await new ReferredDB().GetById(id) == null ? 0 : 1;
        }

        // REGISTER
        public static async Task<long> Register(Referred referred, ILogger logger)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                referred.Status = 1;

                referred.Id = await new ReferredDB().Add(referred);

                String email = await new IdentityDB().GetEmailById(referred.IdentityId, 1);
                if (email != null)
                    await SendEmail(referred, logger);

                scope.Complete();
            }
            return referred.Id;
        }

        // ADD
        public static async Task<long> Add(Referred referred)
        {
            return await new ReferredDB().Add(referred);
        }

        // UPDATE
        public static async Task<bool> Update(Referred referred)
        {
            return await new ReferredDB().Update(referred);
        }

        public static async Task<bool> UpdateStatusByAppUser(long appUserId, int curStatus, int newStatus)
        {
            return await new ReferredDB().UpdateStatusByAppUserId(appUserId, curStatus, newStatus);
        }

        // DELETE
        public static async Task DeleteByAppUserId(long appUserId)
        {
            await new ReferredDB().DeleteByAppUserId(appUserId);
        }

        // Email
        public static async Task<int> SendEmail(Referred referred, ILogger logger)
        {
            Identity identityReferrer = await IdentityFunctions.GetByAppUserId(referred.AppUserId, 1);
            Identity identityReferred = await IdentityFunctions.GetById(referred.IdentityId);

            String appUserName = identityReferrer == null ? "" : $"por {identityReferrer.FirstName1} {identityReferrer.LastName1} ";
            String referredName = $"{identityReferred.FirstName1} {identityReferred.LastName1}";
            String link = "https://lefortech.com/";

            String body = $"Estimad@ {referredName}," +
                          $" fuiste referido {appUserName}para descargar la aplicación móvil de Héroes Migrantes.<br><br>" +
                          " Presiona el siguiente link para descargarla.<br><br>" +
                          $" <a href='{link}'>Descargar</a><br><br>" +
                          " No olvides ingresar el siguiente código al momento de tu registro" +
                          $" para obtener los mejores beneficios: <strong>{referred.Id}</strong>.";

            String message = HtmlHelper.GetConfirmResultHtml("Expande", body, "#666666");
            if (message == null)
                return 3;

            try
            {
                await MailHelper.SendMail(identityReferred.Email, referredName, "Eres referido de " + appUserName + " para descargar nuestra App.", message, true);
            }
            catch (Exception ex)
            {
                logger?.LogError("ERROR : Fail to SendMail On AppUser #{AppUserId} to referred {ReferredId}", referred.AppUserId, referred.Id);
                logger?.LogError("{ExMessage}", ex.Message);
                return 2;
            }

            return 1;
        }
    }
}
