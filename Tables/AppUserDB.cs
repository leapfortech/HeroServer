using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class AppUserDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-AppUser]";

        public static AppUser GetAppUser(SqlDataReader reader)
        {
            return new AppUser(Convert.ToInt32(reader["Id"]),
                               reader["Code"].ToString(),
                               Convert.ToInt32(reader["WebSysUserId"]),
                               reader["CSToken"].ToString(),
                               Convert.ToInt32(reader["News"]),
                               Convert.ToInt32(reader["Options"]),
                               Convert.ToInt32(reader["ReferrerAppUserId"]),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["AppUserStatusId"]));
        }

        public static AppUserNamed GetAppUserNamed(SqlDataReader reader)
        {
            return new AppUserNamed(Convert.ToInt32(reader["Id"]),
                                    reader["Code"].ToString(),
                                    Convert.ToInt32(reader["WebSysUserId"]),
                                    reader["FirstName1"].ToString(),
                                    reader["FirstName2"].ToString(),
                                    reader["LastName1"].ToString(),
                                    reader["LastName2"].ToString(),
                                    reader["LastNameMarried"].ToString(),
                                    reader["Email"].ToString(),
                                    Convert.ToInt32(reader["PhoneCountryId"]),
                                    reader["Phone"].ToString(),
                                    reader["CSToken"].ToString(),
                                    Convert.ToInt32(reader["Options"]),
                                    Convert.ToInt32(reader["ReferrerAppUserId"]),
                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                    Convert.ToDateTime(reader["UpdateDateTime"]),
                                    Convert.ToInt32(reader["AppUserStatusId"]));
        }

        public static AppUserFull GetAppUserFull(SqlDataReader reader)
        {
            return new AppUserFull(Convert.ToInt32(reader["Id"]),
                                   reader["AuthUserId"].ToString(),
                                   reader["Email"].ToString(),
                                   reader["PhonePrefix"].ToString(),
                                   reader["Phone"].ToString(),
                                   Convert.ToDateTime(reader["CreateDateTime"]),
                                   Convert.ToDateTime(reader["UpdateDateTime"]),
                                   Convert.ToInt32(reader["AppUserStatusId"]));
        }

        // SELECT
        public async Task<List<AppUserNamed>> GetNamed(int count, int page)
        {
            String strCmd = $"SELECT {table}.*, FirstName1, FirstName2, LastName1, LastName2, LastNameMarried, Email, PhoneCountryId, Phone FROM {table}" +
                            " INNER JOIN [DD-Identity]" +
                            $" ON {table}.Id = [DD-Identity].AppUserId AND [DD-Identity].Status = 1";
            if (count > 0 && page > 0)
            {
                strCmd += " ORDER BY WebSysUserId";
                strCmd += " OFFSET @Offset ROWS";
                strCmd += " FETCH NEXT @Count ROWS ONLY";
            }

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (count > 0 && page > 0)
            {
                DBHelper.AddParam(command, "@Offset", SqlDbType.Int, (page - 1) * count);
                DBHelper.AddParam(command, "@Count", SqlDbType.Int, count);
            }

            List<AppUserNamed> appUsers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        AppUserNamed appUserNamed = GetAppUserNamed(reader);
                        appUsers.Add(appUserNamed);
                    }
                }
            }
            return appUsers;
        }

        public async Task<List<AppUserNamed>> GetNamedByStatus(int appUserStatusId, int count, int page)
        {
            String strCmd = $"SELECT {table}.*, FirstName1, FirstName2, LastName1, LastName2, LastNameMarried, Email, PhoneCountryId, Phone FROM {table}" +
                            " INNER JOIN [DD-Identity]" +
                            $" ON {table}.Id = [DD-Identity].AppUserId AND [DD-Identity].Status = 1";
            if (appUserStatusId >= 0)
                strCmd += " WHERE AppUserStatusId = @AppUserStatusId";
            else
                strCmd += " WHERE AppUserStatusId >= @AppUserStatusId";
            if (count > 0 && page > 0)
            {
                strCmd += " ORDER BY WebSysUserId";
                strCmd += " OFFSET @Offset ROWS";
                strCmd += " FETCH NEXT @Count ROWS ONLY";
            }

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (appUserStatusId >= 0)
                DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, appUserStatusId);
            else
                DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, -appUserStatusId);
            if (count > 0 && page > 0)
            {
                DBHelper.AddParam(command, "@Offset", SqlDbType.Int, (page - 1) * count);
                DBHelper.AddParam(command, "@Count", SqlDbType.Int, count);
            }

            List<AppUserNamed> appUsersNamed = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        AppUserNamed appUserNamed = GetAppUserNamed(reader);
                        appUsersNamed.Add(appUserNamed);
                    }
                }
            }
            return appUsersNamed;
        }

        public async Task<AppUserNamed> GetNamedById(int id)
        {
            String strCmd = $"SELECT {table}.*, FirstName1, FirstName2, LastName1, LastName2, LastNameMarried, Email, PhoneCountryId, Phone FROM {table}" +
                            $" INNER JOIN [DD-Identity] ON {table}.Id = [DD-Identity].AppUserId AND [DD-Identity].Status = 1" +
                            $" WHERE {table}.Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            AppUserNamed appUserNamed = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUserNamed = GetAppUserNamed(reader);
                    }
                }
            }
            return appUserNamed;
        }

        public async Task<List<AppUserFull>> GetFullByStatus(int status)
        {
            String strCmd = "SELECT AppUser.Id, WebSysUser.AuthUserId, WebSysUser.Email, KPhoneCountry.PhonePrefix, WebSysUser.Phone," +
                            " AppUser.CreateDateTime, AppUser.UpdateDateTime, AppUser.AppUserStatusId" +
                            $" FROM {table} AS AppUser" +
                            " INNER JOIN [DD-WebSysUser] AS WebSysUser ON (WebSysUser.Id = AppUser.WebSysUserId)" +
                            " INNER JOIN [K-Country] AS KPhoneCountry ON (KPhoneCountry.Id = WebSysUser.PhoneCountryId)" +
                            " WHERE AppUser.AppUserStatusId = @AppUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, status);

            List<AppUserFull> appUserFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        AppUserFull webSysUserFull = GetAppUserFull(reader);
                        appUserFulls.Add(webSysUserFull);
                    }
                }
            }

            return appUserFulls;
        }

        public async Task<int> GetCountAll()
        {
            String strCmd = $"SELECT COUNT(Id) Count FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            int count = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        count = Convert.ToInt32(reader["Count"]);
                    }
                }
            }

            return count;
        }

        public async Task<int> GetCountByStatus(int appUserStatusId)
        {
            String strCmd = $"SELECT COUNT(AppUserStatusId) Count FROM {table}";
            if (appUserStatusId >= 0)
                strCmd += " WHERE AppUserStatusId = @AppUserStatusId";
            else
                strCmd += " WHERE AppUserStatusId >= @AppUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            int count = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        count = Convert.ToInt32(reader["Count"]);
                    }
                }
            }

            return count;
        }

        public async Task<AppUser> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            AppUser appUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUser = GetAppUser(reader);
                    }
                }
            }
            return appUser;
        }

        public async Task<AppUser> GetByIdStatus(int id, int appUserStatusId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND AppUserStatusId = @AppUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);
            DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, appUserStatusId);

            AppUser appUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUser = GetAppUser(reader);
                    }
                }
            }
            return appUser;
        }

        public async Task<int> GetIdByAuthUserId(String authUserId)
        {
            String strCmd = $"SELECT {table}.Id FROM {table}" +
                            $" INNER JOIN [DD-WebSysUser] ON ([DD-WebSysUser].Id = {table}.WebSysUserId)" +
                             " WHERE [DD-WebSysUser].AuthUserId = @AuthUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AuthUserId", SqlDbType.VarChar, authUserId);

            int id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt32(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<AppUser> GetByWebSysUserId(int webSysUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            AppUser appUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUser = GetAppUser(reader);
                    }
                }
            }
            return appUser;
        }

        public async Task<int> GetWebSysUserId(int id)
        {
            String strCmd = $"SELECT WebSysUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int webSysUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysUserId = Convert.ToInt32(reader["WebSysUserId"]);
                    }
                }
            }

            return webSysUserId;
        }

        public async Task<int> GetIdByWebSysUserId(int webSysUserId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            int id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt32(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<int> GetIdByEmail(String eMail)
        {
            String strCmd = $"SELECT {table}.Id FROM {table}" +
                            $" INNER JOIN [DD-WebSysUser] ON ([DD-WebSysUser].Id = {table}.WebSysUserId)" +
                             " WHERE [DD-WebSysUser].Email = @Email";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, eMail);

            int id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt32(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<AppUser> GetByCSToken(String csToken)
        {
            String strCmd = $"SELECT * FROM {table} WHERE CSToken = @CSToken";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CSToken", SqlDbType.VarChar, csToken);

            AppUser appUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUser = GetAppUser(reader);
                    }
                }
            }

            return appUser;
        }

        public async Task<List<(int, String)>> GetMailByCSTokenNull()
        {
            String strCmd = $"SELECT {table}.Id, Email FROM {table} INNER JOIN [DD-WebSysUser] ON [DD-AppUser].WebSysUserId = [DD-WebSysUser].Id WHERE CSToken IS NULL";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<(int, String)> appUserMails = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        appUserMails.Add((Convert.ToInt32(reader["Id"]), reader["Email"].ToString()));
                    }
                }
            }

            return appUserMails;
        }

        public async Task<int> GetOptions(int id)
        {
            String strCmd = $"SELECT Options FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int options = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        options = Convert.ToInt32(reader["Options"]);
                    }
                }
            }

            return options;
        }

        // INSERT
        public async Task<int> Add(AppUser appUser)
        {
            String strCmd = $"INSERT INTO {table}(Code, WebSysUserId, CSToken, News, Options, ReferrerAppUserId, CreateDateTime, UpdateDateTime, AppUserStatusId)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Code, @WebSysUserId, @CSToken, @News, @Options, @ReferrerAppUserId, @CreateDateTime, @UpdateDateTime, @AppUserStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, appUser.Code);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, appUser.WebSysUserId);
            DBHelper.AddParam(command, "@CSToken", SqlDbType.VarChar, appUser.CSToken);
            DBHelper.AddParam(command, "@News", SqlDbType.Int, appUser.News);
            DBHelper.AddParam(command, "@Options", SqlDbType.Int, appUser.Options);
            DBHelper.AddParam(command, "@ReferrerAppUserId", SqlDbType.Int, appUser.ReferrerAppUserId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, appUser.AppUserStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(AppUser appUser)
        {
            String strCmd = $"UPDATE {table} SET Code = @Code, WebSysUserId = @WebSysUserId, CSToken = @CSToken, News = @News, Options = @Options, ReferrerAppUserId = @ReferrerAppUserId," +
                            " UpdateDateTime = @UpdateDateTime, AppUserStatusId = @AppUserStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, appUser.Code);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, appUser.WebSysUserId);
            DBHelper.AddParam(command, "@CSToken", SqlDbType.VarChar, appUser.CSToken);
            DBHelper.AddParam(command, "@News", SqlDbType.Int, appUser.News);
            DBHelper.AddParam(command, "@Options", SqlDbType.Int, appUser.Options);
            DBHelper.AddParam(command, "@ReferrerAppUserId", SqlDbType.Int, appUser.ReferrerAppUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, appUser.AppUserStatusId);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, appUser.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateCSToken(int id, String csToken)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET CSToken = @CSToken, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CSToken", SqlDbType.VarChar, csToken);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateOptions(int id, int options)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Options = @Options, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Options", SqlDbType.Int, options);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int appUserStatusId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, AppUserStatusId = @AppUserStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, appUserStatusId);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByWebSysUserId(int webSysUserId, int appUserStatusId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, AppUserStatusId = @AppUserStatusId" +
                            " WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@AppUserStatusId", SqlDbType.Int, appUserStatusId);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateReferredAppUserId(int id, int referrerAppUserId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, ReferrerAppUserId = @ReferrerAppUserId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@ReferrerAppUserId", SqlDbType.Int, referrerAppUserId);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        // DELETE
        public async Task<int> DeleteAll()
        {
            String strCmd = $"DELETE {table}";
            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteById(int id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteByWebSysUserId(int webSysUserid)
        {
            String strCmd = $"DELETE {table} WHERE WebSysUserid = @WebSysUserid";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserid", SqlDbType.Int, webSysUserid);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
