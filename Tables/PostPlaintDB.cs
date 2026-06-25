using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PostPlaintDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-PostPlaint]";

        private static PostPlaint GetPostPlaint(SqlDataReader reader)
        {
            return new PostPlaint(Convert.ToInt64(reader["Id"]),
                                  Convert.ToInt64(reader["PlaintTypeId"]),
                                  Convert.ToInt64(reader["PostId"]),
                                  Convert.ToInt64(reader["AppUserId"]),
                                  Convert.ToDateTime(reader["CreateDateTime"]),
                                  Convert.ToDateTime(reader["UpdateDateTime"]),
                                  Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<PostPlaint>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PostPlaint> postPlaints = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PostPlaint postPlaint = GetPostPlaint(reader);
                         postPlaints.Add(postPlaint);
                    }
                }
            }
            return postPlaints;
        }

        public async Task<PostPlaint> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            PostPlaint postPlaint = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         postPlaint = GetPostPlaint(reader);
                    }
                }
            }
            return postPlaint;
        }

        public async Task<bool> ExistsPlaintByAppUserId(long postId, long appUserId)
        {
            String strCmd = $@"SELECT COUNT(*) FROM {table} WHERE PostId = @PostId AND AppUserId = @AppUserId AND Status = 1";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync() > 0;
            }
        }

        public async Task<int> GetPlaintCountByPostId(long postId)
        {
            String strCmd = $@"SELECT COUNT(*) FROM {table} WHERE PostId = @PostId AND Status = 1";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // INSERT
        public async Task<long> Add(PostPlaint postPlaint)
        {
            String strCmd = $"INSERT INTO {table}(PlaintTypeId, PostId, AppUserId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PlaintTypeId, @PostId, @AppUserId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlaintTypeId", SqlDbType.BigInt, postPlaint.PlaintTypeId);
            command.AddParam("@PostId", SqlDbType.BigInt, postPlaint.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, postPlaint.AppUserId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, postPlaint.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PostPlaint postPlaint)
        {
            String strCmd = $"UPDATE {table} SET PlaintTypeId = @PlaintTypeId, PostId = @PostId, AppUserId = @AppUserId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlaintTypeId", SqlDbType.BigInt, postPlaint.PlaintTypeId);
            command.AddParam("@PostId", SqlDbType.BigInt, postPlaint.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, postPlaint.AppUserId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, postPlaint.Status);
            command.AddParam("@Id", SqlDbType.BigInt, postPlaint.Id);

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
    }
}
