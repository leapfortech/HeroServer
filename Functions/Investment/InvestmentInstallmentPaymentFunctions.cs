using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class InvestmentInstallmentPaymentFunctions
    {
        // GET
        public static async Task<InvestmentInstallmentPayment> GetById(int id)
        {
            return await new InvestmentInstallmentPaymentDB().GetById(id);
        }

        // GET ALL

        public static async Task<IEnumerable<InvestmentInstallmentPayment>> GetByInvestmentPaymentId(int investmentPaymentId)
        {
            return await new InvestmentInstallmentPaymentDB().GetByInvestmentPaymentId(investmentPaymentId);
        }

        public static async Task<IEnumerable<InvestmentInstallmentPayment>> GetByInvestmentInstallmentIdId(int investmentInstallmentIdId)
        {
            return await new InvestmentInstallmentPaymentDB().GetByInvestmentInstallmentId(investmentInstallmentIdId);
        }


        // ADD
        public static async Task<int> Register(InvestmentInstallmentPayment investmentInstallmentPayment)
        {
            return await Add(investmentInstallmentPayment);
        }

        public static async Task<int> Add(InvestmentInstallmentPayment investmentInstallmentPayment)
        {
            return await new InvestmentInstallmentPaymentDB().Add(investmentInstallmentPayment);
        }

        // UPDATE
        public static async Task<bool> Update(InvestmentInstallmentPayment investmentInstallmentPayment)
        {
            return await new InvestmentInstallmentPaymentDB().Update(investmentInstallmentPayment);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentInstallmentPaymentDB().UpdateStatus(id, status);
        }

        //DELETE
        public static async Task DeleteByInstallmentId(int installmentId)
        {
            await new InvestmentInstallmentPaymentDB().DeleteByInstallmentId(installmentId);
        }
    }
}
