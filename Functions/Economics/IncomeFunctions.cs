using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class IncomeFunctions
    {
        // GET
        public static async Task<Income> GetById(int id)
        {
            return await new IncomeDB().GetById(id);
        }

        public static async Task<List<Income>> GetByInvestmentId(int investmentId)
        {
            return await new IncomeDB().GetByInvestmentId(investmentId, 1);
        }

        // ADD
        public static async Task<int> Add(Income income)
        {
            return await new IncomeDB().Add(income);
        }

        // UPDATE
        public static async Task<bool> Update(Income income)
        {
            return await new IncomeDB().Update(income);
        }

        public static async Task<bool> UpdateStatus(int id, int newStatus)
        {
            return await new IncomeDB().UpdateStatus(id, newStatus);
        }

        public static async Task<bool> UpdateStatusByInvestmentId(int investmentId, int curStatus, int newStatus)
        {
            return await new IncomeDB().UpdateStatusByInvestmentId(investmentId, curStatus, newStatus);
        }

        //DELETE
        public static async Task DeleteByInvestmentId(int investmentId)
        {
            await new IncomeDB().DeleteByInvestmentId(investmentId);
        }
    }
}
