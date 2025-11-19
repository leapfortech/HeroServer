using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PhoneDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Phone]";

        public static Phone GetPhone(SqlDataReader reader)
        {
            return new Phone(Convert.ToInt64(reader["Id"]),
                             Convert.ToInt64(reader["CountryId"]),
                             reader["Number"].ToString(),
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
        public async Task<IEnumerable<Phone>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Phone> phoneCodes = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Phone phone = GetPhone(reader);
                        phoneCodes.Add(phone);
                    }
                }
            }
            return phoneCodes;
        }

        public async Task<Phone> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Phone phone = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        phone = GetPhone(reader);
                    }
                }
            }
            return phone;
        }

        public async Task<Phone> GetByPhoneNumber(long phoneCountryId, String phoneNumber, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE CountryId = @CountryId AND Number = @Number AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, phoneCountryId);
            DBHelper.AddParam(command, "@Number", SqlDbType.VarChar, phoneNumber);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            Phone phone = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        phone = GetPhone(reader);
                    }
                }
            }
            return phone;
        }

        // INSERT
        public async Task<long> Add(Phone phone)
        {
            String strCmd = $"INSERT INTO {table}(Id, CountryId, Number, CountryCode, CallerName, CarrierCountryCode, CarrierNetworkCode, CarrierName, CarrierType, CreateDateTime, UpdateDateTime, Status)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @CountryId, @Number, @CountryCode, @CallerName, @CarrierCountryCode, @CarrierNetworkCode, @CarrierName, @CarrierType, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid());
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, phone.CountryId);
            DBHelper.AddParam(command, "@Number", SqlDbType.VarChar, phone.Number);
            DBHelper.AddParam(command, "@CountryCode", SqlDbType.VarChar, phone.CountryCode);
            DBHelper.AddParam(command, "@CallerName", SqlDbType.VarChar, phone.CallerName);
            DBHelper.AddParam(command, "@CarrierCountryCode", SqlDbType.VarChar, phone.CarrierCountryCode);
            DBHelper.AddParam(command, "@CarrierNetworkCode", SqlDbType.VarChar, phone.CarrierNetworkCode);
            DBHelper.AddParam(command, "@CarrierName", SqlDbType.VarChar, phone.CarrierName);
            DBHelper.AddParam(command, "@CarrierType", SqlDbType.VarChar, phone.CarrierType);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, phone.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Phone phone)
        {
            String strCmd = $"UPDATE {table} SET CountryId = @CountryId, Number = @Number, CountryCode = @CountryCode, CallerName = @CallerName," +
                            " CarrierCountryCode = @CarrierCountryCode, CarrierNetworkCode = @CarrierNetworkCode, CarrierName = @CarrierName, CarrierType = @CarrierType," +
                            " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, phone.CountryId);
            DBHelper.AddParam(command, "@Number", SqlDbType.VarChar, phone.Number);
            DBHelper.AddParam(command, "@CountryCode", SqlDbType.VarChar, phone.CountryCode);
            DBHelper.AddParam(command, "@CallerName", SqlDbType.VarChar, phone.CallerName);
            DBHelper.AddParam(command, "@CarrierCountryCode", SqlDbType.VarChar, phone.CarrierCountryCode);
            DBHelper.AddParam(command, "@CarrierNetworkCode", SqlDbType.VarChar, phone.CarrierNetworkCode);
            DBHelper.AddParam(command, "@CarrierName", SqlDbType.VarChar, phone.CarrierName);
            DBHelper.AddParam(command, "@CarrierType", SqlDbType.VarChar, phone.CarrierType);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, phone.Id);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, countryId);
            DBHelper.AddParam(command, "@Number", SqlDbType.VarChar, number);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, countryId);
            DBHelper.AddParam(command, "@Number", SqlDbType.VarChar, number);
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
