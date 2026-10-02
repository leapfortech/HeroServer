using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class RadioTypeDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-RadioType]";

        private static RadioType GetRadioType(SqlDataReader reader)
        {
            return new RadioType(Convert.ToInt64(reader["Id"]),
                                 Convert.ToInt64(reader["RadioId"]),
                                 Convert.ToInt64(reader["RadioTypeId"]),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["Status"]));
        }

        public static RadioTypeFull GetRadioTypeFull(SqlDataReader reader)
        {
            return new RadioTypeFull(Convert.ToInt64(reader["Id"]),
                                     Convert.ToInt64(reader["RadioTypeId"]),
                                     Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<RadioType>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<RadioType> radioTypes = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         RadioType radioType = GetRadioType(reader);
                         radioTypes.Add(radioType);
                    }
                }
            }
            return radioTypes;
        }

        public async Task<List<long>> GetRadioTypeIdsByRadioId(long radioId, int status = -1)
        {
            String strCmd = $"SELECT RadioTypeId FROM {table} WHERE RadioId = @RadioId";

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
                        long radioTypeId = Convert.ToInt64(reader["RadioTypeId"]);
                        list.Add(radioTypeId);
                    }
                }
            }

            return list;
        }

        public async Task<IEnumerable<RadioType>> GetAllByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE RadioId = @RadioId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);
            command.AddParam("@Status", SqlDbType.Int, status);

            List<RadioType> radioTypes = new List<RadioType>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        RadioType radioType = GetRadioType(reader);
                        radioTypes.Add(radioType);
                    }
                }
            }
            return radioTypes;
        }

        public async Task<RadioType> GetById(long id, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@Status", SqlDbType.Int, status);

            RadioType radioType = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         radioType = GetRadioType(reader);
                    }
                }
            }
            return radioType;
        }

        public async Task<RadioType> GetByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE RadioId = @RadioId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);
            command.AddParam("@Status", SqlDbType.Int, status);

            RadioType radioType = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        radioType = GetRadioType(reader);
                    }
                }
            }
            return radioType;
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

            long radioTypeId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        radioTypeId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return radioTypeId;
        }

        // INSERT
        public async Task<long> Add(RadioType radioType)
        {
            String strCmd = $"INSERT INTO {table}(RadioId, RadioTypeId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@RadioId, @RadioTypeId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioType.RadioId);
            command.AddParam("@RadioTypeId", SqlDbType.BigInt, radioType.RadioTypeId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, radioType.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(RadioType radioType)
        {
            String strCmd = $"UPDATE {table} SET RadioId = @RadioId, RadioTypeId = @RadioTypeId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@RadioId", SqlDbType.BigInt, radioType.RadioId);
            command.AddParam("@RadioTypeId", SqlDbType.BigInt, radioType.RadioTypeId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, radioType.Status);
            command.AddParam("@Id", SqlDbType.BigInt, radioType.Id);

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

        public async Task<bool> UpdateStatus(long id, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE Id = @Id AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByRadioId(long radioId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE RadioId = @RadioId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@RadioId", SqlDbType.BigInt, radioId);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);

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
