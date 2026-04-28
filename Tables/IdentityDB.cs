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
                                reader["FirstName1"].ToString(),
                                reader["FirstName2"].ToString(),
                                reader["LastName1"].ToString(),
                                reader["LastName2"].ToString(),
                                Convert.ToInt64(reader["GenderId"]),
                                Convert.ToDateTime(reader["BirthDate"]),
                                Convert.ToInt64(reader["BirthCountryId"]),
                                Convert.ToInt64(reader["BirthStateId"]),
                                Convert.ToInt64(reader["BirthCityId"]),
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
                                    reader["FirstName1"].ToString(),
                                    reader["FirstName2"].ToString(),
                                    reader["LastName1"].ToString(),
                                    reader["LastName2"].ToString(),
                                    reader["Gender"].ToString(),
                                    Convert.ToDateTime(reader["BirthDate"]),
                                    reader["BirthCountry"].ToString(),
                                    reader["BirthState"].ToString(),
                                    reader["BirthCity"].ToString(),

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

        public async Task<List<IdentityFull>> GetFullsByStatus(int status = -1)
        {
            String strCmd = "SELECT Idt.Id, FirstName1, FirstName2,LastName1, LastName2, KGender.Name AS Gender," +
                             " BirthDate, KCountry.Name AS BirthCountry, KState.Name AS BirthState, KCity.Name AS BirthCity," +
                             " KPhoneCountry.PhonePrefix AS PhonePrefix, Phone, Email," +
                             " Idt.CreateDateTime, Idt.UpdateDateTime, AppUser.AppUserStatusId, Idt.Status" +
                            $" FROM {table} AS Idt" +
                             " LEFT JOIN [K-Gender] AS KGender ON (KGender.Id = Idt.GenderId)" +
                             " LEFT JOIN [K-Country] AS KCountry ON (KCountry.Id = Idt.BirthCountryId)" +
                             " LEFT JOIN [K-State] AS KState ON (KState.Id = Idt.BirthStateId)" +
                             " LEFT JOIN [K-City] AS KCity ON (KCity.Id = Idt.BirthCityId)" +
                             " LEFT JOIN [K-Country] AS KPhoneCountry ON (KPhoneCountry.Id = Idt.PhoneCountryId)" +

                             " INNER JOIN [J-IdentityAppUser] AS IAU ON IAU.IdentityId = Idt.Id AND IAU.Status = 1" +
                             " INNER JOIN [D-AppUser] AS AppUser ON AppUser.Id = IAU.AppUserId";

            if (status != -1)
                strCmd += " WHERE Idt.Status = @Status";

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

        public async Task<(String, String, String, String)> GetFullNameByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT {table}.FirstName1, {table}.FirstName2, {table}.LastName1, {table}.LastName2" +
                            $" FROM {table}" +
                            $" INNER JOIN [J-IdentityAppUser] J ON {table}.Id = J.IdentityId" +
                            " WHERE J.AppUserId = @AppUserId AND J.Status = @Status;";


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

        public async Task<(String, String, String, String)> GetFullNameByBoardUserId(long boardUserId, int status = 1)
        {
            String strCmd = $"SELECT {table}.FirstName1, {table}.FirstName2, {table}.LastName1, {table}.LastName2" +
                            $" FROM {table}" +
                            $" INNER JOIN [J-IdentityBoardUser] J ON {table}.Id = J.IdentityId" +
                            " WHERE J.BoardUserId = @BoardUserId AND J.Status = @Status;";


            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.BigInt, boardUserId);
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

        public async Task<IdentityFull> GetFullByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = "SELECT Idt.Id, J.AppUserId, FirstName1, FirstName2, LastName1, LastName2," +
                             " KGender.Name AS Gender, BirthDate, KCountry.Name AS BirthCountry, KState.Name AS BirthState, KCity.Name AS BirthCity," +
                             " KPhoneCountry.PhonePrefix AS PhonePrefix, Phone, Email," +
                             " Idt.CreateDateTime, Idt.UpdateDateTime, Idt.Status" +
                            $" FROM {table} AS Idt" +
                             " INNER JOIN [D-IdentityAppUser] AS J ON J.IdentityId = Idt.Id" +
                             " INNER JOIN [K-Gender] AS KGender ON KGender.Id = Idt.GenderId" +
                             " INNER JOIN [K-Country] AS KCountry ON KCountry.Id = Idt.BirthCountryId" +
                             " INNER JOIN [K-State] AS KState ON KState.Id = Idt.BirthStateId" +
                             " INNER JOIN [K-City] AS KCity ON KCity.Id = Idt.BirthCityId" +
                             " INNER JOIN [K-Country] AS KPhoneCountry ON KPhoneCountry.Id = Idt.PhoneCountryId" +
                             " WHERE J.AppUserId = @AppUserId AND J.Status = @Status";

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
            String strCmd =$"SELECT I.*" +
                           $" FROM {table} AS I" +
                            " INNER JOIN [D-IdentityAppUser] AS J ON J.IdentityId = I.Id" +
                            " WHERE J.AppUserId = @AppUserId AND J.Status = @Status";

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

        // INSERT
        public async Task<long> Add(Identity identity)
        {
            String strCmd = $"INSERT INTO {table}(Id, FirstName1, FirstName2, LastName1, LastName2, GenderId," +
                            " BirthDate, BirthCountryId, BirthStateId, BirthCityId," +
                            " PhoneCountryId, Phone, Email," +
                            " CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @FirstName1, @FirstName2, @LastName1, @LastName2, @GenderId," +
                            " @BirthDate, @BirthCountryId, @BirthStateId, @BirthCityId," +
                            " @PhoneCountryId, @Phone, @Email," +
                            " @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('I'));
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, identity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, identity.FirstName2);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, identity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, identity.LastName2);
            DBHelper.AddParam(command, "@GenderId", SqlDbType.Int, identity.GenderId);

            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, identity.BirthDate);
            DBHelper.AddParam(command, "@BirthCountryId", SqlDbType.Int, identity.BirthCountryId);
            DBHelper.AddParam(command, "@BirthStateId", SqlDbType.Int, identity.BirthStateId);
            DBHelper.AddParam(command, "@BirthCityId", SqlDbType.Int, identity.BirthCityId);

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
            String strCmd = $"UPDATE {table} SET FirstName1 = @FirstName1, FirstName2 = @FirstName2," +
                            " LastName1 = @LastName1, LastName2 = @LastName2, GenderId = @GenderId," +
                            " BirthDate = @BirthDate, BirthCountryId = @BirthCountryId, BirthStateId = @BirthStateId," +
                            " BirthCityId = @BirthCityId, PhoneCountryId = @PhoneCountryId, Phone = @Phone, Email = @Email," +
                            " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, identity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, identity.FirstName2);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, identity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, identity.LastName2);
            DBHelper.AddParam(command, "@GenderId", SqlDbType.Int, identity.GenderId);
            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, identity.BirthDate);
            DBHelper.AddParam(command, "@BirthCountryId", SqlDbType.Int, identity.BirthCountryId);
            DBHelper.AddParam(command, "@BirthStateId", SqlDbType.Int, identity.BirthStateId);
            DBHelper.AddParam(command, "@BirthCityId", SqlDbType.Int, identity.BirthCityId);
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

        public async Task<bool> UpdateStatus(long id, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE Id = @Id AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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
    }
}
