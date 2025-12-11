using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class CommentPlaintDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-CommentPlaint]";

        private static CommentPlaint GetCommentPlaint(SqlDataReader reader)
        {
            return new CommentPlaint(Convert.ToInt64(reader["Id"]),
                                     Convert.ToInt64(reader["PlaintTypeId"]),
                                     Convert.ToInt64(reader["CommentId"]),
                                     Convert.ToInt64(reader["AppUserId"]),
                                     Convert.ToDateTime(reader["CreateDateTime"]),
                                     Convert.ToDateTime(reader["UpdateDateTime"]),
                                     Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<CommentPlaint>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<CommentPlaint> commentPlaints = new List<CommentPlaint>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         CommentPlaint commentPlaint = GetCommentPlaint(reader);
                         commentPlaints.Add(commentPlaint);
                    }
                }
            }
            return commentPlaints;
        }

        public async Task<CommentPlaint> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            CommentPlaint commentPlaint = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         commentPlaint = GetCommentPlaint(reader);
                    }
                }
            }
            return commentPlaint;
        }

        // INSERT
        public async Task<long> Add(CommentPlaint commentPlaint)
        {
            String strCmd = $"INSERT INTO {table}(PlaintTypeId, CommentId, AppUserId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PlaintTypeId, @CommentId, @AppUserId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PlaintTypeId", SqlDbType.BigInt, commentPlaint.PlaintTypeId);
            DBHelper.AddParam(command, "@CommentId", SqlDbType.BigInt, commentPlaint.CommentId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, commentPlaint.AppUserId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, commentPlaint.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(CommentPlaint commentPlaint)
        {
            String strCmd = $"UPDATE {table} SET PlaintTypeId = @PlaintTypeId, CommentId = @CommentId, AppUserId = @AppUserId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PlaintTypeId", SqlDbType.BigInt, commentPlaint.PlaintTypeId);
            DBHelper.AddParam(command, "@CommentId", SqlDbType.BigInt, commentPlaint.CommentId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, commentPlaint.AppUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, commentPlaint.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, commentPlaint.Id);

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
