using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PuzzleDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Puzzle]";

        private static Puzzle GetPuzzle(SqlDataReader reader)
        {
            return new Puzzle(Convert.ToInt64(reader["Id"]),
                              Convert.ToInt64(reader["PostId"]),
                              Convert.ToInt64(reader["PuzzleTypeId"]),
                              Convert.ToInt64(reader["PuzzleSubtypeId"]),
                              reader["Question"].ToString(),
                              reader["Hint"].ToString(),
                              Convert.ToInt32(reader["Difficulty"]),
                              Convert.ToInt32(reader["Points"]),
                              Convert.ToInt32(reader["PlayCount"]),
                              Convert.ToInt32(reader["CreateDateTime"]),
                              Convert.ToDateTime(reader["UpdateDateTime"]),
                              Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Puzzle>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Puzzle> puzzles = new List<Puzzle>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Puzzle puzzle = GetPuzzle(reader);
                         puzzles.Add(puzzle);
                    }
                }
            }
            return puzzles;
        }

        public async Task<Puzzle> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Puzzle puzzle = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         puzzle = GetPuzzle(reader);
                    }
                }
            }
            return puzzle;
        }

        // INSERT
        public async Task<long> Add(Puzzle puzzle)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, PuzzleTypeId, PuzzleSubtypeId, Question, Hint, Difficulty, Points, PlayCount, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @PuzzleTypeId, @PuzzleSubtypeId, @Question, @Hint, @Difficulty, @Points, @PlayCount, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, puzzle.PostId);
            DBHelper.AddParam(command, "@PuzzleTypeId", SqlDbType.BigInt, puzzle.PuzzleTypeId);
            DBHelper.AddParam(command, "@PuzzleSubtypeId", SqlDbType.BigInt, puzzle.PuzzleSubtypeId);
            DBHelper.AddParam(command, "@Question", SqlDbType.VarChar, puzzle.Question);
            DBHelper.AddParam(command, "@Hint", SqlDbType.VarChar, puzzle.Hint);
            DBHelper.AddParam(command, "@Difficulty", SqlDbType.Int, puzzle.Difficulty);
            DBHelper.AddParam(command, "@Points", SqlDbType.Int, puzzle.Points);
            DBHelper.AddParam(command, "@PlayCount", SqlDbType.Int, puzzle.PlayCount);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.Int, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, puzzle.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Puzzle puzzle)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, PuzzleTypeId = @PuzzleTypeId, PuzzleSubtypeId = @PuzzleSubtypeId, Question = @Question, Hint = @Hint, Difficulty = @Difficulty, Points = @Points, PlayCount = @PlayCount, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, puzzle.PostId);
            DBHelper.AddParam(command, "@PuzzleTypeId", SqlDbType.BigInt, puzzle.PuzzleTypeId);
            DBHelper.AddParam(command, "@PuzzleSubtypeId", SqlDbType.BigInt, puzzle.PuzzleSubtypeId);
            DBHelper.AddParam(command, "@Question", SqlDbType.VarChar, puzzle.Question);
            DBHelper.AddParam(command, "@Hint", SqlDbType.VarChar, puzzle.Hint);
            DBHelper.AddParam(command, "@Difficulty", SqlDbType.Int, puzzle.Difficulty);
            DBHelper.AddParam(command, "@Points", SqlDbType.Int, puzzle.Points);
            DBHelper.AddParam(command, "@PlayCount", SqlDbType.Int, puzzle.PlayCount);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, puzzle.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, puzzle.Id);

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
