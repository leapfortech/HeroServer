using System;
using System.Collections.Generic;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using FirebaseAdmin.Auth;

namespace HeroServer
{
    public static class AccessFunctions
    {
        // Register
        public static async Task<String> RegisterApp(RegisterAppRequest registerAppRequest)
        {
            WebSysUser webSysUser = await WebSysUserFunctions.GetByEmail(registerAppRequest.Email);
            long appUserId = webSysUser == null ? -1 : await AppUserFunctions.GetIdByWebSysUserId(webSysUser.Id);

            if (appUserId != -1)
                throw new Exception($"El usuario {registerAppRequest.Email} ya fue registrado.");

            int verified = 0;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (webSysUser == null)
                {
                    UserRecord userRecord = await FirebaseFunctions.Register(null, registerAppRequest.Email, registerAppRequest.Password, false);

                    webSysUser = new WebSysUser(-1, userRecord.Uid, registerAppRequest.Email, registerAppRequest.Roles, registerAppRequest.PhoneCountryId, registerAppRequest.Phone, 1);
                    webSysUser.Id = await WebSysUserFunctions.Add(webSysUser);
                }
                else
                {
                    verified = 1;

                    await WebSysUserFunctions.AddRoles(webSysUser.Id, registerAppRequest.Roles);

                    if (registerAppRequest.PhoneCountryId != -1)
                        await WebSysUserFunctions.UpdatePhone(new PhoneRequest(webSysUser.Id, registerAppRequest.PhoneCountryId, registerAppRequest.Phone));
                }

                long referredAppUserId = await ReferredFunctions.GetAppUserIdById(registerAppRequest.ReferredId);

                AppUser appUser = new AppUser(-1, webSysUser.Id, referredAppUserId, 0);
                appUserId = await new AppUserDB().Add(appUser);

                await UpdateCSToken(appUserId, webSysUser.Email);
                
                scope.Complete();
            }

            return $"{appUserId}|{verified}";
        }

        private static async Task UpdateCSToken(long appUserId, String email)
        {
            String csToken = (await CybersourceFunctions.RegisterCustomer(WebEnvConfig.Flag + appUserId.ToString(), email)).Id;
            await new AppUserDB().UpdateCSToken(appUserId, csToken);
        }

        public static async Task<int> RegisterCSTokens()
        {
            List<(int Id, String Email)> appUserMails = await new AppUserDB().GetMailByCSTokenNull();
            for (int i = 0; i < appUserMails.Count; i++)
            {
                await UpdateCSToken(appUserMails[i].Id, appUserMails[i].Email);
                await Task.Delay(1000);
            }
            return appUserMails.Count;
        }

        // Login
        public static async Task<LoginAppResponse> LoginApp(HttpContext httpContext, LoginRequest loginRequest)
        {
            FirebaseToken token = await FirebaseFunctions.GetAuthToken(httpContext);
            WebSysUser webSysUser = await WebSysUserFunctions.GetByEmail(loginRequest.Email);
            AppUser appUser = await AppUserFunctions.GetByWebSysUserId(webSysUser.Id);

            // Log
            int statusId = appUser == null ? 0 : 1;

            if (token == null)
                statusId = 2;
            else if ((String)token.Claims["email"] != loginRequest.Email)
                statusId = 3;
            else if ((Boolean)token.Claims["email_verified"] == false)
                statusId = 4;

            // Authorize
            if (statusId > 1)
                return null;

            // Login
            if (statusId == 0)
                throw new Exception("App User not found.");


            //if (appUser.AppUserStatusId == 3)  // En validación
            //{
            //    Onboarding onboarding = await OnboardingFunctions.GetByAppUserId(appUser.Id);
            //    if (onboarding.Status == 3)
            //        onboardingStage = OnboardingFunctions.GetStage(onboarding);
            //}
            //else if (appUser.AppUserStatusId == 5)  // Acceptado
            //    await AppUserFunctions.UpdateStatus(appUser.Id, 1);  // Activo
            //else if (appUser.AppUserStatusId == 4)  // Rechazado
            //    await AppUserFunctions.UpdateStatus(appUser.Id, 6);  // Inactivo

            //return new LoginResponse(0, "0|Login|Este es un aviso de bloqueo.");

            return new LoginAppResponse(appUser, webSysUser, 1);
        }

