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
                              Convert.ToInt64(reader["PuzzleSubtypeId"]),
                              Convert.ToInt64(reader["CountryId"]),
                              reader["Question"].ToString(),
                              reader["Hint"].ToString(),
                              Convert.ToInt32(reader["Difficulty"]),
                              Convert.ToInt32(reader["Points"]),
                              Convert.ToInt32(reader["PlayCount"]),
                              Convert.ToInt32(reader["CreateDateTime"]),
                              Convert.ToDateTime(reader["UpdateDateTime"]),
                              Convert.ToInt32(reader["Status"]));
        }

        public static PuzzleFull GetPuzzleFull(SqlDataReader reader)
        {
            return new PuzzleFull(Convert.ToInt64(reader["Id"]),

                                  Convert.ToInt64(reader["PostId"]),
                                  Convert.ToInt64(reader["AppUserId"]),
                                  reader["AppUserAlias"].ToString(),
                                  Convert.ToInt64(reader["PostSubtypeId"]),
                                  Convert.ToInt64(reader["PostCountryId"]),
                                  Convert.ToInt64(reader["PostStateId"]),
                                  reader["Title"].ToString(),
                                  reader["Summary"].ToString(),
                                  reader["Description"].ToString(),
                                  Convert.ToInt32(reader["ImageCount"]),
                                  Convert.ToInt32(reader["LikeCount"]),
                                  Convert.ToDateTime(reader["PublicationDateTime"]),
                                  Convert.ToInt32(reader["PostStatus"]),

                                  Convert.ToInt64(reader["PuzzleSubtypeId"]),
                                  Convert.ToInt64(reader["CountryId"]),
                                  reader["Question"].ToString(),
                                  reader["Hint"].ToString(),
                                  Convert.ToInt32(reader["Difficulty"]),
                                  Convert.ToInt32(reader["Points"]),
                                  Convert.ToInt32(reader["PlayCount"]),
                                  Convert.ToInt32(reader["Status"]),
                                  null);     //PuzzleAnswerFulls 
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

        // GET FULL
        public async Task<PuzzleFull> GetFullById(long id)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.PuzzleSubtypeId, {table}.CountryId, {table}.Question, {table}.Hint," +
                            $" {table}.Difficulty, {table}.Points, {table}.PlayCount, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT Id, PuzzleId, Description, IsCorrect, UpdateDateTime" +
                      " FROM [D-PuzzleAnswer]" +
                      " WHERE PuzzleId = @Id;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            PuzzleFull puzzleFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    puzzleFull = GetPuzzleFull(reader);

                    await reader.NextResultAsync();
                    puzzleFull.PuzzleAnswerFulls = [];
                    while (await reader.ReadAsync())
                        puzzleFull.PuzzleAnswerFulls.Add(PuzzleAnswerDB.GetPuzzleAnswerFull(reader));
                }
            }

            return puzzleFull;
        }

        public async Task<PuzzleFull> GetFullByPostId(long postId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.PuzzleSubtypeId, {table}.CountryId, {table}.Question, {table}.Hint," +
                            $" {table}.Difficulty, {table}.Points, {table}.PlayCount, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT Id, PuzzleId, Description, IsCorrect, UpdateDateTime" +
                      " FROM [D-PuzzleAnswer]" +
                      " WHERE PuzzleId IN" +
                      $" (SELECT Id FROM {table} WHERE PostId = @PostId);";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

            PuzzleFull puzzleFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    puzzleFull = GetPuzzleFull(reader);

                    await reader.NextResultAsync();
                    puzzleFull.PuzzleAnswerFulls = [];
                    while (await reader.ReadAsync())
                        puzzleFull.PuzzleAnswerFulls.Add(PuzzleAnswerDB.GetPuzzleAnswerFull(reader));
                }
            }

            return puzzleFull;
        }

        public async Task<PuzzleDataFull> GetFullsByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.PuzzleSubtypeId, {table}.CountryId, {table}.Question, {table}.Hint," +
                            $" {table}.Difficulty, {table}.Points, {table}.PlayCount, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";

            if (status != -1)
                strCmd += $" WHERE {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd +=  "SELECT PuzzleAnswer.Id, PuzzleAnswer.PuzzleId, PuzzleAnswer.Description," +
                       " PuzzleAnswer.IsCorrect, PuzzleAnswer.UpdateDateTime" +
                       " FROM [D-PuzzleAnswer] AS PuzzleAnswer" +
                      $" JOIN {table} ON (PuzzleAnswer.PuzzleId = {table}.Id)" +
                       " WHERE 1 = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            PuzzleDataFull puzzleDataFull = new PuzzleDataFull();

            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<PuzzleFull> puzzleFulls = [];
                    while (await reader.ReadAsync())
                        puzzleFulls.Add(GetPuzzleFull(reader));
                    puzzleDataFull.PuzzleFulls = puzzleFulls;

                    await reader.NextResultAsync();
                    List<PuzzleAnswerFull> puzzleAnswerFulls = [];
                    while (await reader.ReadAsync())
                        puzzleAnswerFulls.Add(PuzzleAnswerDB.GetPuzzleAnswerFull(reader));
                    puzzleDataFull.PuzzleAnswerFulls = puzzleAnswerFulls;
                }
            }

            return puzzleDataFull;
        }

        // INSERT
        public async Task<long> Add(Puzzle puzzle)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, PuzzleSubtypeId, CountryId, Question, Hint, Difficulty, Points, PlayCount, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @PuzzleSubtypeId, @CountryId, @Question, @Hint, @Difficulty, @Points, @PlayCount, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, puzzle.PostId);
            DBHelper.AddParam(command, "@PuzzleSubtypeId", SqlDbType.BigInt, puzzle.PuzzleSubtypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, puzzle.CountryId);
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
            String strCmd = $"UPDATE {table} SET PostId = @PostId, PuzzleSubtypeId = @PuzzleSubtypeId, CountryId = @CountryId, Question = @Question, Hint = @Hint, Difficulty = @Difficulty, Points = @Points, PlayCount = @PlayCount, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, puzzle.PostId);
            DBHelper.AddParam(command, "@PuzzleSubtypeId", SqlDbType.BigInt, puzzle.PuzzleSubtypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, puzzle.CountryId);
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
