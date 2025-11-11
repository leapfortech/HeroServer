using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentPrepaidDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentPrepaid]";

        private static InvestmentPrepaid GetInvestmentPrepaid(SqlDataReader reader)
        {
            return new InvestmentPrepaid(Convert.ToInt32(reader["Id"]),
                                         Convert.ToInt32(reader["InvestmentId"]),
                                         Convert.ToInt32(reader["ProductPrepaidId"]),
                                         Convert.ToDateTime(reader["CreateDateTime"]),
                                         Convert.ToDateTime(reader["UpdateDateTime"]),
                                         Convert.ToInt32(reader["Status"]));
        }

        public static InvestmentPrepaidFull GetInvestmentPrepaidFull(SqlDataReader reader)
        {
            return new InvestmentPrepaidFull(Convert.ToInt32(reader["Id"]),
                                             Convert.ToInt32(reader["InvestmentId"]),
                                             Convert.ToInt32(reader["ProductPrepaidId"]),

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

                                             Convert.ToInt32(reader["Status"]),

                                             null, //InvestmentPaymentFulls
                                             null  //InvestmentInstallmentFulls
                                            );
        }

        // GET
        public async Task<IEnumerable<InvestmentPrepaid>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentPrepaid> investmentPrepaids = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentPrepaid investmentPrepaid = GetInvestmentPrepaid(reader);
                         investmentPrepaids.Add(investmentPrepaid);
                    }
                }
            }
            return investmentPrepaids;
        }

        public async Task<InvestmentPrepaid> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentPrepaid investmentPrepaid = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investmentPrepaid = GetInvestmentPrepaid(reader);
                    }
                }
            }
            return investmentPrepaid;
        }

        public async Task<InvestmentPrepaid> GetByInvestmentId(int investmentId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentPrepaid investmentPrepaid = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentPrepaid = GetInvestmentPrepaid(reader);
                    }
                }
            }
            return investmentPrepaid;
        }

        public async Task<InvestmentPrepaid> GetByProductPrepaidId(int productPrepaidId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProductPrepaidId = @ProductPrepaidId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProductPrepaidId", SqlDbType.Int, productPrepaidId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentPrepaid investmentPrepaid = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentPrepaid = GetInvestmentPrepaid(reader);
                    }
                }
            }
            return investmentPrepaid;
        }

        // GET FULL
        public async Task<InvestmentPrepaidFull> GetFullById(int id)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductPrepaidId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Status" +
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
                              " InvIns.Balance, InvIns.CompletionDate, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
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

            InvestmentPrepaidFull investmentPrepaidFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    investmentPrepaidFull = GetInvestmentPrepaidFull(reader);

                    reader.NextResult();
                    Dictionary<int, InvestmentBankPayment> investmentBankPaymentsDict = [];
                    investmentPrepaidFull.InvestmentBankPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        InvestmentBankPayment investmentBankPayment = new InvestmentBankPayment(investmentPayment, new BankTransaction(), null);
                        investmentPrepaidFull.InvestmentBankPayments.Add(investmentBankPayment);
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
                    investmentPrepaidFull.InvestmentInstallmentInfos = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        InvestmentInstallmentInfo investmentInstallmentInfo = new InvestmentInstallmentInfo(investmentInstallment, []);
                        investmentPrepaidFull.InvestmentInstallmentInfos.Add(investmentInstallmentInfo);
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
            return investmentPrepaidFull;
        }

        public async Task<InvestmentPrepaidFull> GetFullByInvestmentId(int investmentId)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductPrepaidId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Status" +
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

            InvestmentPrepaidFull investmentPrepaidFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    investmentPrepaidFull = GetInvestmentPrepaidFull(reader);

                    reader.NextResult();
                    Dictionary<int, InvestmentBankPayment> investmentBankPaymentsDict = [];
                    investmentPrepaidFull.InvestmentBankPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        InvestmentBankPayment investmentBankPayment = new InvestmentBankPayment(investmentPayment, new BankTransaction(), null);
                        investmentPrepaidFull.InvestmentBankPayments.Add(investmentBankPayment);
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
                    investmentPrepaidFull.InvestmentInstallmentInfos = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        InvestmentInstallmentInfo investmentInstallmentInfo = new InvestmentInstallmentInfo(investmentInstallment, []);
                        investmentPrepaidFull.InvestmentInstallmentInfos.Add(investmentInstallmentInfo);
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
            return investmentPrepaidFull;
        }

        public async Task<InvestmentPrepaidDataFull> GetFullsByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductPrepaidId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Status" +
                            $" FROM {table}" +
                             " JOIN [DD-Investment] ON (InvestmentId = [DD-Investment].Id)";
            if (status != -1)
                strCmd += " WHERE InvPrep.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId, InvPay.TransactionId," +
                      " InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                     $" FROM [DD-InvestmentPayment] AS InvPay JOIN {table} AS InvPrep ON (InvPay.InvestmentId = InvPrep.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvPrep.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT BankTra.*" +
                      " FROM [DD-BankTransaction] AS BankTra" +
                      " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                      $" JOIN {table} AS InvPrep ON (InvPay.InvestmentId = InvPrep.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvPrep.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate, InvIns.CompletionDate, " +
                      " InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvPrep ON (InvIns.InvestmentId = InvPrep.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvPrep.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentPrepaidDataFull investmentPrepaidDataFull = new InvestmentPrepaidDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<InvestmentPrepaidFull> investmentPrepaidFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPrepaidFull investmentPrepaidFull = GetInvestmentPrepaidFull(reader);
                        investmentPrepaidFulls.Add(investmentPrepaidFull);
                    }
                    investmentPrepaidDataFull.InvestmentPrepaidFulls = investmentPrepaidFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                    investmentPrepaidDataFull.InvestmentPayments = investmentPayments;

                    reader.NextResult();
                    List<BankTransaction> bankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        bankTransactions.Add(bankTransaction);
                    }
                    investmentPrepaidDataFull.BankTransactions = bankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentInstallments.Add(investmentInstallment);
                    }
                    investmentPrepaidDataFull.InvestmentInstallments = investmentInstallments;
                }
            }
            return investmentPrepaidDataFull;
        }

        public async Task<InvestmentPrepaidDataFull> GetFullsByAppUserId(int appUserId, int status)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductPrepaidId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Status" +
                            $" FROM [DD-Investment] JOIN {table} AS InvPrep ON (InvPrep.InvestmentId = [DD-Investment].Id)" +
                             " WHERE [DD-Investment].AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvPrep.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId, InvPay.TransactionId," +
                      " InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                     $" FROM [DD-InvestmentPayment] AS InvPay JOIN {table} AS InvPrep ON(InvPay.InvestmentId = InvPrep.InvestmentId)" +
                      " WHERE InvPay.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvPrep.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT BankTra.*" +
                      " FROM [DD-BankTransaction] AS BankTra" +
                      " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                     $" JOIN {table} AS InvPrep ON(InvPay.InvestmentId = InvPrep.InvestmentId)" +
                      " WHERE InvPay.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvPrep.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate, InvIns.CompletionDate, " +
                      " InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvPrep ON (InvIns.InvestmentId = InvPrep.InvestmentId)" +
                      " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                      " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvPrep.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvInsPay.*" +
                    $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvPre ON (InvIns.InvestmentId = InvPre.InvestmentId)" +
                     " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                     " JOIN [DD-InvestmentInstallmentPayment] AS InvInsPay ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                     " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvPre.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            InvestmentPrepaidDataFull investmentPrepaidDataFull = new InvestmentPrepaidDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<InvestmentPrepaidFull> investmentPrepaidFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPrepaidFull investmentPrepaidFull = GetInvestmentPrepaidFull(reader);
                        investmentPrepaidFulls.Add(investmentPrepaidFull);
                    }
                    investmentPrepaidDataFull.InvestmentPrepaidFulls = investmentPrepaidFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                    investmentPrepaidDataFull.InvestmentPayments = investmentPayments;

                    reader.NextResult();
                    List<BankTransaction> bankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        bankTransactions.Add(bankTransaction);
                    }
                    investmentPrepaidDataFull.BankTransactions = bankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentInstallments.Add(investmentInstallment);
                    }
                    investmentPrepaidDataFull.InvestmentInstallments = investmentInstallments;

                    reader.NextResult();
                    List<InvestmentInstallmentPayment> investmentInstallmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        investmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                    investmentPrepaidDataFull.InvestmentInstallmentPayments = investmentInstallmentPayments;
                }
            }
            return investmentPrepaidDataFull;
        }

        // INSERT
        public async Task<int> Add(InvestmentPrepaid investmentPrepaid)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentId, ProductPrepaidId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentId, @ProductPrepaidId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentPrepaid.InvestmentId);
            DBHelper.AddParam(command, "@ProductPrepaidId", SqlDbType.Int, investmentPrepaid.ProductPrepaidId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentPrepaid.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentPrepaid investmentPrepaid)
        {
            String strCmd = $"UPDATE {table} SET InvestmentId = @InvestmentId, ProductPrepaidId = @ProductPrepaidId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentPrepaid.InvestmentId);
            DBHelper.AddParam(command, "@ProductPrepaidId", SqlDbType.Int, investmentPrepaid.ProductPrepaidId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentPrepaid.Id);

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
