using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class NewsFunctions
    {
        // GET
        public static async Task<News> GetById(int id)
        {
            return await new NewsDB().GetById(id);
        }


        public static async Task<List<NewsInfo>> GetByStatus(int status)
        {
            List<News> news = await new NewsDB().GetByStatus(status);

            List<NewsInfo> newsInfos = [];

            for (int i = 0; i < news.Count; i++)
            {
                byte[] newsImg = await StorageFunctions.ReadFile("news", $"news{news[i].Id:D08}", "jpg");
                newsInfos.Add(new NewsInfo(news[i], news == null ? null : Convert.ToBase64String(newsImg)));
            }

            return newsInfos;
        }


        // Register
        public static async Task<int> Register(NewsInfo newsInfo)
        {
            int newsId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                newsId = await Add(newsInfo.News);

                if (newsInfo.Image != null && newsInfo.Image.Length > 0)
                    await StorageFunctions.UpdateCFile("news", $"news{newsId:D08}", "jpg", Convert.FromBase64String(newsInfo.Image));
                
                scope.Complete();
            }

            return newsId;
        }
                    

        // ADD
        public static async Task<int> Add(News news)
        {
            return await new NewsDB().Add(news);
        }

        // UPDATE
        public static async Task<bool> Update(News news)
        {
            return await new NewsDB().Update(news);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new NewsDB().UpdateStatus(id, status);
        }
    }
}
