using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class TaleFunctions
    {
        // GET
        public static async Task<List<Tale>> GetAllByStatus(int status)
        {
            return await new TaleDB().GetAllByStatus(status);
        }

        public static async Task<Tale> GetById(long id)
        {
            return await new TaleDB().GetById(id);
        }

        public static async Task<TaleFull> GetFullById(long id)
        {
            return await new TaleDB().GetFullById(id);
        }

        public static async Task<TaleFull> GetFullByPostId(long postId)
        {
            return await new TaleDB().GetFullByPostId(postId);
        }

        // REGISTER
        public static async Task<long> Register(Tale tale)
        {
            tale.Status = 1;

            return await Add(tale);
        }

        // ADD
        public static async Task<long> Add(Tale tale)
        {
            return await new TaleDB().Add(tale);
        }

        // UPDATE
        public static async Task<bool> Update(Tale tale)
        {
            return await new TaleDB().Update(tale);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new TaleDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new TaleDB().DeleteById(id);
        }
    }
}