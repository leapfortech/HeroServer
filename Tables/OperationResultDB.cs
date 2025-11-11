using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class OperationResultDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-OperationResult]";

        public static OperationResult GetOperationResult(SqlDataReader reader)
        {
            return new OperationResult(Convert.ToInt32(reader["Id"]),
                                       Convert.ToInt32(reader["ProjectId"]),
                                       Convert.ToDouble(reader["RevenueAmount"]),
                                       Convert.ToDouble(reader["CostAmount"]),
                                       Convert.ToDateTime(reader["CreateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<OperationResult>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<OperationResult> operationResults = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         OperationResult operationResult = GetOperationResult(reader);
                         operationResults.Add(operationResult);
                    }
                }
            }
            return operationResults;
        }

        public async Task<OperationResult> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            OperationResult operationResult = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         operationResult = GetOperationResult(reader);
                    }
                }
            }
            return operationResult;
        }

        public async Task<IEnumerable<OperationResult>> GetByProjectId(int projectId, DateTime createDateTime)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND CreateDateTime > @CreateDateTime";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, createDateTime);

            List<OperationResult> operationResults = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        OperationResult operationResult = GetOperationResult(reader);
                        operationResults.Add(operationResult);
                    }
                }
            }
            return operationResults;
        }

        // INSERT
        public async Task<int> Add(OperationResult operationResult)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, RevenueAmount, CostAmount, CreateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectId, @RevenueAmount, @CostAmount, @CreateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, operationResult.ProjectId);
            DBHelper.AddParam(command, "@RevenueAmount", SqlDbType.Decimal, operationResult.RevenueAmount);
            DBHelper.AddParam(command, "@CostAmount", SqlDbType.Decimal, operationResult.CostAmount);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(OperationResult operationResult)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, RevenueAmount = @RevenueAmount, CostAmount = @CostAmount,  WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, operationResult.ProjectId);
            DBHelper.AddParam(command, "@RevenueAmount", SqlDbType.Decimal, operationResult.RevenueAmount);
            DBHelper.AddParam(command, "@CostAmount", SqlDbType.Decimal, operationResult.CostAmount);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, operationResult.Id);

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
