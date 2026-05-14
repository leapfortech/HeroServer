using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class FaqFunctions
    {
        // GET
        public static async Task<Faq> GetById(long id)
        {
            return await new FaqDB().GetById(id);
        }

        public static async Task<List<Faq>> GetAllByType(long faqTypeId)
        {
            return await new FaqDB().GetAllByType(faqTypeId, 1);
        }

        // REGISTER
        public static async Task<long> Register(Faq faq)
        {
            faq.Status = 1;
            return await Add(faq);
        }

        // ADD
        public static async Task<long> Add(Faq faq)
        {
            return await new FaqDB().Add(faq);
        }

        // UPDATE
        public static async Task<bool> Update(Faq faq)
        {
            return await new FaqDB().Update(faq);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new FaqDB().UpdateStatus(id, status);
        }

        // DELETE
        public static async Task DeleteById(long id)
        {
            await new FaqDB().DeleteById(id);
        }
    }
}