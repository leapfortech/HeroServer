using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PuzzlePlayerDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-PuzzlePlayer]";

        private static PuzzlePlayer GetPuzzlePlayer(SqlDataReader reader)
        {
            return new PuzzlePlayer(Convert.ToInt64(reader["Id"]),
                                    Convert.ToInt64(reader["PlayerId"]),
                                    Convert.ToInt64(reader["PuzzleId"]),
                                    Convert.ToInt32(reader["IsGuessed"]),
                                    reader["AttemptDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["AttemptDateTime"]),
                                    Convert.ToDateTime(reader["CreateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<PuzzlePlayer>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PuzzlePlayer> puzzlePlayers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PuzzlePlayer puzzlePlayer = GetPuzzlePlayer(reader);
                         puzzlePlayers.Add(puzzlePlayer);
                    }
                }
            }
            return puzzlePlayers;
        }

        public async Task<PuzzlePlayer> GetById(long id, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@Status", SqlDbType.Int, status);

            PuzzlePlayer puzzlePlayer = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         puzzlePlayer = GetPuzzlePlayer(reader);
                    }
                }
            }
            return puzzlePlayer;
        }

        public async Task<PuzzlePlayer> GetByPlayerId(long playerId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE PlayerId = @PlayerId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlayerId", SqlDbType.BigInt, playerId);
            command.AddParam("@Status", SqlDbType.Int, status);

            PuzzlePlayer puzzlePlayer = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        puzzlePlayer = GetPuzzlePlayer(reader);
                    }
                }
            }
            return puzzlePlayer;
        }

        public async Task<long> GetIdByPlayerId(long playerId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE PlayerId = @PlayerId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlayerId", SqlDbType.BigInt, playerId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            long puzzlePlayerId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        puzzlePlayerId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return puzzlePlayerId;
        }

        // INSERT
        public async Task<long> Add(PuzzlePlayer puzzlePlayer)
        {
            String strCmd = $"INSERT INTO {table}(PlayerId, PuzzleId, IsGuessed, AttemptDateTime, CreateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@PlayerId, @PuzzleId, @IsGuessed, @AttemptDateTime, @CreateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlayerId", SqlDbType.BigInt, puzzlePlayer.PlayerId);
            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzlePlayer.PuzzleId);
            command.AddParam("@IsGuessed", SqlDbType.Int, puzzlePlayer.IsGuessed);
            command.AddParam("@AttemptDateTime", SqlDbType.DateTime, puzzlePlayer.AttemptDateTime);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PuzzlePlayer puzzlePlayer)
        {
            String strCmd = $"UPDATE {table} SET PlayerId = @PlayerId, PuzzleId = @PuzzleId, IsGuessed = @IsGuessed, AttemptDateTime = @AttemptDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlayerId", SqlDbType.BigInt, puzzlePlayer.PlayerId);
            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzlePlayer.PuzzleId);
            command.AddParam("@IsGuessed", SqlDbType.Int, puzzlePlayer.IsGuessed);
            command.AddParam("@AttemptDateTime", SqlDbType.DateTime, puzzlePlayer.AttemptDateTime);
            command.AddParam("@Id", SqlDbType.BigInt, puzzlePlayer.Id);

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
