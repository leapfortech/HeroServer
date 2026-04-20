using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class ServiceWishFunctions
    {
        // GET
        public static async Task<List<ServiceWish>> GetAllByStatus(int status)
        {
            return await new ServiceWishDB().GetAllByStatus(status);
        }

        public static async Task<ServiceWish> GetById(long id)
        {
            return await new ServiceWishDB().GetById(id);
        }

        // REGISTER
        public static async Task<long> Register(ServiceWish serviceWish)
        {
            serviceWish.Status = 1;
            return await Add(serviceWish);
        }

        // ADD
        public static async Task<long> Add(ServiceWish serviceWish)
        {
            return await new ServiceWishDB().Add(serviceWish);
        }

        // UPDATE
        public static async Task<bool> Update(ServiceWish serviceWish)
        {
            return await new ServiceWishDB().Update(serviceWish);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new ServiceWishDB().UpdateStatus(id, status);
        }

        // DELETE
        public static async Task DeleteById(long id)
        {
            await new ServiceWishDB().DeleteById(id);
        }
    }
}