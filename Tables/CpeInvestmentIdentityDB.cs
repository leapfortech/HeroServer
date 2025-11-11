using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class CpeInvestmentIdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DL-CpeInvestmentIdentity]";

        private static CpeInvestmentIdentity GetCpeInvestmentIdentity(SqlDataReader reader)
        {
            return new CpeInvestmentIdentity(Convert.ToInt32(reader["Id"]),
                                             Convert.ToInt32(reader["InvestmentIdentityId"]),
                                             Convert.ToInt32(reader["CpeId"]),
                                             Convert.ToDateTime(reader["CreateDateTime"]),
                                             Convert.ToDateTime(reader["UpdateDateTime"]),
                                             Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<CpeInvestmentIdentity>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<CpeInvestmentIdentity> cpeInvestmentIdentities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         CpeInvestmentIdentity cpeInvestmentIdentity = GetCpeInvestmentIdentity(reader);
                         cpeInvestmentIdentities.Add(cpeInvestmentIdentity);
                    }
                }
            }
            return cpeInvestmentIdentities;
        }

        public async Task<CpeInvestmentIdentity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            CpeInvestmentIdentity cpeInvestmentIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         cpeInvestmentIdentity = GetCpeInvestmentIdentity(reader);
                    }
                }
            }
            return cpeInvestmentIdentity;
        }

        public async Task<int> GetIdByInvestmentIdentityId(int investmentIdentityId, int status = 1)
        {
            String strCmd = $"SELECT CpeId FROM {table} WHERE InvestmentIdentityId = @InvestmentIdentityId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, investmentIdentityId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            int cpeId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cpeId = Convert.ToInt32(reader["Id"]);
                    }
                }
            }
            return cpeId;
        }

        // INSERT
        public async Task<int> Add(CpeInvestmentIdentity cpeInvestmentIdentity)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentIdentityId, CpeId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentIdentityId, @CpeId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, cpeInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@CpeId", SqlDbType.Int, cpeInvestmentIdentity.CpeId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, cpeInvestmentIdentity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(CpeInvestmentIdentity cpeInvestmentIdentity)
        {
            String strCmd = $"UPDATE {table} SET InvestmentIdentityId = @InvestmentIdentityId, CpeId = @CpeId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, cpeInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@CpeId", SqlDbType.Int, cpeInvestmentIdentity.CpeId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, cpeInvestmentIdentity.Id);

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
