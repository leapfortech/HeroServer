using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class MemoryFunctions
    {
        // GET
        public static async Task<List<Memory>> GetAllByStatus(int status)
        {
            return await new MemoryDB().GetAllByStatus(status);
        }

        public static async Task<Memory> GetById(long id)
        {
            return await new MemoryDB().GetById(id);
        }

        public static async Task<MemoryFull> GetFullById(long id, long likeAppUserId)
        {
            MemoryFull memoryFull = await new MemoryDB().GetFullById(id, likeAppUserId);

            if (memoryFull == null)
                return null;

            memoryFull.Images = await PostFunctions.GetImagesById(memoryFull.PostId, true);
            memoryFull.Thumbnail = await AppUserFunctions.GetThumbnail(memoryFull.AppUserId);

            return memoryFull;
        }

        public static async Task<MemoryFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            MemoryFull memoryFull = await new MemoryDB().GetFullByPostId(postId, likeAppUserId);

            if (memoryFull == null)
                return null;

            memoryFull.Images = await PostFunctions.GetImagesById(memoryFull.PostId, true);
            memoryFull.Thumbnail = await AppUserFunctions.GetThumbnail(memoryFull.AppUserId);

            return memoryFull;
        }

        public static async Task<List<MemoryFull>> GetFullsByStatus(int status)
        {
            MemoryDataFull memoryDataFull = await new MemoryDB().GetDataFullByStatus(status);

            return await GetFulls(memoryDataFull);
        }

        public static async Task<List<MemoryFull>> GetFulls(MemoryDataFull memoryDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in memoryDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in memoryDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in memoryDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // MemoryFull
            List<MemoryFull> memoryFulls = [];
            foreach (MemoryFull memoryFull in memoryDataFull.MemoryFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(memoryFull.PostId, out ContactFull contact))
                    contact = null;

                memoryFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(memoryFull.PostId, out List<LinkFull> links))
                    links = [];

                memoryFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(memoryFull.PostId, out List<CommentFull> comments))
                    comments = [];

                memoryFull.CommentFulls = comments;

                // Images
                memoryFull.Images = await PostFunctions.GetImagesById(memoryFull.PostId, true);
                memoryFull.Thumbnail = await AppUserFunctions.GetThumbnail(memoryFull.AppUserId);

                memoryFulls.Add(memoryFull);
            }

            return memoryFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterMemoryRequest registerMemoryRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerMemoryRequest.Post.PostTypeId = PostType.Memory;
                registerMemoryRequest.Memory.PostId = await PostFunctions.Register(registerMemoryRequest);

                registerMemoryRequest.Memory.Status = 1;
                id = await Add(registerMemoryRequest.Memory);

                scope.Complete();
            }

            return id;
        }

        // ADD
        public static async Task<long> Add(Memory memory)
        {
            return await new MemoryDB().Add(memory);
        }

        // UPDATE
        public static async Task<bool> Update(RegisterMemoryRequest registerMemoryRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerMemoryRequest);

                // Update Memory
                // Soft Delete
                await new MemoryDB().UpdateStatusByPostId(registerMemoryRequest.Post.Id, 1, 0);

                registerMemoryRequest.Memory.PostId = registerMemoryRequest.Post.Id;
                registerMemoryRequest.Memory.Status = 1;

                if (registerMemoryRequest.Memory.Id == -1 || registerMemoryRequest.Memory.Id == 0)
                {
                    await Add(registerMemoryRequest.Memory);
                }
                else
                {
                    await Update(registerMemoryRequest.Memory);
                    await UpdateStatus(registerMemoryRequest.Memory.Id, 1);
                }

                scope.Complete();
                return true;
            }
        }

        public static async Task<bool> Accept(long postId, long memoryId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 1);
                bool memoryOk = await UpdateStatus(memoryId, 1);

                if (!postOk || !memoryOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long memoryId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 4);
                bool memoryOk = await UpdateStatus(memoryId, 4);

                if (!postOk || !memoryOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Memory memory)
        {
            return await new MemoryDB().Update(memory);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new MemoryDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            return await new MemoryDB().UpdateStatusByPostId(postId, curStatus, newStatus);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new MemoryDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            await new MemoryDB().DeleteByPostId(postId);
        }
    }
}