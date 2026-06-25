using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

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

        public static async Task<TaleFull> GetFullById(long id, long likeAppUserId)
        {
            TaleFull taleFull = await new TaleDB().GetFullById(id, likeAppUserId);

            if (taleFull == null)
                return null;

            taleFull.Images = await PostFunctions.GetImagesById(taleFull.PostId, true);

            return taleFull;
        }

        public static async Task<TaleFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            TaleFull taleFull = await new TaleDB().GetFullByPostId(postId, likeAppUserId);

            if (taleFull == null)
                return null;

            taleFull.Images = await PostFunctions.GetImagesById(postId, true);

            return taleFull;
        }

        public static async Task<List<TaleFull>> GetFullsByStatus(int status)
        {
            TaleDataFull taleDataFull = await new TaleDB().GetDataFullByStatus(status);

            return await GetFulls(taleDataFull);
        }

        public static async Task<List<TaleFull>> GetFulls(TaleDataFull taleDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in taleDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in taleDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in taleDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // TaleFull
            List<TaleFull> taleFulls = [];
            foreach (TaleFull taleFull in taleDataFull.TaleFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(taleFull.PostId, out ContactFull contact))
                    contact = null;

                taleFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(taleFull.PostId, out List<LinkFull> links))
                    links = [];

                taleFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(taleFull.PostId, out List<CommentFull> comments))
                    comments = [];

                taleFull.CommentFulls = comments;

                // Images
                taleFull.Images = await PostFunctions.GetImagesById(taleFull.PostId, true);

                taleFulls.Add(taleFull);
            }

            return taleFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterTaleRequest registerTaleRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerTaleRequest.Post.PostTypeId = (long)PostType.Tale;
                registerTaleRequest.Post.Id = await PostFunctions.Register(registerTaleRequest);

                if (registerTaleRequest.Tale == null)
                {
                    registerTaleRequest.Tale = new Tale(-1, registerTaleRequest.Post.Id, DateTime.Now, DateTime.Now, 1);
                }
                else
                {
                    registerTaleRequest.Tale.PostId = registerTaleRequest.Post.Id;
                    registerTaleRequest.Tale.Status = 1;
                }

                id = await Add(registerTaleRequest.Tale);

                scope.Complete();
            }

            return id;
        }

        // ADD
        public static async Task<long> Add(Tale tale)
        {
            return await new TaleDB().Add(tale);
        }

        // UPDATE
        public static async Task<bool> Update(RegisterTaleRequest registerTaleRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerTaleRequest);

                // Update Tale
                // Soft Delete
                await new TaleDB().UpdateStatusByPostId(registerTaleRequest.Post.Id, 1, 0);

                if (registerTaleRequest.Tale == null)
                {
                    registerTaleRequest.Tale = new Tale(-1, registerTaleRequest.Post.Id, DateTime.Now, DateTime.Now, 1);

                    await Add(registerTaleRequest.Tale);
                }
                else
                {
                    registerTaleRequest.Tale.PostId = registerTaleRequest.Post.Id;
                    registerTaleRequest.Tale.Status = 1;

                    if (registerTaleRequest.Tale.Id == -1 || registerTaleRequest.Tale.Id == 0)
                    {
                        await Add(registerTaleRequest.Tale);
                    }
                    else
                    {
                        await Update(registerTaleRequest.Tale);
                        await UpdateStatus(registerTaleRequest.Tale.Id, 1);
                    }
                }

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Accept(long postId, long taleId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 3);
                bool taleOk = await UpdateStatus(taleId, 3);

                if (!postOk || !taleOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long taleId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 0);
                bool taleOk = await UpdateStatus(taleId, 0);

                if (!postOk || !taleOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Tale tale)
        {
            return await new TaleDB().Update(tale);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new TaleDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            return await new TaleDB().UpdateStatusByPostId(postId, curStatus, newStatus);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new TaleDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            await new TaleDB().DeleteByPostId(postId);
        }
    }
}