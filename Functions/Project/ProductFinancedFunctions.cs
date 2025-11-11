using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class ProductFinancedFunctions
    {
        // GET
        public static async Task<IEnumerable<ProductFinanced>> GetAllByStatus(int status)
        {
            return await new ProductFinancedDB().GetAllByStatus(status);
        }

        public static async Task<ProductFinanced> GetById(int id)
        {
            return await new ProductFinancedDB().GetById(id);
        }

        public static async Task<ProductFinanced> GetByProjectId(int projectId, int status = 1)
        {
            return await new ProductFinancedDB().GetByProjectId(projectId, status);
        }

        public static async Task<int> GetIdByProjectId(int projectId)
        {
            return await new ProductFinancedDB().GetIdByProjectId(projectId);
        }

        public static async Task<(double, double)> GetRatesByProjectId(int projectId)
        {
            return await new ProductFinancedDB().GetRatesByProjectId(projectId);
        }

        public static async Task<int> GetCpiMinByProjectId(int projectId, int defaultMin = -1)
        {
            return await new ProductFinancedDB().GetCpiMinByProjectId(projectId, defaultMin);
        }

        // REGISTER
        public static async Task<int> Register(ProductFinanced productFinanced)
        {
            productFinanced.ProductFinancedStatusId = 1;

            return await Add(productFinanced);
        }

        // ADD
        public static async Task<int> Add(ProductFinanced productFinanced)
        {
            return await new ProductFinancedDB().Add(productFinanced);
        }

        // UPDATE
        public static async Task<bool> Update(ProductFinanced productFinanced)
        {
            return await new ProductFinancedDB().Update(productFinanced);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new ProductFinancedDB().UpdateStatus(id, status);
        }
    }
}
