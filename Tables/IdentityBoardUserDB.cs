using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class IdentityBoardUserDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-IdentityBoardUser]";

        private static IdentityBoardUser GetIdentityBoardUser(SqlDataReader reader)
        {
            return new IdentityBoardUser(Convert.ToInt64(reader["Id"]),
                                         Convert.ToInt64(reader["BoardUserId"]),
                                         Convert.ToInt64(reader["IdentityId"]),
                                         Convert.ToDateTime(reader["CreateDateTime"]),
                                         Convert.ToDateTime(reader["UpdateDateTime"]),
                                         Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<IdentityBoardUser>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<IdentityBoardUser> identityBoardUsers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         IdentityBoardUser identityBoardUser = GetIdentityBoardUser(reader);
                         identityBoardUsers.Add(identityBoardUser);
                    }
                }
            }
            return identityBoardUsers;
        }

        public async Task<List<long>> GetIdentityIdsByBoardUserId(long boardUserId, int status = -1)
        {
            String strCmd = $"SELECT IdentityId FROM {table} WHERE BoardUserId = @BoardUserId";

            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, boardUserId);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<long> list = new List<long>();

            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        long identityId = Convert.ToInt32(reader["IdentityId"]);
                        list.Add(identityId);
                    }
                }
            }

            return list;
        }

        public async Task<IdentityBoardUser> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            IdentityBoardUser identityBoardUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         identityBoardUser = GetIdentityBoardUser(reader);
                    }
                }
            }
            return identityBoardUser;
        }

        public async Task<IdentityBoardUser> GetByBoardUserId(long boardUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE BoardUserId = @BoardUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, boardUserId);
            command.AddParam("@Status", SqlDbType.Int, status);

            IdentityBoardUser identityBoardUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        identityBoardUser = GetIdentityBoardUser(reader);
                    }
                }
            }
            return identityBoardUser;
        }

        public async Task<long> GetIdByBoardUserId(long boardUserId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE BoardUserId = @BoardUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, boardUserId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

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

        public async Task<long> GetIdentityIdByBoardUserId(long boardUserId, int status = 1)
        {
            String strCmd = $"SELECT IdentityId FROM {table} WHERE BoardUserId = @BoardUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, boardUserId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            long identityId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        identityId = Convert.ToInt64(reader["IdentityId"]);
                    }
                }
            }
            return identityId;
        }

        public async Task<(long, long)> GetIdsByBoardUserId(long boardUserId, int status = 1)
        {
            String strCmd = $"SELECT Id, IdentityId FROM {table} WHERE BoardUserId = @BoardUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, boardUserId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            long id = -1, identityId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                        identityId = Convert.ToInt64(reader["IdentityId"]);
                    }
                }
            }
            return (id, identityId);
        }

        // INSERT
        public async Task<long> Add(IdentityBoardUser identityBoardUser)
        {
            String strCmd = $"INSERT INTO {table}(BoardUserId, IdentityId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@BoardUserId, @IdentityId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, identityBoardUser.BoardUserId);
            command.AddParam("@IdentityId", SqlDbType.BigInt, identityBoardUser.IdentityId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, identityBoardUser.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(IdentityBoardUser identityBoardUser)
        {
            String strCmd = $"UPDATE {table} SET BoardUserId = @BoardUserId, IdentityId = @IdentityId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, identityBoardUser.BoardUserId);
            command.AddParam("@IdentityId", SqlDbType.BigInt, identityBoardUser.IdentityId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, identityBoardUser.Status);
            command.AddParam("@Id", SqlDbType.BigInt, identityBoardUser.Id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);
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

        public async Task<bool> DeleteByBoardUserId(long boardUserId)
        {
            String strCmd = $"DELETE {table} WHERE BoardUserId = @BoardUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@BoardUserId", SqlDbType.BigInt, boardUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
