using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class RadioLanguageDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-RadioLanguage]";

        private static RadioLanguage GetRadioLanguage(SqlDataReader reader)
        {
            return new RadioLanguage(Convert.ToInt64(reader["Id"]),
                                     Convert.ToInt64(reader["RadioId"]),
                                     Convert.ToInt64(reader["LanguageTypeId"]),
                                     Convert.ToDateTime(reader["CreateDateTime"]),
                                     Convert.ToDateTime(reader["UpdateDateTime"]),
                                     Convert.ToInt32(reader["Status"]));
        }

        public static RadioLanguageFull GetRadioLanguageFull(SqlDataReader reader)
        {
            return new RadioLanguageFull(Convert.ToInt64(reader["Id"]),
                                         Convert.ToInt64(reader["LanguageTypeId"]),
                                         Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<RadioLanguage>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<RadioLanguage> radioLanguages = new List<RadioLanguage>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         RadioLanguage radioLanguage = GetRadioLanguage(reader);
                         radioLanguages.Add(radioLanguage);
                    }
                }
            }
            return radioLanguages;
        }

        public async Task<IEnumerable<RadioLanguage>> GetAllByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE RadioId = @RadioId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<RadioLanguage> radioLanguages = new List<RadioLanguage>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        RadioLanguage radioLanguage = GetRadioLanguage(reader);
                        radioLanguages.Add(radioLanguage);
                    }
                }
            }
            return radioLanguages;
        }

        public async Task<RadioLanguage> GetById(long id, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            RadioLanguage radioLanguage = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         radioLanguage = GetRadioLanguage(reader);
                    }
                }
            }
            return radioLanguage;
        }

        public async Task<RadioLanguage> GetByRadioId(long radioId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE RadioId = @RadioId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            RadioLanguage radioLanguage = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        radioLanguage = GetRadioLanguage(reader);
                    }
                }
            }
            return radioLanguage;
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

            long radioLanguageId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        radioLanguageId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return radioLanguageId;
        }

        // INSERT
        public async Task<long> Add(RadioLanguage radioLanguage)
        {
            String strCmd = $"INSERT INTO {table}(RadioId, LanguageTypeId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@RadioId, @LanguageTypeId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioLanguage.RadioId);
            DBHelper.AddParam(command, "@LanguageTypeId", SqlDbType.BigInt, radioLanguage.LanguageTypeId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, radioLanguage.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(RadioLanguage radioLanguage)
        {
            String strCmd = $"UPDATE {table} SET RadioId = @RadioId, LanguageTypeId = @LanguageTypeId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@RadioId", SqlDbType.BigInt, radioLanguage.RadioId);
            DBHelper.AddParam(command, "@LanguageTypeId", SqlDbType.BigInt, radioLanguage.LanguageTypeId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, radioLanguage.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, radioLanguage.Id);

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
