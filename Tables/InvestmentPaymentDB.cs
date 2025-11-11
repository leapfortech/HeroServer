using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentPaymentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentPayment]";

        public static InvestmentPayment GetInvestmentPayment(SqlDataReader reader)
        {
            return new InvestmentPayment(Convert.ToInt32(reader["Id"]),
                                         Convert.ToInt32(reader["InvestmentId"]),
                                         Convert.ToInt32(reader["AppUserId"]),
                                         Convert.ToInt32(reader["InvestmentPaymentTypeId"]),
                                         Convert.ToInt32(reader["TransactionTypeId"]),
                                         Convert.ToInt32(reader["TransactionId"]),
                                         Convert.ToDateTime(reader["CreateDateTime"]),
                                         Convert.ToDateTime(reader["UpdateDateTime"]),
                                         Convert.ToInt32(reader["Status"]));
        }

        public static InvestmentPaymentBankFull GetBankFullFromReader(SqlDataReader reader)
        {
            return new InvestmentPaymentBankFull(Convert.ToInt32(reader["Id"]),
                                                 Convert.ToInt32(reader["InvestmentId"]),
                                                 reader["ProjectName"].ToString(),
                                                 reader["ProductName"].ToString(),
                                                 Convert.ToInt32(reader["AppUserId"]),
                                                 reader["FirstName1"].ToString(),
                                                 reader["FirstName2"].ToString(),
                                                 reader["LastName1"].ToString(),
                                                 reader["LastName2"].ToString(),
                                                 reader["InvestmentPaymentType"].ToString(),
                                                 reader["AccountHpb"].ToString(),
                                                 Convert.ToInt32(reader["TransactionId"]),
                                                 Convert.ToInt32(reader["TransactionTypeId"]),
                                                 reader["CurrencySymbol"].ToString(),
                                                 Convert.ToDouble(reader["Amount"]),
                                                 reader["Number"].ToString(),
                                                 Convert.ToDateTime(reader["SendDateTime"]),
                                                 null,
                                                 Convert.ToInt32(reader["Status"])
                                                );
        }

        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public String ProjectName { get; set; }
        public String ProductName { get; set; }
        public int AppUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String InvestmentPaymentType { get; set; }
        public String Bank { get; set; }
        public int TransactionTypeId { get; set; }
        public String CurrencySymbol { get; set; }
        public double Amount { get; set; }
        public String Number { get; set; }
        public DateTime SendDateTime { get; set; }
        public int Status { get; set; }

        // GET
        public async Task<IEnumerable<InvestmentPayment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentPayment> investmentPayments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentPayment investmentPayment = GetInvestmentPayment(reader);
                         investmentPayments.Add(investmentPayment);
                    }
                }
            }
            return investmentPayments;
        }

        public async Task<InvestmentPayment> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentPayment investmentPayment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investmentPayment = GetInvestmentPayment(reader);
                    }
                }
            }
            return investmentPayment;
        }

        public async Task<IEnumerable<InvestmentPayment>> GetByInvestmentId(int investmentId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<InvestmentPayment> investmentPayments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                }
            }
            return investmentPayments;
        }

        public async Task<IEnumerable<InvestmentPayment>> GetByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<InvestmentPayment> investmentPayments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                }
            }
            return investmentPayments;
        }

        public async Task<List<InvestmentPaymentBankFull>> GetBankFullsByStatus(int status)
        {
            String strCmd = "SELECT [DD-InvestmentPayment].Id, InvestmentId, [DD-Project].Name AS ProjectName, [K-ProductType].Name AS ProductName," +
                            " [DD-BankTransaction].AppUserId, FirstName1, FirstName2, LastName1, LastName2," +
                            " [K-InvestmentPaymentType].Name AS InvestmentPaymentType, ShortName + ' - ' + [K-AccountHpb].Number AS AccountHpb," +
                            " [DD-InvestmentPayment].TransactionId, [DD-InvestmentPayment].TransactionTypeId," +
                            " [K-Currency].Symbol AS CurrencySymbol, [DD-BankTransaction].Amount, [DD-BankTransaction].Number," +
                            " [DD-BankTransaction].SendDateTime, [DD-InvestmentPayment].Status" +
                            " FROM [DD-InvestmentPayment]" +

                            " INNER JOIN [DD-Investment]" +
                            " ON [DD-InvestmentPayment].InvestmentId = [DD-Investment].Id" +
                            " INNER JOIN [DD-Project]" +
                            " ON [DD-Investment].ProjectId = [DD-Project].Id" +
                            " INNER JOIN [K-ProductType]" +
                            " ON [DD-Investment].ProductTypeId = [K-ProductType].Id" +

                            " INNER JOIN [DD-Identity]" +
                            " ON [DD-InvestmentPayment].AppUserId = [DD-Identity].AppUserId AND [DD-Identity].Status = 1" +

                            " INNER JOIN [K-InvestmentPaymentType]" +
                            " ON [DD-InvestmentPayment].InvestmentPaymentTypeId = [K-InvestmentPaymentType].Id" +

                            " INNER JOIN [DD-BankTransaction]" +
                            " ON [DD-InvestmentPayment].TransactionId = [DD-BankTransaction].Id" +
                            " INNER JOIN [K-AccountHpb]" +
                            " ON [DD-BankTransaction].AccountHpbId = [K-AccountHpb].Id" +
                            " INNER JOIN [K-Bank]" +
                            " ON [K-AccountHpb].BankId = [K-Bank].Id" +
                            " INNER JOIN [K-Currency]" +
                            " ON [K-AccountHpb].CurrencyId = [K-Currency].Id" +

                            " WHERE [DD-InvestmentPayment].Status = @Status AND ([DD-Investment].InvestmentStatusId = 1 OR [DD-Investment].InvestmentStatusId = 2)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<InvestmentPaymentBankFull> investmentPaymentFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentPaymentBankFull investmentPaymentFull = GetBankFullFromReader(reader);
                        investmentPaymentFulls.Add(investmentPaymentFull);
                    }
                }
            }
            return investmentPaymentFulls;
        }

        // INSERT
        public async Task<int> Add(InvestmentPayment investmentPayment)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentId, AppUserId, InvestmentPaymentTypeId, TransactionTypeId, TransactionId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentId, @AppUserId, @InvestmentPaymentTypeId, @TransactionTypeId, @TransactionId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentPayment.InvestmentId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, investmentPayment.AppUserId);
            DBHelper.AddParam(command, "@InvestmentPaymentTypeId", SqlDbType.Int, investmentPayment.InvestmentPaymentTypeId);
            DBHelper.AddParam(command, "@TransactionTypeId", SqlDbType.Int, investmentPayment.TransactionTypeId);
            DBHelper.AddParam(command, "@TransactionId", SqlDbType.Int, investmentPayment.TransactionId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentPayment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentPayment investmentPayment)
        {
            String strCmd = $"UPDATE {table} SET InvestmentId = @InvestmentId, AppUserId = @AppUserId, InvestmentPaymentTypeId = @InvestmentPaymentTypeId, TransactionTypeId = @TransactionTypeId, TransactionId = @TransactionId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentPayment.InvestmentId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, investmentPayment.AppUserId);
            DBHelper.AddParam(command, "@InvestmentPaymentTypeId", SqlDbType.Int, investmentPayment.InvestmentPaymentTypeId);
            DBHelper.AddParam(command, "@TransactionTypeId", SqlDbType.Int, investmentPayment.TransactionTypeId);
            DBHelper.AddParam(command, "@TransactionId", SqlDbType.Int, investmentPayment.TransactionId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentPayment.Id);

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
