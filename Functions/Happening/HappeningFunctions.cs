using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

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

        public static async Task<HappeningFull> GetFullById(long id, long likeAppUserId)
        {
            HappeningFull happeningFull = await new HappeningDB().GetFullById(id, likeAppUserId);

            if (happeningFull == null)
                return null;

            happeningFull.Images = await PostFunctions.GetImagesById(happeningFull.PostId, true);

            return happeningFull;
        }

        public static async Task<HappeningFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            HappeningFull happeningFull = await new HappeningDB().GetFullByPostId(postId, likeAppUserId);

            if (happeningFull == null)
                return null;

            happeningFull.Images = await PostFunctions.GetImagesById(happeningFull.PostId, true);

            return happeningFull;
        }

        public static async Task<List<HappeningFull>> GetFullsByStatus(int status)
        {
            HappeningDataFull happeningDataFull = await new HappeningDB().GetDataFullByStatus(status);

            return await GetFulls(happeningDataFull);
        }

        public static async Task<List<HappeningFull>> GetFulls(HappeningDataFull happeningDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in happeningDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in happeningDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in happeningDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // HappeningFull
            List<HappeningFull> happeningFulls = [];
            foreach (HappeningFull happeningFull in happeningDataFull.HappeningFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(happeningFull.PostId, out ContactFull contact))
                    contact = null;

                happeningFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(happeningFull.PostId, out List<LinkFull> links))
                    links = [];

                happeningFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(happeningFull.PostId, out List<CommentFull> comments))
                    comments = [];

                happeningFull.CommentFulls = comments;

                // Images
                happeningFull.Images = await PostFunctions.GetImagesById(happeningFull.PostId, true);

                happeningFulls.Add(happeningFull);
            }

            return happeningFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterHappeningRequest registerHappeningRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerHappeningRequest.Post.PostTypeId = (long)PostType.Happening;
                registerHappeningRequest.Happening.PostId = await PostFunctions.Register(registerHappeningRequest);

                registerHappeningRequest.Happening.Status = 1;
                id = await Add(registerHappeningRequest.Happening);

                scope.Complete();
            }

            return id;
        }

        // ADD
        public static async Task<long> Add(Happening happening)
        {
            return await new HappeningDB().Add(happening);
        }

        // UPDATE
        public static async Task<bool> Update(RegisterHappeningRequest registerHappeningRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerHappeningRequest);

                // Update Happening
                // Soft Delete
                await new HappeningDB().UpdateStatusByPostId(registerHappeningRequest.Post.Id, 1, 0);

                registerHappeningRequest.Happening.PostId = registerHappeningRequest.Post.Id;
                registerHappeningRequest.Happening.Status = 1;

                if (registerHappeningRequest.Happening.Id == -1 || registerHappeningRequest.Happening.Id == 0)
                {
                    await Add(registerHappeningRequest.Happening);
                }
                else
                {
                    await Update(registerHappeningRequest.Happening);
                    await UpdateStatus(registerHappeningRequest.Happening.Id, 1);
                }

                scope.Complete();
                return true;
            }
        }

        public static async Task<bool> Accept(long postId, long happeningId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 3);
                bool happeningOk = await UpdateStatus(happeningId, 3);

                if (!postOk || !happeningOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long happeningId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 0);
                bool happeningOk = await UpdateStatus(happeningId, 0);

                if (!postOk || !happeningOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Happening happening)
        {
            return await new HappeningDB().Update(happening);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new HappeningDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new HappeningDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            await new HappeningDB().DeleteByPostId(postId);
        }
    }
}