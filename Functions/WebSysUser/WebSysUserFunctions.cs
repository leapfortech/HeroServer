using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Net;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FirebaseAdmin.Auth;

namespace HeroServer
{
    public static class WebSysUserFunctions
    {
        static int PinFailsMax = 3;
        static double PinLockDelay = 60.0;

        public static async void Initialize()
        {
            PinFailsMax = Convert.ToInt32(await new SystemParamDB().GetValue("PinFailsMax"));
            PinLockDelay = Convert.ToDouble(await new SystemParamDB().GetValue("PinLockDelay"));
        }

        // User
        public static async Task<WebSysUser> GetByEmail(String eMail)
        {
            return await new WebSysUserDB().GetByEmail(eMail);
        }

        public static async Task<long> GetIdByEmail(String eMail)
        {
            return await new WebSysUserDB().GetIdByEmail(eMail);
        }

        public static async Task<long> Add(WebSysUser webSysUser)
        {
            return await new WebSysUserDB().Add(webSysUser);
        }

        // Roles
        public static async Task<List<String>> GetRoles(long id)
        {
            return [.. (await new WebSysUserDB().GetRoles(id))?.Split('|')];
        }

        public static async Task AddRoles(long id, String roles)
        {
            List<String> curRoles = await GetRoles(id);

            String[] newRoles = roles.Split("|");

            for (int i = 0; i < newRoles.Length; i++)
                if (!curRoles.Contains(newRoles[i]))
                    curRoles.Add(newRoles[i]);

            await new WebSysUserDB().UpdateRoles(id, String.Join("|", curRoles));
        }

        public static async Task UpdateRoles(long id, String roles)
        {
            await new WebSysUserDB().UpdateRoles(id, roles);
        }


        // Mail
        public static async Task<String> GetEmailByAppUserId(long appUserId)
        {
            return await new WebSysUserDB().GetEmailById(await new AppUserDB().GetWebSysUserId(appUserId));
        }

        public static async Task SendMailLink(String eMail)
        {
            // Token
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(eMail);
            String username = userRecord.DisplayName ?? eMail[..eMail.IndexOf('@')];
            String token = AesHelper.Encrypt(userRecord.Uid);

            // Send Mail
            String title = "Bienvenido a Héroes Migrantes";
            String text = "Para activar tu cuenta, debes presionar el siguiente botón";
            String button = "Activar tu Cuenta";
            String link = "https://" + System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME").ToLower() + ".azurewebsites.net/services/websysuser/ConfirmMail?token=" + token;
            String body = HtmlHelper.GetButtonMailHtml(title, text, button, link);

            await MailHelper.SendMail(eMail, username, "Activación de tu Cuenta", body, true);
        }

        public static String Encrypt(String text)
        {
            String token;

            try
            {
                //token = AesHelper.Encrypt(text);

                byte[] plaintext = Encoding.UTF8.GetBytes(text);
                token = $"1 : {plaintext?.Length}";
                byte[] encryptedBytes = AesHelper.Aes.CreateEncryptor().TransformFinalBlock(plaintext, 0, plaintext.Length);
                String hex = BaseHelper.BytesToBase16(encryptedBytes);
                token = $"2 : {plaintext?.Length} | {encryptedBytes?.Length} | {hex}";
                BigInteger bigInt = new BigInteger(encryptedBytes, true, true);
                token = $"3 : {plaintext?.Length} | {encryptedBytes?.Length} | {hex} | {bigInt}";
                token = $"4 : {plaintext?.Length} | {encryptedBytes?.Length} | {hex} | {bigInt} | {BaseHelper.BytesToBase16(bigInt.ToByteArray(true, true))} | {BaseHelper.BigIntToBase62(bigInt)}";
            }
            catch (Exception ex)
            {
                return $"Encrypt Error : {ex.Message}";
            }

            return token;
        }

