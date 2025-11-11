using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using FirebaseAdmin.Auth;

namespace HeroServer
{
    public static class FirebaseFunctions
    {
        private static String startUid = "StartUid";

        public static async void Initialize()
        {
            startUid = await new SystemParamDB().GetValue("FirebaseStartUid");
        }

        // User
        public static async Task<UserRecord> GetUserByAuthUserId(String authUserId)
        {
            return await FirebaseAuth.DefaultInstance.GetUserAsync(authUserId);
        }

        public static async Task<UserRecord> GetUserByWebSysUserId(int webSysUserId)
        {
            String authUserId = (await new WebSysUserDB().GetById(webSysUserId)).AuthUserId;
            return await FirebaseAuth.DefaultInstance.GetUserAsync(authUserId);
        }

        public static async Task<UserRecord> GetUserByAppUserId(int appUserId)
        {
            String email = await WebSysUserFunctions.GetEmailByAppUserId(appUserId);
            return await GetUserByMail(email);
        }

        public static async Task<UserRecord> GetUserByMail(String email)
        {
            return await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
        }

        // Role
        public static async Task<String> GetClaim(String token, String key)
        {
            FirebaseToken decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            if (!decoded.Claims.TryGetValue(key, out object value))
                return "<No Value>";
            return (String)value;
        }

