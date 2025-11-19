using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class IdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Identity]";

        public static Identity GetIdentity(SqlDataReader reader)
        {
            return new Identity(Convert.ToInt64(reader["Id"]),
                                Convert.ToInt64(reader["AppUserId"]),
                                reader["FirstName1"].ToString(),
                                reader["FirstName2"].ToString(),
                                reader["LastName1"].ToString(),
                                reader["LastName2"].ToString(),
                                Convert.ToInt64(reader["GenderId"]),
                                Convert.ToDateTime(reader["BirthDate"]),
                                Convert.ToInt64(reader["OriginCountryId"]),
                                Convert.ToInt64(reader["OriginStateId"]),
                                Convert.ToInt64(reader["PhoneCountryId"]),
                                reader["Phone"].ToString(),
                                reader["Email"].ToString(),
                                Convert.ToDateTime(reader["CreateDateTime"]),
                                Convert.ToDateTime(reader["UpdateDateTime"]),
                                Convert.ToInt32(reader["Status"]));
        }

        public static IdentityFull GetIdentityFull(SqlDataReader reader)
        {
            return new IdentityFull(Convert.ToInt64(reader["Id"]),
                                    Convert.ToInt64(reader["AppUserId"]),
                                    reader["FirstName1"].ToString(),
                                    reader["FirstName2"].ToString(),
                                    reader["LastName1"].ToString(),
                                    reader["LastName2"].ToString(),
                                    reader["Gender"].ToString(),
                                    Convert.ToDateTime(reader["BirthDate"]),
                                    reader["OriginCountry"].ToString(),
                                    reader["OriginState"].ToString(),
                                  
                                    reader["PhonePrefix"].ToString(),
                                    reader["Phone"].ToString(),
                                    reader["Email"].ToString(),
                                  
                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                    Convert.ToDateTime(reader["UpdateDateTime"]),
                                    Convert.ToInt32(reader["AppUserStatusId"]),
                                    Convert.ToInt32(reader["Status"]));
        }

        // SELECT
        public async Task<List<Identity>> GetAll(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Identity> identities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Identity identity = GetIdentity(reader);
                        identities.Add(identity);
                    }
                }
            }
            return identities;
        }

        public async Task<List<IdentityFull>> GetFullAll(int status = -1)
        {
            String strCmd = "SELECT Identty.Id, AppUserId, FirstName1, FirstName2,LastName1, LastName2, KGender.Name AS Gender," +
                             " BirthDate, KCountry.Name AS OriginCountry, KState.Name AS OriginState," +
                             " KPhoneCountry.PhonePrefix AS PhonePrefix, Phone, Email," +
                             " Identty.CreateDateTime, Identty.UpdateDateTime, AppUser.AppUserStatusId, Identty.Status" +
                            $" FROM {table} AS Identty" +
                             " INNER JOIN [K-Gender] AS KGender ON (KGender.Id = Identty.GenderId)" +
                             " INNER JOIN [K-Country] AS KCountry ON (KCountry.Id = Identty.OriginCountryId)" +
                             " INNER JOIN [K-State] AS KState ON (KState.Id = Identty.OriginStateId)" +
                             " INNER JOIN [K-Country] AS KPhoneCountry ON (KPhoneCountry.Id = Identty.PhoneCountryId)" +
                             " INNER JOIN [D-AppUser] AppUser ON (AppUser.Id = Identty.AppUserId)";
            if (status != -1)
                strCmd += " WHERE Identty.Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<IdentityFull> identityFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        IdentityFull identityFull = GetIdentityFull(reader);
                        identityFulls.Add(identityFull);
                    }
                }
            }
            return identityFulls;
        }

        public async Task<Identity> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Identity identity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        identity = GetIdentity(reader);
                    }
                }
            }
            return identity;
        }

        public async Task<Identity> GetByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            Identity identity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        identity = GetIdentity(reader);
                    }
                }
            }
            return identity;
        }

        public async Task<IdentityFull> GetFullByAppUserId(long appUserId, int status = 1)
        {
            String strCmd =  "SELECT Identty.Id, AppUserId, FirstName1, FirstName2, LastName1, LastName2, KGender.Name AS Gender," +
                             " BirthDate, KCountry.Name AS OriginCountry, KState.Name AS OriginState," +
                             " KPhoneCountry.PhonePrefix AS PhonePrefix, Phone, Email," +
                             " CreateDateTime, UpdateDateTime, Identty.Status" +
                            $" FROM {table} AS Identty" +
                             " INNER JOIN [K-Gender] AS KGender ON (KGender.Id = Identty.GenderId)" +
                             " INNER JOIN [K-Country] AS KCountry ON (KCountry.Id = Identty.OriginCountryId)" +
                             " INNER JOIN [K-State] AS KState ON (KState.Id = Identty.OriginStateId)" +
                             " INNER JOIN [K-Country] AS KPhoneCountry ON (KPhoneCountry.Id = Identty.PhoneCountryId)" +
                             " WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            IdentityFull identityFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        identityFull = GetIdentityFull(reader);
                    }
                }
            }
            return identityFull;
        }

        public async Task<List<Identity>> GetAllByAppUserId(long appUserId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Identity> identities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Identity identity = GetIdentity(reader);
                        identities.Add(identity);
                    }
                }
            }
            return identities;
        }

        public async Task<long> GetIdByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            long identityId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        identityId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return identityId;
        }

        public async Task<(String, String, String, String)> GetFullNameByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT FirstName1, FirstName2, LastName1, LastName2 FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            (String firstName1, String firstName2, String lastName1, String lastName2) names = (null, null, null, null);
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        names.firstName1 = reader["FirstName1"].ToString();
                        names.firstName2 = reader["FirstName2"].ToString();
                        names.lastName1 = reader["LastName1"].ToString();
                        names.lastName2 = reader["LastName2"].ToString();
                    }
                }
            }
            return names;
        }

        public async Task<String> GetEmailById(long id, int status = 1)
        {
            String strCmd = $"SELECT Email FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            string email = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        email = reader["Email"].ToString();
                }
            }
            return email;
        }

        // INSERT
        public async Task<long> Add(Identity identity)
        {
            String strCmd = $"INSERT INTO {table}(Id, AppUserId, FirstName1, FirstName2, LastName1, LastName2, GenderId," +
                            " BirthDate, OriginCountryId, OriginStateId," +
                            " PhoneCountryId, Phone, Email," +
                            " CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @AppUserId, @FirstName1, @FirstName2, @LastName1, @LastName2, @GenderId," +
                            " @BirthDate, @OriginCountryId, @OriginStateId," +
                            " @PhoneCountryId, @Phone, @Email," +
                            " @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid());
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, identity.AppUserId);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, identity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, identity.FirstName2);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, identity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, identity.LastName2);
            DBHelper.AddParam(command, "@GenderId", SqlDbType.Int, identity.GenderId);

            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, identity.BirthDate);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.Int, identity.OriginCountryId);
            DBHelper.AddParam(command, "@OriginStateId", SqlDbType.Int, identity.OriginStateId);
            
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, identity.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, identity.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, identity.Email);
           
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, identity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Identity identity)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, FirstName1 = @FirstName1, FirstName2 = @FirstName2," +
                            " LastName1 = @LastName1, LastName2 = @LastName2, GenderId = @GenderId," +
                            " BirthDate = @BirthDate, OriginCountryId = @OriginCountryId, OriginStateId = @OriginStateId," +
                            " PhoneCountryId = @PhoneCountryId, Phone = @Phone, Email = @Email," +
                            " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, identity.AppUserId);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, identity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, identity.FirstName2);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, identity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, identity.LastName2);
            DBHelper.AddParam(command, "@GenderId", SqlDbType.Int, identity.GenderId);
            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, identity.BirthDate);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.Int, identity.OriginCountryId);
            DBHelper.AddParam(command, "@OriginStateId", SqlDbType.Int, identity.OriginStateId);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, identity.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, identity.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, identity.Email);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, identity.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateVersion(long appUserId, String version, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, DpiVersion = @DpiVersion" +
                            " WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@DpiVersion", SqlDbType.VarChar, version);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateSerie(long appUserId, String serie, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, DpiSerie = @DpiSerie" +
                            " WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@DpiSerie", SqlDbType.VarChar, serie);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByAppUserId(long appUserId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);

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

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteByAppUserId(long appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
