using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class LikeDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Like]";

        private static Like GetLike(SqlDataReader reader)
        {
            return new Like(Convert.ToInt64(reader["Id"]),
                            Convert.ToInt64(reader["PostId"]),
                            Convert.ToInt64(reader["AppUserId"]),
                            Convert.ToInt32(reader["Rank"]),
                            Convert.ToDateTime(reader["CreateDateTime"]),
                            Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Like>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Like> likes = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Like like = GetLike(reader);
                         likes.Add(like);
                    }
                }
            }
            return likes;
        }

        public async Task<Like> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Like like = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         like = GetLike(reader);
                    }
                }
            }
            return like;
        }

        public async Task<Like> Get(long postId, long appUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE PostId = @PostId AND AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

            Like like = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        like = GetLike(reader);
                    }
                }
            }
            return like;
        }

        // INSERT
        public async Task<long> Add(Like like)
        {
            String strCmd = $"INSERT INTO {table}(PostId, AppUserId, Rank, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PostId, @AppUserId, @Rank, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, like.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, like.AppUserId);
            command.AddParam("@Rank", SqlDbType.Int, like.Rank);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, like.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Like like)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, AppUserId = @AppUserId, Rank = @Rank, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, like.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, like.AppUserId);
            command.AddParam("@Rank", SqlDbType.Int, like.Rank);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, like.Status);
            command.AddParam("@Id", SqlDbType.BigInt, like.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateRank(Like like)
        {
            String strCmd = $"UPDATE {table} SET Rank = @Rank, UpdateDateTime = @UpdateDateTime WHERE PostId = @PostId AND AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Rank", SqlDbType.Int, like.Rank);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@PostId", SqlDbType.BigInt, like.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, like.AppUserId);

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

        public async Task<bool> Delete(Like like)
        {
            String strCmd = $"DELETE {table} WHERE PostId = @PostId AND AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, like.PostId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, like.AppUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
