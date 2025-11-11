using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class ProductPrepaidFunctions
    {
        // GET
        public static async Task<IEnumerable<ProductPrepaid>> GetAllByStatus(int status)
        {
            return await new ProductPrepaidDB().GetAllByStatus(status);
        }

        public static async Task<ProductPrepaid> GetById(int id)
        {
            return await new ProductPrepaidDB().GetById(id);
        }

        public static async Task<ProductPrepaid> GetByProjectId(int projectId, int status = 1)
        {
            return await new ProductPrepaidDB().GetByProjectId(projectId, status);
        }

        public static async Task<int> GetIdByProjectId(int projectId)
        {
            return await new ProductPrepaidDB().GetIdByProjectId(projectId);
        }

        public static async Task<double> GetReserveRateByProjectId(int projectId)
        {
            return await new ProductPrepaidDB().GetReserveRateByProjectId(projectId);
        }

        public static async Task<int> GetCpiMinByProjectId(int projectId, int defaultMin = -1)
        {
            return await new ProductPrepaidDB().GetCpiMinByProjectId(projectId, defaultMin);
        }

        // REGISTER
        public static async Task<int> Register(ProductPrepaid productPrepaid)
        {
            productPrepaid.ProductPrepaidStatusId = 1;

            return await Add(productPrepaid);
        }

        // ADD
        public static async Task<int> Add(ProductPrepaid productPrepaid)
        {
            return await new ProductPrepaidDB().Add(productPrepaid);
        }

        // UPDATE
        public static async Task<bool> Update(ProductPrepaid productPrepaid)
        {
            return await new ProductPrepaidDB().Update(productPrepaid);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new ProductPrepaidDB().UpdateStatus(id, status);
        }
    }
}
