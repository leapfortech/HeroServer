using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

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

        public static async Task<NewsFull> GetFullById(long id, long likeAppUserId)
        {
            NewsFull newsFull = await new NewsDB().GetFullById(id, likeAppUserId);

            if (newsFull == null)
                return null;

            newsFull.Images = await PostFunctions.GetImagesById(newsFull.PostId, true);

            return newsFull;
        }

        public static async Task<NewsFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            NewsFull newsFull = await new NewsDB().GetFullByPostId(postId, likeAppUserId);

            if (newsFull == null)
                return null;

            newsFull.Images = await PostFunctions.GetImagesById(newsFull.PostId, true);

            return newsFull;
        }

        public static async Task<List<NewsFull>> GetFullsByStatus(int status)
        {
            NewsDataFull newsDataFull = await new NewsDB().GetDataFullByStatus(status);

            return await GetFulls(newsDataFull);
        }

        public static async Task<List<NewsFull>> GetFulls(NewsDataFull newsDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in newsDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in newsDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in newsDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // NewsFull
            List<NewsFull> newsFulls = [];
            foreach (NewsFull newsFull in newsDataFull.NewsFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(newsFull.PostId, out ContactFull contact))
                    contact = null;

                newsFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(newsFull.PostId, out List<LinkFull> links))
                    links = [];

                newsFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(newsFull.PostId, out List<CommentFull> comments))
                    comments = [];

                newsFull.CommentFulls = comments;

                // Images
                newsFull.Images = await PostFunctions.GetImagesById(newsFull.PostId, true);

                newsFulls.Add(newsFull);
            }

            return newsFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterNewsRequest registerNewsRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerNewsRequest.Post.PostTypeId = (long)PostType.News;
                registerNewsRequest.News.PostId = await PostFunctions.Register(registerNewsRequest);

                registerNewsRequest.News.Status = 1;
                id = await Add(registerNewsRequest.News);

                scope.Complete();
            }

            return id;
        }

        // ADD
        public static async Task<long> Add(News news)
        {
            return await new NewsDB().Add(news);
        }

        // UPDATE
        public static async Task<bool> Update(RegisterNewsRequest registerNewsRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerNewsRequest);

                // Update News
                // Soft Delete
                await new NewsDB().UpdateStatusByPostId(registerNewsRequest.Post.Id, 1, 0);

                registerNewsRequest.News.PostId = registerNewsRequest.Post.Id;
                registerNewsRequest.News.Status = 1;

                if (registerNewsRequest.News.Id == -1 || registerNewsRequest.News.Id == 0)
                {
                    await Add(registerNewsRequest.News);
                }
                else
                {
                    await Update(registerNewsRequest.News);
                    await UpdateStatus(registerNewsRequest.News.Id, 1);
                }

                scope.Complete();
                return true;
            }
        }

        public static async Task<bool> Accept(long postId, long newsId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 1);
                bool newsOk = await UpdateStatus(newsId, 1);

                if (!postOk || !newsOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long newsId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 4);
                bool newsOk = await UpdateStatus(newsId, 4);

                if (!postOk || !newsOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(News news)
        {
            return await new NewsDB().Update(news);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new NewsDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            return await new NewsDB().UpdateStatusByPostId(postId, curStatus, newStatus);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new NewsDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            await new NewsDB().DeleteByPostId(postId);
        }
    }
}