using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PrecheckPhoneDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-PrecheckPhone]";

        public static PrecheckPhone GetPrecheckPhone(SqlDataReader reader)
        {
            return new PrecheckPhone(Convert.ToInt64(reader["Id"]),
                                     Convert.ToInt64(reader["CountryId"]),
                                     reader["Number"].ToString(),
                                     reader["Code"].ToString(),
                                     reader["WACode"].ToString(),
                                     reader["WAStatus"].ToString(),
                                     reader["CountryCode"].ToString(),
                                     reader["CallerName"].ToString(),
                                     reader["CarrierCountryCode"].ToString(),
                                     reader["CarrierNetworkCode"].ToString(),
                                     reader["CarrierName"].ToString(),
                                     reader["CarrierType"].ToString(),
                                     Convert.ToDateTime(reader["CreateDateTime"]),
                                     Convert.ToDateTime(reader["UpdateDateTime"]),
                                     Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<PrecheckPhone>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PrecheckPhone> precheckPhones = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        PrecheckPhone precheckPhone = GetPrecheckPhone(reader);
                        precheckPhones.Add(precheckPhone);
                    }
                }
            }
            return precheckPhones;
        }

        public async Task<PrecheckPhone> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            PrecheckPhone precheckPhone = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        precheckPhone = GetPrecheckPhone(reader);
                    }
                }
            }
            return precheckPhone;
        }

        public async Task<PrecheckPhone> GetByPhoneNumber(long phoneCountryId, String phoneNumber, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE CountryId = @CountryId AND Number = @Number AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@CountryId", SqlDbType.BigInt, phoneCountryId);
            command.AddParam("@Number", SqlDbType.VarChar, phoneNumber);
            command.AddParam("@Status", SqlDbType.Int, status);

            PrecheckPhone precheckPhone = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        precheckPhone = GetPrecheckPhone(reader);
                    }
                }
            }
            return precheckPhone;
        }

        public async Task<PrecheckPhone> GetByWACode(String waCode)
        {
            String strCmd = $"SELECT * FROM {table} WHERE WACode = @WACode";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@WACode", SqlDbType.VarChar, waCode);

            PrecheckPhone precheckPhone = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        precheckPhone = GetPrecheckPhone(reader);
                    }
                }
            }
            return precheckPhone;
        }

        // INSERT
        public async Task<long> Add(PrecheckPhone precheckPhone)
        {
            String strCmd = $"INSERT INTO {table}(Id, CountryId, Number, Code, WACode, WAStatus, CountryCode, CallerName, CarrierCountryCode, CarrierNetworkCode, CarrierName, CarrierType, CreateDateTime, UpdateDateTime, Status)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @CountryId, @Number, @Code, @WACode, @WAStatus, @CountryCode, @CallerName, @CarrierCountryCode, @CarrierNetworkCode, @CarrierName, @CarrierType, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('H'));
            command.AddParam("@CountryId", SqlDbType.BigInt, precheckPhone.CountryId);
            command.AddParam("@Number", SqlDbType.VarChar, precheckPhone.Number);
            command.AddParam("@Code", SqlDbType.VarChar, precheckPhone.Code);
            command.AddParam("@WACode", SqlDbType.VarChar, precheckPhone.WACode);
            command.AddParam("@WAStatus", SqlDbType.VarChar, precheckPhone.WAStatus);
            command.AddParam("@CountryCode", SqlDbType.VarChar, precheckPhone.CountryCode);
            command.AddParam("@CallerName", SqlDbType.VarChar, precheckPhone.CallerName);
            command.AddParam("@CarrierCountryCode", SqlDbType.VarChar, precheckPhone.CarrierCountryCode);
            command.AddParam("@CarrierNetworkCode", SqlDbType.VarChar, precheckPhone.CarrierNetworkCode);
            command.AddParam("@CarrierName", SqlDbType.VarChar, precheckPhone.CarrierName);
            command.AddParam("@CarrierType", SqlDbType.VarChar, precheckPhone.CarrierType);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, precheckPhone.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PrecheckPhone precheckPhone)
        {
            String strCmd = $"UPDATE {table} SET CountryId = @CountryId, Number = @Number, Code = @Code, WACode = @WACode, WAStatus = @WAStatus, CountryCode = @CountryCode, CallerName = @CallerName," +
                            " CarrierCountryCode = @CarrierCountryCode, CarrierNetworkCode = @CarrierNetworkCode, CarrierName = @CarrierName, CarrierType = @CarrierType," +
                            " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@CountryId", SqlDbType.BigInt, precheckPhone.CountryId);
            command.AddParam("@Number", SqlDbType.VarChar, precheckPhone.Number);
            command.AddParam("@Code", SqlDbType.VarChar, precheckPhone.Code);
            command.AddParam("@WACode", SqlDbType.VarChar, precheckPhone.WACode);
            command.AddParam("@WAStatus", SqlDbType.VarChar, precheckPhone.WAStatus);
            command.AddParam("@CountryCode", SqlDbType.VarChar, precheckPhone.CountryCode);
            command.AddParam("@CallerName", SqlDbType.VarChar, precheckPhone.CallerName);
            command.AddParam("@CarrierCountryCode", SqlDbType.VarChar, precheckPhone.CarrierCountryCode);
            command.AddParam("@CarrierNetworkCode", SqlDbType.VarChar, precheckPhone.CarrierNetworkCode);
            command.AddParam("@CarrierName", SqlDbType.VarChar, precheckPhone.CarrierName);
            command.AddParam("@CarrierType", SqlDbType.VarChar, precheckPhone.CarrierType);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, precheckPhone.Id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateWACode(long id, String waCode)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, WACode = @WACode" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@WACode", SqlDbType.VarChar, waCode);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateWAStatus(long id, String waStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, WAStatus = @WAStatus" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@WAStatus", SqlDbType.VarChar, waStatus);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByPhone(long countryId, String number, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE CountryId = @CountryId AND Number = @Number";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CountryId", SqlDbType.BigInt, countryId);
            command.AddParam("@Number", SqlDbType.VarChar, number);
            command.AddParam("@Status", SqlDbType.Int, status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByPhone(long countryId, String number, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE CountryId = @CountryId AND Number = @Number AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CountryId", SqlDbType.BigInt, countryId);
            command.AddParam("@Number", SqlDbType.VarChar, number);
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
    }
}
