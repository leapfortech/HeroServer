using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class NewsFunctions
    {
        // GET
        public static async Task<List<News>> GetAllByStatus(int status)
        {
            return await new NewsDB().GetAllByStatus(status);
        }

        public static async Task<News> GetById(long id)
        {
            return await new NewsDB().GetById(id);
        }

        public static async Task<NewsFull> GetFullById(long id)
        {
            return await new NewsDB().GetFullById(id);
        }

        public static async Task<NewsFull> GetFullByPostId(long postId)
        {
            return await new NewsDB().GetFullByPostId(postId);
        }

        // REGISTER
        public static async Task<long> Register(News news)
        {
            news.Status = 1;

            return await Add(news);
        }

        // ADD
        public static async Task<long> Add(News news)
        {
            return await new NewsDB().Add(news);
        }

        // UPDATE
        public static async Task<bool> Update(News news)
        {
            return await new NewsDB().Update(news);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new NewsDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new NewsDB().DeleteById(id);
        }
    }
}