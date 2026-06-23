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
        readonly String table = "[D-AppUser]";

        public static AppUser GetAppUser(SqlDataReader reader)
        {
            return new AppUser(Convert.ToInt64(reader["Id"]),
                               Convert.ToInt64(reader["WebSysUserId"]),
                               reader["Alias"].ToString(),
                               reader["ReferringCode"].ToString(),
                               reader["CSToken"].ToString(),
                               Convert.ToInt64(reader["Options"]),
                               Convert.ToInt64(reader["ReferrerAppUserId"]),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["AppUserStatusId"]));
        }

        public static AppUserNamed GetAppUserNamed(SqlDataReader reader)
        {
            return new AppUserNamed(Convert.ToInt64(reader["Id"]),
                                    Convert.ToInt64(reader["WebSysUserId"]),
                                    reader["Alias"].ToString(),
                                    reader["ReferringCode"].ToString(),
                                    reader["FirstName1"].ToString(),
                                    reader["FirstName2"].ToString(),
                                    reader["LastName1"].ToString(),
                                    reader["LastName2"].ToString(),
                                    reader["Email"].ToString(),
                                    Convert.ToInt64(reader["PhoneCountryId"]),
                                    reader["Phone"].ToString(),
                                    reader["CSToken"].ToString(),
                                    Convert.ToInt64(reader["Options"]),
                                    Convert.ToInt64(reader["ReferrerAppUserId"]),
                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                    Convert.ToDateTime(reader["UpdateDateTime"]),
                                    Convert.ToInt32(reader["AppUserStatusId"]));
        }

        public static AppUserFull GetAppUserFull(SqlDataReader reader)
        {
            return new AppUserFull(Convert.ToInt64(reader["Id"]),
                                   reader["AuthUserId"].ToString(),
                                   reader["Alias"].ToString(),
                                   reader["ReferringCode"].ToString(),
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
            String strCmd = $"SELECT {table}.*, FirstName1, FirstName2, LastName1, LastName2, Email, PhoneCountryId, Phone FROM {table}" +
                            " INNER JOIN [D-Identity]" +
                            $" ON {table}.Id = [D-Identity].AppUserId AND [D-Identity].Status = 1";
            if (count > 0 && page > 0)
            {
                strCmd += " ORDER BY WebSysUserId";
                strCmd += " OFFSET @Offset ROWS";
                strCmd += " FETCH NEXT @Count ROWS ONLY";
            }

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (count > 0 && page > 0)
            {
                command.AddParam("@Offset", SqlDbType.Int, (page - 1) * count);
                command.AddParam("@Count", SqlDbType.Int, count);
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
            String strCmd = $"SELECT {table}.*, FirstName1, FirstName2, LastName1, LastName2, Email, PhoneCountryId, Phone FROM {table}" +
                            " INNER JOIN [D-Identity]" +
                            $" ON {table}.Id = [D-Identity].AppUserId AND [D-Identity].Status = 1";
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
                command.AddParam("@AppUserStatusId", SqlDbType.Int, appUserStatusId);
            else
                command.AddParam("@AppUserStatusId", SqlDbType.Int, -appUserStatusId);
            if (count > 0 && page > 0)
            {
                command.AddParam("@Offset", SqlDbType.Int, (page - 1) * count);
                command.AddParam("@Count", SqlDbType.Int, count);
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

        public async Task<AppUserNamed> GetNamedById(long id)
        {
            String strCmd = $"SELECT {table}.*, FirstName1, FirstName2, LastName1, LastName2, Email, PhoneCountryId, Phone FROM {table}" +
                            $" INNER JOIN [D-Identity] ON {table}.Id = [D-Identity].AppUserId AND [D-Identity].Status = 1" +
                            $" WHERE {table}.Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

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
            String strCmd = "SELECT AppUser.Id, WebSysUser.AuthUserId, AppUser.Alias, AppUser.ReferringCode, WebSysUser.Email, KPhoneCountry.PhonePrefix, WebSysUser.Phone," +
                            " AppUser.CreateDateTime, AppUser.UpdateDateTime, AppUser.AppUserStatusId" +
                            $" FROM {table} AS AppUser" +
                            " INNER JOIN [D-WebSysUser] AS WebSysUser ON (WebSysUser.Id = AppUser.WebSysUserId)" +
                            " LEFT JOIN [K-Country] AS KPhoneCountry ON (KPhoneCountry.Id = WebSysUser.PhoneCountryId)" +
                            " WHERE AppUser.Options = 1 AND AppUser.AppUserStatusId = @AppUserStatusId";
                            // AppUser.Options = 1 - Onboarding == 1

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserStatusId", SqlDbType.Int, status);

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

        public async Task<UserInfoAllRsp> GetUserInfoAllByAlias(UserInfoAllByAlias req)
        {
            req.Page = Math.Max(1, req.Page);
            req.PageSize = Math.Max(1, req.PageSize);

            int offset = (req.Page - 1) * req.PageSize;

            List<UserInfo> userInfos = new List<UserInfo>();
            List<AppUserFull> appUserFulls = new List<AppUserFull>();
            Dictionary<long, IdentityFull> identityByAppUserId = new Dictionary<long, IdentityFull>();
            Dictionary<long, AddressFull> addressByAppUserId = new Dictionary<long, AddressFull>();

            String strCmd = // Count
                            @"SELECT COUNT(AppUser.Id) AS TotalCount
                              FROM [D-AppUser] AS AppUser
                              INNER JOIN [D-WebSysUser] AS WebSysUser ON WebSysUser.Id = AppUser.WebSysUserId
                              WHERE (@Status = -1 OR AppUser.AppUserStatusId = @Status)
                              AND (@Alias IS NULL OR AppUser.Alias LIKE '%' + @Alias + '%');" +

                            // AppUser
                            @"SELECT 
                                AppUser.Id,
                                WebSysUser.AuthUserId,
                                AppUser.Alias,
                                AppUser.ReferringCode, 
                                WebSysUser.Email,
                                KPhoneCountry.PhonePrefix,
                                WebSysUser.Phone,
                                AppUser.CreateDateTime,
                                AppUser.UpdateDateTime,
                                AppUser.AppUserStatusId
                              FROM [D-AppUser] AS AppUser
                              INNER JOIN [D-WebSysUser] AS WebSysUser ON WebSysUser.Id = AppUser.WebSysUserId
                              LEFT JOIN [K-Country] AS KPhoneCountry ON KPhoneCountry.Id = WebSysUser.PhoneCountryId
                              WHERE (@Status = -1 OR AppUser.AppUserStatusId = @Status)
                              AND (@Alias IS NULL OR AppUser.Alias LIKE '%' + @Alias + '%')
                              ORDER BY AppUser.CreateDateTime DESC
                              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;" +

                            // Identity
                            @"SELECT 
                                Idt.Id,
                                Idt.FirstName1,
                                Idt.FirstName2,
                                Idt.LastName1,
                                Idt.LastName2,
                                KGender.Name AS Gender,
                                Idt.BirthDate,
                                KCountry.Name AS BirthCountry,
                                KState.Name AS BirthState,
                                KCity.Name AS BirthCity,
                                KPhoneCountry.PhonePrefix,
                                Idt.Phone,
                                Idt.Email,
                                Idt.CreateDateTime,
                                Idt.UpdateDateTime,
                                AppUser.AppUserStatusId,
                                Idt.Status,
                                IAU.AppUserId
                            FROM [D-Identity] AS Idt
                            LEFT JOIN [K-Gender] AS KGender ON KGender.Id = Idt.GenderId
                            LEFT JOIN [K-Country] AS KCountry ON KCountry.Id = Idt.BirthCountryId
                            LEFT JOIN [K-State] AS KState ON KState.Id = Idt.BirthStateId
                            LEFT JOIN [K-City] AS KCity ON KCity.Id = Idt.BirthCityId
                            LEFT JOIN [K-Country] AS KPhoneCountry ON KPhoneCountry.Id = Idt.PhoneCountryId
                            INNER JOIN [J-IdentityAppUser] AS IAU ON IAU.IdentityId = Idt.Id AND IAU.Status = 1
                            INNER JOIN [D-AppUser] AS AppUser ON AppUser.Id = IAU.AppUserId
                            WHERE Idt.Status = 1
                            AND IAU.AppUserId IN (
                                SELECT AppUser.Id 
                                FROM [D-AppUser] AS AppUser
                                WHERE (@Status = -1 OR AppUser.AppUserStatusId = @Status)
                                AND (@Alias IS NULL OR AppUser.Alias LIKE '%' + @Alias + '%')
                                ORDER BY AppUser.CreateDateTime DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY);" +

                            // Address
                            @"SELECT 
                                Adr.Id,
                                KCountry.Name AS Country,
                                KState.Name AS State,
                                KCity.Name AS City,
                                Adr.Address1,
                                Adr.Address2,
                                Adr.Zone,
                                Adr.ZipCode,
                                Adr.Latitude,
                                Adr.Longitude,
                                Adr.Status,
                                JAA.AppUserId
                            FROM [D-Address] AS Adr
                            LEFT JOIN [K-Country] AS KCountry ON KCountry.Id = Adr.CountryId
                            LEFT JOIN [K-State] AS KState ON KState.Id = Adr.StateId
                            LEFT JOIN [K-City] AS KCity ON KCity.Id = Adr.CityId
                            INNER JOIN [J-AddressAppUser] AS JAA ON JAA.AddressId = Adr.Id AND JAA.Status = 1
                            WHERE Adr.Status = 1
                            AND JAA.AppUserId IN (
                                SELECT AppUser.Id 
                                FROM [D-AppUser] AS AppUser
                                WHERE (@Status = -1 OR AppUser.AppUserStatusId = @Status)
                                AND (@Alias IS NULL OR AppUser.Alias LIKE '%' + @Alias + '%')
                                ORDER BY AppUser.CreateDateTime DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY);";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Status", SqlDbType.Int, req.Status);
            command.AddParam("@Alias", SqlDbType.VarChar, string.IsNullOrWhiteSpace(req.Alias) ? DBNull.Value : req.Alias);
            command.AddParam("@Offset", SqlDbType.Int, offset);
            command.AddParam("@PageSize", SqlDbType.Int, req.PageSize);

            int totalCount = 0;

            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // 1. Count
                    if (await reader.ReadAsync())
                        totalCount = Convert.ToInt32(reader["TotalCount"]);

                    int totalPages = (int)Math.Ceiling((double)totalCount / req.PageSize);

                    // 2. AppUser
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        AppUserFull appUser = AppUserDB.GetAppUserFull(reader);
                        appUserFulls.Add(appUser);
                    }

                    // 3. Identity
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        IdentityFull identity = IdentityDB.GetIdentityFull(reader);
                        long appUserId = Convert.ToInt64(reader["AppUserId"]);

                        if (!identityByAppUserId.ContainsKey(appUserId))
                            identityByAppUserId.Add(appUserId, identity);
                    }

                    // 4. Address
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        AddressFull address = AddressDB.GetAddressFull(reader);
                        long appUserId = Convert.ToInt64(reader["AppUserId"]);

                        if (!addressByAppUserId.ContainsKey(appUserId))
                            addressByAppUserId.Add(appUserId, address);
                    }

                    // Result
                    for (int i = 0; i < appUserFulls.Count; i++)
                    {
                        AppUserFull appUser = appUserFulls[i];

                        IdentityFull identity = null;
                        AddressFull address = null;

                        if (identityByAppUserId.ContainsKey(appUser.Id))
                            identity = identityByAppUserId[appUser.Id];

                        if (addressByAppUserId.ContainsKey(appUser.Id))
                            address = addressByAppUserId[appUser.Id];

                        userInfos.Add(new UserInfo(appUser, identity, address));
                    }

                    return new UserInfoAllRsp(req.Page, totalPages, userInfos);
                }
            }
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

        public async Task<AppUser> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

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

        public async Task<AppUser> GetByIdStatus(long id, int appUserStatusId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND AppUserStatusId = @AppUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@AppUserStatusId", SqlDbType.Int, appUserStatusId);

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

        public async Task<long> GetIdByAuthUserId(String authUserId)
        {
            String strCmd = $"SELECT {table}.Id FROM {table}" +
                            $" INNER JOIN [D-WebSysUser] ON ([D-WebSysUser].Id = {table}.WebSysUserId)" +
                             " WHERE [D-WebSysUser].AuthUserId = @AuthUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AuthUserId", SqlDbType.VarChar, authUserId);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<AppUser> GetByWebSysUserId(long webSysUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@WebSysUserId", SqlDbType.BigInt, webSysUserId);

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

        public async Task<long> GetWebSysUserId(long id)
        {
            String strCmd = $"SELECT WebSysUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            long webSysUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysUserId = Convert.ToInt64(reader["WebSysUserId"]);
                    }
                }
            }

            return webSysUserId;
        }

        public async Task<long> GetIdByWebSysUserId(long webSysUserId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@WebSysUserId", SqlDbType.BigInt, webSysUserId);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<long> GetIdByEmail(String eMail)
        {
            String strCmd = $"SELECT {table}.Id FROM {table}" +
                            $" INNER JOIN [D-WebSysUser] ON ([D-WebSysUser].Id = {table}.WebSysUserId)" +
                             " WHERE [D-WebSysUser].Email = @Email";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Email", SqlDbType.VarChar, eMail);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<AppUser> GetByCSToken(String csToken)
        {
            String strCmd = $"SELECT * FROM {table} WHERE CSToken = @CSToken";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@CSToken", SqlDbType.VarChar, csToken);

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
            String strCmd = $"SELECT {table}.Id, Email FROM {table} INNER JOIN [D-WebSysUser] ON [D-AppUser].WebSysUserId = [D-WebSysUser].Id WHERE CSToken IS NULL";

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

        public async Task<String> GetMailByAlias(String alias, int status = 1)
        {
            String strCmd = $"SELECT Email FROM {table}" +
                            $" INNER JOIN [D-WebSysUser] ON {table}.WebSysUserId = [D-WebSysUser].Id" +
                            $" WHERE {table}.Alias = @Alias AND {table}.AppUserStatusId = @AppUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Alias", SqlDbType.VarChar, alias);
            command.AddParam("@AppUserStatusId", SqlDbType.Int, status);

            String email = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        email = reader["Email"].ToString();
                    }
                }
            }

            return email;
        }

        public async Task<AppUser> GetByAlias(String alias, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Alias = @Alias AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Alias", SqlDbType.VarChar, alias);
            command.AddParam("@Status", SqlDbType.Int, status);

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

        public async Task<long> GetOptions(long id)
        {
            String strCmd = $"SELECT Options FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            long options = -1;
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
        public async Task<long> Add(AppUser appUser)
        {
            String strCmd = $"INSERT INTO {table}(Id, WebSysUserId, Alias, ReferringCode, CSToken, Options, ReferrerAppUserId, CreateDateTime, UpdateDateTime, AppUserStatusId)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @WebSysUserId, @Alias, @ReferringCode, @CSToken, @Options, @ReferrerAppUserId, @CreateDateTime, @UpdateDateTime, @AppUserStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('A'));
            command.AddParam("@WebSysUserId", SqlDbType.BigInt, appUser.WebSysUserId);
            command.AddParam("@Alias", SqlDbType.VarChar, appUser.Alias);
            command.AddParam("@ReferringCode", SqlDbType.VarChar, appUser.ReferringCode);
            command.AddParam("@CSToken", SqlDbType.VarChar, appUser.CSToken);
            command.AddParam("@Options", SqlDbType.BigInt, appUser.Options);
            command.AddParam("@ReferrerAppUserId", SqlDbType.BigInt, appUser.ReferrerAppUserId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@AppUserStatusId", SqlDbType.Int, appUser.AppUserStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(AppUser appUser)
        {
            String strCmd = $"UPDATE {table} SET WebSysUserId = @WebSysUserId, Alias = @Alias, ReferringCode = @ReferringCode, CSToken = @CSToken, Options = @Options, ReferrerAppUserId = @ReferrerAppUserId," +
                            " UpdateDateTime = @UpdateDateTime, AppUserStatusId = @AppUserStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@WebSysUserId", SqlDbType.BigInt, appUser.WebSysUserId);
            command.AddParam("@Alias", SqlDbType.VarChar, appUser.Alias);
            command.AddParam("@ReferringCode", SqlDbType.VarChar, appUser.ReferringCode);
            command.AddParam("@CSToken", SqlDbType.VarChar, appUser.CSToken);
            command.AddParam("@Options", SqlDbType.BigInt, appUser.Options);
            command.AddParam("@ReferrerAppUserId", SqlDbType.BigInt, appUser.ReferrerAppUserId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@AppUserStatusId", SqlDbType.Int, appUser.AppUserStatusId);
            command.AddParam("@Id", SqlDbType.BigInt, appUser.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateReferringCode(long id, String referringCode)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET ReferringCode = @ReferringCode, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@ReferringCode", SqlDbType.VarChar, referringCode);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateCSToken(long id, String csToken)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET CSToken = @CSToken, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@CSToken", SqlDbType.VarChar, csToken);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateOptions(long id, long options)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Options = @Options, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Options", SqlDbType.BigInt, options);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int appUserStatusId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, AppUserStatusId = @AppUserStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@AppUserStatusId", SqlDbType.Int, appUserStatusId);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByWebSysUserId(long webSysUserId, int appUserStatusId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, AppUserStatusId = @AppUserStatusId" +
                            " WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@AppUserStatusId", SqlDbType.Int, appUserStatusId);
            command.AddParam("@WebSysUserId", SqlDbType.BigInt, webSysUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateReferredAppUserId(long id, long referrerAppUserId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, ReferrerAppUserId = @ReferrerAppUserId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@ReferrerAppUserId", SqlDbType.BigInt, referrerAppUserId);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateAlias(long id, String alias)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Alias = @Alias" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Alias", SqlDbType.VarChar, alias);
            command.AddParam("@Id", SqlDbType.BigInt, id);

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

        public async Task<bool> DeleteById(long id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteByWebSysUserId(long webSysUserid)
        {
            String strCmd = $"DELETE {table} WHERE WebSysUserid = @WebSysUserid";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@WebSysUserid", SqlDbType.BigInt, webSysUserid);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
