using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class LinkFunctions
    {
        // GET
        public static async Task<List<Link>> GetAll()
        {
            return await new LinkDB().GetAll();
        }

        public static async Task<Link> GetById(long id)
        {
            return await new LinkDB().GetById(id);
        }

        // REGISTER
        public static async Task<List<long>> Register(long postId, List<Link> links)
        {
            List<long> ids = [];

            for (int i = 0; i < links.Count; i++)
            {
                links[i].PostId = postId;
                links[i].Status = 1;

                long id = await Add(links[i]);
                ids.Add(id);
            }

            return ids;
        }

        // ADD
        public static async Task<long> Add(Link link)
        {
            return await new LinkDB().Add(link);
        }

        // UPDATE
        public static async Task<bool> Update(Link link)
        {
            return await new LinkDB().Update(link);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new LinkDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new LinkDB().DeleteById(id);
        }
    }
}