using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProjectLikeFunctions
    {
        // GET
        public static async Task<ProjectLike> GetById(int id)
        {
            return await new ProjectLikeDB().GetById(id);
        }

        public static async Task<IEnumerable<ProjectLike>> GetByProjectId(int projectId, int status = 1)
        {
            return await new ProjectLikeDB().GetByProjectId(projectId, status);
        }

        public static async Task<List<int>> GetIdsByAppUserId(int appUserId, int status = 1)
        {
            return await new ProjectLikeDB().GetIdsByAppUserId(appUserId, status);
        }

        // ADD
        public static async Task<int> Register(ProjectLike projectLike)
        {
            ProjectLike proLike = await new ProjectLikeDB().GetByIds(projectLike.ProjectId, projectLike.AppUserId);

            if (proLike != null)
                throw new Exception("El proyecto ya se encuentra dentro de tus favoritos.");

            projectLike.Status = 1;
            return await Add(projectLike);
        }

        public static async Task<int> Add(ProjectLike projectLike)
        {
            return await new ProjectLikeDB().Add(projectLike);
        }

        // UPDATE
        public static async Task<bool> Update(ProjectLike projectLike)
        {
            return await new ProjectLikeDB().Update(projectLike);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new ProjectLikeDB().UpdateStatus(id, status);
        }
    }
}
