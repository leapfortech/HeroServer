using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class ProjectDescriptionFunctions
    {
        // GET
        public static async Task<ProjectInformation> GetById(int id)
        {
            return await new ProjectInformationDB().GetById(id);
        }

        public static async Task<IEnumerable<ProjectInformation>> GetByProjectId(int projectId, int status = 1)
        {
            return await new ProjectInformationDB().GetByProjectId(projectId, status);
        }

        // ADD
        public static async Task<List<int>> Register(int projectId, ProjectInformation[] projectInformations)
        {
            List<int> projectInformationIds = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int projectInformationId;
                for (int i = 0; i < projectInformations.Length; i++)
                {
                    projectInformations[i].ProjectId = projectId;
                    projectInformations[i].Status = 1;
                    projectInformationId = await Add(projectInformations[i]);
                    projectInformationIds.Add(projectInformationId);
                }

                scope.Complete();
            }

            return projectInformationIds;
        }

        public static async Task<int> Add(ProjectInformation projectInformation)
        {
            return await new ProjectInformationDB().Add(projectInformation);
        }

        // UPDATE
        public static async Task<bool> Update(ProjectInformation projectInformation)
        {
            return await new ProjectInformationDB().Update(projectInformation);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new ProjectInformationDB().UpdateStatus(id, status);
        }
    }
}
