using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PepIdentityAppUserDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DL-PepIdentityAppUser]";

        private static PepIdentityAppUser GetPepIdentityAppUser(SqlDataReader reader)
        {
            return new PepIdentityAppUser(Convert.ToInt32(reader["Id"]),
                                          Convert.ToInt32(reader["AppUserId"]),
                                          Convert.ToInt32(reader["PepIdentityId"]),
                                          Convert.ToDateTime(reader["CreateDateTime"]),
                                          Convert.ToDateTime(reader["UpdateDateTime"]),
                                          Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<PepIdentityAppUser>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PepIdentityAppUser> pepIdentityAppUsers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PepIdentityAppUser pepIdentityAppUser = GetPepIdentityAppUser(reader);
                         pepIdentityAppUsers.Add(pepIdentityAppUser);
                    }
                }
            }
            return pepIdentityAppUsers;
        }

        public async Task<PepIdentityAppUser> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            PepIdentityAppUser pepIdentityAppUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        pepIdentityAppUser = GetPepIdentityAppUser(reader);
                    }
                }
            }
            return pepIdentityAppUser;
        }

        public async Task<IEnumerable<PepIdentityAppUser>> GetByAppUserId(int appUserId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<PepIdentityAppUser> pepIdentityAppUsers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        PepIdentityAppUser pepIdentityAppUser = GetPepIdentityAppUser(reader);
                        pepIdentityAppUsers.Add(pepIdentityAppUser);
                    }
                }
            }
            return pepIdentityAppUsers;
        }


        // INSERT
        public async Task<int> Add(PepIdentityAppUser pepIdentityAppUser)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, PepIdentityId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @PepIdentityId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, pepIdentityAppUser.AppUserId);
            DBHelper.AddParam(command, "@PepIdentityId", SqlDbType.Int, pepIdentityAppUser.PepIdentityId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, pepIdentityAppUser.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PepIdentityAppUser pepIdentityAppUser)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, PepIdentityId = @PepIdentityId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, pepIdentityAppUser.AppUserId);
            DBHelper.AddParam(command, "@PepIdentityId", SqlDbType.Int, pepIdentityAppUser.PepIdentityId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, pepIdentityAppUser.Id);

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
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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
    }
}