        // Token
        public static async Task<FirebaseToken> GetAuthToken(HttpContext httpContext)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(await httpContext.GetTokenAsync("access_token"));
            }
            catch
            {
                return null;
            }
        }

        public static async Task<int> GetWebSysUserId(HttpContext httpContext)
        {
            try
            {
                FirebaseToken token = await GetAuthToken(httpContext);
                return await new WebSysUserDB().GetIdByAuthUserId(token.Uid);
            }
            catch
            {
                return -1;
            }
        }

        public static async Task<int> GetAppUserId(HttpContext httpContext)
        {
            try
            {
                int webSysUserId = await GetWebSysUserId(httpContext);
                return await new AppUserDB().GetIdByWebSysUserId(webSysUserId);
            }
            catch
            {
                return -1;
            }
        }

        public static async Task<String> GetCustomToken(String tokenType)
        {
            if (tokenType != "StartAnonymous")
                throw new Exception("Custom Login Error");

            Dictionary<String, object> claims = new Dictionary<String, object>() { { tokenType, true } };

            return await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(startUid, claims);
        }

        // Add
        public static async Task<UserRecord> Register(String name, String email, String password, bool emailVerified)
        {
            UserRecordArgs userRecordArgs = new UserRecordArgs()
            {
                Email = email,
                EmailVerified = emailVerified,
                Password = password,
                DisplayName = name,
                Disabled = false,
            };

            return await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
        }

        public static String GeneratePassword(int length)
        {
            if (length < 8)
                throw new Exception("Password length must be >= 8");

            const String lower = "abcdefghijklmnopqrstuvwxyz";
            const String upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const String numeric = "1234567890";
            const String special = "$%#!@&~.+=<>?*^-_:;";

            Random rnd = new Random((int)DateTime.Now.Ticks);

            int max = length / 4;
            int nbNumeric = rnd.Next(max) + 1;
            int nbSpecial = rnd.Next(max) + 1;

            max = (length - nbNumeric - nbSpecial) / 2;
            int nbUpper = rnd.Next(max) + 1;

            int nbLower = length - nbNumeric - nbSpecial - nbUpper;

            StringBuilder builder = new StringBuilder();
            AppendFromCharset(builder, lower, nbLower);
            AppendFromCharset(builder, upper, nbUpper);
            AppendFromCharset(builder, numeric, nbNumeric);
            AppendFromCharset(builder, special, nbSpecial);

            for (int i = 0; i < builder.Length; ++i)
            {
                int index1 = rnd.Next(builder.Length);
                int index2 = rnd.Next(builder.Length);

                (builder[index2], builder[index1]) = (builder[index1], builder[index2]);
            }

            return builder.ToString();
        }

        private static void AppendFromCharset(StringBuilder builder, String charset, int length)
        {
            Random rnd = new Random();
            while (length-- > 0)
                builder.Append(charset[rnd.Next(charset.Length)]);
        }

        // Update
        public static async Task<IReadOnlyDictionary<String, object>> GetRoles(String authUserId)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(authUserId);
            return userRecord.CustomClaims;
        }

        public static async Task SetRoles(FirebaseRoles fbRoles)
        {
            //UserRecord userRecord = await GetUserByAppUserId(appUserId);

            Dictionary<String, object> claims = [];
            for (int i = 0; i < fbRoles.Roles.Length; i++)
                claims.Add(ClaimTypes.Role, fbRoles.Roles[i]);
            //claims.Add("AppUserId", appUserId.ToString());
            //claims.Add("BoardUserId", boardUserId.ToString());

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(fbRoles.AuthUserId, claims);
        }

        public static async Task UpdatePhone(String authUserId, String phoneNumber)
        {
            UserRecordArgs userRecordArgs = new UserRecordArgs()
            {
                Uid = authUserId,
                PhoneNumber = phoneNumber,
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs);
        }

        // Authorize
        public static async Task<bool> AuthorizeAppUser(HttpContext httpContext, int appUserId, String context)
        {
            int authAppUserId = -1;

            try
            {
                FirebaseToken token = await FirebaseFunctions.GetAuthToken(httpContext);
                if (token != null)
                {
                    int systemUserId = await new WebSysUserDB().GetIdByAuthUserId(token.Uid);
                    authAppUserId = await new AppUserDB().GetIdByWebSysUserId(systemUserId);
                }

                if (CheckAppUserAdmin(authAppUserId))
                    return true;

                if (authAppUserId == appUserId)
                    return true;

                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, (String)token?.Claims["email"],
                                                      token?.Uid, authAppUserId, appUserId, null);

                await new SecurityLogDB().Add(log);
                return false;
            }
            catch
            {
                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, null, null, authAppUserId, appUserId, null);

                await new SecurityLogDB().Add(log);
                return false;
            }
        }

        public static async Task<bool> AuthorizeWebSysUser(HttpContext httpContext, int webSysUserId, String context)
        {
            int authWebSysUserId = -1;

            try
            {
                FirebaseToken token = await FirebaseFunctions.GetAuthToken(httpContext);
                if (token != null)
                    authWebSysUserId = await new WebSysUserDB().GetIdByAuthUserId(token.Uid);

                if (CheckWebSysAdmin(authWebSysUserId))
                    return true;

                if (authWebSysUserId == webSysUserId)
                    return true;

                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, (String)token?.Claims["email"],
                                                      token?.Uid, authWebSysUserId, webSysUserId, null);

                await new SecurityLogDB().Add(log);
                return false;
            }
            catch
            {
                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, null, null, authWebSysUserId, webSysUserId, null);

                await new SecurityLogDB().Add(log);
                return false;
            }
        }

        public static async Task<bool> AuthorizeMail(HttpContext httpContext, String eMail, String context)
        {
            FirebaseToken token = null;
            WebSysUser webSysUser = null;
            String authEmail = null;

            try
            {
                token = await FirebaseFunctions.GetAuthToken(httpContext);

                if (token != null)
                    webSysUser = await new WebSysUserDB().GetByAuthUserId(token.Uid);

                if (CheckWebSysAdmin(webSysUser.Id))
                    return true;

                authEmail = (String)token?.Claims["email"];
                if (webSysUser.Email == eMail && authEmail == eMail)
                    return true;

                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, authEmail, token.Uid, webSysUser.Id, -1, eMail);

                await new SecurityLogDB().Add(log);
                return false;
            }
            catch
            {
                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, authEmail, token?.Uid, webSysUser == null ? -1 : webSysUser.Id, -1, eMail);

                await new SecurityLogDB().Add(log);
                return false;
            }
        }

        // Admin
        public static async Task<bool> AuthorizeWebSysAdmin(HttpContext httpContext, String context)
        {
            int authWebSysUserId = -1;

            try
            {
                FirebaseToken token = await FirebaseFunctions.GetAuthToken(httpContext);
                if (token != null)
                    authWebSysUserId = await new WebSysUserDB().GetIdByAuthUserId(token.Uid);

                if (CheckWebSysAdmin(authWebSysUserId))
                    return true;

                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, (String)token?.Claims["email"],
                                                      token?.Uid, authWebSysUserId, -1, null);

                await new SecurityLogDB().Add(log);
                return false;
            }
            catch
            {
                SecurityLog log = new SecurityLog(-1, DateTime.Now, "Identity", context, null, null, authWebSysUserId, -1, null);

                await new SecurityLogDB().Add(log);
                return false;
            }
        }

        private static bool CheckAppUserAdmin(int authAppUserId)
        {
            return true;
        }

        private static bool CheckWebSysAdmin(int authWebSysUserId)
        {
            return true;
        }

        // Mail
        public static async Task ActivateAppUserMail(int webSysUserId, bool active)
        {
            String authUserId = (await new WebSysUserDB().GetById(webSysUserId)).AuthUserId;

            UserRecordArgs userRecordArgs = new UserRecordArgs()
            {
                Uid = authUserId,
                EmailVerified = active
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs);
        }
    }
}