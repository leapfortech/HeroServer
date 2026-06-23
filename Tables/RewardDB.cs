using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class RewardDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Reward]";

        private static Reward GetReward(SqlDataReader reader)
        {
            return new Reward(Convert.ToInt64(reader["Id"]),
                              Convert.ToInt64(reader["PlayerId"]),
                              Convert.ToInt64(reader["RewardTypeId"]),
                              reader["EarnedDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EarnedDateTime"]),
                              Convert.ToDateTime(reader["CreateDateTime"]),
                              Convert.ToDateTime(reader["UpdateDateTime"]),
                              Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Reward>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Reward> rewards = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Reward reward = GetReward(reader);
                         rewards.Add(reward);
                    }
                }
            }
            return rewards;
        }

        public async Task<Reward> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Reward reward = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         reward = GetReward(reader);
                    }
                }
            }
            return reward;
        }

        // INSERT
        public async Task<long> Add(Reward reward)
        {
            String strCmd = $"INSERT INTO {table}(Id, PlayerId, RewardTypeId, EarnedDateTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PlayerId, @RewardTypeId, @EarnedDateTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('W'));
            command.AddParam("@PlayerId", SqlDbType.BigInt, reward.PlayerId);
            command.AddParam("@RewardTypeId", SqlDbType.BigInt, reward.RewardTypeId);
            command.AddParam("@EarnedDateTime", SqlDbType.DateTime, reward.EarnedDateTime);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, reward.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Reward reward)
        {
            String strCmd = $"UPDATE {table} SET PlayerId = @PlayerId, RewardTypeId = @RewardTypeId, EarnedDateTime = @EarnedDateTime, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PlayerId", SqlDbType.BigInt, reward.PlayerId);
            command.AddParam("@RewardTypeId", SqlDbType.BigInt, reward.RewardTypeId);
            command.AddParam("@EarnedDateTime", SqlDbType.DateTime, reward.EarnedDateTime);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, reward.Status);
            command.AddParam("@Id", SqlDbType.BigInt, reward.Id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

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
