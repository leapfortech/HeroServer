using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PlayerDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Player]";

        private static Player GetPlayer(SqlDataReader reader)
        {
            return new Player(Convert.ToInt64(reader["Id"]),
                              Convert.ToInt64(reader["AppUserId"]),
                              Convert.ToInt32(reader["Rank"]),
                              Convert.ToInt32(reader["PuzzleCount"]),
                              Convert.ToInt32(reader["TotalPoints"]),
                              Convert.ToDateTime(reader["LastPlayDateTime"]),
                              Convert.ToDateTime(reader["CreateDateTime"]),
                              Convert.ToDateTime(reader["UpdateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<Player>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Player> players = new List<Player>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Player player = GetPlayer(reader);
                         players.Add(player);
                    }
                }
            }
            return players;
        }

        public async Task<Player> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Player player = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         player = GetPlayer(reader);
                    }
                }
            }
            return player;
        }

        // INSERT
        public async Task<long> Add(Player player)
        {
            String strCmd = $"INSERT INTO {table}(Id, AppUserId, Rank, PuzzleCount, TotalPoints, LastPlayDateTime, CreateDateTime, UpdateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @AppUserId, @Rank, @PuzzleCount, @TotalPoints, @LastPlayDateTime, @CreateDateTime, @UpdateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, player.AppUserId);
            DBHelper.AddParam(command, "@Rank", SqlDbType.Int, player.Rank);
            DBHelper.AddParam(command, "@PuzzleCount", SqlDbType.Int, player.PuzzleCount);
            DBHelper.AddParam(command, "@TotalPoints", SqlDbType.Int, player.TotalPoints);
            DBHelper.AddParam(command, "@LastPlayDateTime", SqlDbType.DateTime, player.LastPlayDateTime);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Player player)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, Rank = @Rank, PuzzleCount = @PuzzleCount, TotalPoints = @TotalPoints, LastPlayDateTime = @LastPlayDateTime, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, player.AppUserId);
            DBHelper.AddParam(command, "@Rank", SqlDbType.Int, player.Rank);
            DBHelper.AddParam(command, "@PuzzleCount", SqlDbType.Int, player.PuzzleCount);
            DBHelper.AddParam(command, "@TotalPoints", SqlDbType.Int, player.TotalPoints);
            DBHelper.AddParam(command, "@LastPlayDateTime", SqlDbType.DateTime, player.LastPlayDateTime);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, player.Id);

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
