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
        readonly String table = "[DL-AddressAppUser]";

        private static AddressAppUser GetAddressAppUser(SqlDataReader reader)
        {
            return new AddressAppUser(Convert.ToInt32(reader["Id"]),
                                      Convert.ToInt32(reader["AppUserId"]),
                                      Convert.ToInt32(reader["AddressId"]),
                                      Convert.ToInt32(reader["HouseholdBillCount"]),
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

        public async Task<AddressAppUser> GetById(int id, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);
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

        public async Task<AddressAppUser> GetByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<int> GetIdByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            int addressId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressId = Convert.ToInt32(reader["Id"]);
                    }
                }
            }
            return addressId;
        }

        public async Task<int> GetAddressIdByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT AddressId FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            int addressId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressId = Convert.ToInt32(reader["AddressId"]);
                    }
                }
            }
            return addressId;
        }

        public async Task<(int, int)> GetIdCountByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT AddressId, HouseholdBillCount FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            int addressId = -1;
            int householdCount = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressId = Convert.ToInt32(reader["AddressId"]);
                        householdCount = Convert.ToInt32(reader["HouseholdBillCount"]);
                    }
                }
            }
            return (addressId, householdCount);
        }

        // INSERT
        public async Task<int> Add(AddressAppUser addressAppUser)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, AddressId, HouseholdBillCount, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @AddressId, @HouseholdBillCount, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, addressAppUser.AppUserId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.Int, addressAppUser.AddressId);
            DBHelper.AddParam(command, "@HouseholdBillCount", SqlDbType.Int, addressAppUser.HouseholdBillCount);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, addressAppUser.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(AddressAppUser addressAppUser)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, AddressId = @AddressId, HouseholdBillCount = @HouseholdBillCount, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, addressAppUser.AppUserId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.Int, addressAppUser.AddressId);
            DBHelper.AddParam(command, "@HouseholdBillCount", SqlDbType.Int, addressAppUser.HouseholdBillCount);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, addressAppUser.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateHouseholdCount(int id, int householdBillCount)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET HouseholdBillCount = @HouseholdBillCount, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@HouseholdBillCount", SqlDbType.Int, householdBillCount);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE Id = @Id AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByAppUserId(int appUserId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<bool> DeleteById(int id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