        public static String Decrypt(String token)
        {
            String authUserId = "0";

            try
            {
                //authUserId = AesHelper.Decrypt(token);

                BigInteger bigInt = BaseHelper.Base62ToBigInt(token);
                authUserId = $"1 : {bigInt}";
                byte[] encryptedBytes = bigInt.ToByteArray(true, true);
                if (encryptedBytes.Length < 32)
                {
                    byte[] littleBytes = encryptedBytes;
                    encryptedBytes = new byte[32];
                    littleBytes.CopyTo(encryptedBytes, 32 - littleBytes.Length);
                }
                authUserId = $"2 : {bigInt} | {encryptedBytes?.Length}";
                byte[] decyptedBytes = AesHelper.Aes.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                authUserId = $"3 : {bigInt} | {encryptedBytes?.Length} | {decyptedBytes?.Length}";
                authUserId = $"4 : {bigInt} | {encryptedBytes?.Length} | {decyptedBytes?.Length} | {Encoding.UTF8.GetString(decyptedBytes)}";
            }
            catch (Exception ex)
            {
                return $"Decrypt Error : {ex.Message}\r\n{authUserId}";
            }

            return authUserId;
        }

        public static async Task<ContentResult> ConfirmMail(String token)
        {
            String authUserId;

            try
            {
                authUserId = AesHelper.Decrypt(token);
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No se pudo activar tu cuenta.", "#F00000")
                };
            }

            return await ConfirmAuthUser(authUserId);
        }

