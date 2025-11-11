using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProjectDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Project]";

        private static Project GetProject(SqlDataReader reader)
        {
            return new Project(Convert.ToInt32(reader["Id"]),
                               Convert.ToInt32(reader["ProjectTypeId"]),
                               reader["Code"].ToString(),
                               reader["Name"].ToString(),
                               reader["Description"].ToString(),
                               reader["Details"].ToString(),
                               Convert.ToInt32(reader["ImageCount"]),
                               Convert.ToDouble(reader["TotalArea"]),
                               Convert.ToDouble(reader["TotalBuiltArea"]),
                               Convert.ToInt32(reader["LevelCount"]),
                               Convert.ToInt32(reader["CurrencyId"]),
                               Convert.ToDouble(reader["CpiValue"]),
                               Convert.ToInt32(reader["CpiTotal"]),
                               Convert.ToInt32(reader["CpiCount"]),
                               Convert.ToDouble(reader["TotalValue"]),
                               Convert.ToDateTime(reader["StartDate"]),
                               Convert.ToInt32(reader["DevelopmentTerm"]),
                               Convert.ToDouble(reader["RentalGrowthRate"]),
                               Convert.ToDouble(reader["CapitalGrowthRate"]),
                               Convert.ToInt32(reader["EarningPeriodId"]),
                               Convert.ToDouble(reader["ManagementCost"]),
                               Convert.ToInt32(reader["Status"]));
        }

        public static ProjectFull GetProjectFull(SqlDataReader reader)
        {
            return new ProjectFull(Convert.ToInt32(reader["ProjectId"]),
                                   reader["ProjectType"].ToString(),
                                   reader["Code"].ToString(),
                                   reader["Name"].ToString(),
                                   reader["Description"].ToString(),
                                   reader["Details"].ToString(),
                                   Convert.ToInt32(reader["ImageCount"]),
                                   Convert.ToDouble(reader["TotalArea"]),
                                   Convert.ToDouble(reader["TotalBuiltArea"]),
                                   Convert.ToInt32(reader["LevelCount"]),
                                   reader["Currency"].ToString(),
                                   reader["CurrencySymbol"].ToString(),
                                   Convert.ToDouble(reader["CpiValue"]),
                                   Convert.ToInt32(reader["CpiTotal"]),
                                   Convert.ToInt32(reader["CpiCount"]),
                                   Convert.ToDouble(reader["TotalValue"]),
                                   Convert.ToDateTime(reader["StartDate"]),
                                   Convert.ToInt32(reader["DevelopmentTerm"]),
                                   Convert.ToDouble(reader["RentalGrowthRate"]),
                                   Convert.ToDouble(reader["CapitalGrowthRate"]),
                                   reader["EarningPeriod"].ToString(),
                                   Convert.ToDouble(reader["ManagementCost"]),
                                   null, // CpiRanges
                                   null, // Address
                                   null, // ProjectDescriptions
                                   null, // OperationResults
                                   null,  // CoverImage
                                   Convert.ToInt32(reader["Status"])
                                   );
        }

        // GET
        public async Task<IEnumerable<Project>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Project> projects = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Project project = GetProject(reader);
                         projects.Add(project);
                    }
                }
            }
            return projects;
        }

        public async Task<Project> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Project project = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         project = GetProject(reader);
                    }
                }
            }
            return project;
        }

        public async Task<IEnumerable<Project>> GetByProjectTypeId(int projectTypeId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectTypeId = @ProjectTypeId";

            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectTypeId", SqlDbType.Int, projectTypeId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Project> projects = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Project project = GetProject(reader);
                        projects.Add(project);
                    }
                }
            }
            return projects;
        }

        public async Task<int> GetCount()
        {
            String strCmd = $"SELECT COUNT(Id) AS Count FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            int count = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        count = Convert.ToInt32(reader["Count"]);
                    }
                }
            }
            return count;
        }

        public async Task<List<(int, int)>> GetIdImageCounts(int status)
        {
            String strCmd = $"SELECT Id, ImageCount FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<(int, int)> ids = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ids.Add((Convert.ToInt32(reader["Id"]), Convert.ToInt32(reader["ImageCount"])));
                    }
                }
            }
            return ids;
        }

        public async Task<int> GetImageCount(int id)
        {
            String strCmd = $"SELECT ImageCount FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int imageCount = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        imageCount = Convert.ToInt32(reader["ImageCount"]);
                    }
                }
            }
            return imageCount;
        }

        public async Task<int> GetCpiCount(int id)
        {
            String strCmd = $"SELECT CpiCount FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int cpiCount = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cpiCount = Convert.ToInt32(reader["CpiCount"]);
                    }
                }
            }
            return cpiCount;
        }

        public async Task<(int, int)> GetCpiTotalCount(int id)
        {
            String strCmd = $"SELECT CpiTotal, CpiCount FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int cpiTotal = -1, cpiCount = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cpiTotal = Convert.ToInt32(reader["CpiTotal"]);
                        cpiCount = Convert.ToInt32(reader["CpiCount"]);
                    }
                }
            }
            return (cpiTotal, cpiCount);
        }

        public async Task<DateTime> GetStartDate(int id)
        {
            String strCmd = $"SELECT StartDate FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            DateTime startDate = DateTime.Today;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        startDate = Convert.ToDateTime(reader["StartDate"]);
                    }
                }
            }
            return startDate;
        }

        public async Task<(DateTime, int)> GetStartDateTerm(int id)
        {
            String strCmd = $"SELECT StartDate, DevelopmentTerm FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            DateTime startDate = DateTime.Today;
            int developmentTerm = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        startDate = Convert.ToDateTime(reader["StartDate"]);
                        developmentTerm = Convert.ToInt32(reader["DevelopmentTerm"]);
                    }
                }
            }
            return (startDate, developmentTerm);
        }

        // FULL
        public async Task<ProjectProductDataFull> GetFulls(int status)
        {
            String strCmd = "SELECT Project.Id AS ProjectId, KProjectType.Name AS ProjectType," +
                            " Project.Code, Project.Name, Project.Description, Project.Details, Project.ImageCount, Project.TotalArea, Project.TotalBuiltArea," +
                            " Project.LevelCount, KCurrency.Name AS Currency, KCurrency.Symbol AS CurrencySymbol, Project.CpiValue, Project.CpiTotal, Project.CpiCount, Project.TotalValue," +
                            " Project.StartDate, Project.DevelopmentTerm, Project.RentalGrowthRate, Project.CapitalGrowthRate, KEarningPeriod.Name AS EarningPeriod," +
                            " Project.ManagementCost, Project.Status" +
                            " FROM [DD-Project] AS Project" +
                            " INNER JOIN [K-ProjectType] AS KProjectType ON (Project.ProjectTypeId = KProjectType.Id)" +
                            " INNER JOIN [K-Currency] AS KCurrency ON (Project.CurrencyId = KCurrency.Id)" +
                            " INNER JOIN [K-EarningPeriod] AS KEarningPeriod ON (Project.EarningPeriodId = KEarningPeriod.Id)" +
                            (status != -1 ? " WHERE Project.Status = @Status;" : ";") +

                            "SELECT Cpi.Id, Cpi.ProjectId, Cpi.ProductTypeId, Cpi.AmountMin, Cpi.AmountMax, Cpi.DiscountRate," +
                            " Cpi.ProfitablityRate, Cpi.CreateDateTime, Cpi.UpdateDateTime, Cpi.Status" +
                            " FROM [DD-CpiRange] AS Cpi" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Cpi.ProjectId)" +
                            " WHERE Cpi.Status = 1" +
                            (status != -1 ? " AND Project.Status = @Status;" : ";") +

                            "SELECT AddressProject.ProjectId AS EntityId, KCountry.Name AS Country, KState.Name AS State, KCity.Name AS City, Address.Address1, Address.Address2, Address.Zone," +
                            " Address.ZipCode, Address.Latitude, Address.Longitude, AddressProject.Status" +
                            " FROM [DD-Address] AS Address" +
                            " INNER JOIN [DL-AddressProject] AS AddressProject ON (AddressProject.AddressId = Address.Id)" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = AddressProject.ProjectId)" +
                            " INNER JOIN [K-Country] AS KCountry ON (Address.CountryId = KCountry.Id)" +
                            " INNER JOIN [K-State] AS KState ON (Address.StateId = KState.Id)" +
                            " INNER JOIN [K-City] AS KCity ON (Address.CityId = KCity.Id)" +
                            " WHERE AddressProject.Status = 1" +
                            (status != -1 ? " AND Project.Status = @Status;" : ";") +

                            "SELECT ProjectInformation.Id, ProjectInformation.ProjectId, ProjectInformation.ProjectInformationTypeId, ProjectInformation.Information," +
                            " ProjectInformation.CreateDateTime, ProjectInformation.UpdateDateTime, ProjectInformation.Status" +
                            " FROM [DD-ProjectInformation] AS ProjectInformation" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = ProjectInformation.ProjectId)" +
                            " WHERE ProjectInformation.Status = 1" +
                            (status != -1 ? " AND Project.Status = @Status;" : ";") +

                            "SELECT Result.Id, Result.ProjectId, Result.RevenueAmount, Result.CostAmount, Result.CreateDateTime" +
                            " FROM [DD-OperationResult] AS Result" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Result.ProjectId)" +
                            (status != -1 ? " WHERE Project.Status = @Status;" : ";") +

                            "SELECT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.ReserveRate, Product.OverdueMax," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductFractionatedStatusId" +
                            " FROM [DD-ProductFractionated] AS Product;" +

                            "SELECT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.AdvRate, Product.ReserveRate, Product.OverdueMax," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductFinancedStatusId" +
                            " FROM [DD-ProductFinanced] AS Product;" +

                            "SELECT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.ReserveRate," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductPrepaidStatusId" +
                            " FROM [DD-ProductPrepaid] AS Product;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            ProjectProductDataFull projectProductDataFull = new ProjectProductDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<ProjectFull> projectFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProjectFull ProjectFull = GetProjectFull(reader);
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
                }
            }
            return projectProductDataFull;
        }


        public async Task<ProjectProductDataFull> GetFullsByAppUser(int appUserId)
        {
            String strCmd = "SELECT DISTINCT Project.Id AS ProjectId, KProjectType.Name AS ProjectType," +
                            " Project.Code, Project.Name, Project.Description, Project.Details, Project.ImageCount, Project.TotalArea, Project.TotalBuiltArea," +
                            " Project.LevelCount, KCurrency.Name AS Currency, KCurrency.Symbol AS CurrencySymbol, Project.CpiValue, Project.CpiTotal, Project.CpiCount, Project.TotalValue," +
                            " Project.StartDate, Project.DevelopmentTerm, Project.RentalGrowthRate, Project.CapitalGrowthRate, EarningPeriod.Name AS KEarningPeriod," +
                            " Project.ManagementCost, Project.Status" +
                            " FROM [DD-Project] AS Project" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " INNER JOIN [K-ProjectType] AS KProjectType ON (Project.ProjectTypeId = KProjectType.Id)" +
                            " INNER JOIN [K-Currency] AS KCurrency ON (Project.CurrencyId = KCurrency.Id)" +
                            " INNER JOIN [K-EarningPeriod] AS KEarningPeriod ON (Project.EarningPeriodId = KEarningPeriod.Id)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Cpi.Id, Cpi.ProjectId, Cpi.ProductTypeId, Cpi.AmountMin, Cpi.AmountMax, Cpi.DiscountRate," +
                            " Cpi.ProfitablityRate, Cpi.CreateDateTime, Cpi.UpdateDateTime, Cpi.Status" +
                            " FROM [DD-CpiRange] AS Cpi" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Cpi.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Cpi.Status = 1 AND Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT AddressProject.ProjectId AS EntityId, KCountry.Name AS Country, KState.Name AS State, KCity.Name AS City, Address.Address1, Address.Address2, Address.Zone," +
                            " Address.ZipCode, Address.Latitude, Address.Longitude, AddressProject.Status" +
                            " FROM [DD-Address] AS Address" +
                            " INNER JOIN [DL-AddressProject] AS AddressProject ON (AddressProject.AddressId = Address.Id)" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = AddressProject.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " INNER JOIN [K-Country] AS KCountry ON (Address.CountryId = KCountry.Id)" +
                            " INNER JOIN [K-State] AS KState ON (Address.StateId = KState.Id)" +
                            " INNER JOIN [K-City] AS KCity ON (Address.CityId = KCity.Id)" +
                            " WHERE AddressProject.Status = 1 AND Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT ProjectInformation.Id, ProjectInformation.ProjectId, ProjectInformation.ProjectInformationTypeId, ProjectInformation.Information," +
                            " ProjectInformation.CreateDateTime, ProjectInformation.UpdateDateTime, ProjectInformation.Status" +
                            " FROM [DD-ProjectInformation] AS ProjectInformation" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = ProjectInformation.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE ProjectInformation.Status = 1 AND Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Result.Id, Result.ProjectId, Result.RevenueAmount, Result.CostAmount, Result.CreateDateTime" +
                            " FROM [DD-OperationResult] AS Result" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Result.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.OverdueMax," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductFractionatedStatusId" +
                            " FROM [DD-ProductFractionated] AS Product" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Product.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.AdvRate, Product.OverdueMax," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductFinancedStatusId" +
                            " FROM [DD-ProductFinanced] AS Product" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Product.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Inv.AppUserId = @AppUserId;" +

                            "SELECT DISTINCT Product.Id, Product.ProjectId, Product.CpiMin, Product.CpiMax, Product.CpiDefault, Product.ReserveRate," +
                            " Product.CreateDateTime, Product.UpdateDateTime, Product.ProductPrepaidStatusId" +
                            " FROM [DD-ProductPrepaid] AS Product" +
                            " INNER JOIN [DD-Project] AS Project ON (Project.Id = Product.ProjectId)" +
                            " LEFT JOIN [DD-Investment] AS Inv ON (Project.Id = Inv.ProjectId)" +
                            " WHERE Inv.AppUserId = @AppUserId;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            ProjectProductDataFull projectProductDataFull = new ProjectProductDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<ProjectFull> projectFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProjectFull ProjectFull = GetProjectFull(reader);
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
                }
            }
            return projectProductDataFull;
        }


        // INSERT
        public async Task<int> Add(Project project)
        {
            String strCmd = $"INSERT INTO {table}(ProjectTypeId, Code, Name, Description, Details, ImageCount, TotalArea, TotalBuiltArea, LevelCount, CurrencyId, CpiValue, CpiTotal, CpiCount, TotalValue, StartDate, DevelopmentTerm, RentalGrowthRate, CapitalGrowthRate, EarningPeriodId, ManagementCost, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectTypeId, @Code, @Name, @Description, @Details, @ImageCount, @TotalArea, @TotalBuiltArea, @LevelCount, @CurrencyId, @CpiValue, @CpiTotal, @CpiCount, @TotalValue, @StartDate, @DevelopmentTerm, @RentalGrowthRate, @CapitalGrowthRate, @EarningPeriodId, @ManagementCost, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectTypeId", SqlDbType.Int, project.ProjectTypeId);
            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, project.Code);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, project.Name);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, project.Description);
            DBHelper.AddParam(command, "@Details", SqlDbType.VarChar, project.Details);
            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, project.ImageCount);
            DBHelper.AddParam(command, "@TotalArea", SqlDbType.Decimal, project.TotalArea);
            DBHelper.AddParam(command, "@TotalBuiltArea", SqlDbType.Decimal, project.TotalBuiltArea);
            DBHelper.AddParam(command, "@LevelCount", SqlDbType.Int, project.LevelCount);
            DBHelper.AddParam(command, "@CurrencyId", SqlDbType.Int, project.CurrencyId);
            DBHelper.AddParam(command, "@CpiValue", SqlDbType.Decimal, project.CpiValue);
            DBHelper.AddParam(command, "@CpiTotal", SqlDbType.Int, project.CpiTotal);
            DBHelper.AddParam(command, "@CpiCount", SqlDbType.Int, project.CpiCount);
            DBHelper.AddParam(command, "@TotalValue", SqlDbType.Decimal, project.TotalValue);
            DBHelper.AddParam(command, "@StartDate", SqlDbType.DateTime2, project.StartDate);
            DBHelper.AddParam(command, "@DevelopmentTerm", SqlDbType.Int, project.DevelopmentTerm);
            DBHelper.AddParam(command, "@RentalGrowthRate", SqlDbType.Decimal, project.RentalGrowthRate);
            DBHelper.AddParam(command, "@CapitalGrowthRate", SqlDbType.Decimal, project.CapitalGrowthRate);
            DBHelper.AddParam(command, "@EarningPeriodId", SqlDbType.Int, project.EarningPeriodId);
            DBHelper.AddParam(command, "@ManagementCost", SqlDbType.Decimal, project.ManagementCost);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, project.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Project project)
        {
            String strCmd = $"UPDATE {table} SET ProjectTypeId = @ProjectTypeId, Code = @Code, Name = @Name, Description = @Description, Details = @Details, ImageCount = @ImageCount, TotalArea = @TotalArea, TotalBuiltArea = @TotalBuiltArea," +
                             " LevelCount = @LevelCount, CurrencyId = @CurrencyId, CpiValue = @CpiValue, CpiTotal = @CpiTotal, CpiCount = @CpiCount, TotalValue = @TotalValue, StartDate = @StartDate, DevelopmentTerm = @DevelopmentTerm," +
                             " RentalGrowthRate = @RentalGrowthRate, CapitalGrowthRate = @CapitalGrowthRate, EarningPeriodId = @EarningPeriodId, ManagementCost = @ManagementCost WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectTypeId", SqlDbType.Int, project.ProjectTypeId);
            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, project.Code);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, project.Name);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, project.Description);
            DBHelper.AddParam(command, "@Details", SqlDbType.VarChar, project.Details);
            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, project.ImageCount);
            DBHelper.AddParam(command, "@TotalArea", SqlDbType.Decimal, project.TotalArea);
            DBHelper.AddParam(command, "@TotalBuiltArea", SqlDbType.Decimal, project.TotalBuiltArea);
            DBHelper.AddParam(command, "@LevelCount", SqlDbType.Int, project.LevelCount);
            DBHelper.AddParam(command, "@CurrencyId", SqlDbType.Int, project.CurrencyId);
            DBHelper.AddParam(command, "@CpiValue", SqlDbType.Decimal, project.CpiValue);
            DBHelper.AddParam(command, "@CpiTotal", SqlDbType.Int, project.CpiTotal);
            DBHelper.AddParam(command, "@CpiCount", SqlDbType.Int, project.CpiCount);
            DBHelper.AddParam(command, "@TotalValue", SqlDbType.Decimal, project.TotalValue);
            DBHelper.AddParam(command, "@StartDate", SqlDbType.DateTime2, project.StartDate);
            DBHelper.AddParam(command, "@DevelopmentTerm", SqlDbType.Int, project.DevelopmentTerm);
            DBHelper.AddParam(command, "@RentalGrowthRate", SqlDbType.Decimal, project.RentalGrowthRate);
            DBHelper.AddParam(command, "@CapitalGrowthRate", SqlDbType.Decimal, project.CapitalGrowthRate);
            DBHelper.AddParam(command, "@EarningPeriodId", SqlDbType.Int, project.EarningPeriodId);
            DBHelper.AddParam(command, "@ManagementCost", SqlDbType.Decimal, project.ManagementCost);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, project.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateImageCount(int id, int imageCount)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET ImageCount = @ImageCount" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, imageCount);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateCpiCount(int id, int cpiCount, int status = -1)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET CpiCount = @CpiCount" +
                            (status >= 0 ? ", Status = @Status" : "") +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@CpiCount", SqlDbType.Int, cpiCount);
            if (status >= 0)
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
                            " SET Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

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
    }
}
