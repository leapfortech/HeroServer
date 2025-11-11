using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Investment]";

        public static Investment GetInvestment(SqlDataReader reader)
        {
            return new Investment(Convert.ToInt32(reader["Id"]),
                                  Convert.ToInt32(reader["ProjectId"]),
                                  Convert.ToInt32(reader["ProductTypeId"]),
                                  Convert.ToInt32(reader["AppUserId"]),
                                  Convert.ToDateTime(reader["EffectiveDate"]),
                                  Convert.ToInt32(reader["DevelopmentTerm"]),
                                  Convert.ToInt32(reader["CpiCount"]),
                                  Convert.ToDouble(reader["TotalAmount"]),
                                  Convert.ToDouble(reader["ReserveAmount"]),
                                  Convert.ToDouble(reader["DueAmount"]),
                                  Convert.ToDouble(reader["DiscountRate"]),
                                  Convert.ToDouble(reader["DiscountAmount"]),
                                  Convert.ToDouble(reader["Balance"]),
                                  reader["CompletionDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["CompletionDate"]),
                                  reader["DocuSignReference"].ToString(),
                                  Convert.ToInt32(reader["BoardUserId"]),
                                  Convert.ToInt32(reader["InvestmentMotiveId"]),
                                  reader["BoardComment"].ToString(),
                                  Convert.ToDateTime(reader["CreateDateTime"]),
                                  Convert.ToDateTime(reader["UpdateDateTime"]),
                                  Convert.ToInt32(reader["InvestmentStatusId"]));
        }

        // GET
        public async Task<List<Investment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Investment> investments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Investment investment = GetInvestment(reader);
                         investments.Add(investment);
                    }
                }
            }
            return investments;
        }

        public async Task<Investment> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Investment investment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investment = GetInvestment(reader);
                    }
                }
            }
            return investment;
        }

        public async Task<Investment> GetByProjectId(int projectId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

            Investment investment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investment = GetInvestment(reader);
                    }
                }
            }
            return investment;
        }

        public async Task<List<Investment>> GetByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            List<Investment> investments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Investment investment = GetInvestment(reader);
                        investments.Add(investment);
                    }
                }
            }
            return investments;
        }

        public async Task<List<int>> GetIdsByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            List<int> ids = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        ids.Add(id);
                    }
                }
            }
            return ids;
        }

        public async Task<int> GetAppUserIdById(int id)
        {
            String strCmd = $"SELECT AppUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int appUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUserId = Convert.ToInt32(reader["AppUserId"]);
                    }
                }
            }
            return appUserId;
        }

        public async Task<(int, int)> GetUsersIdById(int id)
        {
            String strCmd = $"SELECT AppUserId, BoardUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int appUserId = -1;
            int boardUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUserId = Convert.ToInt32(reader["AppUserId"]);
                        boardUserId = Convert.ToInt32(reader["BoardUserId"]);
                    }
                }
            }
            return (appUserId, boardUserId);
        }

        public async Task<int> GetProjectIdById(int id)
        {
            String strCmd = $"SELECT ProjectId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int projectId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        projectId = Convert.ToInt32(reader["ProjectId"]);
                    }
                }
            }
            return projectId;
        }

        public async Task<int> GetProductTypeIdById(int id)
        {
            String strCmd = $"SELECT ProductTypeId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int productTypeId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        productTypeId = Convert.ToInt32(reader["ProductTypeId"]);
                    }
                }
            }
            return productTypeId;
        }

        public async Task<int> GetCpiCountById(int id)
        {
            String strCmd = $"SELECT CpiCount FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int cpiCount = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cpiCount = Convert.ToInt32(reader["CpiCount"]);
                    }
                }
            }
            return cpiCount;
        }

        public async Task<double> GetReserveAmountById(int id)
        {
            String strCmd = $"SELECT ReserveAmount FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            double reserveAmount = double.NaN;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        reserveAmount = Convert.ToDouble(reader["ReserveAmount"]);
                    }
                }
            }
            return reserveAmount;
        }

        public async Task<double> GetBalanceById(int id)
        {
            String strCmd = $"SELECT Balance FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            double balance = double.NaN;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        balance = Convert.ToDouble(reader["Balance"]);
                    }
                }
            }
            return balance;
        }

        public async Task<List<Investment>> GetByStatus(int investmentStatusId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentStatusId = @InvestmentStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentStatusId", SqlDbType.Int, investmentStatusId);

            List<Investment> investments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Investment investment = GetInvestment(reader);
                        investments.Add(investment);
                    }
                }
            }
            return investments;
        }

        // INSERT
        public async Task<int> Add(Investment investment)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, ProductTypeId, AppUserId, EffectiveDate, DevelopmentTerm, CpiCount, TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount," +
                             " Balance, CompletionDate, DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, CreateDateTime, UpdateDateTime, InvestmentStatusId)" + 
                             " OUTPUT INSERTED.Id" +
                             " VALUES (@ProjectId, @ProductTypeId, @AppUserId, @EffectiveDate, @DevelopmentTerm, @CpiCount, @TotalAmount, @ReserveAmount, @DueAmount, @DiscountRate, @DiscountAmount," +
                             " @Balance, @CompletionDate, @DocuSignReference, @BoardUserId, @InvestmentMotiveId, @BoardComment, @CreateDateTime, @UpdateDateTime, @InvestmentStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, investment.ProjectId);
            DBHelper.AddParam(command, "@ProductTypeId", SqlDbType.Int, investment.ProductTypeId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, investment.AppUserId);
            DBHelper.AddParam(command, "@EffectiveDate", SqlDbType.Date, investment.EffectiveDate);
            DBHelper.AddParam(command, "@DevelopmentTerm", SqlDbType.Int, investment.DevelopmentTerm);
            DBHelper.AddParam(command, "@CpiCount", SqlDbType.Int, investment.CpiCount);
            DBHelper.AddParam(command, "@TotalAmount", SqlDbType.Decimal, investment.TotalAmount);
            DBHelper.AddParam(command, "@ReserveAmount", SqlDbType.Decimal, investment.ReserveAmount);
            DBHelper.AddParam(command, "@DueAmount", SqlDbType.Decimal, investment.DueAmount);
            DBHelper.AddParam(command, "@DiscountRate", SqlDbType.Decimal, investment.DiscountRate);
            DBHelper.AddParam(command, "@DiscountAmount", SqlDbType.Decimal, investment.DiscountAmount);
            DBHelper.AddParam(command, "@Balance", SqlDbType.Decimal, investment.Balance);
            DBHelper.AddParam(command, "@CompletionDate", SqlDbType.DateTime2, investment.CompletionDate);
            DBHelper.AddParam(command, "@DocuSignReference", SqlDbType.VarChar, investment.DocuSignReference);
            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, investment.BoardUserId);
            DBHelper.AddParam(command, "@InvestmentMotiveId", SqlDbType.Int, investment.InvestmentMotiveId);
            DBHelper.AddParam(command, "@BoardComment", SqlDbType.VarChar, investment.BoardComment);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@InvestmentStatusId", SqlDbType.Int, investment.InvestmentStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Investment investment, bool status = false)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, ProductTypeId = @ProductTypeId, AppUserId = @AppUserId, EffectiveDate = @EffectiveDate, DevelopmentTerm = @DevelopmentTerm, CpiCount = @CpiCount," +
                             " TotalAmount = @TotalAmount, ReserveAmount = @ReserveAmount, DueAmount = @DueAmount, DiscountRate = @DiscountRate, DiscountAmount = @DiscountAmount, Balance = @Balance, CompletionDate = @CompletionDate," +
                             " DocuSignReference = @DocuSignReference, BoardUserId = @BoardUserId, InvestmentMotiveId = @InvestmentMotiveId, BoardComment = @BoardComment, CreateDateTime = @CreateDateTime, UpdateDateTime = @UpdateDateTime" +
                             (status ? ", InvestmentStatusId = @InvestmentStatusId" : "") +
                             " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, investment.ProjectId);
            DBHelper.AddParam(command, "@ProductTypeId", SqlDbType.Int, investment.ProductTypeId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, investment.AppUserId);
            DBHelper.AddParam(command, "@EffectiveDate", SqlDbType.Date, investment.EffectiveDate);
            DBHelper.AddParam(command, "@DevelopmentTerm", SqlDbType.Int, investment.DevelopmentTerm);
            DBHelper.AddParam(command, "@CpiCount", SqlDbType.Int, investment.CpiCount);
            DBHelper.AddParam(command, "@TotalAmount", SqlDbType.Decimal, investment.TotalAmount);
            DBHelper.AddParam(command, "@ReserveAmount", SqlDbType.Decimal, investment.ReserveAmount);
            DBHelper.AddParam(command, "@DueAmount", SqlDbType.Decimal, investment.DueAmount);
            DBHelper.AddParam(command, "@DiscountRate", SqlDbType.Decimal, investment.DiscountRate);
            DBHelper.AddParam(command, "@DiscountAmount", SqlDbType.Decimal, investment.DiscountAmount);
            DBHelper.AddParam(command, "@Balance", SqlDbType.Decimal, investment.Balance);
            DBHelper.AddParam(command, "@CompletionDate", SqlDbType.DateTime2, investment.CompletionDate);
            DBHelper.AddParam(command, "@DocuSignReference", SqlDbType.VarChar, investment.DocuSignReference);
            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, investment.BoardUserId);
            DBHelper.AddParam(command, "@InvestmentMotiveId", SqlDbType.Int, investment.InvestmentMotiveId);
            DBHelper.AddParam(command, "@BoardComment", SqlDbType.VarChar, investment.BoardComment);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            if (status)
                DBHelper.AddParam(command, "@InvestmentStatusId", SqlDbType.Int, investment.InvestmentStatusId);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investment.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateMotive(int id, int boardUserId, int motiveId, String boardComment)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET BoardUserId = @BoardUserId, InvestmentMotiveId = @InvestmentMotiveId, BoardComment = @BoardComment, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, boardUserId);
            DBHelper.AddParam(command, "@InvestmentMotiveId", SqlDbType.Int, motiveId);
            DBHelper.AddParam(command, "@BoardComment", SqlDbType.VarChar, boardComment);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateMotiveStatus(int id, int boardUserId, int motiveId, String boardComment, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET BoardUserId = @BoardUserId, InvestmentMotiveId = @InvestmentMotiveId, BoardComment = @BoardComment, UpdateDateTime = @UpdateDateTime, InvestmentStatusId = @InvestmentStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, boardUserId);
            DBHelper.AddParam(command, "@InvestmentMotiveId", SqlDbType.Int, motiveId);
            DBHelper.AddParam(command, "@BoardComment", SqlDbType.VarChar, boardComment);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@InvestmentStatusId", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateBalance(int id, double balance, DateTime? completionDate)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Balance = @Balance, CompletionDate = @CompletionDate, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Balance", SqlDbType.Decimal, balance);
            DBHelper.AddParam(command, "@CompletionDate", SqlDbType.DateTime2, completionDate);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateBalanceStatus(int id, double balance, DateTime? completionDate, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Balance = @Balance, CompletionDate = @CompletionDate, UpdateDateTime = @UpdateDateTime, InvestmentStatusId = @InvestmentStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Balance", SqlDbType.Decimal, balance);
            DBHelper.AddParam(command, "@CompletionDate", SqlDbType.DateTime2, completionDate);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@InvestmentStatusId", SqlDbType.Int, status);
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
                            " SET UpdateDateTime = @UpdateDateTime, InvestmentStatusId = @InvestmentStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@InvestmentStatusId", SqlDbType.Int, status);
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

        public async Task<bool> DeleteByAppUserId(int appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
