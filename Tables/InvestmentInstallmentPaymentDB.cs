using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentInstallmentPaymentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentInstallmentPayment]";

        public static InvestmentInstallmentPayment GetInvestmentInstallmentPayment(SqlDataReader reader)
        {
            return new InvestmentInstallmentPayment(Convert.ToInt32(reader["Id"]),
                                                    Convert.ToInt32(reader["InvestmentPaymentId"]),
                                                    Convert.ToInt32(reader["InvestmentInstallmentId"]),
                                                    Convert.ToDouble(reader["Amount"]),
                                                    Convert.ToDouble(reader["DiscountAmount"]),
                                                    Convert.ToDouble(reader["NewBalance"]),
                                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                                    Convert.ToDateTime(reader["UpdateDateTime"]),
                                                    Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<InvestmentInstallmentPayment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentInstallmentPayment> investmentIInstallmentPayments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentInstallmentPayment investmentIInstallmentPayment = GetInvestmentInstallmentPayment(reader);
                         investmentIInstallmentPayments.Add(investmentIInstallmentPayment);
                    }
                }
            }
            return investmentIInstallmentPayments;
        }

        public async Task<InvestmentInstallmentPayment> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentInstallmentPayment investmentIInstallmentPayment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investmentIInstallmentPayment = GetInvestmentInstallmentPayment(reader);
                    }
                }
            }
            return investmentIInstallmentPayment;
        }

        public async Task<IEnumerable<InvestmentInstallmentPayment>> GetByInvestmentPaymentId(int investmentPaymentId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentPaymentId = @InvestmentPaymentId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentPaymentId", SqlDbType.Int, investmentPaymentId);

            List<InvestmentInstallmentPayment> investmentIInstallmentPayments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentIInstallmentPayment = GetInvestmentInstallmentPayment(reader);
                        investmentIInstallmentPayments.Add(investmentIInstallmentPayment);
                    }
                }
            }
            return investmentIInstallmentPayments;
        }

        public async Task<IEnumerable<InvestmentInstallmentPayment>> GetByInvestmentInstallmentId(int investmentInstallmentId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentInstallmentId = @InvestmentInstallmentId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentInstallmentId", SqlDbType.Int, investmentInstallmentId);

            List<InvestmentInstallmentPayment> investmentIInstallmentPayments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentIInstallmentPayment = GetInvestmentInstallmentPayment(reader);
                        investmentIInstallmentPayments.Add(investmentIInstallmentPayment);
                    }
                }
            }
            return investmentIInstallmentPayments;
        }

        // INSERT
        public async Task<int> Add(InvestmentInstallmentPayment investmentInstallmentPayment)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentPaymentId, InvestmentInstallmentId, Amount, DiscountAmount, NewBalance, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentPaymentId, @InvestmentInstallmentId, @Amount, @DiscountAmount, @NewBalance, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentPaymentId", SqlDbType.Int, investmentInstallmentPayment.InvestmentPaymentId);
            DBHelper.AddParam(command, "@InvestmentInstallmentId", SqlDbType.Int, investmentInstallmentPayment.InvestmentInstallmentId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, investmentInstallmentPayment.Amount);
            DBHelper.AddParam(command, "@DiscountAmount", SqlDbType.Decimal, investmentInstallmentPayment.DiscountAmount);
            DBHelper.AddParam(command, "@NewBalance", SqlDbType.Decimal, investmentInstallmentPayment.NewBalance);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentInstallmentPayment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentInstallmentPayment investmentInstallmentPayment)
        {
            String strCmd = $"UPDATE {table} SET InvestmentPaymentId = @InvestmentPaymentId, InvestmentInstallmentId = @InvestmentInstallmentId, Amount = @Amount, NewBalance = @NewBalance, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentPaymentId", SqlDbType.Int, investmentInstallmentPayment.InvestmentPaymentId);
            DBHelper.AddParam(command, "@InvestmentInstallmentId", SqlDbType.Int, investmentInstallmentPayment.InvestmentInstallmentId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, investmentInstallmentPayment.Amount);
            DBHelper.AddParam(command, "@DiscountAmount", SqlDbType.Decimal, investmentInstallmentPayment.DiscountAmount);
            DBHelper.AddParam(command, "@NewBalance", SqlDbType.Decimal, investmentInstallmentPayment.NewBalance);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentInstallmentPayment.Id);

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

        public async Task<bool> DeleteByInstallmentId(int installmentId)
        {
            String strCmd = $"DELETE {table} WHERE InvestmentInstallmentId = @InvestmentInstallmentId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentInstallmentId", SqlDbType.Int, installmentId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
