using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class AddressInvestmentIdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DL-AddressInvestmentIdentity]";

        private static AddressInvestmentIdentity GetAddressInvestmentIdentity(SqlDataReader reader)
        {
            return new AddressInvestmentIdentity(Convert.ToInt32(reader["Id"]),
                                                 Convert.ToInt32(reader["InvestmentIdentityId"]),
                                                 Convert.ToInt32(reader["AddressId"]),
                                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                                 Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<AddressInvestmentIdentity>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<AddressInvestmentIdentity> addressInvestmentIdentities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         AddressInvestmentIdentity addressInvestmentIdentity = GetAddressInvestmentIdentity(reader);
                         addressInvestmentIdentities.Add(addressInvestmentIdentity);
                    }
                }
            }
            return addressInvestmentIdentities;
        }

        public async Task<AddressInvestmentIdentity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            AddressInvestmentIdentity addressInvestmentIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         addressInvestmentIdentity = GetAddressInvestmentIdentity(reader);
                    }
                }
            }
            return addressInvestmentIdentity;
        }

        public async Task<AddressInvestmentIdentity> GetByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentIdentityId = @InvestmentIdentityId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, investmentIdentityId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            AddressInvestmentIdentity addressInvestmentIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressInvestmentIdentity = GetAddressInvestmentIdentity(reader);
                    }
                }
            }
            return addressInvestmentIdentity;
        }

        public async Task<int> GetIdByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            String strCmd = $"SELECT AddressId FROM {table} WHERE InvestmentIdentityId = @InvestmentIdentityId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, investmentIdentityId);
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

        // INSERT
        public async Task<int> Add(AddressInvestmentIdentity addressInvestmentIdentity)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentIdentityId, AddressId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentIdentityId, @AddressId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, addressInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.Int, addressInvestmentIdentity.AddressId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, addressInvestmentIdentity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(AddressInvestmentIdentity addressInvestmentIdentity)
        {
            String strCmd = $"UPDATE {table} SET InvestmentIdentityId = @InvestmentIdentityId, AddressId = @AddressId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, addressInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@AddressId", SqlDbType.Int, addressInvestmentIdentity.AddressId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, addressInvestmentIdentity.Id);

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

        public async Task<bool> UpdateStatusByInvestmentIdentityId(int investmentIdentityId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE InvestmentIdentityId = @InvestmentIdentityId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, investmentIdentityId);
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
