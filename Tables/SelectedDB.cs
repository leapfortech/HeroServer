using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class SelectedDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-Selected]";

        private static Selected GetSelected(SqlDataReader reader)
        {
            return new Selected(Convert.ToInt64(reader["Id"]),
                                Convert.ToInt64(reader["PostId"]),
                                Convert.ToInt64(reader["AppUserId"]),
                                Convert.ToDateTime(reader["CreateDateTime"]),
                                Convert.ToDateTime(reader["UpdateDateTime"]),
                                Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Selected>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Selected> selecteds = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Selected selected = GetSelected(reader);
                         selecteds.Add(selected);
                    }
                }
            }
            return selecteds;
        }

        public async Task<IEnumerable<Selected>> GetAllByPostId(long postId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE PostId = @PostId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@Status", SqlDbType.Int, status);

            List<Selected> selecteds = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Selected selected = GetSelected(reader);
                        selecteds.Add(selected);
                    }
                }
            }
            return selecteds;
        }

        public async Task<List<long>> GetAllIdsByPostId(long postId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE PostId = @PostId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<long> selectedIds = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        selectedIds.Add(Convert.ToInt64(reader["Id"]));
                    }
                }
            }
            return selectedIds;
        }

        public async Task<Selected> GetById(long id, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@Status", SqlDbType.Int, status);

            Selected selected = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        selected = GetSelected(reader);
                    }
                }
            }
            return selected;
        }

        public async Task<Selected> Get(long postId, long appUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE PostId = @Id AND AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

            Selected selected = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        selected = GetSelected(reader);
                    }
                }
            }
            return selected;
        }

        // INSERT
        public async Task<long> Add(Selected selected)
        {
            String strCmd = $"INSERT INTO {table}(PostId, AppUserId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PostId, @AppUserId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, selected.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, selected.AppUserId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, selected.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        //public async Task<bool> Update(Selected selected)
        //{
        //    String strCmd = $"UPDATE {table} SET PostId = @PostId, AppUserId = @AppUserId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

        //    SqlCommand command = new SqlCommand(strCmd, conn);

        //    command.AddParam("@PostId", SqlDbType.BigInt, selected.PostId);
        //    command.AddParam("@AppUserId", SqlDbType.BigInt, selected.AppUserId);
        //    command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
        //    command.AddParam("@Status", SqlDbType.Int, selected.Status);
        //    command.AddParam("@Id", SqlDbType.BigInt, selected.Id);

        //    using (conn)
        //    {
        //        await conn.OpenAsync();
        //        return await command.ExecuteNonQueryAsync() == 1;
        //    }
        //}

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

        public async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE PostId = @PostId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);

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

        public async Task<bool> DeleteByPostId(long postId)
        {
            String strCmd = $"DELETE {table} WHERE PostId = @PostId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> Delete(Selected selected)
        {
            String strCmd = $"DELETE {table} WHERE PostId = @PostId AND AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, selected.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, selected.AppUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
