using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PostReadDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-PostRead]";

        private static PostRead GetPostRead(SqlDataReader reader)
        {
            return new PostRead(Convert.ToInt64(reader["Id"]),
                                Convert.ToInt64(reader["PostId"]),
                                Convert.ToInt64(reader["AppUserId"]),
                                Convert.ToDateTime(reader["CreateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<PostRead>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PostRead> postReads = new List<PostRead>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PostRead postRead = GetPostRead(reader);
                         postReads.Add(postRead);
                    }
                }
            }
            return postReads;
        }

        public async Task<IEnumerable<PostRead>> GetAllByPostId(long postId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE PostId = @PostId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<PostRead> postReads = new List<PostRead>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        PostRead postRead = GetPostRead(reader);
                        postReads.Add(postRead);
                    }
                }
            }
            return postReads;
        }

        public async Task<PostRead> GetByPostId(long postId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE PostId = @PostId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            PostRead postRead = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        postRead = GetPostRead(reader);
                    }
                }
            }
            return postRead;
        }

        public async Task<long> GetIdByPostId(long postId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE PostId = @PostId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            long postReadId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        postReadId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return postReadId;
        }

        // INSERT
        public async Task<long> Add(PostRead postRead)
        {
            String strCmd = $"INSERT INTO {table}(PostId, AppUserId, CreateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PostId, @AppUserId, @CreateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postRead.PostId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, postRead.AppUserId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PostRead postRead)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, AppUserId = @AppUserId WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postRead.PostId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, postRead.AppUserId);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, postRead.Id);

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
