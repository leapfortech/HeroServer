using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;

namespace HeroServer
{
    public static class InvestmentFunctions
    {
        // Get Full
        public static async Task<InvestmentResponse> GetFullsByStatus(int status)
        {
            InvestmentResponse investmentResponse = new InvestmentResponse
            {
                InvestmentFractionatedFulls = await InvestmentFractionatedFunctions.GetFullsByStatus(status),
                InvestmentFinancedFulls = await InvestmentFinancedFunctions.GetFullsByStatus(status),
                InvestmentPrepaidFulls = await InvestmentPrepaidFunctions.GetFullsByStatus(status)
            };

            return investmentResponse;
        }

        public static async Task<InvestmentResponse> GetFullsByAppUserId(int appUserId)
        {
            InvestmentResponse investmentResponse = new InvestmentResponse
            {
                InvestmentFractionatedFulls = await InvestmentFractionatedFunctions.GetFullsByAppUserId(appUserId),
                InvestmentFinancedFulls = await InvestmentFinancedFunctions.GetFullsByAppUserId(appUserId),
                InvestmentPrepaidFulls = await InvestmentPrepaidFunctions.GetFullsByAppUserId(appUserId)
            };

            return investmentResponse;
        }

        // Get
        public static async Task<Investment> GetById(int id)
        {
            return await new InvestmentDB().GetById(id);
        }

        public static async Task<Investment> GetByProjectId(int projectId)
        {
            return await new InvestmentDB().GetByProjectId(projectId);
        }

        public static async Task<List<Investment>> GetByAppUserId(int appUserId)
        {
            return await new InvestmentDB().GetByAppUserId(appUserId);
        }

        public static async Task<List<int>> GetIdsByAppUserId(int appUserId)
        {
            return await new InvestmentDB().GetIdsByAppUserId(appUserId);
        }

        public static async Task<int> GetProjectIdById(int id)
        {
            return await new InvestmentDB().GetProjectIdById(id);
        }

        public static async Task<int> GetCpiCountById(int id)
        {
            return await new InvestmentDB().GetCpiCountById(id);
        }

        public static async Task<double> GetReserveAmountById(int id)
        {
            return await new InvestmentDB().GetReserveAmountById(id);
        }

        public static async Task<double> GetBalanceById(int id)
        {
            return await new InvestmentDB().GetBalanceById(id);
        }

        // Get Docs
        public static async Task<List<String>> GetDocRtu(int id)
        {
            return await GetDocs(id, "docrtu");
        }

        public static async Task<List<String>> GetDocBank(int id)
        {
            return await GetDocs(id, "docbank");
        }

        public static async Task<List<String>> GetDocs(int id, String prefix)
        {
            int appUserId = await new InvestmentDB().GetAppUserIdById(id);

            String containerName = $"invest{appUserId:D08}";
            String filename = $"{prefix}{id:D08}";

            List<String> docs = [];
            for (int idx = 0; ; idx++)
            {
                byte[] img = await StorageFunctions.ReadFile(containerName, $"{filename}|{idx:D02}", "jpg");
                if (img == null)
                    break;
                docs.Add(Convert.ToBase64String(img));
            }

            return docs;
        }

        public static async Task<List<InvestmentDocInfo>> GetDocInfosByStatus(int status)
        {
            List<Investment> investments = await new InvestmentDB().GetByStatus(status);
            List<InvestmentDocInfo> investmentDocInfos = new List<InvestmentDocInfo>(investments.Count);

            for (int i = 0; i < investments.Count; i++)
            {
                InvestmentDocInfo investmentDocInfo = new InvestmentDocInfo()
                {
                    Investment = investments[i],
                    DocRtus = await GetDocRtu(investments[i].Id),
                    EconomicsInfo = await EconomicsFunctions.GetInfoByInvestmentId(investments[i].Id),
                    DocBanks = await GetDocBank(investments[i].Id)
                };

                investmentDocInfos.Add(investmentDocInfo);
            }

            return investmentDocInfos;
        }

        // Register Docs
        public static async Task RegisterDocRtu(InvestmentDocRequest docRequest)
        {
            await RegisterDocs(docRequest.InvestmentId, "docrtu", docRequest.Docs);

            await new InvestmentDB().UpdateStatus(docRequest.InvestmentId, 4);
        }

        public static async Task RegisterDocBank(InvestmentDocRequest docRequest)
        {
            await RegisterDocs(docRequest.InvestmentId, "docbank", docRequest.Docs);

            await new InvestmentDB().UpdateStatus(docRequest.InvestmentId, 6);
        }

        public static async Task RegisterDocs(int id, String prefix, List<String> docs)
        {
            if (docs == null || docs.Count == 0)
                throw new Exception(prefix + " list should NOT be empty");

            int appUserId = await new InvestmentDB().GetAppUserIdById(id);

            String containerName = $"invest{appUserId:D08}";
            String filename = $"{prefix}{id:D08}";

            await DeleteSoftDocs(containerName, filename);

            await StorageFunctions.CreateContainer(containerName);
            for (int i = 0; i < docs.Count; i++)
            {
                if (String.IsNullOrEmpty(docs[i]))
                    continue;

                await StorageFunctions.UpdateFile(containerName, $"{filename}|{i:D02}", "jpg", Convert.FromBase64String(docs[i]));
            }
        }

