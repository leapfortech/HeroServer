using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentFractionatedDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentFractionated]";

        private static InvestmentFractionated GetInvestmentFractionated(SqlDataReader reader)
        {
            return new InvestmentFractionated(Convert.ToInt32(reader["Id"]),
                                              Convert.ToInt32(reader["InvestmentId"]),
                                              Convert.ToInt32(reader["ProductFractionatedId"]),
                                              Convert.ToDouble(reader["Amount"]),
                                              Convert.ToInt32(reader["InstallmentCount"]),
                                              Convert.ToDateTime(reader["CreateDateTime"]),
                                              Convert.ToDateTime(reader["UpdateDateTime"]),
                                              Convert.ToInt32(reader["Status"]));
        }

        public static InvestmentFractionatedFull GetInvestmentFractionatedFull(SqlDataReader reader)
        {
            return new InvestmentFractionatedFull(Convert.ToInt32(reader["Id"]),
                                                  Convert.ToInt32(reader["InvestmentId"]),
                                                  Convert.ToInt32(reader["ProductFractionatedId"]),

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

                                                  Convert.ToDouble(reader["Amount"]),
                                                  Convert.ToInt32(reader["InstallmentCount"]),
                                                  Convert.ToInt32(reader["Status"]),

                                                  null, //InvestmentPaymentFulls
                                                  null  //InvestmentInstallmentFulls
                                                 );  
        }

        // GET
        public async Task<IEnumerable<InvestmentFractionated>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentFractionated> investmentsFractionated = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentFractionated investmentFractionated = GetInvestmentFractionated(reader);
                         investmentsFractionated.Add(investmentFractionated);
                    }
                }
            }
            return investmentsFractionated;
        }

        public async Task<InvestmentFractionated> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentFractionated investmentFractionated = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investmentFractionated = GetInvestmentFractionated(reader);
                    }
                }
            }
            return investmentFractionated;
        }

        public async Task<InvestmentFractionated> GetByInvestmentId(int investmentId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentFractionated investmentFractionated = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentFractionated = GetInvestmentFractionated(reader);
                    }
                }
            }
            return investmentFractionated;
        }

        public async Task<InvestmentFractionated> GetByProductFractionatedId(int productFractionatedId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProductFractionatedId = @ProductFractionatedId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProductFractionatedId", SqlDbType.Int, productFractionatedId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentFractionated investmentFractionated = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        investmentFractionated = GetInvestmentFractionated(reader);
                    }
                }
            }
            return investmentFractionated;
        }

        // GET FULL
        public async Task<InvestmentFractionatedFull> GetFullById(int id)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFractionatedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Amount, InstallmentCount, Status" +
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

            InvestmentFractionatedFull investmentFractionatedFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    investmentFractionatedFull = GetInvestmentFractionatedFull(reader);

                    reader.NextResult();
                    Dictionary<int, InvestmentBankPayment> investmentBankPaymentsDict = [];
                    investmentFractionatedFull.InvestmentBankPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        InvestmentBankPayment investmentBankPayment = new InvestmentBankPayment(investmentPayment, new BankTransaction(), null);
                        investmentFractionatedFull.InvestmentBankPayments.Add(investmentBankPayment);
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
                    investmentFractionatedFull.InvestmentInstallmentInfos = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        InvestmentInstallmentInfo investmentInstallmentInfo = new InvestmentInstallmentInfo(investmentInstallment, []);
                        investmentFractionatedFull.InvestmentInstallmentInfos.Add(investmentInstallmentInfo);
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
            return investmentFractionatedFull;
        }

        public async Task<InvestmentFractionatedFull> GetFullByInvestmentId(int investmentId)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFractionatedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Amount, InstallmentCount, Status" +
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

                   strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate, InvIns.CompletionDate, " +
                             " InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
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

            InvestmentFractionatedFull investmentFractionatedFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    investmentFractionatedFull = GetInvestmentFractionatedFull(reader);

                    reader.NextResult();
                    Dictionary<int, InvestmentBankPayment> investmentBankPaymentsDict = [];
                    investmentFractionatedFull.InvestmentBankPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        InvestmentBankPayment investmentBankPayment = new InvestmentBankPayment(investmentPayment, new BankTransaction(), null);
                        investmentFractionatedFull.InvestmentBankPayments.Add(investmentBankPayment);
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
                    investmentFractionatedFull.InvestmentInstallmentInfos = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        InvestmentInstallmentInfo investmentInstallmentInfo = new InvestmentInstallmentInfo(investmentInstallment, []);
                        investmentFractionatedFull.InvestmentInstallmentInfos.Add(investmentInstallmentInfo);
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
            return investmentFractionatedFull;
        }

        public async Task<InvestmentFractionatedDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFractionatedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Amount, InstallmentCount, Status" +
                            $" FROM {table}" +
                             " JOIN [DD-Investment] ON ([DD-Investment].Id = InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvFra.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId," +
                      " InvPay.TransactionId,  InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                     $" FROM [DD-InvestmentPayment] AS InvPay JOIN {table} AS InvFra ON (InvPay.InvestmentId = InvFra.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvFra.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT BankTra.*" +
                      " FROM [DD-BankTransaction] AS BankTra" +
                      " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                      $" JOIN {table} AS InvFra ON (InvPay.InvestmentId = InvFra.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvFra.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate," +
                      " InvIns.CompletionDate, InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvFra ON (InvIns.InvestmentId = InvFra.InvestmentId)";
            if (status != -1)
                strCmd += " WHERE InvFra.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            InvestmentFractionatedDataFull investmentFractionatedDataFull = new InvestmentFractionatedDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<InvestmentFractionatedFull> investmentFractionatedFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentFractionatedFull investmentFractionatedFull = GetInvestmentFractionatedFull(reader);
                        investmentFractionatedFulls.Add(investmentFractionatedFull);
                    }
                    investmentFractionatedDataFull.InvestmentFractionatedFulls = investmentFractionatedFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                    investmentFractionatedDataFull.InvestmentPayments = investmentPayments;

                    reader.NextResult();
                    List<BankTransaction> bankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        bankTransactions.Add(bankTransaction);
                    }
                    investmentFractionatedDataFull.BankTransactions = bankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentInstallments.Add(investmentInstallment);
                    }
                    investmentFractionatedDataFull.InvestmentInstallments = investmentInstallments;
                }
            }
            return investmentFractionatedDataFull;
        }

        public async Task<InvestmentFractionatedDataFull> GetDataFullByAppUserId(int appUserId, int status)
        {
            String strCmd = $"SELECT {table}.Id, InvestmentId, ProductFractionatedId, ProjectId, ProductTypeId," +
                             " AppUserId, EffectiveDate, DevelopmentTerm, CpiCount," +
                             " TotalAmount, ReserveAmount, DueAmount, DiscountRate, DiscountAmount, Balance, CompletionDate," +
                             " DocuSignReference, BoardUserId, InvestmentMotiveId, BoardComment, InvestmentStatusId, Amount, InstallmentCount, Status" +
                            $" FROM {table}" +
                             " JOIN [DD-Investment] ON ([DD-Investment].Id = InvestmentId)" +
                             " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFra.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId, InvPay.TransactionId, InvPay.Status" +
                     $" FROM [DD-InvestmentPayment] AS InvPay JOIN {table} AS InvFra ON(InvPay.InvestmentId = InvFra.InvestmentId)" +
                      " WHERE InvPay.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFra.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT BankTra.*" +
                      " FROM [DD-BankTransaction] AS BankTra" +
                      " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                     $" JOIN {table} AS InvFra ON(InvPay.InvestmentId = InvFra.InvestmentId)" +
                      " WHERE InvPay.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFra.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate, InvIns.CompletionDate, " +
                      " InvIns.Balance, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvFra ON (InvIns.InvestmentId = InvFra.InvestmentId)" +
                      " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                      " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFra.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT InvInsPay.*" +
                     $" FROM [DD-InvestmentInstallment] AS InvIns JOIN {table} AS InvFra ON (InvIns.InvestmentId = InvFra.InvestmentId)" +
                      " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                      " JOIN [DD-InvestmentInstallmentPayment] AS InvInsPay ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                      " WHERE Inv.AppUserId = @AppUserId";
            if (status != -1)
                strCmd += " AND InvFra.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            InvestmentFractionatedDataFull investmentFractionatedDataFull = new InvestmentFractionatedDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<InvestmentFractionatedFull> investmentFractionatedFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentFractionatedFull investmentFractionatedFull = GetInvestmentFractionatedFull(reader);
                        investmentFractionatedFulls.Add(investmentFractionatedFull);
                    }
                    investmentFractionatedDataFull.InvestmentFractionatedFulls = investmentFractionatedFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentPayments.Add(investmentPayment);
                    }
                    investmentFractionatedDataFull.InvestmentPayments = investmentPayments;

                    reader.NextResult();
                    List<BankTransaction> bankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        bankTransactions.Add(bankTransaction);
                    }
                    investmentFractionatedDataFull.BankTransactions = bankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentInstallments.Add(investmentInstallment);
                    }
                    investmentFractionatedDataFull.InvestmentInstallments = investmentInstallments;

                    reader.NextResult();
                    List<InvestmentInstallmentPayment> investmentInstallmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        investmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                    investmentFractionatedDataFull.InvestmentInstallmentPayments = investmentInstallmentPayments;
                }
            }
            return investmentFractionatedDataFull;
        }


        // INSERT
        public async Task<int> Add(InvestmentFractionated investmentFractionated)
        {
            String strCmd = $"INSERT INTO {table}(InvestmentId, ProductFractionatedId, Amount, InstallmentCount, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InvestmentId, @ProductFractionatedId, @Amount, @InstallmentCount, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentFractionated.InvestmentId);
            DBHelper.AddParam(command, "@ProductFractionatedId", SqlDbType.Int, investmentFractionated.ProductFractionatedId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, investmentFractionated.Amount);
            DBHelper.AddParam(command, "@InstallmentCount", SqlDbType.Int, investmentFractionated.InstallmentCount);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentFractionated.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentFractionated investmentFractionated)
        {
            String strCmd = $"UPDATE {table} SET InvestmentId = @InvestmentId, ProductFractionatedId = @ProductFractionatedId, Amount = @Amount, InstallmentCount = @InstallmentCount, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentFractionated.InvestmentId);
            DBHelper.AddParam(command, "@ProductFractionatedId", SqlDbType.Int, investmentFractionated.ProductFractionatedId);
            DBHelper.AddParam(command, "@Amount", SqlDbType.Decimal, investmentFractionated.Amount);
            DBHelper.AddParam(command, "@InstallmentCount", SqlDbType.Int, investmentFractionated.InstallmentCount);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentFractionated.Id);

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
