using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PepInvestmentIdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DL-PepInvestmentIdentity]";

        private static PepInvestmentIdentity GetPepInvestmentIdentity(SqlDataReader reader)
        {
            return new PepInvestmentIdentity(Convert.ToInt32(reader["Id"]),
                                             Convert.ToInt32(reader["InvestmentIdentityId"]),
                                             Convert.ToInt32(reader["PepId"]),
                                             Convert.ToDateTime(reader["CreateDateTime"]),
                                             Convert.ToDateTime(reader["UpdateDateTime"]),
                                             Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<PepInvestmentIdentity>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PepInvestmentIdentity> pepInvestmentIdentities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PepInvestmentIdentity pepInvestmentIdentity = GetPepInvestmentIdentity(reader);
                         pepInvestmentIdentities.Add(pepInvestmentIdentity);
                    }
                }
            }
            return pepInvestmentIdentities;
        }

        public async Task<PepInvestmentIdentity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            PepInvestmentIdentity pepInvestmentIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         pepInvestmentIdentity = GetPepInvestmentIdentity(reader);
                    }
                }
            }
            return pepInvestmentIdentity;
        }

        public async Task<int> GetIdByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            String strCmd = $"SELECT PepId FROM {table} WHERE InvestmentIdentityId = @InvestmentIdentityId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, investmentIdentityId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            int pepId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        pepId = Convert.ToInt32(reader["Id"]);
                    }
                }
            }
            return pepId;
        }

        // INSERT
        public async Task<int> Add(PepInvestmentIdentity pepInvestmentIdentity)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentIdentityId, PepId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentIdentityId, @PepId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, pepInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@PepId", SqlDbType.Int, pepInvestmentIdentity.PepId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, pepInvestmentIdentity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PepInvestmentIdentity pepInvestmentIdentity)
        {
            String strCmd = $"UPDATE {table} SET InvestmentIdentityId = @InvestmentIdentityId, PepId = @PepId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, pepInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@PepId", SqlDbType.Int, pepInvestmentIdentity.PepId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, pepInvestmentIdentity.Id);

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
