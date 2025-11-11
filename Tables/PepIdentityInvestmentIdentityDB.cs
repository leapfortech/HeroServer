using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PepIdentityInvestmentIdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DL-PepIdentityInvestmentIdentity]";

        private static PepIdentityInvestmentIdentity GetPepIdentityInvestmentIdentity(SqlDataReader reader)
        {
            return new PepIdentityInvestmentIdentity(Convert.ToInt32(reader["Id"]),
                                                     Convert.ToInt32(reader["InvestmentIdentityId"]),
                                                     Convert.ToInt32(reader["PepIdentityId"]),
                                                     Convert.ToDateTime(reader["CreateDateTime"]),
                                                     Convert.ToDateTime(reader["UpdateDateTime"]),
                                                     Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<PepIdentityInvestmentIdentity>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PepIdentityInvestmentIdentity> pepIdentityInvestmentIdentities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PepIdentityInvestmentIdentity pepIdentityInvestmentIdentity = GetPepIdentityInvestmentIdentity(reader);
                         pepIdentityInvestmentIdentities.Add(pepIdentityInvestmentIdentity);
                    }
                }
            }
            return pepIdentityInvestmentIdentities;
        }

        public async Task<PepIdentityInvestmentIdentity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            PepIdentityInvestmentIdentity pepIdentityInvestmentIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         pepIdentityInvestmentIdentity = GetPepIdentityInvestmentIdentity(reader);
                    }
                }
            }
            return pepIdentityInvestmentIdentity;
        }

        public async Task<IEnumerable<PepIdentityInvestmentIdentity>> GetByInvestmentIdentityId(int investmentIdentityId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentIdentityId = @InvestmentIdentityId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, investmentIdentityId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<PepIdentityInvestmentIdentity> pepIdentityInvestmentIdentities = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        PepIdentityInvestmentIdentity pepIdentityInvestmentIdentity = GetPepIdentityInvestmentIdentity(reader);
                        pepIdentityInvestmentIdentities.Add(pepIdentityInvestmentIdentity);
                    }
                }
            }
            return pepIdentityInvestmentIdentities;
        }

        // INSERT
        public async Task<int> Add(PepIdentityInvestmentIdentity pepIdentityInvestmentIdentity)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentIdentityId, PepIdentityId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentIdentityId, @PepIdentityId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, pepIdentityInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@PepIdentityId", SqlDbType.Int, pepIdentityInvestmentIdentity.PepIdentityId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, pepIdentityInvestmentIdentity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PepIdentityInvestmentIdentity pepIdentityInvestmentIdentity)
        {
            String strCmd = $"UPDATE {table} SET InvestmentIdentityId = @InvestmentIdentityId, PepIdentityId = @PepIdentityId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentIdentityId", SqlDbType.Int, pepIdentityInvestmentIdentity.InvestmentIdentityId);
            DBHelper.AddParam(command, "@PepIdentityId", SqlDbType.Int, pepIdentityInvestmentIdentity.PepIdentityId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, pepIdentityInvestmentIdentity.Id);

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