        public static async Task<LoginAppInfo> GetLoginAppInfo(int appUserId, int webSysUserId)
        {
            LoginAppInfo loginAppInfo = await new LoginDB().GetLoginAppInfo(appUserId, webSysUserId, DateTime.Now, DateTime.Now.AddDays(30));
            loginAppInfo.Portrait = await IdentityFunctions.GetPortraitByAppUserId(appUserId);

            return loginAppInfo;
        }

        // BOARD

        // Register Board
        public static async Task<long> RegisterBoard(RegisterBoardRequest registerBoardRequest)
        {
            WebSysUser webSysUser = await WebSysUserFunctions.GetByEmail(registerBoardRequest.Email);
            long boardUserId = webSysUser == null ? -1 : await BoardUserFunctions.GetIdByWebSysUserId(webSysUser.Id);

            if (boardUserId != -1)
                throw new Exception($"El usuario {registerBoardRequest.Email} ya fue registrado.");

            BoardUser boardUser;
            bool emptyPassword = String.IsNullOrEmpty(registerBoardRequest.Password);
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (webSysUser == null)
                {
                    registerBoardRequest.Password = emptyPassword ? FirebaseFunctions.GeneratePassword(10) : registerBoardRequest.Password;

                    UserRecord userRecord = await FirebaseFunctions.Register(registerBoardRequest.GetCompleteName(), registerBoardRequest.Email, registerBoardRequest.Password, true);

                    webSysUser = new WebSysUser(-1, userRecord.Uid, registerBoardRequest.Email, registerBoardRequest.Roles, registerBoardRequest.PhoneCountryId, registerBoardRequest.Phone, 1);
                    webSysUser.Id = await WebSysUserFunctions.Add(webSysUser);
                }
                else
                {
                    await WebSysUserFunctions.AddRoles(webSysUser.Id, registerBoardRequest.Roles);

                    if (registerBoardRequest.PhoneCountryId != -1)
                        await WebSysUserFunctions.UpdatePhone(new PhoneRequest(webSysUser.Id, registerBoardRequest.PhoneCountryId, registerBoardRequest.Phone));
                }

                long identityId = await IdentityFunctions.RegisterByBoardUser(boardUserId, registerBoardRequest);

                boardUser = new BoardUser(-1, webSysUser.Id, identityId, 1);

                boardUser.Id = await BoardUserFunctions.Add(boardUser);

                scope.Complete();
            }

            await SendBoardUserEmail(boardUser, webSysUser.Email, registerBoardRequest.Password);

            return boardUser.Id;
        }

        public static async Task<int> SendBoardUserEmail(BoardUser boardUser, String eMail, String password)
        {
            Identity identity = await IdentityFunctions.GetByBoardUserId(boardUser.Id, 1);

            String subject = "Registro plataforma Héroes Migrantes";
            String body = $"{identity.FirstName1 + " " + identity.LastName1}, fuiste registrado en la plataforma HpbBoard con las credenciales siguientes.<br><br>" +
                          $"Email: {eMail}<br>" +
                          $"Contraseña: {password}";

            String message = HtmlHelper.GetConfirmResultHtml(subject, body, "#666666");
            if (message == null)
                return 3;

            try
            {
                await MailHelper.SendMail(eMail, identity.FirstName1 + " " + identity.LastName1, subject, message, true);
            }
            catch
            {
                return 2;
            }

            return 1;
        }

        // Login Board
        public static async Task<LoginBoardResponse> LoginBoard(HttpContext httpContext, LoginRequest loginRequest)
        {
            FirebaseToken token = await FirebaseFunctions.GetAuthToken(httpContext);
            WebSysUser webSysUser = await WebSysUserFunctions.GetByEmail(loginRequest.Email);
            BoardUser boardUser = await BoardUserFunctions.GetByWebSysUserId(webSysUser.Id);

            // Log
            int statusId = boardUser == null ? 0 : 1;

            if (token == null)
                statusId = 2;
            else if ((String)token.Claims["email"] != loginRequest.Email)
                statusId = 3;
            else if ((Boolean)token.Claims["email_verified"] == false)
                statusId = 4;

            // Authorize
            if (statusId > 1)
                return null;

            // Login
            if (statusId == 0)
                throw new Exception("Board User not found.");

            return new LoginBoardResponse(boardUser, webSysUser, 1);
        }
    }
}
