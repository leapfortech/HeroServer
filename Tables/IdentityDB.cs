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
        readonly String table = "[DD-Identity]";

        public static Identity GetIdentity(SqlDataReader reader)
        {
            return new Identity(Convert.ToInt32(reader["Id"]),
                                Convert.ToInt32(reader["AppUserId"]),
                                reader["FirstName1"].ToString(),
                                reader["FirstName2"].ToString(),
                                reader["FirstName3"].ToString(),
                                reader["LastName1"].ToString(),
                                reader["LastName2"].ToString(),
                                reader["LastNameMarried"].ToString(),
                                Convert.ToInt32(reader["GenderId"]),
                                Convert.ToDateTime(reader["BirthDate"]),
                                Convert.ToInt32(reader["BirthCountryId"]),
                                Convert.ToInt32(reader["BirthStateId"]),
                                Convert.ToInt32(reader["BirthCityId"]),
                                reader["NationalityIds"].ToString(),
                                Convert.ToInt32(reader["MaritalStatusId"]),
                                reader["Occupation"].ToString(),
                                reader["Nit"].ToString(),
                                reader["DpiCui"].ToString(),
                                Convert.ToDateTime(reader["DpiIssueDate"]),
                                Convert.ToDateTime(reader["DpiDueDate"]),
                                reader["DpiVersion"].ToString(),
                                reader["DpiSerie"].ToString(),
                                reader["DpiMrz"].ToString(),
                                Convert.ToInt32(reader["PhoneCountryId"]),
                                reader["Phone"].ToString(),
                                reader["Email"].ToString(),
                                Convert.ToInt32(reader["IsPep"]),
                                Convert.ToInt32(reader["HasPepIdentity"]),
                                Convert.ToInt32(reader["IsCpe"]),
                                Convert.ToDateTime(reader["CreateDateTime"]),
                                Convert.ToDateTime(reader["UpdateDateTime"]),
                                Convert.ToInt32(reader["Status"]));
        }

        public static IdentityFull GetIdentityFull(SqlDataReader reader)
        {
            return new IdentityFull(Convert.ToInt32(reader["Id"]),
                                    Convert.ToInt32(reader["AppUserId"]),
                                    reader["FirstName1"].ToString(),
                                    reader["FirstName2"].ToString(),
                                    reader["FirstName3"].ToString(),
                                    reader["LastName1"].ToString(),
                                    reader["LastName2"].ToString(),
                                    reader["LastNameMarried"].ToString(),
                                    reader["Gender"].ToString(),
                                    Convert.ToDateTime(reader["BirthDate"]),
                                    reader["BirthCountry"].ToString(),
                                    reader["BirthState"].ToString(),
                                    reader["BirthCity"].ToString(),
                                    null,
                                    reader["MaritalStatus"].ToString(),
                                    reader["Occupation"].ToString(),
                                    reader["Nit"].ToString(),
                                    reader["DpiCui"].ToString(),
                                    Convert.ToDateTime(reader["DpiIssueDate"]),
                                    Convert.ToDateTime(reader["DpiDueDate"]),
                                    reader["DpiVersion"].ToString(),
                                    reader["DpiSerie"].ToString(),
                                    reader["DpiMrz"].ToString(),
                                    reader["PhonePrefix"].ToString(),
                                    reader["Phone"].ToString(),
                                    reader["Email"].ToString(),
                                    Convert.ToInt32(reader["IsPep"]),
                                    Convert.ToInt32(reader["HasPepIdentity"]),
                                    Convert.ToInt32(reader["IsCpe"]),
                                    Convert.ToInt32(reader["Investments"]),
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
            String strCmd = "SELECT Identty.Id, AppUserId, FirstName1, FirstName2, FirstName3, LastName1, LastName2, LastNameMarried, KGender.Name AS Gender," +
                             " BirthDate, KCountry.Name AS BirthCountry, KState.Name AS BirthState, KCity.Name AS BirthCity, KMaritalStatus.Name AS MaritalStatus," +
                             " Occupation, Nit, DpiCui, DpiIssueDate, DpiDueDate, DpiVersion, DpiSerie, DpiMrz, KPhoneCountry.PhonePrefix AS PhonePrefix, Phone, Email," +
                             " IsPep, HasPepIdentity, IsCpe, (SELECT COUNT(DISTINCT ProjectId) FROM [DD-Investment] Invest WHERE Invest.AppUserId = Identty.AppUserId) AS Investments," +
                             " Identty.CreateDateTime, Identty.UpdateDateTime, AppUser.AppUserStatusId, Identty.Status" +
                            $" FROM {table} AS Identty" +
                             " INNER JOIN [K-Gender] AS KGender ON (KGender.Id = Identty.GenderId)" +
                             " INNER JOIN [K-MaritalStatus] AS KMaritalStatus ON (KMaritalStatus.Id = Identty.MaritalStatusId)" +
                             " INNER JOIN [K-Country] AS KCountry ON (KCountry.Id = Identty.BirthCountryId)" +
                             " INNER JOIN [K-State] AS KState ON (KState.Id = Identty.BirthStateId)" +
                             " INNER JOIN [K-City] AS KCity ON (KCity.Id = Identty.BirthCityId)" +
                             " INNER JOIN [K-Country] AS KPhoneCountry ON (KPhoneCountry.Id = Identty.PhoneCountryId)" +
                             " INNER JOIN [DD-AppUser] AppUser ON (AppUser.Id = Identty.AppUserId)";
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

        public async Task<Identity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

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

        public async Task<Identity> GetByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<IdentityFull> GetFullByAppUserId(int appUserId, int status = 1)
        {
            String strCmd =  "SELECT Identty.Id, AppUserId, FirstName1, FirstName2, FirstName3, LastName1, LastName2, LastNameMarried, KGender.Name AS Gender," +
                             " BirthDate, KCountry.Name AS BirthCountry, KState.Name AS BirthState, KCity.Name AS BirthCity, KMaritalStatus.Name AS MaritalStatus," +
                             " Occupation, Nit, DpiCui, DpiIssueDate, DpiDueDate, DpiVersion, DpiSerie, DpiMrz, KPhoneCountry.PhonePrefix AS PhonePrefix, Phone, Email," +
                             " IsPep, HasPepIdentity, IsCpe, CreateDateTime, UpdateDateTime, Identty.Status" +
                            $" FROM {table} AS Identty" +
                             " INNER JOIN [K-Gender] AS KGender ON (KGender.Id = Identty.GenderId)" +
                             " INNER JOIN [K-MaritalStatus] AS KMaritalStatus ON (KMaritalStatus.Id = Identty.MaritalStatusId)" +
                             " INNER JOIN [K-Country] AS KCountry ON (KCountry.Id = Identty.BirthCountryId)" +
                             " INNER JOIN [K-State] AS KState ON (KState.Id = Identty.BirthStateId)" +
                             " INNER JOIN [K-City] AS KCity ON (KCity.Id = Identty.BirthCityId)" +
                             " INNER JOIN [K-Country] AS KPhoneCountry ON (KPhoneCountry.Id = Identty.PhoneCountryId)" +
                             " WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<List<Identity>> GetAllByAppUserId(int appUserId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<int> GetIdByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            int identityId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        identityId = Convert.ToInt32(reader["Id"]);
                    }
                }
            }
            return identityId;
        }

        public async Task<(String, String, String, String)> GetFullNameByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT FirstName1, FirstName2, LastName1, LastName2 FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<Identity> GetByCui(String cui)
        {
            String strCmd = $"SELECT * FROM {table} WHERE DpiCui = @DpiCui AND Status = 1";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@DpiCui", SqlDbType.VarChar, cui);

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

        public async Task<String> GetCuiByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT DpiCui FROM {table} WHERE AppUserId = @AppUserId AND Status = 1";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            String cui = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cui = reader["DpiCui"].ToString();
                    }
                }
            }
            return cui;
        }

        // INSERT
        public async Task<int> Add(Identity identity)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, FirstName1, FirstName2, FirstName3, LastName1, LastName2, LastNameMarried, GenderId," +
                            " BirthDate, BirthCountryId, BirthStateId, BirthCityId, NationalityIds, MaritalStatusId, Occupation, Nit," +
                            " DpiCui, DpiIssueDate, DpiDueDate, DpiVersion, DpiSerie, DpiMrz, PhoneCountryId, Phone, Email, IsPep, HasPepIdentity, IsCpe," +
                            " CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @FirstName1, @FirstName2, @FirstName3, @LastName1, @LastName2, @LastNameMarried, @GenderId," +
                            " @BirthDate, @BirthCountryId, @BirthStateId, @BirthCityId, @NationalityIds, @MaritalStatusId, @Occupation, @Nit," +
                            " @DpiCui, @DpiIssueDate, @DpiDueDate, @DpiVersion, @DpiSerie, @DpiMrz, @PhoneCountryId, @Phone, @Email, @IsPep, @HasPepIdentity, @IsCpe," +
                            " @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, identity.AppUserId);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, identity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, identity.FirstName2);
            DBHelper.AddParam(command, "@FirstName3", SqlDbType.VarChar, identity.FirstName3);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, identity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, identity.LastName2);
            DBHelper.AddParam(command, "@LastNameMarried", SqlDbType.VarChar, identity.LastNameMarried);
            DBHelper.AddParam(command, "@GenderId", SqlDbType.Int, identity.GenderId);

            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, identity.BirthDate);
            DBHelper.AddParam(command, "@BirthCountryId", SqlDbType.Int, identity.BirthCountryId);
            DBHelper.AddParam(command, "@BirthStateId", SqlDbType.Int, identity.BirthStateId);
            DBHelper.AddParam(command, "@BirthCityId", SqlDbType.Int, identity.BirthCityId);
            DBHelper.AddParam(command, "@NationalityIds", SqlDbType.VarChar, identity.NationalityIds);
            DBHelper.AddParam(command, "@MaritalStatusId", SqlDbType.Int, identity.MaritalStatusId);
            DBHelper.AddParam(command, "@Occupation", SqlDbType.VarChar, identity.Occupation);
            DBHelper.AddParam(command, "@Nit", SqlDbType.VarChar, identity.Nit);

            DBHelper.AddParam(command, "@DpiCui", SqlDbType.VarChar, identity.DpiCui);
            DBHelper.AddParam(command, "@DpiIssueDate", SqlDbType.DateTime2, identity.DpiIssueDate);
            DBHelper.AddParam(command, "@DpiDueDate", SqlDbType.DateTime2, identity.DpiDueDate);
            DBHelper.AddParam(command, "@DpiVersion", SqlDbType.VarChar, identity.DpiVersion);
            DBHelper.AddParam(command, "@DpiSerie", SqlDbType.VarChar, identity.DpiSerie);
            DBHelper.AddParam(command, "@DpiMrz", SqlDbType.VarChar, identity.DpiMrz);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, identity.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, identity.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, identity.Email);
            DBHelper.AddParam(command, "@IsPep", SqlDbType.Int, identity.IsPep);
            DBHelper.AddParam(command, "@HasPepIdentity", SqlDbType.Int, identity.HasPepIdentity);
            DBHelper.AddParam(command, "@IsCpe", SqlDbType.Int, identity.IsCpe);

            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, identity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Identity identity)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, FirstName1 = @FirstName1, FirstName2 = @FirstName2, FirstName3 = @FirstName3," +
                            " LastName1 = @LastName1, LastName2 = @LastName2, LastNameMarried = @LastNameMarried, GenderId = @GenderId," +
                            " BirthDate = @BirthDate, BirthCountryId = @BirthCountryId, BirthStateId = @BirthStateId, BirthCityId = @BirthCityId, NationalityIds = @NationalityIds," +
                            " Occupation = @Occupation, Nit = @Nit, DpiCui = @DpiCui, DpiIssueDate = @DpiIssueDate, DpiDueDate = @DpiDueDate, DpiVersion = @DpiVersion, DpiSerie = @DpiSerie, DpiMrz = @DpiMrz," +
                            " PhoneCountryId = @PhoneCountryId, Phone = @Phone, Email = @Email, IsPep = @IsPep, HasPepIdentity = @HasPepIdentity, IsCpe = @IsCpe," +
                            " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, identity.AppUserId);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, identity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, identity.FirstName2);
            DBHelper.AddParam(command, "@FirstName3", SqlDbType.VarChar, identity.FirstName3);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, identity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, identity.LastName2);
            DBHelper.AddParam(command, "@LastNameMarried", SqlDbType.VarChar, identity.LastNameMarried);
            DBHelper.AddParam(command, "@GenderId", SqlDbType.Int, identity.GenderId);
            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, identity.BirthDate);
            DBHelper.AddParam(command, "@BirthCountryId", SqlDbType.Int, identity.BirthCountryId);
            DBHelper.AddParam(command, "@BirthStateId", SqlDbType.Int, identity.BirthStateId);
            DBHelper.AddParam(command, "@BirthCityId", SqlDbType.Int, identity.BirthCityId);
            DBHelper.AddParam(command, "@NationalityIds", SqlDbType.VarChar, identity.NationalityIds);
            DBHelper.AddParam(command, "@Occupation", SqlDbType.VarChar, identity.Occupation);
            DBHelper.AddParam(command, "@Nit", SqlDbType.VarChar, identity.Nit);
            DBHelper.AddParam(command, "@DpiCui", SqlDbType.VarChar, identity.DpiCui);
            DBHelper.AddParam(command, "@DpiIssueDate", SqlDbType.DateTime2, identity.DpiIssueDate);
            DBHelper.AddParam(command, "@DpiDueDate", SqlDbType.DateTime2, identity.DpiDueDate);
            DBHelper.AddParam(command, "@DpiVersion", SqlDbType.VarChar, identity.DpiVersion);
            DBHelper.AddParam(command, "@DpiSerie", SqlDbType.VarChar, identity.DpiSerie);
            DBHelper.AddParam(command, "@DpiMrz", SqlDbType.VarChar, identity.DpiMrz);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, identity.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, identity.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, identity.Email);
            DBHelper.AddParam(command, "@IsPep", SqlDbType.Int, identity.IsPep);
            DBHelper.AddParam(command, "@HasPepIdentity", SqlDbType.Int, identity.HasPepIdentity);
            DBHelper.AddParam(command, "@IsCpe", SqlDbType.Int, identity.IsCpe);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, identity.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateVersion(int appUserId, String version, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, DpiVersion = @DpiVersion" +
                            " WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@DpiVersion", SqlDbType.VarChar, version);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateSerie(int appUserId, String serie, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, DpiSerie = @DpiSerie" +
                            " WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@DpiSerie", SqlDbType.VarChar, serie);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByAppUserId(int appUserId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<bool> DeleteByAppUserId(int appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
