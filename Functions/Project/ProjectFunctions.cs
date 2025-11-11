using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class ProjectFunctions
    {
        // GET
        public static async Task<Project> GetById(int id)
        {
            return await new ProjectDB().GetById(id);
        }

        public static async Task<IEnumerable<Project>> GetByProjectTypeId(int projectTypeId)
        {
            return await new ProjectDB().GetByProjectTypeId(projectTypeId, 1);
        }

        public static async Task<int> GetCpiCount(int id)
        {
            return await new ProjectDB().GetCpiCount(id);
        }

        public static async Task<(int, int)> GetCpiTotalCount(int id)
        {
            return await new ProjectDB().GetCpiTotalCount(id);
        }

        public static async Task<DateTime> GetStartDate(int id)
        {
            return await new ProjectDB().GetStartDate(id);
        }

        public static async Task<(DateTime, int)> GetStartDateTerm(int id)
        {
            return await new ProjectDB().GetStartDateTerm(id);
        }

        public static async Task<double> GetDiscountRate(int id, int productTypeId, int cpiCount)
        {
            List<CpiRange> cpiRanges = await new CpiRangeDB().GetByProjectProductId(id, productTypeId, 1);

            for (int i = 0; i < cpiRanges.Count; i++)
            {
                if (cpiCount >= cpiRanges[i].AmountMin && cpiCount <= cpiRanges[i].AmountMax)
                    return cpiRanges[i].DiscountRate;
            }

            return 0d;
        }

        public static int CalculateInvestmentTerm(DateTime effectiveDate, DateTime endDate)
        {
            if (effectiveDate >= endDate)
                return 0;

            return (int)((endDate - effectiveDate).TotalDays / 365d * 12d);
        }

        public static async Task<List<CpiRange>> GetCpiRangesById(int id, int status = 1)
        {
            return await new CpiRangeDB().GetByProjectId(id, status);
        }

        public static async Task<List<ProjectInformation>> GetInformationsById(int id, int status = 1)
        {
            return await new ProjectInformationDB().GetByProjectId(id, status);
        }

        // GET IMAGES
        public static async Task<List<ProjectImages>> GetImages(bool first, int status = 1)
        {
            List<(int id, int count)> res = await new ProjectDB().GetIdImageCounts(status);

            List<ProjectImages> images = [];
            for (int i = 0; i < res.Count; i++)
                images.Add(new ProjectImages(res[i].id, await GetImages(res[i].id, res[i].count, first)));

            return images;
        }

        public static async Task<List<String>> GetImagesById(int id, bool first)
        {
            return await GetImages(id, await new ProjectDB().GetImageCount(id), first);
        }

        public static async Task<List<String>> GetImages(int id, int count, bool first)
        {
            List<String> images = [];
            String filename = $"project{id:D08}";
            for (int i = first ? 0 : 1; i < count; i++)
            {
                byte[] img = await StorageFunctions.ReadFile("projects", $"{filename}|{i:D02}", "jpg");
                if (img == null) continue;
                images.Add(Convert.ToBase64String(img));
            }

            return images;
        }

        // GET FULL
        public static async Task<List<ProjectProductFull>> GetFulls(int status)
        {
            return await GetFulls(await new ProjectDB().GetFulls(status));
        }

        public static async Task<List<ProjectProductFull>> GetFullsByAppUser(int appUserId)
        {
            return await GetFulls(await new ProjectDB().GetFullsByAppUser(appUserId));
        }

        public static async Task<List<ProjectProductFull>> GetFulls(ProjectProductDataFull projectProductDataFull)
        {
            // CpiRanges
            Dictionary<int, List<CpiRange>> cpiRangesDict = [];
            foreach (CpiRange cpiRange in projectProductDataFull.CpiRanges)
            {
                if (cpiRangesDict.TryGetValue(cpiRange.ProjectId, out List<CpiRange> value))
                    value.Add(cpiRange);
                else
                    cpiRangesDict[cpiRange.ProjectId] = [cpiRange];
            }

            // Address Full
            Dictionary<int, AddressFull> addressFullDict = [];
            foreach (AddressFull addressFull in projectProductDataFull.AddressFulls)
                addressFullDict.Add(addressFull.EntityId, addressFull);

            // ProjectDescriptionss
            Dictionary<int, List<ProjectInformation>> projectInformationsDict = [];
            foreach (ProjectInformation projectInformation in projectProductDataFull.ProjectInformations)
            {
                if (projectInformationsDict.TryGetValue(projectInformation.ProjectId, out List<ProjectInformation> value))
                    value.Add(projectInformation);
                else
                    projectInformationsDict[projectInformation.ProjectId] = [projectInformation];
            }

            // OperationResuls
            Dictionary<int, List<OperationResult>> operationResultsDict = [];
            foreach (OperationResult operationResult in projectProductDataFull.OperationResults)
            {
                if (operationResultsDict.TryGetValue(operationResult.ProjectId, out List<OperationResult> value))
                    value.Add(operationResult);
                else
                    operationResultsDict[operationResult.ProjectId] = [operationResult];
            }

            // ProductFractionated
            Dictionary<int, ProductFractionated> productFractionatedDict = [];
            foreach (ProductFractionated productFractionated in projectProductDataFull.ProductFractionateds)
                productFractionatedDict.Add(productFractionated.ProjectId, productFractionated);

            // ProductFinanced
            Dictionary<int, ProductFinanced> productFinancedDict = [];
            foreach (ProductFinanced productFinanced in projectProductDataFull.ProductFinanceds)
                productFinancedDict.Add(productFinanced.ProjectId, productFinanced);

            // ProductPrepaid
            Dictionary<int, ProductPrepaid> productPrepaidDict = [];
            foreach (ProductPrepaid productPrepaid in projectProductDataFull.ProductPrepaids)
                productPrepaidDict.Add(productPrepaid.ProjectId, productPrepaid);

            // ProjectProductFulls
            List<ProjectProductFull> projectProductFulls = [];

            foreach (ProjectFull projectFull in projectProductDataFull.ProjectFulls)
            {
                projectFull.CpiRanges = cpiRangesDict.TryGetValue(projectFull.ProjectId, out List<CpiRange> cpiRangesValue) ? cpiRangesValue : [];
                projectFull.AddressFull = addressFullDict[projectFull.ProjectId];

                projectFull.Informations = projectInformationsDict.TryGetValue(projectFull.ProjectId, out List<ProjectInformation> projectInformations) ? projectInformations : [];
                projectFull.OperationResults = operationResultsDict.TryGetValue(projectFull.ProjectId, out List<OperationResult> operationResults) ? operationResults : [];

                byte[] image = await StorageFunctions.ReadFile("projects", $"project{projectFull.ProjectId:D08}|00", "jpg");
                projectFull.CoverImage = image == null ? null : Convert.ToBase64String(image);

                ProjectProductFull projectProductfull = new ProjectProductFull
                {
                    ProjectFull = projectFull,
                    ProductFractionated = productFractionatedDict.TryGetValue(projectFull.ProjectId, out ProductFractionated productFractionatedValue) ? productFractionatedValue : null,
                    ProductFinanced = productFinancedDict.TryGetValue(projectFull.ProjectId, out ProductFinanced productFinancedValue) ? productFinancedValue : null,
                    ProductPrepaid = productPrepaidDict.TryGetValue(projectFull.ProjectId, out ProductPrepaid productPrepaidValue) ? productPrepaidValue : null
                };

                projectProductFulls.Add(projectProductfull);
            }

            return projectProductFulls;
        }

        // GET REQUEST
        public static async Task<ProjectInfo> GetInfo(int projectId, bool images)
        {
            ProjectInfo projectInfo = new ProjectInfo
            {
                Project = await GetById(projectId),
                Address = await AddressFunctions.GetByProjectId(projectId),
                ProductFractionated = await ProductFractionatedFunctions.GetByProjectId(projectId),
                ProductFinanced = await ProductFinancedFunctions.GetByProjectId(projectId),
                ProductPrepaid = await ProductPrepaidFunctions.GetByProjectId(projectId),
                CpiRanges = await GetCpiRangesById(projectId),
                Informations = await GetInformationsById(projectId),
                Images = images ? await GetImagesById(projectId, true) : null
            };

            // Project
            projectInfo.Project.RentalGrowthRate *= 100d;
            projectInfo.Project.CapitalGrowthRate *= 100d;

            // Products
            if (projectInfo.ProductFractionated != null)
                projectInfo.ProductFractionated.ReserveRate *= 100d;
            if (projectInfo.ProductFinanced != null)
            {
                projectInfo.ProductFinanced.ReserveRate *= 100d;
                projectInfo.ProductFinanced.AdvRate *= 100d;
            }
            if (projectInfo.ProductPrepaid != null)
                projectInfo.ProductPrepaid.ReserveRate *= 100d;

            // CpiRanges
            for (int i = 0; i < projectInfo.CpiRanges.Count; i++)
            {
                projectInfo.CpiRanges[i].DiscountRate *= 100d;
                projectInfo.CpiRanges[i].ProfitablityRate *= 100d;
            }

            return projectInfo;
        }

        // REGISTER
        public static async Task<int> Register(ProjectInfo projectInfo)
        {
            int projectId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int count = await new ProjectDB().GetCount();
                projectInfo.Project.Code = $"P{count + 1:D04}";
                projectInfo.Project.CpiCount = 0;
                projectInfo.Project.CurrencyId = 47;
                projectInfo.Project.Status = 1;

                projectId = await Add(projectInfo.Project);

                await AddressFunctions.RegisterByProject(projectId, projectInfo.Address);

                if (projectInfo.ProductFractionated != null)
                {
                    projectInfo.ProductFractionated.ProjectId = projectId;
                    await ProductFractionatedFunctions.Register(projectInfo.ProductFractionated);
                }

                if (projectInfo.ProductFinanced != null)
                {
                    projectInfo.ProductFinanced.ProjectId = projectId;
                    await ProductFinancedFunctions.Register(projectInfo.ProductFinanced);
                }

                if (projectInfo.ProductPrepaid != null)
                {
                    projectInfo.ProductPrepaid.ProjectId = projectId;
                    await ProductPrepaidFunctions.Register(projectInfo.ProductPrepaid);
                }

                for (int i = 0; i < projectInfo.CpiRanges.Count; i++)
                {
                    projectInfo.CpiRanges[i].ProjectId = projectId;
                    projectInfo.CpiRanges[i].Status = 1;
                    await new CpiRangeDB().Add(projectInfo.CpiRanges[i]);
                }

                for (int i = 0; i < projectInfo.Informations.Count; i++)
                {
                    projectInfo.Informations[i].ProjectId = projectId;
                    projectInfo.Informations[i].Status = 1;
                    await new ProjectInformationDB().Add(projectInfo.Informations[i]);
                }

                scope.Complete();
            }

            await RegisterImages(projectId, projectInfo.Images);

            return projectId;
        }

        public static async Task RegisterImages(int projectId, List<String> images)
        {
            if (images == null || images.Count == 0)
                throw new Exception("Images list should NOT be empty");

            String containerName = "projects";
            String filename = $"project{projectId:D08}";

            await DeleteImages(containerName, filename);

            await StorageFunctions.CreateContainer(containerName);
            int count = 0;
            for (int i = 0; i < images.Count; i++)
            {
                if (String.IsNullOrEmpty(images[i]))
                    continue;

                await StorageFunctions.UpdateFile(containerName, $"{filename}|{i:D02}", "jpg", Convert.FromBase64String(images[i]));
                count++;
            }

            await new ProjectDB().UpdateImageCount(projectId, count);
        }

        public static async Task DeleteSoftImages(String containerName, String filename)
        {
            for (int idx = 0; ; idx++)
                if (!await StorageFunctions.DeleteSoftFile(containerName, $"{filename}|{idx:D02}", "jpg"))
                    break;
        }

        public static async Task DeleteImages(String containerName, String filename)
        {
            for (int idx = 0; ; idx++)
                if (!await StorageFunctions.DeleteFile(containerName, $"{filename}|{idx:D02}.jpg"))
                    break;
        }

        // ADD
        public static async Task<int> Add(Project project)
        {
            return await new ProjectDB().Add(project);
        }

        // UPDATE
        public static async Task Update(ProjectInfo projectInfo)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                projectInfo.Project.CurrencyId = 47;

                await new ProjectDB().Update(projectInfo.Project);
                await AddressFunctions.Update(projectInfo.Address);

                if (projectInfo.ProductFractionated != null)
                {
                    projectInfo.ProductFractionated.ProjectId = projectInfo.Project.Id;
                    if (projectInfo.ProductFractionated.Id == -1)
                        await ProductFractionatedFunctions.Register(projectInfo.ProductFractionated);
                    else
                        await ProductFractionatedFunctions.Update(projectInfo.ProductFractionated);
                }
                else
                {
                    int id = await ProductFractionatedFunctions.GetIdByProjectId(projectInfo.Project.Id);
                    if (id != -1)
                        await ProductFractionatedFunctions.UpdateStatus(id, 0);
                }
                if (projectInfo.ProductFinanced != null)
                {
                    projectInfo.ProductFinanced.ProjectId = projectInfo.Project.Id;
                    if (projectInfo.ProductFinanced.Id == -1)
                        await ProductFinancedFunctions.Register(projectInfo.ProductFinanced);
                    else
                        await ProductFinancedFunctions.Update(projectInfo.ProductFinanced);
                }
                else
                {
                    int id = await ProductFinancedFunctions.GetIdByProjectId(projectInfo.Project.Id);
                    if (id != -1)
                        await ProductFinancedFunctions.UpdateStatus(id, 0);
                }
                if (projectInfo.ProductPrepaid != null)
                {
                    projectInfo.ProductPrepaid.ProjectId = projectInfo.Project.Id;
                    if (projectInfo.ProductPrepaid.Id == -1)
                        await ProductPrepaidFunctions.Register(projectInfo.ProductPrepaid);
                    else
                        await ProductPrepaidFunctions.Update(projectInfo.ProductPrepaid);
                }
                else
                {
                    int id = await ProductPrepaidFunctions.GetIdByProjectId(projectInfo.Project.Id);
                    if (id != -1)
                        await ProductPrepaidFunctions.UpdateStatus(id, 0);
                }

                await new CpiRangeDB().UpdateStatusByProjectId(projectInfo.Project.Id, 0);
                for (int i = 0; i < projectInfo.CpiRanges.Count; i++)
                {
                    projectInfo.CpiRanges[i].Status = 1;
                    if (projectInfo.CpiRanges[i].Id == -1)
                    {
                        projectInfo.CpiRanges[i].ProjectId = projectInfo.Project.Id;
                        await new CpiRangeDB().Add(projectInfo.CpiRanges[i]);
                    }
                    else
                        await new CpiRangeDB().Update(projectInfo.CpiRanges[i], true);
                }

                await new ProjectInformationDB().UpdateStatusByProjectId(projectInfo.Project.Id, 0);
                for (int i = 0; i < projectInfo.Informations.Count; i++)
                {
                    projectInfo.Informations[i].Status = 1;
                    if (projectInfo.Informations[i].Id == -1)
                    {
                        projectInfo.Informations[i].ProjectId = projectInfo.Project.Id;
                        await new ProjectInformationDB().Add(projectInfo.Informations[i]);
                    }
                    else
                        await new ProjectInformationDB().Update(projectInfo.Informations[i], true);
                }

                await RegisterImages(projectInfo.Project.Id, projectInfo.Images);

                scope.Complete();
            }
        }

        public static async Task<bool> UpdateCpiCount(int id, int cpiCount, int status = -1)
        {
            return await new ProjectDB().UpdateCpiCount(id, cpiCount, status);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new ProjectDB().UpdateStatus(id, status);
        }
    }
}
