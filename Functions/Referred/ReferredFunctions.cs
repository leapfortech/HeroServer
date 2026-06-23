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

        public static async Task<ReferredFullAllRsp> GetFullAllByCode(ReferredAllByCodeReq req)
        {
            return await new ReferredDB().GetFullAllByCode(req);
        }

        public static async Task<Referred> GetById(long id)
        {
            return await new ReferredDB().GetById(id);
        }

        public static async Task<IEnumerable<Referred>> GetByAppUserId(long appUserId, int status = 1)
        {
            return await new ReferredDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<IEnumerable<ReferredFull>> GetHistory(ReferredHistoryRequest referredHistoryRequest)
        {
            return await new ReferredDB().GetHistory(referredHistoryRequest.AppUserId, referredHistoryRequest.DateStart, referredHistoryRequest.DateEnd);
        }

        public static async Task<long> GetAppUserIdById(long id)
        {
            return await new ReferredDB().GetAppUserIdById(id);
        }

        public static async Task<Referred> GetByCode(String code)
        {
            if (String.IsNullOrEmpty(code))
                return null;
            return await new ReferredDB().GetByCode(code);
        }

        public static async Task<long> GetIdByCode(String code)
        {
            if (String.IsNullOrEmpty(code))
                return -1;
            return await new ReferredDB().GetIdByCode(code);
        }

        public static async Task<long> GetAppUserIdByCode(String code)
        {
            if (String.IsNullOrEmpty(code))
                return -1;
            return await new ReferredDB().GetAppUserIdByCode(code);
        }

        public static async Task<long> Validate(String code)   // JAD : Remove
        {
            return await new ReferredDB().GetIdByCode(code) == -1 ? 0 : 1;
        }

        // REGISTER
        public static async Task<String> Register(RegisterReferredRequest registerReferredRequest, ILogger logger)
        {
            Referred referred = new Referred();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerReferredRequest.Identity.Status = 1;
                long identityId = await new IdentityDB().Add(registerReferredRequest.Identity);

                referred.AppUserId = registerReferredRequest.AppUserId;
                //referred.Code = String.Format("{0}{1:yyMMddHHmm}", referred.AppUserId, DateTime.Now);
                referred.Code = GenerateCode(referred.AppUserId);
                referred.IdentityId = identityId;
                referred.Status = 1;
                referred.Id = await new ReferredDB().Add(referred);

                if (registerReferredRequest.Identity.Email != null)
                    await SendEmail(referred, logger);

                scope.Complete();
            }
            return referred.Id + "|" + referred.Code;
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

        public static String GenerateCode(long appUserId)
        {
            long mixed = appUserId ^ DateTime.UtcNow.Ticks;

            String code = BaseHelper.Base36(Math.Abs(mixed));

            if (code.Length > 8)
                code = code[..8];
            else
                code = code.PadLeft(8, '0');

            return code;
        }

        // Email
        public static async Task<int> SendEmail(Referred referred, ILogger logger)
        {
            Identity identityReferrer = await IdentityFunctions.GetByAppUserId(referred.AppUserId, 1);
            Identity identityReferred = await IdentityFunctions.GetById(referred.IdentityId);

            String appUserName = identityReferrer == null ? "" : $"{identityReferrer.FirstName1} {identityReferrer.LastName1}";
            String referredName = $"{identityReferred.FirstName1} {identityReferred.LastName1}";
            String link = "https://www.heroesmigrantes.com/";

            String body = $"Estimad@ {referredName}," +
                          $" fuiste referid@ por {appUserName} para descargar la aplicación móvil de Héroes Migrantes.<br><br>" +
                          " Presiona el siguiente link para descargarla.<br><br>" +
                          $" <a href='{link}'>Descargar</a><br><br>" +
                          " No olvides ingresar el siguiente código al momento de tu registro:" +
                          $" <strong>{referred.Code}</strong>.";

            String message = HtmlHelper.GetConfirmResultHtml("Heroes Migrantes", body, "#666666");
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
