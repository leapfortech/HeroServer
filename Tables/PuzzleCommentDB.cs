using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PuzzleCommentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-PuzzleComment]";

        private static PuzzleComment GetPuzzleComment(SqlDataReader reader)
        {
            return new PuzzleComment(Convert.ToInt64(reader["Id"]),
                                     Convert.ToInt64(reader["PuzzleId"]),
                                     Convert.ToInt64(reader["AppUserId"]),
                                     reader["Comment"].ToString(),
                                     Convert.ToDateTime(reader["CreateDateTime"]),
                                     Convert.ToDateTime(reader["UpdateDateTime"]),
                                     Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<PuzzleComment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PuzzleComment> puzzleComments = new List<PuzzleComment>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PuzzleComment puzzleComment = GetPuzzleComment(reader);
                         puzzleComments.Add(puzzleComment);
                    }
                }
            }
            return puzzleComments;
        }

        public async Task<PuzzleComment> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            PuzzleComment puzzleComment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         puzzleComment = GetPuzzleComment(reader);
                    }
                }
            }
            return puzzleComment;
        }

        // INSERT
        public async Task<long> Add(PuzzleComment puzzleComment)
        {
            String strCmd = $"INSERT INTO {table}(PuzzleId, AppUserId, Comment, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PuzzleId, @AppUserId, @Comment, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PuzzleId", SqlDbType.BigInt, puzzleComment.PuzzleId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, puzzleComment.AppUserId);
            DBHelper.AddParam(command, "@Comment", SqlDbType.VarChar, puzzleComment.Comment);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, puzzleComment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PuzzleComment puzzleComment)
        {
            String strCmd = $"UPDATE {table} SET PuzzleId = @PuzzleId, AppUserId = @AppUserId, Comment = @Comment, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PuzzleId", SqlDbType.BigInt, puzzleComment.PuzzleId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, puzzleComment.AppUserId);
            DBHelper.AddParam(command, "@Comment", SqlDbType.VarChar, puzzleComment.Comment);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, puzzleComment.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, puzzleComment.Id);

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
