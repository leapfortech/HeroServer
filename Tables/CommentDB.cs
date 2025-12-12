using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class CommentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Comment]";

        private static Comment GetComment(SqlDataReader reader)
        {
            return new Comment(Convert.ToInt64(reader["Id"]),
                               Convert.ToInt64(reader["PostId"]),
                               Convert.ToInt64(reader["AppUserId"]),
                               reader["Message"].ToString(),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["Status"]));
        }

        public static CommentFull GetCommentFull(SqlDataReader reader)
        {
            return new CommentFull(Convert.ToInt64(reader["Id"]),
                                   Convert.ToInt64(reader["AppUserId"]),
                                   reader["AppUserAlias"].ToString(),
                                   reader["Message"].ToString(),
                                   Convert.ToDateTime(reader["UpdateDateTime"]),
                                   Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Comment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Comment> comments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Comment comment = GetComment(reader);
                         comments.Add(comment);
                    }
                }
            }
            return comments;
        }

        public async Task<Comment> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Comment comment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         comment = GetComment(reader);
                    }
                }
            }
            return comment;
        }

        // INSERT
        public async Task<long> Add(Comment comment)
        {
            String strCmd = $"INSERT INTO {table}(PostId, AppUserId, Message, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PostId, @AppUserId, @Message, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, comment.PostId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, comment.AppUserId);
            DBHelper.AddParam(command, "@Message", SqlDbType.VarChar, comment.Message);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, comment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Comment comment)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, AppUserId = @AppUserId, Message = @Message, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, comment.PostId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, comment.AppUserId);
            DBHelper.AddParam(command, "@Message", SqlDbType.VarChar, comment.Message);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, comment.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, comment.Id);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
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
