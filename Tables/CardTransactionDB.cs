using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HeroServer
{
    public class CardTransactionDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-CardTransaction]";

        public static CardTransaction GetCardTransaction(SqlDataReader reader)
        {
            return new CardTransaction(Convert.ToInt32(reader["Id"]),
                                       Convert.ToInt32(reader["CardId"]),
                                       reader["Reference"].ToString(),
                                       reader["TransactionCode"].ToString(),
                                       Convert.ToDouble(reader["Amount"]),
                                       reader["ApprovalCode"].ToString(),
                                       reader["DeclinedCode"].ToString(),
                                       reader["DeclinedMotive"].ToString(),
                                       reader["CancellationReference"].ToString(),
                                       reader["CancellationTransactionCode"].ToString(),
                                       reader["CancellationStatus"].ToString(),
                                       Convert.ToDateTime(reader["CreateDateTime"]),
                                       Convert.ToDateTime(reader["UpdateDateTime"]),
                                       Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<CardTransaction>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<CardTransaction> cardTransactions = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CardTransaction cardTransaction = GetCardTransaction(reader);
                        cardTransactions.Add(cardTransaction);
                    }
                }
            }
            return cardTransactions;
        }

        public async Task<CardTransaction> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            CardTransaction cardTransaction = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cardTransaction = GetCardTransaction(reader);
                    }
                }
            }
            return cardTransaction;
        }

        //public async Task<IEnumerable<CardTransaction>> GetByLoanId(int loanId)
        //{
        //    String strCmd = $"SELECT * FROM {table} WHERE LoanId = @LoanId";

        //    SqlCommand command = new SqlCommand(strCmd, conn);

        //    DBHelper.AddParam(command, "@LoanId", SqlDbType.Int, loanId);

        //    List<CardTransaction> cardTransactions = [];
        //    using (conn)
        //    {
        //        await conn.OpenAsync();
        //        using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                CardTransaction cardTransaction = GetCardTransaction(reader);
        //                cardTransactions.Add(cardTransaction);
        //            }
        //        }
        //    }
        //    return cardTransactions;
        //}

        // INSERT
        public async Task<int> Add(CardTransaction cardTransaction)
        {
            String strCmd = $"INSERT INTO {table}(CardId, Reference, TransactionCode, Amount, ApprovalCode, DeclinedCode, DeclinedMotive, CancellationReference, CancellationTransactionCode, CancellationStatus, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@CardId, @Reference, @TransactionCode, @Amount, @ApprovalCode, @DeclinedCode, @DeclinedMotive, @CancellationReference, @CancellationTransactionCode, @CancellationStatus, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CardId", SqlDbType.Int, cardTransaction.CardId);
            DBHelper.AddParam(command, "@Reference", SqlDbType.VarChar, cardTransaction.Reference);
            DBHelper.AddParam(command, "@TransactionCode", SqlDbType.VarChar, cardTransaction.TransactionCode);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, cardTransaction.Amount);
            DBHelper.AddParam(command, "@ApprovalCode", SqlDbType.VarChar, cardTransaction.ApprovalCode);
            DBHelper.AddParam(command, "@DeclinedCode", SqlDbType.VarChar, cardTransaction.DeclinedCode);
            DBHelper.AddParam(command, "@DeclinedMotive", SqlDbType.VarChar, cardTransaction.DeclinedMotive);
            DBHelper.AddParam(command, "@CancellationReference", SqlDbType.VarChar, cardTransaction.CancellationReference);
            DBHelper.AddParam(command, "@CancellationTransactionCode", SqlDbType.VarChar, cardTransaction.CancellationTransactionCode);
            DBHelper.AddParam(command, "@CancellationStatus", SqlDbType.VarChar, cardTransaction.CancellationStatus);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, cardTransaction.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(CardTransaction cardTransaction)
        {
            String strCmd = $"UPDATE {table} SET CardId = @CardId, Reference = @Reference, TransactionCode = @TransactionCode, Amount = @Amount, ApprovalCode = @ApprovalCode, DeclinedCode = @DeclinedCode, DeclinedMotive = @DeclinedMotive, CancellationReference = @CancellationReference, CancellationTransactionCode = @CancellationTransactionCode, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CardId", SqlDbType.Int, cardTransaction.CardId);
            DBHelper.AddParam(command, "@Reference", SqlDbType.VarChar, cardTransaction.Reference);
            DBHelper.AddParam(command, "@TransactionCode", SqlDbType.VarChar, cardTransaction.TransactionCode);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, cardTransaction.Amount);
            DBHelper.AddParam(command, "@ApprovalCode", SqlDbType.VarChar, cardTransaction.ApprovalCode);
            DBHelper.AddParam(command, "@DeclinedCode", SqlDbType.VarChar, cardTransaction.DeclinedCode);
            DBHelper.AddParam(command, "@DeclinedMotive", SqlDbType.VarChar, cardTransaction.DeclinedMotive);
            DBHelper.AddParam(command, "@CancellationReference", SqlDbType.VarChar, cardTransaction.CancellationReference);
            DBHelper.AddParam(command, "@CancellationTransactionCode", SqlDbType.VarChar, cardTransaction.CancellationTransactionCode);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, cardTransaction.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetStatus(int id, int status)
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

        public async Task<bool> DeleteByCardId(int cardId)
        {
            String strCmd = $"DELETE {table} WHERE CardId = @CardId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CardId", SqlDbType.Int, cardId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
