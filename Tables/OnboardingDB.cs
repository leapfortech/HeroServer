using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class OnboardingDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Onboarding]";

        public static Onboarding GetFromReader(SqlDataReader reader)
        {
            return new Onboarding(
                                 Convert.ToInt32(reader["Id"]),
                                 Convert.ToInt32(reader["AppUserId"]),
                                 Convert.ToInt64(reader["DpiFront"]),
                                 Convert.ToInt64(reader["DpiBack"]),
                                 Convert.ToInt64(reader["Renap"]),
                                 Convert.ToInt64(reader["Portrait"]),
                                 Convert.ToInt64(reader["Address"]),
                                 Convert.ToInt32(reader["IdentityId"]),
                                 Convert.ToInt32(reader["RenapIdentityId"]),
                                 Convert.ToInt32(reader["AddressId"]),
                                 reader["Comment"].ToString(),
                                 Convert.ToInt32(reader["BoardUserId"]),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["Status"])
                                 );
        }

        // GET
        public async Task<IEnumerable<Onboarding>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Onboarding> onboardings = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Onboarding onboarding = GetFromReader(reader);
                         onboardings.Add(onboarding);
                    }
                }
            }
            return onboardings;
        }

        public async Task<Onboarding> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Onboarding onboarding = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         onboarding = GetFromReader(reader);
                    }
                }
            }
            return onboarding;
        }

        public async Task<Onboarding> GetByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT TOP 1 * FROM {table} WHERE AppUserId = @AppUserId ORDER BY Id DESC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            Onboarding onboarding = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        onboarding = GetFromReader(reader);
                    }
                }
            }
            return onboarding;
        }

        public async Task<List<Onboarding>> GetAllByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId ORDER BY Id DESC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            List<Onboarding> onboardings = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Onboarding onboarding = GetFromReader(reader);
                        onboardings.Add(onboarding);
                    }
                }
            }
            return onboardings;
        }

        public async Task<int> GetAppUserIdById(int id)
        {
            String strCmd = $"SELECT AppUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int clientId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        clientId = Convert.ToInt32(reader["AppUserId"]);
                    }
                }
            }
            return clientId;
        }

        // INSERT
        public async Task<int> Add(Onboarding onboarding)
        {
            String strCmd = $"INSERT INTO {table} (AppUserId, DpiFront, DpiBack, Renap, Portrait, Address, IdentityId, RenapIdentityId, AddressId, Comment, BoardUserId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @DpiFront, @DpiBack, @Renap, @Portrait, @Address, @IdentityId, @RenapIdentityId, @AddressId, @Comment, @BoardUserId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, onboarding.AppUserId);
            DBHelper.AddParam(command, "@DpiFront", SqlDbType.BigInt, onboarding.DpiFront);
            DBHelper.AddParam(command, "@DpiBack", SqlDbType.BigInt, onboarding.DpiBack);
            DBHelper.AddParam(command, "@Renap", SqlDbType.BigInt, onboarding.Renap);
            DBHelper.AddParam(command, "@Portrait", SqlDbType.BigInt, onboarding.Portrait);
            DBHelper.AddParam(command, "@Address", SqlDbType.BigInt, onboarding.Address);
            DBHelper.AddParam(command, "@IdentityId", SqlDbType.Int, onboarding.IdentityId);
            DBHelper.AddParam(command, "@RenapIdentityId", SqlDbType.Int, onboarding.RenapIdentityId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.Int, onboarding.AddressId);
            DBHelper.AddParam(command, "@Comment", SqlDbType.VarChar, onboarding.Comment);
            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, onboarding.BoardUserId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, onboarding.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Onboarding onboarding)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, DpiFront = @DpiFront, DpiBack = @DpiBack, Renap = @Renap, Portrait = @Portrait, Address = @Address, IdentityId = @IdentityId," +
                            " RenapIdentityId = @RenapIdentityId, AddressId = @AddressId, Comment = @Comment, BoardUserId = @BoardUserId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, onboarding.AppUserId);
            DBHelper.AddParam(command, "@DpiFront", SqlDbType.BigInt, onboarding.DpiFront);
            DBHelper.AddParam(command, "@DpiBack", SqlDbType.BigInt, onboarding.DpiBack);
            DBHelper.AddParam(command, "@Renap", SqlDbType.BigInt, onboarding.Renap);
            DBHelper.AddParam(command, "@Portrait", SqlDbType.BigInt, onboarding.Portrait);
            DBHelper.AddParam(command, "@Address", SqlDbType.BigInt, onboarding.Address);
            DBHelper.AddParam(command, "@IdentityId", SqlDbType.Int, onboarding.IdentityId);
            DBHelper.AddParam(command, "@RenapIdentityId", SqlDbType.Int, onboarding.RenapIdentityId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.Int, onboarding.AddressId);
            DBHelper.AddParam(command, "@Comment", SqlDbType.VarChar, onboarding.Comment);
            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, onboarding.BoardUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, onboarding.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateIdentityId(int appUserId, int identityId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, IdentityId = @IdentityId" +
                            " WHERE AppUserId = @AppUserId AND Status = 1";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@IdentityId", SqlDbType.Int, identityId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

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