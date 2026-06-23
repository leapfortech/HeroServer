using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PuzzleResultDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-PuzzleResult]";

        private static PuzzleResult GetPuzzleResult(SqlDataReader reader)
        {
            return new PuzzleResult(Convert.ToInt64(reader["Id"]),
                                    Convert.ToInt64(reader["PlayerId"]),
                                    Convert.ToInt64(reader["PuzzleId"]),
                                    Convert.ToInt32(reader["TotalPoints"]),
                                    Convert.ToInt32(reader["Time"]),
                                    Convert.ToInt32(reader["TotalWinPoints"]),
                                    Convert.ToDateTime(reader["LastPlayDateTime"]),
                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                    Convert.ToDateTime(reader["UpdateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<PuzzleResult>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PuzzleResult> puzzleResults = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PuzzleResult puzzleResult = GetPuzzleResult(reader);
                         puzzleResults.Add(puzzleResult);
                    }
                }
            }
            return puzzleResults;
        }

        public async Task<PuzzleResult> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            PuzzleResult puzzleResult = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         puzzleResult = GetPuzzleResult(reader);
                    }
                }
            }
            return puzzleResult;
        }

        // INSERT
        public async Task<long> Add(PuzzleResult puzzleResult)
        {
            String strCmd = $"INSERT INTO {table}(PlayerId, PuzzleId, TotalPoints, Time, TotalWinPoints, LastPlayDateTime, CreateDateTime, UpdateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PlayerId, @PuzzleId, @TotalPoints, @Time, @TotalWinPoints, @LastPlayDateTime, @CreateDateTime, @UpdateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlayerId", SqlDbType.BigInt, puzzleResult.PlayerId);
            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzleResult.PuzzleId);
            command.AddParam("@TotalPoints", SqlDbType.Int, puzzleResult.TotalPoints);
            command.AddParam("@Time", SqlDbType.Int, puzzleResult.Time);
            command.AddParam("@TotalWinPoints", SqlDbType.Int, puzzleResult.TotalWinPoints);
            command.AddParam("@LastPlayDateTime", SqlDbType.DateTime, puzzleResult.LastPlayDateTime);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PuzzleResult puzzleResult)
        {
            String strCmd = $"UPDATE {table} SET PlayerId = @PlayerId, PuzzleId = @PuzzleId, TotalPoints = @TotalPoints, Time = @Time, TotalWinPoints = @TotalWinPoints, LastPlayDateTime = @LastPlayDateTime, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlayerId", SqlDbType.BigInt, puzzleResult.PlayerId);
            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzleResult.PuzzleId);
            command.AddParam("@TotalPoints", SqlDbType.Int, puzzleResult.TotalPoints);
            command.AddParam("@Time", SqlDbType.Int, puzzleResult.Time);
            command.AddParam("@TotalWinPoints", SqlDbType.Int, puzzleResult.TotalWinPoints);
            command.AddParam("@LastPlayDateTime", SqlDbType.DateTime, puzzleResult.LastPlayDateTime);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, puzzleResult.Id);

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
    }
}
