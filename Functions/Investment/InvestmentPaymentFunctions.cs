using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;

namespace HeroServer
{
    public static class InvestmentPaymentFunctions
    {
        static double ExchangeRate = 7.90d;

        public static async void Initialize()
        {
            ExchangeRate = Convert.ToDouble(await new SystemParamDB().GetValue("ExchangeRate"));
        }

        // GET
        public static async Task<InvestmentPayment> GetById(int id)
        {
            return await new InvestmentPaymentDB().GetById(id);
        }

        public static async Task<IEnumerable<InvestmentPayment>> GetByInvestmentId(int investmentId, int status = 1)
        {
            return await new InvestmentPaymentDB().GetByInvestmentId(investmentId, status);
        }

        public static async Task<IEnumerable<InvestmentPayment>> GetByAppUserId(int appUserId, int status = 1)
        {
            return await new InvestmentPaymentDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<List<InvestmentPaymentBankFull>> GetBankFullsByStatus(int status)
        {
            List<InvestmentPaymentBankFull> bankPaymentFulls = await new InvestmentPaymentDB().GetBankFullsByStatus(status);

            for (int i = 0; i < bankPaymentFulls.Count; i++)
                if (bankPaymentFulls[i].TransactionTypeId == 2)
                    bankPaymentFulls[i].Receipt = await BankTransactionFunctions.GetReceipt(bankPaymentFulls[i].TransactionId);

            return bankPaymentFulls;
        }

        // PAYMENT
        public static async Task<InvestmentBankPayment> PaymentBank(InvestmentBankPayment payment)
        {
            InvestmentPayment investmentPayment = null;
            BankTransaction bankTransaction = new BankTransaction(payment, 2);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bankTransaction.Id = await new BankTransactionDB().Add(bankTransaction);

                if (bankTransaction.TransactionTypeId == 2)  // Depósito
                {
                    String containerName = "user" + bankTransaction.AppUserId.ToString("D08");
                    await StorageFunctions.CreateFile(containerName, "bnkrc" + bankTransaction.Id.ToString("D08"), "jpg", Convert.FromBase64String(payment.Receipt));
                }

                investmentPayment = new InvestmentPayment(-1, payment.InvestmentPayment.InvestmentId, payment.InvestmentPayment.AppUserId,
                                                              payment.InvestmentPayment.InvestmentPaymentTypeId, payment.InvestmentPayment.TransactionTypeId,
                                                              bankTransaction.Id, 2);

                investmentPayment.Id = await new InvestmentPaymentDB().Add(investmentPayment);

                if (payment.InvestmentPayment.InvestmentPaymentTypeId == 0) // Reserva
                    await InvestmentFunctions.UpdateStatus(payment.InvestmentPayment.InvestmentId, 2);       

                scope.Complete();
            }

            return investmentPayment == null ? null : new InvestmentBankPayment(investmentPayment, bankTransaction, null);
        }

        public static async Task<InvestmentCardPayment> PaymentCard(InvestmentCardPayment payment)
        {
            InvestmentPayment investmentPayment = null;
            CardTransaction cardTransaction = new CardTransaction(payment, 1);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                cardTransaction.Id = await new CardTransactionDB().Add(cardTransaction);
                Card card = await CardFunctions.GetById(cardTransaction.CardId);

                investmentPayment = new InvestmentPayment(-1, payment.InvestmentPayment.InvestmentId, payment.InvestmentPayment.AppUserId,
                                                            payment.InvestmentPayment.InvestmentPaymentTypeId, payment.InvestmentPayment.TransactionTypeId,
                                                            cardTransaction.Id, 1);

                investmentPayment.Id = await new InvestmentPaymentDB().Add(investmentPayment);

                if (payment.InvestmentPayment.InvestmentPaymentTypeId == 0) // Reserva
                    await InvestmentFunctions.UpdateStatus(payment.InvestmentPayment.InvestmentId, 2);

                scope.Complete();
            }

            return investmentPayment == null ? null : new InvestmentCardPayment(investmentPayment, cardTransaction);
        }

        // ADD
        public static async Task<List<int>> Register(int investmentId, InvestmentPayment[] investmentPayments)
        {
            List<int> investmentPaymentsIds = [];

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int investmentPaymentId;
                for (int i = 0; i < investmentPayments.Length; i++)
                {
                    investmentPayments[i].InvestmentId = investmentId;
                    investmentPayments[i].Status = 1;
                    investmentPaymentId = await new InvestmentPaymentDB().Add(investmentPayments[i]);
                    investmentPaymentsIds.Add(investmentPaymentId);
                }

                scope.Complete();
            }

