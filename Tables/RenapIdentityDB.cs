using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class RenapIdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-RenapIdentity]";

        private static RenapIdentity GetRenapIdentity(SqlDataReader reader)
        {
            return new RenapIdentity(
                             Convert.ToInt32(reader["Id"]),
                             Convert.ToInt32(reader["AppUserId"]),
                             reader["Cui"].ToString(),
                             reader["FirstName1"].ToString(),
                             reader["FirstName2"].ToString(),
                             reader["FirstName3"].ToString(),
                             reader["LastName1"].ToString(),
                             reader["LastName2"].ToString(),
                             reader["LastNameMarried"].ToString(),
                             Convert.ToDateTime(reader["BirthDate"]),
                             reader["DeathDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["DeathDate"]),
                             reader["Gender"].ToString(),
                             reader["MaritalStatus"].ToString(),
                             reader["Nationality"].ToString(),
                             reader["BirthCountry"].ToString(),
                             reader["BirthState"].ToString(),
                             reader["BirthCity"].ToString(),
                             reader["CedulaResidence"].ToString(),
                             reader["CedulaOrder"].ToString(),
                             reader["CedulaRegister"].ToString(),
                             reader["Occupation"].ToString(),
                             reader["DpiDueDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["DpiDueDate"]),
                             reader["DpiVersion"].ToString(),
                             Convert.ToDateTime(reader["CreateDateTime"]),
                             Convert.ToInt32(reader["Status"])
             );
        }

        // GET
        public async Task<IEnumerable<RenapIdentity>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<RenapIdentity> renapIdentities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        RenapIdentity renapIdentity = GetRenapIdentity(reader);
                        renapIdentities.Add(renapIdentity);
                    }
                }
            }
            return renapIdentities;
        }

        public async Task<RenapIdentity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            RenapIdentity renapIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        renapIdentity = GetRenapIdentity(reader);
                    }
                }
            }
            return renapIdentity;
        }

        public async Task<RenapIdentity> GetByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = 1";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            RenapIdentity renapIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        renapIdentity = GetRenapIdentity(reader);
                    }
                }
            }
            return renapIdentity;
        }

        // INSERT
        public async Task<int> Add(RenapIdentity renapIdentity)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, Cui, FirstName1, FirstName2, FirstName3, LastName1, LastName2, LastNameMarried, BirthDate, DeathDate, Gender, MaritalStatus, Nationality, BirthCountry, BirthState, BirthCity, CedulaResidence, CedulaOrder, CedulaRegister, Occupation, DpiDueDate, DpiVersion, CreateDateTime, Status)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @Cui, @FirstName1, @FirstName2, @FirstName3, @LastName1, @LastName2, @LastNameMarried, @BirthDate, @DeathDate, @Gender, @MaritalStatus, @Nationality, @BirthCountry, @BirthState, @BirthCity, @CedulaResidence, @CedulaOrder, @CedulaRegister, @Occupation, @DpiDueDate, @DpiVersion, @CreateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, renapIdentity.AppUserId);
            DBHelper.AddParam(command, "@Cui", SqlDbType.VarChar, renapIdentity.Cui);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, renapIdentity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, renapIdentity.FirstName2);
            DBHelper.AddParam(command, "@FirstName3", SqlDbType.VarChar, renapIdentity.FirstName3);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, renapIdentity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, renapIdentity.LastName2);
            DBHelper.AddParam(command, "@LastNameMarried", SqlDbType.VarChar, renapIdentity.LastNameMarried);
            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, renapIdentity.BirthDate);
            DBHelper.AddParam(command, "@DeathDate", SqlDbType.DateTime2, renapIdentity.DeathDate);
            DBHelper.AddParam(command, "@Gender", SqlDbType.VarChar, renapIdentity.Gender);
            DBHelper.AddParam(command, "@MaritalStatus", SqlDbType.VarChar, renapIdentity.MaritalStatus);
            DBHelper.AddParam(command, "@Nationality", SqlDbType.VarChar, renapIdentity.Nationality);
            DBHelper.AddParam(command, "@BirthCountry", SqlDbType.VarChar, renapIdentity.BirthCountry);
            DBHelper.AddParam(command, "@BirthState", SqlDbType.VarChar, renapIdentity.BirthState);
            DBHelper.AddParam(command, "@BirthCity", SqlDbType.VarChar, renapIdentity.BirthCity);
            DBHelper.AddParam(command, "@CedulaResidence", SqlDbType.VarChar, renapIdentity.CedulaResidence);
            DBHelper.AddParam(command, "@CedulaOrder", SqlDbType.VarChar, renapIdentity.CedulaOrder);
            DBHelper.AddParam(command, "@CedulaRegister", SqlDbType.VarChar, renapIdentity.CedulaRegister);
            DBHelper.AddParam(command, "@Occupation", SqlDbType.VarChar, renapIdentity.Occupation);
            DBHelper.AddParam(command, "@DpiDueDate", SqlDbType.DateTime2, renapIdentity.DpiDueDate);
            DBHelper.AddParam(command, "@DpiVersion", SqlDbType.VarChar, renapIdentity.DpiVersion);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, renapIdentity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(RenapIdentity renapIdentity)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, Cui = @Cui, FirstName1 = @FirstName1, FirstName2 = @FirstName2, FirstName3 = @FirstName3, LastName1 = @LastName1, LastName2 = @LastName2, LastNameMarried = @LastNameMarried, BirthDate = @BirthDate, DeathDate = @DeathDate, Gender = @Gender, Nationality = @Nationality, BirthCountry = @BirthCountry, BirthState = @BirthState, BirthCity = @BirthCity, CedulaResidence = @CedulaResidence, CedulaOrder = @CedulaOrder, CedulaRegister = @CedulaRegister, Occupation = @Occupation, DpiDueDate = @DpiDueDate, DpiVersion = @DpiVersion WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, renapIdentity.AppUserId);
            DBHelper.AddParam(command, "@Cui", SqlDbType.VarChar, renapIdentity.Cui);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, renapIdentity.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, renapIdentity.FirstName2);
            DBHelper.AddParam(command, "@FirstName3", SqlDbType.VarChar, renapIdentity.FirstName3);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, renapIdentity.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, renapIdentity.LastName2);
            DBHelper.AddParam(command, "@LastNameMarried", SqlDbType.VarChar, renapIdentity.LastNameMarried);
            DBHelper.AddParam(command, "@BirthDate", SqlDbType.DateTime2, renapIdentity.BirthDate);
            DBHelper.AddParam(command, "@DeathDate", SqlDbType.DateTime2, renapIdentity.DeathDate);
            DBHelper.AddParam(command, "@Gender", SqlDbType.VarChar, renapIdentity.Gender);
            DBHelper.AddParam(command, "@Nationality", SqlDbType.VarChar, renapIdentity.Nationality);
            DBHelper.AddParam(command, "@BirthCountry", SqlDbType.VarChar, renapIdentity.BirthCountry);
            DBHelper.AddParam(command, "@BirthState", SqlDbType.VarChar, renapIdentity.BirthState);
            DBHelper.AddParam(command, "@BirthCity", SqlDbType.VarChar, renapIdentity.BirthCity);
            DBHelper.AddParam(command, "@CedulaResidence", SqlDbType.VarChar, renapIdentity.CedulaResidence);
            DBHelper.AddParam(command, "@CedulaOrder", SqlDbType.VarChar, renapIdentity.CedulaOrder);
            DBHelper.AddParam(command, "@CedulaRegister", SqlDbType.VarChar, renapIdentity.CedulaRegister);
            DBHelper.AddParam(command, "@Occupation", SqlDbType.VarChar, renapIdentity.Occupation);
            DBHelper.AddParam(command, "@DpiDueDate", SqlDbType.DateTime2, renapIdentity.DpiDueDate);
            DBHelper.AddParam(command, "@DpiVersion", SqlDbType.VarChar, renapIdentity.DpiVersion);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, renapIdentity.Id);

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
