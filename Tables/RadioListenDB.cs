using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class RadioListenDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-RadioListen]";

        private static RadioListen GetRadioListen(SqlDataReader reader)
        {
            return new RadioListen(Convert.ToInt64(reader["Id"]),
                                   Convert.ToInt64(reader["RadioId"]),
                                   Convert.ToInt64(reader["AppUserId"]),
                                   Convert.ToDateTime(reader["CreateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<RadioListen>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<RadioListen> radioListens = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         RadioListen radioListen = GetRadioListen(reader);
                         radioListens.Add(radioListen);
                    }
                }
            }
            return radioListens;
        }

        public async Task<IEnumerable<RadioListen>> GetAllByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE RadioId = @RadioId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);
            command.AddParam("@Status", SqlDbType.Int, status);

            List<RadioListen> radioListens = new List<RadioListen>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        RadioListen radioListen = GetRadioListen(reader);
                        radioListens.Add(radioListen);
                    }
                }
            }
            return radioListens;
        }

        public async Task<List<long>> GetRadioListenIdsByRadioId(long radioId, int status = -1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE RadioId = @RadioId";

            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<long> list = new List<long>();

            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        long id = Convert.ToInt64(reader["Id"]);
                        list.Add(id);
                    }
                }
            }

            return list;
        }

        public async Task<RadioListen> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            RadioListen radioListen = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         radioListen = GetRadioListen(reader);
                    }
                }
            }
            return radioListen;
        }

        public async Task<RadioListen> GetByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE RadioId = @RadioId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);
            command.AddParam("@Status", SqlDbType.Int, status);

            RadioListen radioListen = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        radioListen = GetRadioListen(reader);
                    }
                }
            }
            return radioListen;
        }

        public async Task<long> GetIdByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE RadioId = @RadioId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);
            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            long radioListenId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        radioListenId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return radioListenId;
        }

        // INSERT
        public async Task<long> Add(RadioListen radioListen)
        {
            String strCmd = $"INSERT INTO {table}(RadioId, AppUserId, CreateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@RadioId, @AppUserId, @CreateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioListen.RadioId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, radioListen.AppUserId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(RadioListen radioListen)
        {
            String strCmd = $"UPDATE {table} SET RadioId = @RadioId, AppUserId = @AppUserId WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioListen.RadioId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, radioListen.AppUserId);
            command.AddParam("@Id", SqlDbType.BigInt, radioListen.Id);

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

        public async Task<bool> DeleteByRadioId(long radioId)
        {
            String strCmd = $"DELETE {table} WHERE RadioId = @RadioId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
