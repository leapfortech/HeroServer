using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class HappeningFunctions
    {
        // GET
        public static async Task<List<Happening>> GetAllByStatus(int status)
        {
            return await new HappeningDB().GetAllByStatus(status);
        }

        public static async Task<Happening> GetById(long id)
        {
            return await new HappeningDB().GetById(id);
        }

        public static async Task<HappeningFull> GetFullById(long id)
        {
            return await new HappeningDB().GetFullById(id);
        }

        public static async Task<HappeningFull> GetFullByPostId(long postId)
        {
            return await new HappeningDB().GetFullByPostId(postId);
        }

        // REGISTER
        public static async Task<long> Register(Happening happening)
        {
            happening.Status = 1;

            return await Add(happening);
        }

        // ADD
        public static async Task<long> Add(Happening happening)
        {
            return await new HappeningDB().Add(happening);
        }

        // UPDATE
        public static async Task<bool> Update(Happening happening)
        {
            return await new HappeningDB().Update(happening);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new HappeningDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new HappeningDB().DeleteById(id);
        }
    }
}