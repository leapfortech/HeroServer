using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class BankTransactionDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-BankTransaction]";

        public static BankTransaction GetBankTransaction(SqlDataReader reader)
        {
            return new BankTransaction(Convert.ToInt32(reader["Id"]),
                                       Convert.ToInt32(reader["AppUserId"]),
                                       Convert.ToInt32(reader["AccountHpbId"]),
                                       Convert.ToInt32(reader["TransactionTypeId"]),
                                       Convert.ToDouble(reader["Amount"]),
                                       reader["Number"].ToString(),
                                       Convert.ToDateTime(reader["SendDateTime"]),
                                       reader["ApprovalCode"].ToString(),
                                       Convert.ToDateTime(reader["CreateDateTime"]),
                                       Convert.ToDateTime(reader["UpdateDateTime"]),
                                       Convert.ToInt32(reader["Status"]));
        }

        //public static BankTransactionNamed GetNamedFromReader(SqlDataReader reader)
        //{
        //    return new BankTransactionNamed(Convert.ToInt32(reader["Id"]),
        //                                    Convert.ToInt32(reader["AppUserId"]),
        //                                    Convert.ToInt32(reader["InvestmentPaymentId"]),
        //                                    reader["FirstName1"].ToString(),
        //                                    reader["FirstName2"].ToString(),
        //                                    reader["LastName1"].ToString(),
        //                                    reader["LastName2"].ToString(),
        //                                    reader["LastNameMarried"].ToString(),
        //                                    reader["Bank"].ToString(),
        //                                    Convert.ToInt32(reader["TransactionTypeId"]),
        //                                    reader["CurrencySymbol"].ToString(),
        //                                    Convert.ToDouble(reader["Amount"]),
        //                                    reader["Number"].ToString(),
        //                                    Convert.ToDateTime(reader["SendDateTime"]),
        //                                    Convert.ToInt32(reader["Status"])
        //                                   );
        //}

        // GET
        public async Task<IEnumerable<BankTransaction>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<BankTransaction> bankTransactions = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         BankTransaction bankTransaction = GetBankTransaction(reader);
                         bankTransactions.Add(bankTransaction);
                    }
                }
            }
            return bankTransactions;
        }

        public async Task<BankTransaction> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            BankTransaction bankTransaction = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         bankTransaction = GetBankTransaction(reader);
                    }
                }
            }
            return bankTransaction;
        }

        public async Task<List<BankTransaction>> GetByAppUserId(int appUserId, int status = -1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<BankTransaction> bankTransactions = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = GetBankTransaction(reader);
                        bankTransactions.Add(bankTransaction);
                    }
                }
            }
            return bankTransactions;
        }

        public async Task<List<int>> GetIdsByAppUserId(int appUserId, int status = -1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

        //public async Task<List<BankTransactionNamed>> GetByStatus(int status = -1)
        //{
        //    String strCmd = "SELECT [D-BankTransaction].Id, [D-Repayment].Id AS RepaymentId, [D-BankTransaction].AppUserId," +
        //                    " FirstName1, FirstName2, LastName1, LastName2, LastNameMarried," +
        //                    " ShortName + ' - ' + [K-AccountHpb].Number AS Bank, [D-BankTransaction].TransactionTypeId," +
        //                    " [K-Currency].Symbol AS CurrencySymbol, [D-BankTransaction].Amount, [D-BankTransaction].Number," +
        //                    " SendDateTime, [D-BankTransaction].Status" +
        //                    " FROM [D-BankTransaction]" +
        //                    " INNER JOIN [D-Identity]" +
        //                    " ON [D-BankTransaction].AppUserId = [D-Identity].AppUserId AND [D-Identity].Status = 1" +
        //                    " INNER JOIN [K-AccountHpb]" +
        //                    " ON [D-BankTransaction].AccountHpbId = [K-AccountHpb].Id" +
        //                    " INNER JOIN [K-Currency]" +
        //                    " ON [K-Currency].Id = [K-AccountHpb].CurrencyId" +
        //                    " INNER JOIN [K-Bank]" +
        //                    " ON [K-AccountHpb].BankId = [K-Bank].Id" +
        //                    " WHERE [D-BankTransaction].Status = @Status";

        //    SqlCommand command = new SqlCommand(strCmd, conn);

        //    DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

        //    List<BankTransactionNamed> bankTransactionsNamed = [];
        //    using (conn)
        //    {
        //        await conn.OpenAsync();
        //        using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                BankTransactionNamed bankTransactionNamed = GetNamedFromReader(reader);
        //                bankTransactionsNamed.Add(bankTransactionNamed);
        //            }
        //        }
        //    }
        //    return bankTransactionsNamed;
        //}

        public async Task<int> GetAppUserId(int id)
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

        public async Task<int> GetCurrencyId(int accountHpbId)
        {
            String strCmd = "SELECT CurrencyId FROM [K-AccountHpb] WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, accountHpbId);

            int currencyId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        currencyId = Convert.ToInt32(reader["CurrencyId"]);
                    }
                }
            }
            return currencyId;
        }

        // INSERT
        public async Task<int> Add(BankTransaction bankTransaction)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, AccountHpbId, TransactionTypeId, Amount, Number, SendDateTime, ApprovalCode, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @AccountHpbId, @TransactionTypeId, @Amount, @Number, @SendDateTime, @ApprovalCode, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, bankTransaction.AppUserId);
            DBHelper.AddParam(command, "@AccountHpbId", SqlDbType.Int, bankTransaction.AccountHpbId);
            DBHelper.AddParam(command, "@TransactionTypeId", SqlDbType.Int, bankTransaction.TransactionTypeId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, bankTransaction.Amount);
            DBHelper.AddParam(command, "@Number", SqlDbType.VarChar, bankTransaction.Number);
            DBHelper.AddParam(command, "@SendDateTime", SqlDbType.DateTime2, bankTransaction.SendDateTime);
            DBHelper.AddParam(command, "@ApprovalCode", SqlDbType.VarChar, bankTransaction.ApprovalCode);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, bankTransaction.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(BankTransaction bankTransaction)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, AccountHpbId = @AccountHpbId, TransactionTypeId = @TransactionTypeId, Amount = @Amount, Number = @Number, SendDateTime = @SendDateTime," +
                            " ApprovalCode = @ApprovalCode, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, bankTransaction.AppUserId);
            DBHelper.AddParam(command, "@AccountHpbId", SqlDbType.Int, bankTransaction.AccountHpbId);
            DBHelper.AddParam(command, "@TransactionTypeId", SqlDbType.Int, bankTransaction.TransactionTypeId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, bankTransaction.Amount);
            DBHelper.AddParam(command, "@Number", SqlDbType.VarChar, bankTransaction.Number);
            DBHelper.AddParam(command, "@SendDateTime", SqlDbType.DateTime2, bankTransaction.SendDateTime);
            DBHelper.AddParam(command, "@ApprovalCode", SqlDbType.VarChar, bankTransaction.ApprovalCode);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, bankTransaction.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateApprovalCode(int id, String approvalCode, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET ApprovalCode = @ApprovalCode, UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ApprovalCode", SqlDbType.VarChar, approvalCode);
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
