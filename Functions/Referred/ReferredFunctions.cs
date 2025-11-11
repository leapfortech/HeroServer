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

        public static async Task<Referred> GetById(int id)
        {
            return await new ReferredDB().GetById(id);
        }

        public static async Task<IEnumerable<Referred>> GetByAppUserId(int appUserId, int status = 1)
        {
            return await new ReferredDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<IEnumerable<Referred>> GetHistory(ReferredHistoryRequest referredHistoryRequest)
        {
            return await new ReferredDB().GetHistory(referredHistoryRequest.AppUserId, referredHistoryRequest.DateStart, referredHistoryRequest.DateEnd);
        }

        public static async Task<Referred> GetByCode(String code)
        {
            if (String.IsNullOrEmpty(code))
                return null;
            return await new ReferredDB().GetByCode(code);
        }

        public static async Task<int> GetIdByCode(String code)
        {
            if (String.IsNullOrEmpty(code))
                return -1;
            return await new ReferredDB().GetIdByCode(code);
        }

        public static async Task<int> GetAppUserIdByCode(String code)
        {
            if (String.IsNullOrEmpty(code))
                return -1;
            return await new ReferredDB().GetAppUserIdByCode(code);
        }

        public static async Task<int> Validate(String code)   // JAD : Remove
        {
            return await new ReferredDB().GetIdByCode(code) == -1 ? 0 : 1;
        }

        // REGISTER
        public static async Task<String> Register(Referred referred, ILogger logger)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                referred.Code = String.Format("{0}{1:yyMMddHHmm}", referred.AppUserId, DateTime.Now);
                referred.Status = 1;

                referred.Id = await new ReferredDB().Add(referred);

                if (referred.Email != null)
                    await SendEmail(referred, logger);

                scope.Complete();
            }
            return referred.Id + "|" + referred.Code;
        }

        // ADD
        public static async Task<int> Add(Referred referred)
        {
            return await new ReferredDB().Add(referred);
        }

        // UPDATE
        public static async Task<bool> Update(Referred referred)
        {
            return await new ReferredDB().Update(referred);
        }

        public static async Task<bool> UpdateStatusByAppUser(int appUserId, int curStatus, int newStatus)
        {
            return await new ReferredDB().UpdateStatusByAppUserId(appUserId, curStatus, newStatus);
        }

        // DELETE
        public static async Task DeleteByAppUserId(int appUserId)
        {
            await new ReferredDB().DeleteByAppUserId(appUserId);
        }

        // Email
        public static async Task<int> SendEmail(Referred referred, ILogger logger)
        {
            Identity identity = await new IdentityDB().GetByAppUserId(referred.AppUserId, 1);

            String appUserName = identity == null ? "" : $"por {identity.FirstName1} {identity.LastName1} ";
            String referredName = $"{referred.FirstName} {referred.LastName}";
            String link = "https://grupohpb.com/";

            String body = $"Estimad@ {referredName}," +
                          $" fuiste referido {appUserName}para descargar la aplicación móvil de inversiones inmobiliarias Expande.<br><br>" +
                          " Presiona el siguiente link para descargarla.<br><br>" +
                          $" <a href='{link}'>Descargar</a><br><br>" +
                          " No olvides ingresar el siguiente código al momento de tu registro" +
                          $" para obtener los mejores beneficios: <strong>{referred.Code}</strong>.";

            String message = HtmlHelper.GetConfirmResultHtml("Expande", body, "#666666");
            if (message == null)
                return 3;

            try
            {
                await MailHelper.SendMail(referred.Email, referredName, "Eres referido de " + appUserName + " para descargar nuestra App.", message, true);
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
