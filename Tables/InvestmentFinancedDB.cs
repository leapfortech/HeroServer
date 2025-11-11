using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentFinancedDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentFinanced]";

        private static InvestmentFinanced GetInvestmentFinanced(SqlDataReader reader)
        {
            return new InvestmentFinanced(Convert.ToInt32(reader["Id"]),
                                          Convert.ToInt32(reader["InvestmentId"]),
                                          Convert.ToInt32(reader["ProductFinancedId"]),
                                          Convert.ToDouble(reader["AdvAmount"]),
                                          Convert.ToInt32(reader["AdvInstallmentTotal"]),
                                          Convert.ToDouble(reader["LoanInterestRate"]),
                                          Convert.ToInt32(reader["LoanTerm"]),
                                          Convert.ToDateTime(reader["CreateDateTime"]),
                                          Convert.ToDateTime(reader["UpdateDateTime"]),
                                          Convert.ToInt32(reader["Status"]));
        }

        public static InvestmentFinancedFull GetInvestmentFinancedFull(SqlDataReader reader)
        {
            return new InvestmentFinancedFull(Convert.ToInt32(reader["Id"]),
                                              Convert.ToInt32(reader["InvestmentId"]),
                                              Convert.ToInt32(reader["ProductFinancedId"]),

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
                                              Convert.ToInt32(reader["InvestmentStatusId"]),

                                              Convert.ToDouble(reader["AdvAmount"]),
                                              Convert.ToInt32(reader["AdvInstallmentTotal"]),
                                              Convert.ToDouble(reader["LoanInterestRate"]),
                                              Convert.ToInt32(reader["LoanTerm"]),
                                              Convert.ToInt32(reader["Status"]),

                                              null, //InvestmentPaymentFulls
                                              null  //InvestmentInstallmentFulls
                                             );  
        }

        // GET
        public async Task<IEnumerable<InvestmentFinanced>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentFinanced> investmentsFinanced = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentFinanced investmentFinanced = GetInvestmentFinanced(reader);
                         investmentsFinanced.Add(investmentFinanced);
                    }
                }
            }
            return investmentsFinanced;
        }

        public async Task<InvestmentFinanced> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentFinanced investmentFinanced = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investmentFinanced = GetInvestmentFinanced(reader);
                    }
                }
            }
            return investmentFinanced;
        }

        public async Task<InvestmentFinanced> GetByInvestmentId(int investmentId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentFinanced investmentFinanced = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentFinanced = GetInvestmentFinanced(reader);
                    }
                }
            }
            return investmentFinanced;
        }

        public async Task<InvestmentFinanced> GetByProductFinancedId(int productFinancedId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProductFinancedId = @ProductFinancedId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProductFinancedId", SqlDbType.Int, productFinancedId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentFinanced investmentFinanced = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentFinanced = GetInvestmentFinanced(reader);
                    }
                }
            }
            return investmentFinanced;
        }

        // GET FULL
        public async Task<InvestmentFinancedFull> GetFullById(int id)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFinancedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId," +
                             " AdvAmount, AdvInstallmentTotal, LoanInterestRate, LoanTerm, Status" +
                            $" FROM {table}" +
                             " JOIN [DD-Investment] ON (InvestmentId = [DD-Investment].Id)" +
                            $" WHERE {table}.Id = @Id;";

                   strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId," +
                             " InvPay.TransactionId,  InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                             " FROM [DD-InvestmentPayment] AS InvPay" +
                            $" JOIN {table} ON (InvPay.InvestmentId = {table}.InvestmentId)" +
                            $" WHERE {table}.Id = @Id;";

                   strCmd += "SELECT BankTra.*" +
                             " FROM [DD-BankTransaction] AS BankTra" +
                             " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                            $" JOIN {table} ON (InvPay.InvestmentId = {table}.InvestmentId)" +
                            $" WHERE {table}.Id = @Id;";

                   strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate," +
                             " InvIns.CompletionDate, InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                             " FROM [DD-InvestmentInstallment] AS InvIns" +
                            $" JOIN {table} ON (InvIns.InvestmentId = {table}.InvestmentId)" +
                            $" WHERE {table}.Id = @Id;";

                   strCmd += "SELECT InvInsPay.*" +
                             " FROM [DD-InvestmentInstallmentPayment] AS InvInsPay" +
                             " JOIN [DD-InvestmentInstallment] AS InvIns ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                            $" JOIN {table} ON (InvIns.InvestmentId = {table}.InvestmentId)" +
                            $" WHERE {table}.Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentFinancedFull investmentFinancedFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    investmentFinancedFull = GetInvestmentFinancedFull(reader);

                    reader.NextResult();
                    Dictionary<int, InvestmentBankPayment> investmentBankPaymentsDict = [];
                    investmentFinancedFull.InvestmentBankPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        InvestmentBankPayment investmentBankPayment = new InvestmentBankPayment(investmentPayment, new BankTransaction(), null);
                        investmentFinancedFull.InvestmentBankPayments.Add(investmentBankPayment);
                        investmentBankPaymentsDict.Add(investmentPayment.TransactionId, investmentBankPayment);
                    }

                    reader.NextResult();
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        investmentBankPaymentsDict[bankTransaction.Id].BankTransaction = bankTransaction;
                    }

                    reader.NextResult();
                    Dictionary<int, InvestmentInstallmentInfo> investmentInstallmentInfosDict = [];
                    investmentFinancedFull.InvestmentInstallmentInfos = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        InvestmentInstallmentInfo investmentInstallmentInfo = new InvestmentInstallmentInfo(investmentInstallment, []);
                        investmentFinancedFull.InvestmentInstallmentInfos.Add(investmentInstallmentInfo);
                        investmentInstallmentInfosDict.Add(investmentInstallment.Id, investmentInstallmentInfo);
                    }

                    reader.NextResult();
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        investmentInstallmentInfosDict[investmentInstallmentPayment.InvestmentInstallmentId].InvestmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                }
            }
            return investmentFinancedFull;
        }

        public async Task<InvestmentFinancedFull> GetFullByInvestmentId(int investmentId)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFinancedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId," +
                             " AdvAmount, AdvInstallmentTotal, LoanInterestRate, LoanTerm, Status" +
                            $" FROM {table}" +
                             " JOIN [DD-Investment] ON (InvestmentId = [DD-Investment].Id)" +
                            $" WHERE {table}.InvestmentId = @InvestmentId;";

                    strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId," +
                              " InvPay.TransactionId,  InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                              " FROM [DD-InvestmentPayment] AS InvPay" +
                             $" JOIN {table} ON (InvPay.InvestmentId = {table}.InvestmentId)" +
                             $" WHERE {table}.InvestmentId = @InvestmentId;";

                    strCmd += "SELECT BankTra.*" +
                              " FROM [DD-BankTransaction] AS BankTra" +
                              " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                             $" JOIN {table} ON (InvPay.InvestmentId = {table}.InvestmentId)" +
                             $" WHERE {table}.InvestmentId = @InvestmentId;";

                    strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate," +
                              " InvIns.CompletionDate, InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                              " FROM [DD-InvestmentInstallment] AS InvIns" +
                             $" JOIN {table} ON (InvIns.InvestmentId = {table}.InvestmentId)" +
                             $" WHERE {table}.InvestmentId = @InvestmentId;";

                    strCmd += "SELECT InvInsPay.*" +
                              " FROM [DD-InvestmentInstallmentPayment] AS InvInsPay" +
                              " JOIN [DD-InvestmentInstallment] AS InvIns ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                             $" JOIN {table} ON (InvIns.InvestmentId = {table}.InvestmentId)" +
                             $" WHERE {table}.InvestmentId = @InvestmentId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

            InvestmentFinancedFull investmentFinancedFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    investmentFinancedFull = GetInvestmentFinancedFull(reader);

                    reader.NextResult();
                    Dictionary<int, InvestmentBankPayment> investmentBankPaymentsDict = [];
                    investmentFinancedFull.InvestmentBankPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        InvestmentBankPayment investmentBankPayment = new InvestmentBankPayment(investmentPayment, new BankTransaction(), null);
                        investmentFinancedFull.InvestmentBankPayments.Add(investmentBankPayment);
                        investmentBankPaymentsDict.Add(investmentPayment.TransactionId, investmentBankPayment);
                    }

                    reader.NextResult();
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        investmentBankPaymentsDict[bankTransaction.Id].BankTransaction = bankTransaction;
                    }

                    reader.NextResult();
                    Dictionary<int, InvestmentInstallmentInfo> investmentInstallmentInfosDict = [];
                    investmentFinancedFull.InvestmentInstallmentInfos = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        InvestmentInstallmentInfo investmentInstallmentInfo = new InvestmentInstallmentInfo(investmentInstallment, []);
                        investmentFinancedFull.InvestmentInstallmentInfos.Add(investmentInstallmentInfo);
                        investmentInstallmentInfosDict.Add(investmentInstallment.Id, investmentInstallmentInfo);
                    }

                    reader.NextResult();
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        investmentInstallmentInfosDict[investmentInstallmentPayment.InvestmentInstallmentId].InvestmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                }
            }
            return investmentFinancedFull;
        }

        public async Task<InvestmentFinancedDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFinancedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId," +
                             " AdvAmount, AdvInstallmentTotal, LoanInterestRate, LoanTerm, Status" +
                            $" FROM {table}" +
                             " JOIN [DD-Investment] ON (InvestmentId = [DD-Investment].Id)";
            if (status != -1)
                strCmd += " WHERE InvFin.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.appUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId," +
                      " InvPay.TransactionId, InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                     $" FROM [DD-InvestmentPayment] AS InvPay JOIN {table} AS InvFin ON(InvPay.InvestmentId = InvFin.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvFin.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT BankTra.*" +
                      " FROM [DD-BankTransaction] AS BankTra" +
                      " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                      $" JOIN {table} AS InvFin ON (InvPay.InvestmentId = InvFin.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvFin.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate," +
                      " InvIns.CompletionDate, InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvFin ON (InvIns.InvestmentId = InvFin.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvFin.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentFinancedDataFull investmentFinancedDataFull = new InvestmentFinancedDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<InvestmentFinancedFull> investmentFinancedFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentFinancedFull investmentFinancedFull = GetInvestmentFinancedFull(reader);
                        investmentFinancedFulls.Add(investmentFinancedFull);
                    }
                    investmentFinancedDataFull.InvestmentFinancedFulls = investmentFinancedFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                    investmentFinancedDataFull.InvestmentPayments = investmentPayments;

                    reader.NextResult();
                    List<BankTransaction> bankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        bankTransactions.Add(bankTransaction);
                    }
                    investmentFinancedDataFull.BankTransactions = bankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentInstallments.Add(investmentInstallment);
                    }
                    investmentFinancedDataFull.InvestmentInstallments = investmentInstallments;
                }
            }
            return investmentFinancedDataFull;
        }

        public async Task<InvestmentFinancedDataFull> GetDataFullByAppUserId(int appUserId, int status)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFinancedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId," +
                             " AdvAmount, AdvInstallmentTotal, LoanInterestRate, LoanTerm, Status" +
                            $" FROM {table}" +
                             " JOIN [DD-Investment] ON (InvestmentId = [DD-Investment].Id)" +
                             " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFin.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId, InvPay.TransactionId," +
                      " InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                     $" FROM [DD-InvestmentPayment] AS InvPay JOIN {table} AS InvFin ON(InvPay.InvestmentId = InvFin.InvestmentId)" +
                      " WHERE InvPay.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFin.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT BankTra.*" +
                      " FROM [DD-BankTransaction] AS BankTra" +
                      " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                     $" JOIN {table} AS InvFin ON(InvPay.InvestmentId = InvFin.InvestmentId)" +
                      " WHERE InvPay.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFin.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate, InvIns.CompletionDate, " +
                      " InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvFin ON (InvIns.InvestmentId = InvFin.InvestmentId)" +
                      " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                      " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFin.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvInsPay.*" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvFin ON (InvIns.InvestmentId = InvFin.InvestmentId)" +
                      " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                      " JOIN [DD-InvestmentInstallmentPayment] AS InvInsPay ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                      " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFin.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            InvestmentFinancedDataFull investmentFinancedDataFull = new InvestmentFinancedDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<InvestmentFinancedFull> investmentsFinancedFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentFinancedFull investmentFinancedFull = GetInvestmentFinancedFull(reader);
                        investmentsFinancedFulls.Add(investmentFinancedFull);
                    }
                    investmentFinancedDataFull.InvestmentFinancedFulls = investmentsFinancedFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                    investmentFinancedDataFull.InvestmentPayments = investmentPayments;

                    reader.NextResult();
                    List<BankTransaction> bankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        bankTransactions.Add(bankTransaction);
                    }
                    investmentFinancedDataFull.BankTransactions = bankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentInstallments.Add(investmentInstallment);
                    }
                    investmentFinancedDataFull.InvestmentInstallments = investmentInstallments;

                    reader.NextResult();
                    List<InvestmentInstallmentPayment> investmentInstallmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        investmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                    investmentFinancedDataFull.InvestmentInstallmentPayments = investmentInstallmentPayments;
                }
            }
            return investmentFinancedDataFull;
        }

        // INSERT
        public async Task<int> Add(InvestmentFinanced investmentFinanced)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentId, ProductFinancedId, AdvAmount, AdvInstallmentTotal, LoanInterestRate, LoanTerm, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentId, @ProductFinancedId, @AdvAmount, @AdvInstallmentTotal, @LoanInterestRate, @LoanTerm, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentFinanced.InvestmentId);
            DBHelper.AddParam(command, "@ProductFinancedId", SqlDbType.Int, investmentFinanced.ProductFinancedId);
            DBHelper.AddParam(command, "@AdvAmount", SqlDbType.Decimal, investmentFinanced.AdvAmount);
            DBHelper.AddParam(command, "@AdvInstallmentTotal", SqlDbType.Int, investmentFinanced.AdvInstallmentTotal);
            DBHelper.AddParam(command, "@LoanInterestRate", SqlDbType.Decimal, investmentFinanced.LoanInterestRate);
            DBHelper.AddParam(command, "@LoanTerm", SqlDbType.Int, investmentFinanced.LoanTerm);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentFinanced.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentFinanced investmentFinanced)
        {
            String strCmd = $"UPDATE {table} SET InvestmentId = @InvestmentId, ProductFinancedId = @ProductFinancedId, AdvAmount = @AdvAmount, AdvInstallmentTotal = @AdvInstallmentTotal," +
                                               " LoanInterestRate = @LoanInterestRate, LoanTerm = @LoanTerm, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentFinanced.InvestmentId);
            DBHelper.AddParam(command, "@ProductFinancedId", SqlDbType.Int, investmentFinanced.ProductFinancedId);
            DBHelper.AddParam(command, "@AdvAmount", SqlDbType.Decimal, investmentFinanced.AdvAmount);
            DBHelper.AddParam(command, "@AdvInstallmentTotal", SqlDbType.Int, investmentFinanced.AdvInstallmentTotal);
            DBHelper.AddParam(command, "@LoanInterestRate", SqlDbType.Decimal, investmentFinanced.LoanInterestRate);
            DBHelper.AddParam(command, "@LoanTerm", SqlDbType.Int, investmentFinanced.LoanTerm);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentFinanced.Id);

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

        public async Task<bool> UpdateStatusByInvestmentId(int investmentId, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE InvestmentId = @InvestmentId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

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
