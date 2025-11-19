using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class AddressAppUserDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-AddressAppUser]";

        private static AddressAppUser GetAddressAppUser(SqlDataReader reader)
        {
            return new AddressAppUser(Convert.ToInt64(reader["Id"]),
                                      Convert.ToInt64(reader["AppUserId"]),
                                      Convert.ToInt64(reader["AddressId"]),
                                      Convert.ToDateTime(reader["CreateDateTime"]),
                                      Convert.ToDateTime(reader["UpdateDateTime"]),
                                      Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<AddressAppUser>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<AddressAppUser> addressAppUsers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         AddressAppUser addressAppUser = GetAddressAppUser(reader);
                         addressAppUsers.Add(addressAppUser);
                    }
                }
            }
            return addressAppUsers;
        }

        public async Task<AddressAppUser> GetById(long id, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            AddressAppUser addressAppUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressAppUser = GetAddressAppUser(reader);
                    }
                }
            }
            return addressAppUser;
        }

        public async Task<AddressAppUser> GetByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            AddressAppUser addressAppUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressAppUser = GetAddressAppUser(reader);
                    }
                }
            }
            return addressAppUser;
        }

        public async Task<long> GetIdByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            long addressId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return addressId;
        }

        public async Task<long> GetAddressIdByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT AddressId FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            long addressId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressId = Convert.ToInt64(reader["AddressId"]);
                    }
                }
            }
            return addressId;
        }

        public async Task<long> GetIdCountByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT AddressId FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            long addressId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressId = Convert.ToInt64(reader["AddressId"]);
                    }
                }
            }
            return addressId;
        }

        // INSERT
        public async Task<long> Add(AddressAppUser addressAppUser)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, AddressId, HouseholdBillCount, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @AddressId, @HouseholdBillCount, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, addressAppUser.AppUserId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.BigInt, addressAppUser.AddressId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, addressAppUser.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(AddressAppUser addressAppUser)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, AddressId = @AddressId, HouseholdBillCount = @HouseholdBillCount, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, addressAppUser.AppUserId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.BigInt, addressAppUser.AddressId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, addressAppUser.Id);

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

        public async Task<bool> UpdateStatusByAppUserId(long appUserId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
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