            return investmentPaymentsIds;
        }

        // VALIDATION
        public static async Task Authorize(int boardUserId, int investmentPaymentId)
        {
            BankTransaction bankTransaction;
            InvestmentPayment investmentPayment;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                investmentPayment = await new InvestmentPaymentDB().GetById(investmentPaymentId);

                // Transaction
                bankTransaction = await new BankTransactionDB().GetById(investmentPayment.TransactionId);
                bankTransaction.ApprovalCode = $"A-{investmentPayment.Id}-{boardUserId}";
                bankTransaction.Status = 1;
                await new BankTransactionDB().UpdateApprovalCode(bankTransaction.Id, bankTransaction.ApprovalCode, bankTransaction.Status);

                // Payment
                investmentPayment.Status = 1;  // 5
                await UpdateStatus(investmentPayment.Id, investmentPayment.Status);

                int currencyId = await new BankTransactionDB().GetCurrencyId(bankTransaction.AccountHpbId);

                double bankTransactionAmount = currencyId == 47 ? bankTransaction.Amount : Math.Round(bankTransaction.Amount / ExchangeRate, 2);

                // Balance
                for (double remainAmount = bankTransactionAmount; remainAmount > 0d; )
                {
                    InvestmentInstallment investmentInstallment = await InvestmentInstallmentFunctions.GetCurrentByInvestmentId(investmentPayment.InvestmentId);

                    double investmentInstallmentBalance, discount, paidAmount;
                    if (remainAmount <= investmentInstallment.Balance)
                    {
                        investmentInstallmentBalance = investmentInstallment.Balance - remainAmount;
                        discount = investmentInstallment.DiscountAmount * remainAmount / investmentInstallment.Amount;
                        paidAmount = remainAmount;
                        remainAmount = 0d;
                    }
                    else
                    {
                        investmentInstallmentBalance = 0d;
                        discount = investmentInstallment.DiscountAmount * investmentInstallment.Balance / investmentInstallment.Amount;
                        remainAmount -= investmentInstallment.Balance;
                        paidAmount = investmentInstallment.Balance;
                    }

                    await InvestmentInstallmentPaymentFunctions.Register(new InvestmentInstallmentPayment(investmentPayment.Id, investmentInstallment.Id, paidAmount, discount, investmentInstallmentBalance));
                    await InvestmentInstallmentFunctions.UpdateBalance(investmentInstallment.Id, investmentInstallmentBalance);
                }

                double investmentBalance = await InvestmentFunctions.GetBalanceById(investmentPayment.InvestmentId);
                investmentBalance -= bankTransactionAmount;

                if (investmentPayment.InvestmentPaymentTypeId == 0) // Reserva
                    await InvestmentFunctions.UpdateBalanceStatus(investmentPayment.InvestmentId, investmentBalance, 3);
                else
                    await InvestmentFunctions.UpdateBalance(investmentPayment.InvestmentId, investmentBalance);

                scope.Complete();
            }

            await SendTransactionMessage(bankTransaction, investmentPayment);
        }

        public static async Task Reject(int boardUserId, int investmentPaymentId, bool receipt)
        {
            BankTransaction bankTransaction;
            InvestmentPayment investmentPayment;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                investmentPayment = await new InvestmentPaymentDB().GetById(investmentPaymentId);

                bankTransaction = await new BankTransactionDB().GetById(investmentPayment.TransactionId);
                bankTransaction.Status = receipt ? 4 : 3;
                bankTransaction.ApprovalCode = $"R{bankTransaction.Status}-{investmentPayment.Id}-{boardUserId}";
                await new BankTransactionDB().UpdateApprovalCode(bankTransaction.Id, bankTransaction.ApprovalCode, bankTransaction.Status);

                investmentPayment.Status = receipt ? 4 : 3;   // 7 : 6
                await UpdateStatus(investmentPayment.Id, investmentPayment.Status);

                if (investmentPayment.InvestmentPaymentTypeId == 0) // Reserva
                    await InvestmentFunctions.UpdateStatus(investmentPayment.InvestmentId, 0);

                scope.Complete();
            }

            await SendTransactionMessage(bankTransaction, investmentPayment, receipt);
        }

        //public static async Task<InvestmentPayment> Acknowledge(int investmentPaymentId)
        //{
        //    InvestmentPayment investmentPayment = await new InvestmentPaymentDB().GetById(investmentPaymentId);
        //    if (investmentPayment.Status < 5)
        //        return null;

        //    investmentPayment.Status = investmentPayment.Status == 5 ? 1 : investmentPayment.Status == 6 ? 3 : 4;

        //    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        await new InvestmentPaymentDB().UpdateStatus(investmentPaymentId, investmentPayment.Status);

        //        //if (investmentPayment.Status == 1)
        //        //await LoanFunctions.Repay(connString, repayment.LoanId, repayment.Id, repayment.Amount, repayment.NewBalance, null);

        //        scope.Complete();
        //    }

        //    return investmentPayment;
        //}

        // UPDATE
        public static async Task<bool> Update(InvestmentPayment investmentPayment)
        {
            return await new InvestmentPaymentDB().Update(investmentPayment);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new InvestmentPaymentDB().UpdateStatus(id, status);
        }

        //DELETE
        public static async Task DeleteByInvestmentId(int investmentId)
        {
            await new InvestmentPaymentDB().DeleteByInvestmentId(investmentId);
        }

        // MESSAGE
        public static async Task<int> SendTransactionMessage(BankTransaction bankTransaction, InvestmentPayment investmentPayment, bool receipt = false, ILogger logger = null)
        {
            bool valid = bankTransaction.Status == 1;
            bool trsfr = bankTransaction.TransactionTypeId == 1;
            String body = $", tu {(trsfr ? "transferencia" : "depósito")} {bankTransaction.Number} fue {(valid ? "aceptad" : "rechazad")}{(trsfr ? "a" : "o")}.";

            String parameter = $"{investmentPayment.InvestmentId}";

            return await FirebaseHelper.SendMessage(bankTransaction.AppUserId, "InvestmentPayment", investmentPayment.Id, trsfr ? "Transferencia" : "Depósito", body, "InvestmentPayment", valid ? "Accept" : receipt ? "RejectReceipt" : "Reject", parameter, 0, logger);
        }
    }
}
