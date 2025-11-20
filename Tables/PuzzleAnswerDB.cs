using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PuzzleAnswerDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-PuzzleAnswer]";

        private static PuzzleAnswer GetPuzzleAnswer(SqlDataReader reader)
        {
            return new PuzzleAnswer(Convert.ToInt64(reader["Id"]),
                                    Convert.ToInt64(reader["PuzzleId"]),
                                    reader["Description"].ToString(),
                                    Convert.ToInt32(reader["IsCorrect"]),
                                    Convert.ToDateTime(reader["CreateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<PuzzleAnswer>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PuzzleAnswer> puzzleAnswers = new List<PuzzleAnswer>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PuzzleAnswer puzzleAnswer = GetPuzzleAnswer(reader);
                         puzzleAnswers.Add(puzzleAnswer);
                    }
                }
            }
            return puzzleAnswers;
        }

        public async Task<PuzzleAnswer> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            PuzzleAnswer puzzleAnswer = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         puzzleAnswer = GetPuzzleAnswer(reader);
                    }
                }
            }
            return puzzleAnswer;
        }

        // INSERT
        public async Task<long> Add(PuzzleAnswer puzzleAnswer)
        {
            String strCmd = $"INSERT INTO {table}(Id, PuzzleId, Description, IsCorrect, CreateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PuzzleId, @Description, @IsCorrect, @CreateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PuzzleId", SqlDbType.BigInt, puzzleAnswer.PuzzleId);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, puzzleAnswer.Description);
            DBHelper.AddParam(command, "@IsCorrect", SqlDbType.Int, puzzleAnswer.IsCorrect);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PuzzleAnswer puzzleAnswer)
        {
            String strCmd = $"UPDATE {table} SET PuzzleId = @PuzzleId, Description = @Description, IsCorrect = @IsCorrect WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PuzzleId", SqlDbType.BigInt, puzzleAnswer.PuzzleId);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, puzzleAnswer.Description);
            DBHelper.AddParam(command, "@IsCorrect", SqlDbType.Int, puzzleAnswer.IsCorrect);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, puzzleAnswer.Id);

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
