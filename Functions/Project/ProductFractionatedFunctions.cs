using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class ProductFractionatedFunctions
    {
        // GET
        public static async Task<IEnumerable<ProductFractionated>> GetAllByStatus(int status)
        {
            return await new ProductFractionatedDB().GetAllByStatus(status);
        }

        public static async Task<ProductFractionated> GetById(int id)
        {
            return await new ProductFractionatedDB().GetById(id);
        }

        public static async Task<ProductFractionated> GetByProjectId(int projectId, int status = 1)
        {
            return await new ProductFractionatedDB().GetByProjectId(projectId, status);
        }

        public static async Task<int> GetIdByProjectId(int projectId)
        {
            return await new ProductFractionatedDB().GetIdByProjectId(projectId);
        }

        public static async Task<double> GetReserveRateByProjectId(int projectId)
        {
            return await new ProductFractionatedDB().GetReserveRateByProjectId(projectId);
        }

        public static async Task<int> GetCpiMinByProjectId(int projectId, int defaultMin = -1)
        {
            return await new ProductFractionatedDB().GetCpiMinByProjectId(projectId, defaultMin);
        }

        // REGISTER
        public static async Task<int> Register(ProductFractionated productFractionated)
        {
            productFractionated.ProductFractionatedStatusId = 1;

            return await Add(productFractionated);
        }

        // ADD
        public static async Task<int> Add(ProductFractionated productFractionated)
        {
            return await new ProductFractionatedDB().Add(productFractionated);
        }

        // UPDATE
        public static async Task<bool> Update(ProductFractionated productFractionated)
        {
            return await new ProductFractionatedDB().Update(productFractionated);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new ProductFractionatedDB().UpdateStatus(id, status);
        }
    }
}
