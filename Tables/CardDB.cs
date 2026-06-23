using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class CardDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Card]";

        public static Card GetCard(SqlDataReader reader)
        {
            return new Card(Convert.ToInt64(reader["Id"]), Convert.ToInt64(reader["AppUserId"]), reader["CSToken"].ToString(),
                            Convert.ToInt32(reader["TypeId"]), reader["Number"].ToString(), Convert.ToInt32(reader["Digits"]),
                            Convert.ToDateTime(reader["ExpirationDate"]), reader["Holder"].ToString(),
                            Convert.ToDateTime(reader["CreateDateTime"]), Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        // SELECT
        public async Task<IEnumerable<Card>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Card> cards = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Card card = GetCard(reader);
                        cards.Add(card);
                    }
                }
            }

            return cards;
        }


        public async Task<Card> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Card card = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        card = GetCard(reader);
                    }
                }
            }
            return card;
        }

        public async Task<Card> GetByAppUserId(long appUserId, int status = -1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            Card card = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        card = GetCard(reader);
                    }
                }
            }

            return card;
        }

        public async Task<long> GetIdByAppUserId(long appUserId, int status = -1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<Card> GetByCSToken(String csToken)
        {
            String strCmd = $"SELECT * FROM {table} WHERE CSToken = @CSToken";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@CSToken", SqlDbType.VarChar, csToken);

            Card card = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        card = GetCard(reader);
                    }
                }
            }
            return card;
        }

        public async Task<int> GetTodayCount(long appUserId, float utcOffset)
        {
            String strCmd = $"SELECT COUNT(1) AS Count FROM {table}" +
                            " WHERE AppUserId = @AppUserId" +
                            " AND CONVERT(DATE, DATEADD(hour, @UtcOffset, CreateDateTime)) = CONVERT(DATE, @TodayDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            command.AddParam("@UtcOffset", SqlDbType.Float, utcOffset);
            command.AddParam("@TodayDateTime", SqlDbType.DateTime2, DateTime.Now.AddHours(utcOffset));

            int count = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        count = Convert.ToInt32(reader["Count"]);
                }
            }

            return count;
        }

        // INSERT
        public async Task<long> Add(Card card)
        {
            String strCmd = $"INSERT INTO {table} (Id, AppUserId, CSToken, TypeId, Number, Digits, ExpirationDate, Holder, CreateDateTime, UpdateDateTime, Status)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @AppUserId, @CSToken, @TypeId, @Number, @Digits, @ExpirationDate, @Holder, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('C'));
            command.AddParam("@AppUserId", SqlDbType.BigInt, card.AppUserId);
            command.AddParam("@CSToken", SqlDbType.VarChar, card.CSToken);
            command.AddParam("@TypeId", SqlDbType.Int, card.TypeId);
            command.AddParam("@Number", SqlDbType.VarChar, card.Number);
            command.AddParam("@Digits", SqlDbType.Int, card.Digits);
            command.AddParam("@ExpirationDate", SqlDbType.DateTime2, card.ExpirationDate);
            command.AddParam("@Holder", SqlDbType.VarChar, card.Holder);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, card.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Card card)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET AppUserId = @AppUserId, CSToken = @CSToken, TypeId = @TypeId, Number = @Number, Digits = @Digits, ExpirationDate = @ExpirationDate," +
                            " Holder = @Holder, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, card.AppUserId);
            command.AddParam("@CSToken", SqlDbType.VarChar, card.CSToken);
            command.AddParam("@TypeId", SqlDbType.Int, card.TypeId);
            command.AddParam("@Number", SqlDbType.VarChar, card.Number);
            command.AddParam("@Digits", SqlDbType.Int, card.Digits);
            command.AddParam("@ExpirationDate", SqlDbType.DateTime2, card.ExpirationDate);
            command.AddParam("@Holder", SqlDbType.VarChar, card.Holder);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, card.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetStatus(String csToken, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE CSToken = @CSToken AND Status != @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@CSToken", SqlDbType.VarChar, csToken);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetHolder(long id, String holder, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Holder = @Holder, UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Holder", SqlDbType.VarChar, holder);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
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
            String strCmd = $"DELETE FROM {table}";

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

        public async Task<bool> DeleteByAppUserId(long appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
