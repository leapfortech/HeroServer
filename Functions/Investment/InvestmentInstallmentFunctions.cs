using System;
using System.Collections.Generic;
using System.Transactions;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class InvestmentInstallmentFunctions
    {
        // ALL
        public static async Task<List<InvestmentInstallment>> GetByInvestmentId(int investmentId)
        {
            return await new InvestmentInstallmentDB().GetByInvestmentId(investmentId);
        }

        // GET
        public static async Task<InvestmentInstallment> GetById(int id)
        {
            return await new InvestmentInstallmentDB().GetById(id);
        }

        public static async Task<List<int>> GetIdsByInvestmentId(int investmentId)
        {
            return await new InvestmentInstallmentDB().GetIdsByInvestmentId(investmentId);
        }

        public static async Task<InvestmentInstallment> GetCurrentByInvestmentId(int investmentId)
        {
            return await new InvestmentInstallmentDB().GetCurrentByInvestmentId(investmentId);
        }

        public static async Task<(int, double)> GetIdBalanceByInvestmentId(int investmentId)
        {
            return await new InvestmentInstallmentDB().GetIdBalanceByInvestmentId(investmentId);
        }

        // ADD
        public static async Task Register(Investment investment, InvestmentAmounts amounts)
        {
            InvestmentInstallment investmentInstallment;

            if (investment.ProductTypeId == 1)   // Fractionated
            {
                double discountMonthRate = investment.DiscountRate / 12d;

                double fullDiscount = amounts.ReserveDiscount;
                for (int i = 0; i < amounts.FullTerm; i++)
                {
                    double installmentDiscount = (amounts.InstallmentAmount * discountMonthRate * (investment.DevelopmentTerm - 1 - i)).RoundAmount();
                    fullDiscount += installmentDiscount;
                    investmentInstallment = new InvestmentInstallment(-1, investment.Id, 1, amounts.InstallmentAmount, installmentDiscount, investment.EffectiveDate.AddMonths(i),
                                                                          investment.EffectiveDate.AddMonths(i + 1).AddDays(-1), amounts.InstallmentAmount, null, DateTime.Now, DateTime.Now, 1);
                    await Add(investmentInstallment);
                }

                double lastDiscount = investment.DiscountAmount - fullDiscount;
                investmentInstallment = new InvestmentInstallment(-1, investment.Id, 1, amounts.LastAmount, lastDiscount, investment.EffectiveDate.AddMonths(amounts.FullTerm),
                                                                      investment.EffectiveDate.AddMonths(amounts.FullTerm + 1).AddDays(-1), amounts.LastAmount, null, DateTime.Now, DateTime.Now, 1);
                await Add(investmentInstallment);

                return;
            }
            
            if (investment.ProductTypeId == 2)   // Financed
            {
                double installmentAmount = ((investment.DueAmount - investment.ReserveAmount) / investment.DevelopmentTerm).RoundAmount();

                for (int i = 0; i < investment.DevelopmentTerm - 1; i++)
                {
                    investmentInstallment = new InvestmentInstallment(-1, investment.Id, 2, installmentAmount, 0d, investment.EffectiveDate.AddMonths(i), investment.EffectiveDate.AddMonths(i + 1).AddDays(-1),
                                                                          installmentAmount, null, DateTime.Now, DateTime.Now, 1);
                    await Add(investmentInstallment);
                }

                double lastInstallmentAmount = investment.DueAmount - investment.ReserveAmount - installmentAmount * (investment.DevelopmentTerm - 1);
                investmentInstallment = new InvestmentInstallment(-1, investment.Id, 2, lastInstallmentAmount, 0d, investment.EffectiveDate.AddMonths(investment.DevelopmentTerm - 1),
                                                                      investment.EffectiveDate.AddMonths(investment.DevelopmentTerm - 1 + 1).AddDays(-1), lastInstallmentAmount, null, DateTime.Now, DateTime.Now, 1);
                await Add(investmentInstallment);

                return;
            }
            
            if (investment.ProductTypeId == 3)   // Prepaid
            {
                double installmentAmount = investment.DueAmount - investment.ReserveAmount;

                investmentInstallment = new InvestmentInstallment(-1, investment.Id, 3, installmentAmount, investment.DiscountAmount, investment.EffectiveDate, investment.EffectiveDate.AddMonths(1).AddDays(-1),
                                                                      installmentAmount, null, DateTime.Now, DateTime.Now, 1);
                await Add(investmentInstallment);

                return;
            }

            throw new Exception($"Installment Register ProductType #{investment.ProductTypeId} on Investment #{investment.Id}");
        }

        public static double RoundAmount(this double amount) => Math.Round(amount, 2); // Math.Floor(amount * 100d + 0.5d) / 100d;

        public static async Task RegisterReserve(Investment investment, double discountAmount)
        {
            // RM Review DueDate / JAD
            await new InvestmentInstallmentDB().Add(new InvestmentInstallment(-1, investment.Id, 0, investment.ReserveAmount, discountAmount, DateTime.Today, DateTime.Today.AddDays(5),
                                                                                  investment.ReserveAmount, null, DateTime.Now, DateTime.Now, 1));
        }

        public static async Task<int> Add(InvestmentInstallment investmentInstallment)
        {
            return await new InvestmentInstallmentDB().Add(investmentInstallment);
        }

        // UPDATE
        public static async Task<bool> Update(InvestmentInstallment investmentInstallment)
        {
            return await new InvestmentInstallmentDB().Update(investmentInstallment);
        }

        public static async Task<bool> UpdateBalance(int id, double balance)
        {
            int status = balance == 0.0d ? 2 : 3;
            return await new InvestmentInstallmentDB().UpdateBalance(id, balance, balance == 0d ? DateTime.Now : null, status);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentInstallmentDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByInvestmentId(int investmentId, int curStatus, int newStatus)
        {
            return await new InvestmentInstallmentDB().UpdateStatusByInvestmentId(investmentId, curStatus, newStatus);
        }

        //DELETE
        public static async Task DeleteByInvestmentId(int investmentId)
        {
            List<int> ids = await GetIdsByInvestmentId(investmentId);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                for (int i = 0; i < ids.Count; i++)
                    await InvestmentInstallmentPaymentFunctions.DeleteByInstallmentId(ids[i]);
                await new InvestmentInstallmentDB().DeleteByInvestmentId(investmentId);

                scope.Complete();
            }
        }
    }
}
