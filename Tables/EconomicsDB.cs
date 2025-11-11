using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class EconomicsDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Economics]";

        private static Economics GetEconomics(SqlDataReader reader)
        {
            return new Economics(Convert.ToInt32(reader["Id"]),
                                 Convert.ToInt32(reader["InvestmentId"]),
                                 Convert.ToInt32(reader["IncomeCurrencyId"]),
                                 Convert.ToDouble(reader["IncomeAmount"]),
                                 Convert.ToInt32(reader["ExpensesCurrencyId"]),
                                 Convert.ToDouble(reader["ExpensesAmount"]),
                                 reader["Activity"].ToString(),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Economics>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Economics> economicss = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Economics economics = GetEconomics(reader);
                         economicss.Add(economics);
                    }
                }
            }
            return economicss;
        }

        public async Task<Economics> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Economics economics = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         economics = GetEconomics(reader);
                    }
                }
            }
            return economics;
        }

        public async Task<Economics> GetByInvestmentId(int investmentId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            Economics economics = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         economics = GetEconomics(reader);
                    }
                }
            }
            return economics;
        }

        // INSERT
        public async Task<int> Add(Economics economics)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentId, IncomeCurrencyId, IncomeAmount, ExpensesCurrencyId, ExpensesAmount, Activity, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentId, @IncomeCurrencyId, @IncomeAmount, @ExpensesCurrencyId, @ExpensesAmount, @Activity, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, economics.InvestmentId);
            DBHelper.AddParam(command, "@IncomeCurrencyId", SqlDbType.Int, economics.IncomeCurrencyId);
            DBHelper.AddParam(command, "@IncomeAmount", SqlDbType.Decimal, economics.IncomeAmount);
            DBHelper.AddParam(command, "@ExpensesCurrencyId", SqlDbType.Int, economics.ExpensesCurrencyId);
            DBHelper.AddParam(command, "@ExpensesAmount", SqlDbType.Decimal, economics.ExpensesAmount);
            DBHelper.AddParam(command, "@Activity", SqlDbType.VarChar, economics.Activity);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, economics.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Economics economics)
        {
            String strCmd = $"UPDATE {table} SET InvestmentId = @InvestmentId, IncomeCurrencyId = @IncomeCurrencyId, IncomeAmount = @IncomeAmount, ExpensesCurrencyId = @ExpensesCurrencyId," +
                            $" ExpensesAmount = @ExpensesAmount, Activity = @Activity, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, economics.InvestmentId);
            DBHelper.AddParam(command, "@IncomeCurrencyId", SqlDbType.Int, economics.IncomeCurrencyId);
            DBHelper.AddParam(command, "@IncomeAmount", SqlDbType.Decimal, economics.IncomeAmount);
            DBHelper.AddParam(command, "@ExpensesCurrencyId", SqlDbType.Int, economics.ExpensesCurrencyId);
            DBHelper.AddParam(command, "@ExpensesAmount", SqlDbType.Decimal, economics.ExpensesAmount);
            DBHelper.AddParam(command, "@Activity", SqlDbType.VarChar, economics.Activity);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, economics.Id);

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

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
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
