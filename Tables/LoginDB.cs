using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class LoginDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);

        // SELECT
        public async Task<LoginAppInfo> GetLoginAppInfo(int appUserId, int webSysUserId, DateTime meetingStartDateTime, DateTime meetingEndDateTime)
        {
            String strCmd = "SELECT * FROM [DD-News] WHERE Status = 1;" +

                            "SELECT Mtg.Id, Mtt.Name AS MeetingType, Mtg.Subject, Mtg.StartDateTime, Mtg.EndDateTime, Mtg.Description, Mtg.Status" +
                            " FROM [DD-Meeting] AS Mtg JOIN [K-MeetingType] AS Mtt ON (Mtt.Id = Mtg.MeetingTypeId)" +
                            " WHERE Mtg.StartDateTime >= @MeetingStartDateTime AND Mtg.EndDateTime <= @MeetingEndDateTime AND Mtg.Status = 1;" +

                            // Referred Count
                            "SELECT COUNT(1) AS Count FROM [DD-Referred] WHERE AppUserId = @AppUserId AND Status = 1;" +

                            // Investment Count
                            "SELECT COUNT(DISTINCT AppUser.Id) AS InvestmentCount" +
                            " FROM [DD-AppUser] AS AppUser" +
                            " INNER JOIN [DD-Investment] AS Inv ON (AppUser.ReferrerAppUserId = Inv.AppUserId AND Inv.InvestmentStatusId = 1)" +
                            " WHERE AppUser.ReferrerAppUserId = @AppUserId AND AppUser.Id <> @AppUserId;" +

                            // Identity
                            "SELECT * FROM [DD-Identity] WHERE AppUserId = @AppUserId AND Status = 1; " +

                            // Address AppUser
                            "SELECT Adr.* FROM [DD-Address] AS Adr INNER JOIN [DL-AddressAppUser] AS AdrApp ON (AdrApp.AddressId = Adr.Id) WHERE AdrApp.AppUserId = @AppUserId AND AdrApp.Status = 1; " +

                            // Card
                            "SELECT * FROM [DD-Card] WHERE AppUserId = @AppUserId AND Status = 1; " +

                            // Notification
                            "SELECT TOP (50) * FROM [DD-Notification] WHERE WebSysUserId = @WebSysUserId AND NotificationStatusId = 1 ORDER BY DateTime DESC;" +

                            // ProjectProductFull
                            "SELECT DISTINCT Project.Id AS ProjectId, ProjectType.Name AS ProjectType," +
                            " Project.Code, Project.Name, Project.Description, Project.Details, Project.ImageCount, Project.TotalArea, Project.TotalBuiltArea," +
                            " Project.LevelCount, Currency.Name AS Currency, Currency.Symbol AS CurrencySymbol, Project.CpiValue, Project.CpiTotal, Project.CpiCount, Project.TotalValue," +
                            " Project.StartDate, Project.DevelopmentTerm, Project.RentalGrowthRate, Project.CapitalGrowthRate, EarPer.Name AS EarningPeriod," +
                            " Project.ManagementCost, Project.Status" +
                            " FROM [DD-Project] AS Project" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " INNER JOIN [K-ProjectType] AS ProjectType ON (Project.ProjectTypeId = ProjectType.Id)" +
                            " INNER JOIN [K-Currency] AS Currency ON (Project.CurrencyId = Currency.Id)" +
                            " INNER JOIN [K-EarningPeriod] AS EarPer ON (Project.EarningPeriodId = EarPer.Id)" +
                            " WHERE Project.Status = 1 OR Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Cpi.Id, Cpi.ProjectId, Cpi.ProductTypeId, Cpi.AmountMin, Cpi.AmountMax, Cpi.DiscountRate," +
                            " Cpi.ProfitablityRate, Cpi.CreateDateTime, Cpi.UpdateDateTime, Cpi.Status" +
                            " FROM [DD-CpiRange] AS Cpi" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Cpi.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Cpi.Status = 1 AND (Project.Status = 1 OR Inv.AppUserId = @AppUserId);" +

                            "SELECT DISTINCT AdrPro.ProjectId AS EntityId, Country.Name AS Country, State.Name AS State, City.Name AS City, Adr.Address1, Adr.Address2, Adr.Zone," +
                            " Adr.ZipCode, Adr.Latitude, Adr.Longitude, AdrPro.Status" +
                            " FROM [DD-Address] AS Adr" +
                            " INNER JOIN [DL-AddressProject] AS AdrPro ON (AdrPro.AddressId = Adr.Id)" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = AdrPro.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " INNER JOIN [K-Country] AS Country ON (Adr.CountryId = Country.Id)" +
                            " INNER JOIN [K-State] AS State ON (Adr.StateId = State.Id)" +
                            " INNER JOIN [K-City] AS City ON (Adr.CityId = City.Id)" +
                            " WHERE AdrPro.Status = 1 AND (Project.Status = 1 OR Inv.AppUserId = @AppUserId);" +

                            "SELECT DISTINCT ProjectInformation.Id, ProjectInformation.ProjectId, ProjectInformation.ProjectInformationTypeId, ProjectInformation.Information," +
                            " ProjectInformation.CreateDateTime, ProjectInformation.UpdateDateTime, ProjectInformation.Status" +
                            " FROM [DD-ProjectInformation] AS ProjectInformation" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = ProjectInformation.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE ProjectInformation.Status = 1 AND (Project.Status = 1 OR Inv.AppUserId = @AppUserId);" +

                            "SELECT DISTINCT Result.Id, Result.ProjectId, Result.RevenueAmount, Result.CostAmount, Result.CreateDateTime" +
                            " FROM [DD-OperationResult] AS Result" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Result.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Project.Status = 1 OR Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.ReserveRate, Product.OverdueMax," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductFractionatedStatusId" +
                            " FROM [DD-ProductFractionated] AS Product" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Product.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Project.Status = 1 OR Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.AdvRate, Product.ReserveRate, Product.OverdueMax," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductFinancedStatusId" +
                            " FROM [DD-ProductFinanced] AS Product" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Product.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Project.Status = 1 OR Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.ReserveRate," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductPrepaidStatusId" +
                            " FROM [DD-ProductPrepaid] AS Product" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Product.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Project.Status = 1 OR Inv.AppUserId = @AppUserId;" +

                            // Investment Fractionated
                            "SELECT InvFra.Id, InvFra.InvestmentId, InvFra.ProductFractionatedId, Inv.ProjectId, Inv.ProductTypeId, Inv.AppUserId," +
                            " Inv.EffectiveDate, Inv.DevelopmentTerm, Inv.CpiCount," +
                            " Inv.TotalAmount, Inv.ReserveAmount, Inv.DueAmount, Inv.DiscountRate, Inv.DiscountAmount, Inv.Balance, Inv.CompletionDate," +
                            " Inv.DocuSignReference, Inv.BoardUserId, Inv.InvestmentMotiveId, Inv.BoardComment, Inv.InvestmentStatusId, InvFra.Amount, InvFra.InstallmentCount, InvFra.Status" +
                            " FROM [DD-Investment] AS Inv JOIN [DD-InvestmentFractionated] AS InvFra ON (InvFra.InvestmentId = Inv.Id)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId," +
                            " InvPay.TransactionId, InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                            " FROM [DD-InvestmentPayment] AS InvPay JOIN [DD-InvestmentFractionated] AS InvFra ON(InvPay.InvestmentId = InvFra.InvestmentId)" +
                            " WHERE InvPay.AppUserId = @AppUserId;" +

                            "SELECT BankTra.*" +
                            " FROM [DD-BankTransaction] AS BankTra" +
                            " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                            " JOIN [DD-InvestmentFractionated] AS InvFra ON(InvPay.InvestmentId = InvFra.InvestmentId)" +
                            " WHERE InvPay.AppUserId = @AppUserId;" +

                            "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate," +
                            " InvIns.Balance, InvIns.CompletionDate, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                            " FROM [DD-InvestmentInstallment] AS InvIns JOIN [DD-InvestmentFractionated] AS InvFra ON (InvIns.InvestmentId = InvFra.InvestmentId)" +
                            " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT InvInsPay.*" +
                            " FROM [DD-InvestmentInstallment] AS InvIns JOIN [DD-InvestmentFractionated] AS InvFra ON (InvIns.InvestmentId = InvFra.InvestmentId)" +
                            " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                            " JOIN [DD-InvestmentInstallmentPayment] AS InvInsPay ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                            " WHERE Inv.AppUserId = @AppUserId" +
                            " AND InvFra.Status = 1;" +

                            // Investment Financed
                            "SELECT InvFin.Id, InvFin.InvestmentId, InvFin.ProductFinancedId, Inv.ProjectId, Inv.ProductTypeId, Inv.AppUserId," +
                            " Inv.EffectiveDate, Inv.DevelopmentTerm, Inv.CpiCount," +
                            " Inv.TotalAmount, Inv.ReserveAmount, Inv.DueAmount, Inv.DiscountRate, Inv.DiscountAmount, Inv.Balance, Inv.CompletionDate," +
                            " Inv.DocuSignReference, Inv.BoardUserId, Inv.InvestmentMotiveId, Inv.BoardComment, Inv.InvestmentStatusId, InvFin.AdvAmount, InvFin.AdvInstallmentTotal," +
                            " InvFin.LoanInterestRate, InvFin.LoanTerm, InvFin.Status" +
                            " FROM [DD-Investment] AS Inv JOIN [DD-InvestmentFinanced] AS InvFin ON(InvFin.InvestmentId = Inv.Id)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId," +
                            " InvPay.TransactionId, InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                            " FROM [DD-InvestmentPayment] AS InvPay JOIN [DD-InvestmentFinanced] AS InvFin ON(InvPay.InvestmentId = InvFin.InvestmentId)" +
                            " WHERE InvPay.AppUserId = @AppUserId;" +

                            "SELECT BankTra.*" +
                            " FROM [DD-BankTransaction] AS BankTra" +
                            " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                            " JOIN [DD-InvestmentFinanced] AS InvFin ON(InvPay.InvestmentId = InvFin.InvestmentId)" +
                            " WHERE InvPay.AppUserId = @AppUserId;" +

                            "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate," +
                            " InvIns.Balance, InvIns.CompletionDate, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                            " FROM [DD-InvestmentInstallment] AS InvIns JOIN [DD-InvestmentFinanced] AS InvFin ON (InvIns.InvestmentId = InvFin.InvestmentId)" +
                            " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT InvInsPay.*" +
                            $" FROM [DD-InvestmentInstallment] AS InvIns JOIN [DD-InvestmentFinanced] AS InvFin ON (InvIns.InvestmentId = InvFin.InvestmentId)" +
                            " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                            " JOIN [DD-InvestmentInstallmentPayment] AS InvInsPay ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                            " WHERE Inv.AppUserId = @AppUserId" +
                            " AND InvFin.Status = 1;" +

                            // Investment Prepaid
                            "SELECT InvPrep.Id, InvPrep.InvestmentId, InvPrep.ProductPrepaidId, Inv.ProjectId, Inv.ProductTypeId, Inv.AppUserId," +
                            " Inv.EffectiveDate, Inv.DevelopmentTerm, Inv.CpiCount," +
                            " Inv.TotalAmount, Inv.ReserveAmount, Inv.DueAmount, Inv.DiscountRate, Inv.DiscountAmount, Inv.Balance, Inv.CompletionDate," +
                            " Inv.DocuSignReference, Inv.BoardUserId, Inv.InvestmentMotiveId, Inv.BoardComment, Inv.InvestmentStatusId, InvPrep.Status" +
                            " FROM [DD-Investment] AS Inv JOIN [DD-InvestmentPrepaid] AS InvPrep ON (InvPrep.InvestmentId = Inv.Id)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT InvPay.Id, InvPay.InvestmentId, InvPay.AppUserId, InvPay.InvestmentPaymentTypeId, InvPay.TransactionTypeId," +
                            " InvPay.TransactionId, InvPay.CreateDateTime, InvPay.UpdateDateTime, InvPay.Status" +
                            " FROM [DD-InvestmentPayment] AS InvPay JOIN [DD-InvestmentPrepaid] AS InvPrep ON(InvPay.InvestmentId = InvPrep.InvestmentId)" +
                            " WHERE InvPay.AppUserId = @AppUserId;" +

                            "SELECT BankTra.*" +
                            " FROM [DD-BankTransaction] AS BankTra" +
                            " JOIN [DD-InvestmentPayment] AS InvPay ON (BankTra.Id = InvPay.TransactionId)" +
                            " JOIN [DD-InvestmentPrepaid] AS InvPrep ON(InvPay.InvestmentId = InvPrep.InvestmentId)" +
                            " WHERE InvPay.AppUserId = @AppUserId;" +

                            "SELECT InvIns.Id, InvIns.InvestmentId, InvIns.InvestmentPaymentTypeId, InvIns.Amount, InvIns.DiscountAmount, InvIns.EffectiveDate, InvIns.DueDate," +
                            " InvIns.Balance, InvIns.CompletionDate, InvIns.CreateDateTime, InvIns.UpdateDateTime, InvIns.Status" +
                            " FROM [DD-InvestmentInstallment] AS InvIns JOIN [DD-InvestmentPrepaid] AS InvPrep ON (InvIns.InvestmentId = InvPrep.InvestmentId)" +
                            " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT InvInsPay.*" +
                            " FROM [DD-InvestmentInstallment] AS InvIns JOIN [DD-InvestmentPrepaid] AS InvPre ON (InvIns.InvestmentId = InvPre.InvestmentId)" +
                            " JOIN [DD-Investment] AS Inv ON (Inv.Id = InvIns.InvestmentId)" +
                            " JOIN [DD-InvestmentInstallmentPayment] AS InvInsPay ON (InvInsPay.InvestmentInstallmentId = InvIns.Id)" +
                            " WHERE Inv.AppUserId = @AppUserId" +
                            " AND InvPre.Status = 1;" +

                            // ProjectLikeIds
                            "SELECT ProjectId FROM [DD-ProjectLike] WHERE Status = 1 AND AppUserId = @AppUserId;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);
            DBHelper.AddParam(command, "@MeetingStartDateTime", SqlDbType.DateTime2, meetingStartDateTime);
            DBHelper.AddParam(command, "@MeetingEndDateTime", SqlDbType.DateTime2, meetingEndDateTime);

            LoginAppInfo loginAppInfo = new LoginAppInfo();

            ProjectProductDataFull projectProductDataFull = new ProjectProductDataFull();

            InvestmentFractionatedDataFull investmentFractionatedDataFull = new InvestmentFractionatedDataFull();
            InvestmentFinancedDataFull investmentFinancedDataFull = new InvestmentFinancedDataFull();
            InvestmentPrepaidDataFull investmentPrepaidDataFull = new InvestmentPrepaidDataFull();

            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<NewsInfo> newsInfos = [];
                    while (await reader.ReadAsync())
                    {
                        News news = NewsDB.GetNews(reader);
                        newsInfos.Add(new NewsInfo(news, null));
                    }
                    loginAppInfo.NewsInfos = newsInfos;

                    reader.NextResult();
                    List<MeetingFull> meetingFulls = [];
                    while (await reader.ReadAsync())
                    {
                        MeetingFull meetingFull = MeetingDB.GetMeetingFull(reader);
                        meetingFulls.Add(meetingFull);
                    }
                    loginAppInfo.MeetingFulls = meetingFulls;

                    reader.NextResult();
                    ReferredCount referredCount = new ReferredCount();
                    if (await reader.ReadAsync())
                        referredCount.Count = Convert.ToInt32(reader["Count"]);

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        referredCount.InvestmentCount = Convert.ToInt32(reader["InvestmentCount"]);

                    loginAppInfo.ReferredCount = referredCount;

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        loginAppInfo.Identity = IdentityDB.GetIdentity(reader);

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        loginAppInfo.Address = AddressDB.GetAddress(reader);

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        loginAppInfo.Card = CardDB.GetCard(reader);

                    reader.NextResult();
                    List<Notification> notifications = [];
                    while (await reader.ReadAsync())
                        notifications.Add(NotificationDB.GetNotification(reader));
                    loginAppInfo.Notifications = notifications;

                    reader.NextResult();
                    List<ProjectFull> projectFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProjectFull ProjectFull = ProjectDB.GetProjectFull(reader);
                        projectFulls.Add(ProjectFull);
                    }
                    projectProductDataFull.ProjectFulls = projectFulls;

                    reader.NextResult();
                    List<CpiRange> cpiRanges = [];
                    while (await reader.ReadAsync())
                    {
                        CpiRange cpiRange = CpiRangeDB.GetCpiRange(reader);
                        cpiRanges.Add(cpiRange);
                    }
                    projectProductDataFull.CpiRanges = cpiRanges;

                    reader.NextResult();
                    List<AddressFull> addressFulls = [];
                    while (await reader.ReadAsync())
                    {
                        AddressFull addressFull = AddressDB.GetAddressFull(reader);
                        addressFulls.Add(addressFull);
                    }
                    projectProductDataFull.AddressFulls = addressFulls;

                    reader.NextResult();
                    List<ProjectInformation> projectInformations = [];
                    while (await reader.ReadAsync())
                    {
                        ProjectInformation projectInformation = ProjectInformationDB.GetProjectInformation(reader);
                        projectInformations.Add(projectInformation);
                    }
                    projectProductDataFull.ProjectInformations = projectInformations;

                    reader.NextResult();
                    List<OperationResult> operationResults = [];
                    while (await reader.ReadAsync())
                    {
                        OperationResult operationResult = OperationResultDB.GetOperationResult(reader);
                        operationResults.Add(operationResult);
                    }
                    projectProductDataFull.OperationResults = operationResults;

                    reader.NextResult();
                    List<ProductFractionated> productFractionateds = [];
                    while (await reader.ReadAsync())
                    {
                        ProductFractionated productFractionated = ProductFractionatedDB.GetProductFractionated(reader);
                        productFractionateds.Add(productFractionated);
                    }
                    projectProductDataFull.ProductFractionateds = productFractionateds;

                    reader.NextResult();
                    List<ProductFinanced> productFinanceds = [];
                    while (await reader.ReadAsync())
                    {
                        ProductFinanced productFinanced = ProductFinancedDB.GetProductFinanced(reader);
                        productFinanceds.Add(productFinanced);
                    }
                    projectProductDataFull.ProductFinanceds = productFinanceds;

                    reader.NextResult();
                    List<ProductPrepaid> productPrepaids = [];
                    while (await reader.ReadAsync())
                    {
                        ProductPrepaid productPrepaid = ProductPrepaidDB.GetProductPrepaid(reader);
                        productPrepaids.Add(productPrepaid);
                    }
                    projectProductDataFull.ProductPrepaids = productPrepaids;

                    // Investment Fractionated
                    reader.NextResult();
                    List<InvestmentFractionatedFull> investmentsFractionatedFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentFractionatedFull investmentFractionatedFull = InvestmentFractionatedDB.GetInvestmentFractionatedFull(reader);
                        investmentsFractionatedFulls.Add(investmentFractionatedFull);
                    }
                    investmentFractionatedDataFull.InvestmentFractionatedFulls = investmentsFractionatedFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentFractionatedPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentFractionatedPayments.Add(investmentPayment);
                    }
                    investmentFractionatedDataFull.InvestmentPayments = investmentFractionatedPayments;

                    reader.NextResult();
                    List<BankTransaction> fractionatedBankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        fractionatedBankTransactions.Add(bankTransaction);
                    }
                    investmentFractionatedDataFull.BankTransactions = fractionatedBankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentFractionatedInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentFractionatedInstallments.Add(investmentInstallment);
                    }
                    investmentFractionatedDataFull.InvestmentInstallments = investmentFractionatedInstallments;

                    reader.NextResult();
                    List<InvestmentInstallmentPayment> fractionatedInvestmentInstallmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        fractionatedInvestmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                    investmentFractionatedDataFull.InvestmentInstallmentPayments = fractionatedInvestmentInstallmentPayments;

                    // Investment Financed
                    reader.NextResult();
                    List<InvestmentFinancedFull> investmentsFinancedFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentFinancedFull investmentFinancedFull = InvestmentFinancedDB.GetInvestmentFinancedFull(reader);
                        investmentsFinancedFulls.Add(investmentFinancedFull);
                    }
                    investmentFinancedDataFull.InvestmentFinancedFulls = investmentsFinancedFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentFinancedPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentFinancedPayments.Add(investmentPayment);
                    }
                    investmentFinancedDataFull.InvestmentPayments = investmentFinancedPayments;

                    reader.NextResult();
                    List<BankTransaction> financedBankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        financedBankTransactions.Add(bankTransaction);
                    }
                    investmentFinancedDataFull.BankTransactions = financedBankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentFinancedInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentFinancedInstallments.Add(investmentInstallment);
                    }
                    investmentFinancedDataFull.InvestmentInstallments = investmentFinancedInstallments;

                    reader.NextResult();
                    List<InvestmentInstallmentPayment> financedInvestmentInstallmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        financedInvestmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                    investmentFinancedDataFull.InvestmentInstallmentPayments = financedInvestmentInstallmentPayments;

                    // Investment Prepaid
                    reader.NextResult();
                    List<InvestmentPrepaidFull> investmentPrepaidFulls = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPrepaidFull investmentPrepaidFull = InvestmentPrepaidDB.GetInvestmentPrepaidFull(reader);
                        investmentPrepaidFulls.Add(investmentPrepaidFull);
                    }
                    investmentPrepaidDataFull.InvestmentPrepaidFulls = investmentPrepaidFulls;

                    reader.NextResult();
                    List<InvestmentPayment> investmentPrepaidPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentPayment investmentPayment = InvestmentPaymentDB.GetInvestmentPayment(reader);
                        investmentPrepaidPayments.Add(investmentPayment);
                    }
                    investmentPrepaidDataFull.InvestmentPayments = investmentPrepaidPayments;

                    reader.NextResult();
                    List<BankTransaction> prepaidBankTransactions = [];
                    while (await reader.ReadAsync())
                    {
                        BankTransaction bankTransaction = BankTransactionDB.GetBankTransaction(reader);
                        prepaidBankTransactions.Add(bankTransaction);
                    }
                    investmentPrepaidDataFull.BankTransactions = prepaidBankTransactions;

                    reader.NextResult();
                    List<InvestmentInstallment> investmentPrepaidInstallments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallment investmentInstallment = InvestmentInstallmentDB.GetInvestmentInstallment(reader);
                        investmentPrepaidInstallments.Add(investmentInstallment);
                    }
                    investmentPrepaidDataFull.InvestmentInstallments = investmentPrepaidInstallments;

                    reader.NextResult();
                    List<InvestmentInstallmentPayment> prepaidInvestmentInstallmentPayments = [];
                    while (await reader.ReadAsync())
                    {
                        InvestmentInstallmentPayment investmentInstallmentPayment = InvestmentInstallmentPaymentDB.GetInvestmentInstallmentPayment(reader);
                        prepaidInvestmentInstallmentPayments.Add(investmentInstallmentPayment);
                    }
                    investmentPrepaidDataFull.InvestmentInstallmentPayments = prepaidInvestmentInstallmentPayments;

                    // PROJECT LIKE IDS
                    reader.NextResult();
                    List<int> projectIds = [];
                    while (await reader.ReadAsync())
                    {
                        int rojectId = Convert.ToInt32(reader["ProjectId"]);
                        projectIds.Add(rojectId);
                    }
                    loginAppInfo.ProjectLikeIds = projectIds;

                }
            }

            loginAppInfo.ProjectProductFulls = await ProjectFunctions.GetFulls(projectProductDataFull);

            loginAppInfo.InvestmentFractionatedFulls = InvestmentFractionatedFunctions.GetFulls(investmentFractionatedDataFull);
            loginAppInfo.InvestmentFinancedFulls = InvestmentFinancedFunctions.GetFulls(investmentFinancedDataFull);
            loginAppInfo.InvestmentPrepaidFulls = InvestmentPrepaidFunctions.GetFulls(investmentPrepaidDataFull);

            return loginAppInfo;
        }
    }
}
