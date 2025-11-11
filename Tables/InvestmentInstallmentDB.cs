using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentInstallmentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentInstallment]";

        public static InvestmentInstallment GetInvestmentInstallment(SqlDataReader reader)
        {
            return new InvestmentInstallment(Convert.ToInt32(reader["Id"]),
                                             Convert.ToInt32(reader["InvestmentId"]),
                                             Convert.ToInt32(reader["InvestmentPaymentTypeId"]),
                                             Convert.ToDouble(reader["Amount"]),
                                             Convert.ToDouble(reader["DiscountAmount"]),
                                             Convert.ToDateTime(reader["EffectiveDate"]),
                                             Convert.ToDateTime(reader["DueDate"]),
                                             Convert.ToDouble(reader["Balance"]),
                                             reader["CompletionDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["CompletionDate"]),
                                             Convert.ToDateTime(reader["CreateDateTime"]),
                                             Convert.ToDateTime(reader["UpdateDateTime"]),
                                             Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<InvestmentInstallment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentInstallment> investmentIInstallments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentInstallment investmentIInstallment = GetInvestmentInstallment(reader);
                         investmentIInstallments.Add(investmentIInstallment);
                    }
                }
            }
            return investmentIInstallments;
        }

        public async Task<InvestmentInstallment> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentInstallment investmentIInstallment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investmentIInstallment = GetInvestmentInstallment(reader);
                    }
                }
            }
            return investmentIInstallment;
        }

        public async Task<List<InvestmentInstallment>> GetByInvestmentId(int investmentId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

            List<InvestmentInstallment> investmentIInstallments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentIInstallment = GetInvestmentInstallment(reader);
                        investmentIInstallments.Add(investmentIInstallment);
                    }
                }
            }
            return investmentIInstallments;
        }

        public async Task<List<int>> GetIdsByInvestmentId(int investmentId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE InvestmentId = @InvestmentId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

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

        public async Task<InvestmentInstallment> GetCurrentByInvestmentId(int investmentId)
        {
            String strCmd = $"SELECT TOP 1 * FROM {table} WHERE InvestmentId = @InvestmentId AND Balance > 0.00 ORDER BY Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

            InvestmentInstallment investmentInstallment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentInstallment = GetInvestmentInstallment(reader);
                    }
                }
            }
            return investmentInstallment;
        }

        public async Task<(int, double)> GetIdBalanceByInvestmentId(int investmentId)
        {
            String strCmd = $"SELECT TOP 1 Id, Balance FROM {table} WHERE InvestmentId = @InvestmentId AND Balance > 0.00 ORDER BY Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

            int id = -1;
            double balance = double.NaN;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt32(reader["Id"]);
                        balance = Convert.ToDouble(reader["Balance"]);
                    }
                }
            }
            return (id, balance);
        }

        // INSERT
        public async Task<int> Add(InvestmentInstallment investmentIInstallment)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentId, InvestmentPaymentTypeId, Amount, DiscountAmount, EffectiveDate, DueDate, Balance, CompletionDate, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentId, @InvestmentPaymentTypeId, @Amount, @DiscountAmount, @EffectiveDate, @DueDate, @Balance, @CompletionDate, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentIInstallment.InvestmentId);
            DBHelper.AddParam(command, "@InvestmentPaymentTypeId", SqlDbType.Int, investmentIInstallment.InvestmentPaymentTypeId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, investmentIInstallment.Amount);
            DBHelper.AddParam(command, "@DiscountAmount", SqlDbType.Decimal, investmentIInstallment.DiscountAmount);
            DBHelper.AddParam(command, "@EffectiveDate", SqlDbType.DateTime2, investmentIInstallment.EffectiveDate);
            DBHelper.AddParam(command, "@DueDate", SqlDbType.DateTime2, investmentIInstallment.DueDate);
            DBHelper.AddParam(command, "@Balance", SqlDbType.Decimal, investmentIInstallment.Balance);
            DBHelper.AddParam(command, "@CompletionDate", SqlDbType.DateTime2, investmentIInstallment.CompletionDate);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentIInstallment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentInstallment investmentIInstallment)
        {
            String strCmd = $"UPDATE {table} SET InvestmentId = @InvestmentId, InvestmentPaymentTypeId = @InvestmentPaymentTypeId, Amount = @Amount, DiscountAmount = @DiscountAmount, EffectiveDate = @EffectiveDate," +
                             " DueDate = @DueDate, Balance = @Balance, CompletionDate = @CompletionDate, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentIInstallment.InvestmentId);
            DBHelper.AddParam(command, "@InvestmentPaymentTypeId", SqlDbType.Int, investmentIInstallment.InvestmentPaymentTypeId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, investmentIInstallment.Amount);
            DBHelper.AddParam(command, "@DiscountAmount", SqlDbType.Decimal, investmentIInstallment.DiscountAmount);
            DBHelper.AddParam(command, "@EffectiveDate", SqlDbType.DateTime2, investmentIInstallment.EffectiveDate);
            DBHelper.AddParam(command, "@DueDate", SqlDbType.DateTime2, investmentIInstallment.DueDate);
            DBHelper.AddParam(command, "@Balance", SqlDbType.Decimal, investmentIInstallment.Balance);
            DBHelper.AddParam(command, "@CompletionDate", SqlDbType.DateTime2, investmentIInstallment.CompletionDate);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentIInstallment.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateBalance(int id, double balance, DateTime? completionDate, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Balance = @Balance, CompletionDate = @CompletionDate, UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Balance", SqlDbType.Decimal, balance);
            DBHelper.AddParam(command, "@CompletionDate", SqlDbType.DateTime2, completionDate);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
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

        public async Task<bool> UpdateStatusByInvestmentId(int investmentId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE InvestmentId = @InvestmentId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
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

        public async Task<bool> DeleteByInvestmentId(int investmentId)
        {
            String strCmd = $"DELETE FROM {table} WHERE InvestmentId = @InvestmentId";

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
