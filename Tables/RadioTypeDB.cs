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

        // GET
        public async Task<IEnumerable<RadioType>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

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

        public async Task<IEnumerable<RadioType>> GetAllByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE RadioId = @RadioId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioType.RadioId);
            DBHelper.AddParam(command, "@RadioTypeId", SqlDbType.BigInt, radioType.RadioTypeId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, radioType.Status);

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

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioType.RadioId);
            DBHelper.AddParam(command, "@RadioTypeId", SqlDbType.BigInt, radioType.RadioTypeId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, radioType.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, radioType.Id);

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

        public async Task<bool> UpdateStatus(long id, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE Id = @Id AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioId);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);

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