        // Register Reference
        public static async Task<List<int>> RegisterReference(InvestmentReference[] investmentReferences)
        {
            List<int> ids = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ids = await InvestmentReferenceFunctions.Register(investmentReferences);

                await new InvestmentDB().UpdateStatus(investmentReferences[0].InvestmentId, 7);

                scope.Complete();
            }
            return ids;
        }

        public static async Task<List<int>> RegisterSignatory(InvestmentIdentityRequest[] investmentIdentityRequests)
        {
            List<int> ids = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ids = await InvestmentIdentityFunctions.Register(investmentIdentityRequests);

                await new InvestmentDB().UpdateStatus(investmentIdentityRequests[0].InvestmentIdentity.InvestmentId, 8);

                scope.Complete();
            }
            return ids;
        }

        public static async Task CreateSignatory(int investmentId)
        {
            await new InvestmentDB().UpdateStatus(investmentId, 8);
        }

        public static async Task<List<int>> RegisterBeneficiary(InvestmentIdentityRequest[] investmentIdentityRequests)
        {
            List<int> ids = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ids = await InvestmentIdentityFunctions.Register(investmentIdentityRequests);

                await new InvestmentDB().UpdateStatus(investmentIdentityRequests[0].InvestmentIdentity.InvestmentId, 9);

                scope.Complete();
            }
            return ids;
        }

        public static async Task CreateBeneficiary(int investmentId)
        {
            await new InvestmentDB().UpdateStatus(investmentId, 9);
        }

        // Add
        public static async Task<int> Add(Investment investment)
        {
            return await new InvestmentDB().Add(investment);
        }

        // Validation
        public static async Task<bool> RequestUpdate(InvestmentBoardResponse boardResponse)
        {
            if (!await new InvestmentDB().UpdateMotive(boardResponse.InvestmentId, boardResponse.BoardUserId, boardResponse.InvestmentMotiveId, boardResponse.BoardComment))
                return false;

            await SendRequestMessage(boardResponse);
            return true;
        }

        public static async Task<bool> Authorize(InvestmentBoardResponse boardResponse)
        {
            Investment investment = await new InvestmentDB().GetById(boardResponse.InvestmentId);
            InvestmentAmounts amounts = null;

            if (investment.ProductTypeId == 1)
                (investment, amounts) = await InvestmentFractionatedFunctions.RefreshAmounts(investment, true);
            else if (investment.ProductTypeId == 2)
                investment = await InvestmentFinancedFunctions.RefreshAmounts(investment, true);
            else if (investment.ProductTypeId == 3)
                investment = await InvestmentPrepaidFunctions.RefreshAmounts(investment, true);

            investment.BoardUserId = boardResponse.BoardUserId;
            investment.InvestmentMotiveId = boardResponse.InvestmentMotiveId;
            investment.BoardComment = boardResponse.BoardComment;
            investment.InvestmentStatusId = 1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new InvestmentDB().Update(investment, true);

                await InvestmentInstallmentFunctions.Register(investment, amounts);

                if (investment.ProductTypeId == 1)
                    await InvestmentFractionatedFunctions.UpdateStatusByInvestmentId(investment.Id, 1);
                else if (investment.ProductTypeId == 2)
                    await InvestmentFinancedFunctions.UpdateStatusByInvestmentId(investment.Id, 1);
                else if (investment.ProductTypeId == 3)
                    await InvestmentPrepaidFunctions.UpdateStatusByInvestmentId(investment.Id, 1);

                (int cpiTotal, int cpiCount) = await ProjectFunctions.GetCpiTotalCount(investment.ProjectId);
                int cpiRemain = cpiTotal - cpiCount;

                if (investment.CpiCount > cpiRemain)
                    throw new Exception("No quedan suficientes CPIs.");

                cpiCount += investment.CpiCount;
                cpiRemain = cpiTotal - cpiCount;

                int cpiMin = await ProductFractionatedFunctions.GetCpiMinByProjectId(investment.ProjectId, cpiTotal);
                cpiMin = Math.Min(cpiMin, await ProductFinancedFunctions.GetCpiMinByProjectId(investment.ProjectId, cpiTotal));
                cpiMin = Math.Min(cpiMin, await ProductPrepaidFunctions.GetCpiMinByProjectId(investment.ProjectId, cpiTotal));

                if (cpiRemain < cpiMin)
                    await ProjectFunctions.UpdateCpiCount(investment.ProjectId, cpiCount, 2);
                else
                    await ProjectFunctions.UpdateCpiCount(investment.ProjectId, cpiCount);

                scope.Complete();
            }

            await SendInvestmentMessage(boardResponse, 1);
            return true;
        }

        public static async Task<bool> Reject(InvestmentBoardResponse boardResponse)
        {
            if (!await new InvestmentDB().UpdateMotiveStatus(boardResponse.InvestmentId, boardResponse.BoardUserId, boardResponse.InvestmentMotiveId, boardResponse.BoardComment, 11))
                return false;

            await SendInvestmentMessage(boardResponse, 11);
            return true;
        }

        // Update
        public static async Task<bool> Update(Investment investment)
        {
            return await new InvestmentDB().Update(investment);
        }

        public static async Task UpdateDocRtu(InvestmentDocRequest docRequest)
        {
            await RegisterDocs(docRequest.InvestmentId, "docrtu", docRequest.Docs);

            await new InvestmentDB().UpdateMotive(docRequest.InvestmentId, -1, 0, null);

            await SendUpdateMessage(docRequest.InvestmentId);
        }

        public static async Task UpdateDocBank(InvestmentDocRequest docRequest)
        {
            await RegisterDocs(docRequest.InvestmentId, "docbank", docRequest.Docs);

            await new InvestmentDB().UpdateMotive(docRequest.InvestmentId, -1, 0, null);

            await SendUpdateMessage(docRequest.InvestmentId);
        }

        public static async Task<bool> UpdateBalance(int id, double balance)
        {
            return await new InvestmentDB().UpdateBalance(id, balance, balance == 0d ? DateTime.Now : null);
        }

        public static async Task<bool> UpdateBalanceStatus(int id, double balance, int status)
        {
            return await new InvestmentDB().UpdateBalanceStatus(id, balance, balance == 0d ? DateTime.Now : null, status);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentDB().UpdateStatus(id, status);
        }

        // Delete
        public static async Task DeleteSoftDocs(int id, String prefix)
        {
            int appUserId = await new InvestmentDB().GetAppUserIdById(id);

            String containerName = $"invest{appUserId:D08}";
            String filename = $"{prefix}{id:D08}";

            await DeleteSoftDocs(containerName, filename);
        }

        public static async Task DeleteSoftDocs(String containerName, String filename)
        {
            for (int idx = 0; ; idx++)
                if (!await StorageFunctions.DeleteSoftFile(containerName, $"{filename}|{idx:D02}", "jpg"))
                    break;
        }

        // Messages
        public static async Task<int> SendRequestMessage(InvestmentBoardResponse boardResponse, ILogger logger = null)
        {
            String body = ", tu inversión requiere actualizar algunos datos.";

            String parameter = $"{boardResponse.InvestmentId}";

            return await FirebaseHelper.SendMessage(boardResponse.AppUserId, "Investment", boardResponse.InvestmentId, "Inversión", body, "Investment", "Request", parameter, 1, logger);
        }

        public static async Task<int> SendUpdateMessage(int investmentId, ILogger logger = null)
        {
            (int appUserId, int boardUserId) = await new InvestmentDB().GetUsersIdById(investmentId);

            (String firstName1, String _1, String lastName1, String _2) = await new IdentityDB().GetFullNameByAppUserId(appUserId);
            String body = $", {firstName1} {lastName1} ha actualizado sus datos.";

            String parameter = appUserId.ToString();

            return await FirebaseHelper.SendMessage(boardUserId, "Investment", investmentId, "Inversión", body, "Investment", "Update", parameter, 0, logger);
        }

        public static async Task<int> SendInvestmentMessage(InvestmentBoardResponse boardResponse, int investmentStatusId, ILogger logger = null)
        {
            String body;
            if (investmentStatusId == 1)
            {
                body = ", ¡felicidades! tu inversión fue aceptada.";
            }
            else if (investmentStatusId == 11)
            {
                body = ", tu inversión fue rechazada.";
            }
            else
                return 3;

            String parameter = $"{boardResponse.InvestmentId}";

            return await FirebaseHelper.SendMessage(boardResponse.AppUserId, "Investment", boardResponse.InvestmentId, "Inversión", body, "Investment", "Finalize", parameter, 1, logger);
        }

        // DELETE
        public static async Task DeleteByAppUserId(int appUserId)
        {
            List<int> ids = await GetIdsByAppUserId(appUserId);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    int projectId = await GetProjectIdById(ids[i]);
                    int cpiCount = await GetCpiCountById(ids[i]);
                    await EconomicsFunctions.DeleteByInvestmentId(ids[i]);
                    await IncomeFunctions.DeleteByInvestmentId(ids[i]);
                    await InvestmentFractionatedFunctions.DeleteByInvestmentId(ids[i], projectId, cpiCount);
                    await InvestmentFinancedFunctions.DeleteByInvestmentId(ids[i], projectId, cpiCount);
                    await InvestmentPrepaidFunctions.DeleteByInvestmentId(ids[i], projectId, cpiCount);
                    await InvestmentIdentityFunctions.DeleteByInvestmentId(ids[i]);
                    await InvestmentInstallmentFunctions.DeleteByInvestmentId(ids[i]);
                    await InvestmentPaymentFunctions.DeleteByInvestmentId(ids[i]);
                    await InvestmentReferenceFunctions.DeleteByInvestmentId(ids[i]);
                }

                scope.Complete();
            }

            await new InvestmentDB().DeleteByAppUserId(appUserId);
        }
    }
}
