using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentIdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentIdentity]";

        private static InvestmentIdentity GetInvestmentIdentity(SqlDataReader reader)
        {
            return new InvestmentIdentity(Convert.ToInt32(reader["Id"]),
                                          Convert.ToInt32(reader["InvestmentId"]),
                                          Convert.ToInt32(reader["IdentityId"]),
                                          Convert.ToInt32(reader["InvestmentIdentityTypeId"]),
                                          reader["Relationship"].ToString(),
                                          Convert.ToDouble(reader["Pourcentage"]),
                                          Convert.ToDateTime(reader["CreateDateTime"]),
                                          Convert.ToDateTime(reader["UpdateDateTime"]),
                                          Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<InvestmentIdentity>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentIdentity> investmentIdentities = new List<InvestmentIdentity>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentIdentity investmentIdentity = GetInvestmentIdentity(reader);
                         investmentIdentities.Add(investmentIdentity);
                    }
                }
            }
            return investmentIdentities;
        }

        public async Task<InvestmentIdentity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentIdentity investmentIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentIdentity = GetInvestmentIdentity(reader);
                    }
                }
            }
            return investmentIdentity;
        }

        public async Task<List<InvestmentIdentity>> GetByInvestmentId(int investmentId, int status = -1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId";
            if (status == -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            if (status == -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<InvestmentIdentity> investmentIdentities = new List<InvestmentIdentity>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentIdentity investmentIdentity = GetInvestmentIdentity(reader);
                        investmentIdentities.Add(investmentIdentity);
                    }
                }
            }
            return investmentIdentities;
        }

        public async Task<List<int>> GetIdentityIdsByInvestmentId(int investmentId, int status = -1)
        {
            String strCmd = $"SELECT IdentityId FROM {table} WHERE InvestmentId = @InvestmentId";
            if (status == -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            if (status == -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<int> identityIds = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int identityId = Convert.ToInt32(reader["IdentityId"]);
                        identityIds.Add(identityId);
                    }
                }
            }
            return identityIds;
        }

        // INSERT
        public async Task<int> Add(InvestmentIdentity investmentIdentity)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentId, IdentityId, InvestmentIdentityTypeId, Relationship, Pourcentage, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentId, @IdentityId, @InvestmentIdentityTypeId, @Relationship, @Pourcentage, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentIdentity.InvestmentId);
            DBHelper.AddParam(command, "@IdentityId", SqlDbType.Int, investmentIdentity.IdentityId);
            DBHelper.AddParam(command, "@InvestmentIdentityTypeId", SqlDbType.Int, investmentIdentity.InvestmentIdentityTypeId);
            DBHelper.AddParam(command, "@Relationship", SqlDbType.VarChar, investmentIdentity.Relationship);
            DBHelper.AddParam(command, "@Pourcentage", SqlDbType.Decimal, investmentIdentity.Pourcentage);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentIdentity.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentIdentity investmentIdentity)
        {
            String strCmd = $"UPDATE {table} SET InvestmentId = @InvestmentId, IdentityId = @IdentityId, InvestmentIdentityTypeId = @InvestmentIdentityTypeId, Relationship = @Relationship, Pourcentage = @Pourcentage, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentIdentity.InvestmentId);
            DBHelper.AddParam(command, "@IdentityId", SqlDbType.Int, investmentIdentity.IdentityId);
            DBHelper.AddParam(command, "@InvestmentIdentityTypeId", SqlDbType.Int, investmentIdentity.InvestmentIdentityTypeId);
            DBHelper.AddParam(command, "@Relationship", SqlDbType.VarChar, investmentIdentity.Relationship);
            DBHelper.AddParam(command, "@Pourcentage", SqlDbType.Decimal, investmentIdentity.Pourcentage);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentIdentity.Id);

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

        public async Task<bool> DeleteByInvestmentId(int investmentId)
        {
            String strCmd = $"DELETE {table} WHERE InvestmentId = @InvestmentId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
