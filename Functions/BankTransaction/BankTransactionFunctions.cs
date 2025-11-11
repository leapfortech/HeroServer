using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class BankTransactionFunctions
    {
        // GET
        public static async Task<BankTransaction> GetById(int id)
        {
            return await new BankTransactionDB().GetById(id);
        }

        public static async Task<List<BankTransaction>> GetByAppUserId(int appUserId, int status = 1)
        {
            return await new BankTransactionDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<List<int>> GetIdsByAppUserId(int appUserId, int status = 1)
        {
            return await new BankTransactionDB().GetIdsByAppUserId(appUserId, status);
        }

        //public static async Task<List<BankTransactionNamed>> GetByStatus(int status)
        //{
        //    return await new BankTransactionDB().GetByStatus(status);
        //}

        public static async Task<String> GetReceipt(int id)
        {
            int appUserId = await new BankTransactionDB().GetAppUserId(id);
            byte[] image = await StorageFunctions.ReadFile("user" + appUserId.ToString("D08"), "bnkrc" + id.ToString("D08"), "jpg");
            return image == null ? throw new Exception("No existe la foto del recibo.") : Convert.ToBase64String(image);
        }

        // ADD
        public static async Task<int> Add(BankTransaction bankTransaction)
        {
            return await new BankTransactionDB().Add(bankTransaction);
        }

        // UPDATE
        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new BankTransactionDB().UpdateStatus(id, status);
        }

        // DELETE
        public static async Task DeleteById(int id)
        {
            await new BankTransactionDB().DeleteById(id);
        }

        public static async Task DeleteByAppUserId(int appUserId)
        {
            //List<int> ids = await GetIdsByAppUserId(appUserId);
            await new BankTransactionDB().DeleteByAppUserId(appUserId);
        }
    }
}