        public static async Task<ContentResult> ConfirmAuthUser(String authUserId)
        {
            try
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(authUserId);

                // User Doesn't Exist
                if (userRecord == null)
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No se pudo activar tu usuario.", "#F00000")
                    };
                }

                // Timeout
                if (userRecord.EmailVerified)
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "La solicitud de activación de tu cuenta caducó o ya se usó el vínculo.", "#F00000")
                    };
                }

                // Update Verified
                UserRecordArgs userRecordArgs = new UserRecordArgs()
                {
                    Uid = authUserId,
                    EmailVerified = true
                };

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs);

                // Success
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = HtmlHelper.GetConfirmResultHtml("Bienvenido a Expande", "Tu cuenta fue activada exitosamente.", "#666666")
                };
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No fue posible activar tu cuenta.", "#F00000")
                };
            }
        }

        public static async Task UpdateMail(int webSysUserId, String newEmail)
        {
            String authUserId = (await new WebSysUserDB().GetById(webSysUserId)).AuthUserId;

            UserRecordArgs userRecordArgs = new UserRecordArgs()
            {
                Uid = authUserId,
                Email = newEmail
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs);
            await new WebSysUserDB().UpdateMail(webSysUserId, newEmail);
        }

        public static async Task UpdatePhone(PhoneRequest phoneRequest)
        {
            await new WebSysUserDB().UpdatePhone(phoneRequest);
        }

        // Password
        public static async Task<int> SetPassword(WebSysPasswordRequest passwordRequest)
        {
            WebSysUser webSysUser = await new WebSysUserDB().GetById(passwordRequest.WebSysUserId);
            if (webSysUser == null)
                return -1;

            UserRecordArgs userRecordArgs = new UserRecordArgs()
            {
                Uid = webSysUser.AuthUserId,
                Password = passwordRequest.Password
            };

            try
            {
                await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public static async Task ResetPassword(String eMail)
        {
            // Token
            UserRecord userRecord;
            try
            {
                userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(eMail);
            }
            catch
            {
                throw new Exception("El Correo ingresado no existe.");
            }

            String username = userRecord.DisplayName ?? eMail[..eMail.IndexOf('@')];
            String token = AesHelper.Encrypt($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}|{userRecord.Uid}");

            // Send Mail
            String title = "Reinicio de Contraseña";
            String text = "Para reiniciar tu contraseña, debes presionar el siguiente botón";
            String button = "Reiniciar Contraseña";
            String link = "https://" + System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME").ToLower() + ".azurewebsites.net/services/websysuser/EnterPassword?token=" + token;
            String body = HtmlHelper.GetButtonMailHtml(title, text, button, link);

            await MailHelper.SendMail(eMail, username, "Reinicio de tu Contraseña", body, true);
        }

        public static ContentResult EnterPassword(String token)
        {
            try
            {
                // Token
                String authUserId;
                DateTime datetime;
                try
                {
                    String[] data = AesHelper.Decrypt(token).Split('|');
                    authUserId = data[1];
                    datetime = WebHelper.ConvertDateTime(data[0]);
                }
                catch
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No se pudo reiniciar tu contraseña..", "#F00000")
                    };
                }

                // Timeout
                if ((DateTime.Now - datetime).TotalMinutes > AesHelper.AesDelay)
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "La solicitud de reinicio de tu contraseña caducó o ya se usó el vínculo.", "#F00000")
                    };
                }

                // Send Html
                token = AesHelper.Encrypt($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}|{authUserId}");

                String title = "Nueva Contrase&ntilde;a";
                String text = "Tu Contrase&ntilde;a debe incluir al menos una min&uacute;scula, una may&uacute;scula, un n&uacute;mero y un car&aacute;cter especial (por ejemplo : !\"~#@\\$%?&*)";
                String pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#~$\!%*?&" + "\"" + @"])[A-Za-z\d@#~$\!%*?&" + "\"" + @"]{8,}$";
                String placeholder = "Contrase&ntilde;a";
                String button = "Reiniciar tu Contrase&ntilde;a";
                String link = "https://" + System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME").ToLower() + ".azurewebsites.net/services/websysuser/ChangePassword?token=" + token;

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = HtmlHelper.GetSingleInputHtml(title, text, "password", pattern, placeholder, button, link)
                };
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No fue posible reiniciar tu contraseña.", "#F00000")
                };
            }
        }

        public static async Task<ContentResult> ChangePassword(String token, String body)
        {
            try
            {
                // Token
                String authUserId;
                DateTime datetime;
                try
                {
                    String[] data = AesHelper.Decrypt(token).Split('|');
                    authUserId = data[1];
                    datetime = WebHelper.ConvertDateTime(data[0]);
                }
                catch
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No se pudo reiniciar tu contraseña.", "#F00000")
                    };
                }

                // Timeout
                if ((DateTime.Now - datetime).TotalMinutes > AesHelper.AesDelay)
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "La solicitud de reinicio de tu contraseña caducó o ya se usó el vínculo.", "#F00000")
                    };
                }

                // Update Password
                UserRecordArgs userRecordArgs = new UserRecordArgs()
                {
                    Uid = authUserId,
                    Password = body[(body.IndexOf('=') + 1)..].Replace("\r", "").Replace("\n", "")
                };

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs);

                // Success
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = HtmlHelper.GetConfirmResultHtml("Contraseña reiniciada", "Tu contraseña fue reiniciada exitosamente.", "#666666")
                };
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No fue posible reiniciar tu contraseña.", "#F00000")
                };
            }
        }

        // Pin
        public static async Task<String> VerifyPin(WebSysPinRequest pinRequest)
        {
            (String pin, int pinFails, DateTime? pinDateTime) = await new WebSysUserDB().GetPin(pinRequest.WebSysUserId);
            if (pin == null)
                return null;

            if (pinFails >= PinFailsMax)
            {
                double minutes = (DateTime.Now - (DateTime)pinDateTime).TotalMinutes;
                if (minutes < PinLockDelay)
                    return "LOCKED|" + ((PinLockDelay - minutes) * 60.0).ToString("F0", CultureInfo.InvariantCulture);

                pinFails = 0;
                await new WebSysUserDB().UpdatePinFails(pinRequest.WebSysUserId, pinFails);
            }

            if (pin == pinRequest.Pin)
            {
                if (pinFails > 0)
                {
                    pinFails = 0;
                    await new WebSysUserDB().UpdatePinFails(pinRequest.WebSysUserId, pinFails);
                }
                return "OK|0";
            }

            pinFails++;
            if (pinFails >= PinFailsMax)
            {
                await new WebSysUserDB().UpdatePinFails(pinRequest.WebSysUserId, pinFails, DateTime.Now);
                return "FAILED|" + (PinLockDelay * 60.0).ToString("F0", CultureInfo.InvariantCulture);
            }

            await new WebSysUserDB().UpdatePinFails(pinRequest.WebSysUserId, pinFails);
            return "TRIED|" + (PinFailsMax - pinFails).ToString();
        }

        public static async Task<bool> SetPin(WebSysPinRequest pinRequest)
        {
            return await new WebSysUserDB().UpdatePin(pinRequest);
        }

        public static async Task ResetPin(long webSysUserId)
        {
            // Token
            long appUserId = await new AppUserDB().GetIdByWebSysUserId(webSysUserId);
            String email = await new WebSysUserDB().GetEmailById(webSysUserId);

            (String firstName1, String _1, String lastName1, String _2) = await new IdentityDB().GetFullNameByAppUserId(appUserId);

            String name;
            if (firstName1 == null)
                name = email[..email.IndexOf('@')];
            else
                name = firstName1 + " " + lastName1;

            String token = AesHelper.Encrypt($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}|{webSysUserId}|{email}");

            // Send Mail
            String title = "Reinicio de Pin";
            String text = "Para reiniciar tu pin, debes presionar el siguiente botón";
            String button = "Reiniciar Pin";
            String link = "https://" + System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME").ToLower() + ".azurewebsites.net/services/websysuser/EnterPin?token=" + token;
            String body = HtmlHelper.GetButtonMailHtml(title, text, button, link);

            await MailHelper.SendMail(email, name, "Reinicio de tu Pin", body, true);
        }

        public static ContentResult EnterPin(String token)
        {
            try
            {
                // Token
                int webSysUserId;
                String eMail;
                DateTime datetime;
                try
                {
                    String[] data = AesHelper.Decrypt(token).Split('|');
                    webSysUserId = Convert.ToInt32(data[1]);
                    eMail = data[2];
                    datetime = WebHelper.ConvertDateTime(data[0]);
                }
                catch
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No se pudo reiniciar tu pin.", "#F00000")
                    };
                }

                // Timeout
                if ((DateTime.Now - datetime).TotalMinutes > AesHelper.AesDelay)
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "La solicitud de reinicio de tu pin caducó o ya se usó el vínculo.", "#F00000")
                    };
                }

                // Send Html
                token = AesHelper.Encrypt($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}|{webSysUserId}|{eMail}");

                String title = "Nuevo Pin";
                String text = "Tu Pin debe incluir exactamente 4 digitos";
                String pattern = @"[0-9]{4}";
                String placeholder = "Pin";
                String button = "Reiniciar tu Pin";
                String link = "https://" + System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME").ToLower() + ".azurewebsites.net/services/websysuser/ChangePin?token=" + token;

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = HtmlHelper.GetSingleInputHtml(title, text, "password", pattern, placeholder, button, link)
                };
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No fue posible reiniciar tu pin.", "#F00000")
                };
            }
        }

        public static async Task<ContentResult> ChangePin(String token, String body)
        {
            try
            {
                // Token
                int webSysUserId;
                String eMail;
                DateTime datetime;
                try
                {
                    String[] data = AesHelper.Decrypt(token).Split('|');
                    webSysUserId = Convert.ToInt32(data[1]);
                    eMail = data[2];
                    datetime = WebHelper.ConvertDateTime(data[0]);
                }
                catch
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No se pudo reiniciar tu pin.", "#F00000")
                    };
                }

                // Timeout
                if ((DateTime.Now - datetime).TotalMinutes > AesHelper.AesDelay)
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "La solicitud de reinicio de tu pin caducó o ya se usó el vínculo.", "#F00000")
                    };
                }

                // Update Pin
                String newPin = body[(body.IndexOf('=') + 1)..].Replace("\r", "").Replace("\n", "");

                if (!await SetPin(new WebSysPinRequest(webSysUserId, newPin)))
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No fue posible reiniciar tu pin.", "#F00000")
                    };
                }

                // Success
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = HtmlHelper.GetConfirmResultHtml("Pin Reset", "Tu pin fue reiniciado exitosamente.", "#666666")
                };
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", "No fue posible reiniciar tu pin.", "#F00000")
                };
            }
        }

        // DELETE
        public static async Task DeleteById(long id, bool delAuthUser = true)
        {
            await new WebSysUserDB().DeleteById(id);

            if (!delAuthUser)
                return;

            String authUserId = await new WebSysUserDB().GetAuthUserIdById(id);
            if (authUserId == null)
                return;

            try
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(authUserId);
            }
            catch
            {
                throw new Exception("Delete AuthUserId " + authUserId);
            }

        }
    }
}
