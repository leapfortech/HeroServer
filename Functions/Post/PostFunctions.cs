using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class PostFunctions
    {
        public static async void Initialize()
        {
            PostDB.InitParams(Convert.ToInt32(await new SystemParamDB().GetValue("TaleExpirationTime")),
                              Convert.ToInt32(await new SystemParamDB().GetValue("RecipeExpirationTime")),
                              Convert.ToInt32(await new SystemParamDB().GetValue("TreatmentExpirationTime")),
                              Convert.ToInt32(await new SystemParamDB().GetValue("RadioExpirationTime")),
                              Convert.ToInt32(await new SystemParamDB().GetValue("ProductExpirationTime")),
                              Convert.ToInt32(await new SystemParamDB().GetValue("HappeningExpirationTime")),
                              Convert.ToInt32(await new SystemParamDB().GetValue("NewsExpirationTime")));
        }

        // GET
        public static async Task<List<Post>> GetAllByStatus(int status)
        {
            return await new PostDB().GetAllByStatus(status);
        }

        public static async Task<Post> GetById(long id)
        {
            return await new PostDB().GetById(id);
        }

        // FEED
        public static async Task<PostFeedResponse> GetPostFeed(PostFeedRequest request)
        {
            PostFeedResponse response = await new PostDB().GetPostFeed(request);

            // TitleImages
            List<Task<String>> tasks = [];
            for (int i = 0; i < response.PostFulls.Count; i++)
                tasks.Add(GetTitleImageByPostId(response.PostFulls[i].PostId));

            String[] images = await Task.WhenAll(tasks);

            for (int i = 0; i < response.PostFulls.Count; i++)
                response.PostFulls[i].TitleImage = images[i];

            // Thumbnails
            tasks = [];
            for (int i = 0; i < response.PostFulls.Count; i++)
                tasks.Add(AppUserFunctions.GetThumbnail(response.PostFulls[i].AppUserId));

            images = await Task.WhenAll(tasks);

            for (int i = 0; i < response.PostFulls.Count; i++)
                response.PostFulls[i].Thumbnail = images[i];

            return response;
        }

        public static async Task<CommentFeedResponse> GetCommentFeed(CommentFeedRequest request)
        {
            return await new PostDB().GetCommentFeed(request);
        }

        // PAGED
        public static async Task<PostFullsPagedResponse> GetFullsPagedByType(PostTypePagedRequest request)
        {
            PostFullsPagedResponse response = await new PostDB().GetFullsPagedByType(request);

            for (int i = 0; i < response.PostFulls.Count; i++)
                response.PostFulls[i].TitleImage = await GetTitleImageByPostId(response.PostFulls[i].PostId);

            return response;
        }

        // REGISTER
        public static async Task<long> Register(RegisterPostRequest registerPostRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Register Post
                registerPostRequest.Post.PublicationDateTime = DateTime.Now;
                registerPostRequest.Post.ApprovalDateTime = null;
                registerPostRequest.Post.ExpirationDateTime = null;
                registerPostRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerPostRequest.Post);

                // Register Contact
                if (registerPostRequest.Contact != null)
                {
                    registerPostRequest.Contact.PostId = postId;
                    registerPostRequest.Contact.Status = 1;
                    await new ContactDB().Add(registerPostRequest.Contact);
                }

                // Register Links
                if (registerPostRequest.Links != null && registerPostRequest.Links.Count > 0)
                {
                    for (int i = 0; i < registerPostRequest.Links.Count; i++)
                    {
                        registerPostRequest.Links[i].PostId = postId;
                        registerPostRequest.Links[i].Status = 1;
                        await new LinkDB().Add(registerPostRequest.Links[i]);
                    }
                }

                scope.Complete();
            }

            // Register Images
            if (registerPostRequest.Images != null && registerPostRequest.Images.Count != 0)
                await RegisterImages(postId, registerPostRequest.Images);

            return postId;
        }

        // FAVORITE
        public static async Task<long> RegisterFavorite(Favorite favorite)
        {
            favorite.Status = 1;
            return await new FavoriteDB().Add(favorite);
        }

        public static async Task<bool> DeleteFavorite(Favorite favorite)
        {
            return await new FavoriteDB().Delete(favorite);
        }

        // LIKE
        public static async Task<long> RegisterLike(Like like)
        {
            long likeId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                like.Status = 1;
                likeId = await new LikeDB().Add(like);
                if (like.Rank == 5)
                    await new PostDB().IncrementLikeCount(like.PostId);

                scope.Complete();
            }

            return likeId;
        }

        public static async Task<long> UpdateLike(Like like)
        {
            long likeId = -1L;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                like.Status = 1;
                
                Like curLike = await new LikeDB().Get(like.PostId, like.AppUserId);
                if (curLike == null)
                {
                    likeId = await new LikeDB().Add(like);
                    if (like.Rank == 5)
                        await new PostDB().IncrementLikeCount(like.PostId);
                }
                else if (like.Rank < curLike.Rank)
                {
                    likeId = curLike.Id;
                    await new LikeDB().UpdateRank(like);
                    await new PostDB().DecrementLikeCount(like.PostId);
                }
                else if (like.Rank > curLike.Rank)
                {
                    likeId = curLike.Id;
                    await new LikeDB().UpdateRank(like);
                    await new PostDB().IncrementLikeCount(like.PostId);
                }

                scope.Complete();
            }

            return likeId;
        }

        public static async Task<long> DeleteLike(Like like)
        {
            long likeId = -1L;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Like curLike = await new LikeDB().Get(like.PostId, like.AppUserId);
                if (curLike != null)
                {
                    likeId = curLike.Id;
                    await new LikeDB().Delete(like);
                    if (like.Rank != -1)
                        await new PostDB().DecrementLikeCount(like.PostId);
                }

                scope.Complete();
            }

            return likeId;
        }

        public static async Task<long> RegisterReaction(Reaction reaction)
        {
            reaction.Status = 1;
            return await new ReactionDB().Add(reaction);
        }

        public static async Task<bool> DeleteReaction(Reaction reaction)
        {
            return await new ReactionDB().Delete(reaction);
        }

        // SHARE
        public static async Task<long> RegisterShare(Share share)
        {
            return await new ShareDB().Add(share);
        }

        // COMMENT
        public static async Task<long> RegisterComment(Comment comment)
        {
            comment.Status = 1;
            return await new CommentDB().Add(comment);
        }

        public static async Task<long> RegisterCommentPlaint(CommentPlaint commentPlaint)
        {
            commentPlaint.Status = 1;
            return await new CommentPlaintDB().Add(commentPlaint);
        }

        // PLAINT
        public static async Task<long> RegisterPostPlaint(PostPlaint postPlaint)
        {
            if (await new PostPlaintDB().ExistsPlaintByAppUserId(postPlaint.PostId, postPlaint.AppUserId))
                throw new Exception("La publicación ya fue reportada previamente.");

            long id = -1L;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                postPlaint.Status = 1;
                id = await new PostPlaintDB().Add(postPlaint);

                int plaintCount = await new PostPlaintDB().GetPlaintCountByPostId(postPlaint.PostId);

                if (plaintCount >= 3)
                {
                    bool updated = await new PostDB().UpdateStatus(postPlaint.PostId, 1, 3);

                    if (updated)
                    {
                        long postTypeId = await new PostDB().GetPostTypeId(postPlaint.PostId);

                        switch (postTypeId)
                        {
                            case 1: await TaleFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                            case 2: await RecipeFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                            case 3: await TreatmentFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                            case 4: await RadioFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                            case 5: await ProductFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                            case 6: await HappeningFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                            case 7: await NewsFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                            case 8: await PuzzleFunctions.UpdateStatusByPostId(postPlaint.PostId, 1, 3); break;
                        }
                    }
                }

                scope.Complete();
            }
            return id;
        }

        public static async Task<long> RegisterPostRead(PostRead postRead)
        {
            return await new PostReadDB().Add(postRead);
        }

        // ADD
        public static async Task<long> Add(Post post)
        {
            return await new PostDB().Add(post);
        }

        // UPDATE
        public static async Task<bool> UpdatePost(RegisterPostRequest registerPostRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                registerPostRequest.Post.PublicationDateTime = DateTime.Now;
                registerPostRequest.Post.ApprovalDateTime = null;
                registerPostRequest.Post.ExpirationDateTime = null;
                await new PostDB().Update(registerPostRequest.Post);
                
                // Update Contact
                // Soft Delete
                if (registerPostRequest.Contact != null)
                {
                    await new ContactDB().UpdateStatusByPostId(registerPostRequest.Post.Id, 1, 0);

                    registerPostRequest.Contact.PostId = registerPostRequest.Post.Id;
                    registerPostRequest.Contact.Status = 1;
                    await new ContactDB().Add(registerPostRequest.Contact);
                }

                // Update Links
                // Soft Delete
                await new LinkDB().UpdateStatusByPostId(registerPostRequest.Post.Id, 1, 0);

                if (registerPostRequest.Links != null && registerPostRequest.Links.Count > 0)
                {
                    for (int i = 0; i < registerPostRequest.Links.Count; i++)
                    {
                        Link link = registerPostRequest.Links[i];
                        link.PostId = registerPostRequest.Post.Id;

                        if (link.Id == -1 || link.Id == 0)
                        {
                            link.Status = 1;
                            await new LinkDB().Add(link);
                        }
                        else
                        {
                            await new LinkDB().Update(link);
                            await new LinkDB().UpdateStatus(link.Id, 1);
                        }
                    }
                }

                scope.Complete();
            }

            await UpdateImages(registerPostRequest.Post.Id, registerPostRequest.Images);

            return true;
        }

        public static async Task<bool> Update(Post post)
        {
            return await new PostDB().Update(post);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new PostDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await DeleteShareByPostId(id);
                await DeleteFavoriteByPostId(id);
                await DeleteContactByPostId(id);
                await DeleteCommentByPostId(id);
                await DeletePostPlaintByPostId(id);
                await DeleteLinkByPostId(id);
                await DeletePostReadByPostId(id);
                await DeleteReactionByPostId(id);
                await DeleteLikeByPostId(id);

                long postTypeId = await new PostDB().GetPostTypeId(id);

                switch (postTypeId)
                {
                    case 1: await TaleFunctions.DeleteByPostId(id); break;
                    case 2: await RecipeFunctions.DeleteByPostId(id); break;
                    case 3: await TreatmentFunctions.DeleteByPostId(id); break;
                    case 4: await RadioFunctions.DeleteByPostId(id); break;
                    case 5: await ProductFunctions.DeleteByPostId(id); break;
                    case 6: await HappeningFunctions.DeleteByPostId(id); break;
                    case 7: await NewsFunctions.DeleteByPostId(id); break;
                    case 8: await PuzzleFunctions.DeleteByPostId(id); break;
                }

                await new PostDB().DeleteById(id);

                scope.Complete();
            }

            String containerName = "posts";
            String filename = $"post{id:D08}";
            await DeleteImages(containerName, filename);
        }

        public static async Task DeleteShareByPostId(long postId)
        {
                await new ShareDB().DeleteByPostId(postId);
        }

        public static async Task DeleteFavoriteByPostId(long postId)
        {
            await new FavoriteDB().DeleteByPostId(postId);
        }

        public static async Task DeleteContactByPostId(long postId)
        {
            await new ContactDB().DeleteByPostId(postId);
        }

        public static async Task DeleteCommentByPostId(long postId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<long> commentIds = await new CommentDB().GetCommentIdsByPostId(postId);

                for (int i = 0; i < commentIds.Count; i++)
                    await new CommentPlaintDB().DeleteById(commentIds[i]);

                await new CommentDB().DeleteByPostId(postId);

                scope.Complete();
            }
        }

        public static async Task DeletePostPlaintByPostId(long postId)
        {
            await new PostPlaintDB().DeleteByPostId(postId);
        }

        public static async Task DeleteLinkByPostId(long postId)
        {
            await new LinkDB().DeleteByPostId(postId);
        }

        public static async Task DeletePostReadByPostId(long postId)
        {
            await new PostReadDB().DeleteByPostId(postId);
        }

        public static async Task DeleteLikeByPostId(long postId)
        {
            await new LikeDB().DeleteByPostId(postId);
        }

        public static async Task<bool> DeleteReactionByPostId(long postId)
        {
            return await new ReactionDB().DeleteByPostId(postId);
        }

        // IMAGES
        public static async Task<String> GetTitleImageByPostId(long id)
        {
            byte[] image = await StorageFunctions.ReadFile("posts", $"post{id:D08}|00", "jpg");
            return image == null ? null : Convert.ToBase64String(image);
        }

        public static async Task<List<String>> GetImagesById(long id, bool first)
        {
            return await GetImages(id, await new PostDB().GetImageCount(id), first);
        }

        public static async Task<List<String>> GetImages(long id, int count, bool first)
        {
            List<String> images = [];
            String filename = $"post{id:D08}";
            for (int i = first ? 0 : 1; i < count; i++)
            {
                byte[] img = await StorageFunctions.ReadFile("posts", $"{filename}|{i:D02}", "jpg");
                if (img == null) continue;
                images.Add(Convert.ToBase64String(img));
            }

            return images;
        }

        public static async Task RegisterImages(long postId, List<String> images)  // JAD
        {
            if (images == null || images.Count == 0)
                throw new Exception("Images list should NOT be empty");

            String containerName = "posts";
            String filename = $"post{postId:D08}";

            await DeleteImages(containerName, filename);

            await StorageFunctions.CreateContainer(containerName);
            int count = 0;
            for (int i = 0; i < images.Count; i++)
            {
                if (String.IsNullOrEmpty(images[i]))
                    continue;

                await StorageFunctions.UpdateFile(containerName, $"{filename}|{i:D02}", "jpg", Convert.FromBase64String(images[i]));
                count++;
            }

            await new PostDB().UpdateImageCount(postId, count);
        }

        public static async Task UpdateImages(long postId, List<String> images)
        {
            String containerName = "posts";
            String filename = $"post{postId:D08}";

            await DeleteImages(containerName, filename);

            if (images == null)
            {
                await new PostDB().UpdateImageCount(postId, 0);
                return;
            }

            await StorageFunctions.CreateContainer(containerName);

            int count = 0;
            for (int i = 0; i < images.Count; i++)
            {
                if (String.IsNullOrWhiteSpace(images[i]))
                    continue;

                await StorageFunctions.UpdateFile(containerName, $"{filename}|{count:D02}", "jpg", Convert.FromBase64String(images[i]));
                count++;
            }

            await new PostDB().UpdateImageCount(postId, count);
        }

        //public static async Task DeleteSoftImages(String containerName, String filename)
        //{
        //    for (int idx = 0; ; idx++)
        //        if (!await StorageFunctions.DeleteSoftFile(containerName, $"{filename}|{idx:D02}", "jpg"))
        //            break;
        //}

        public static async Task DeleteImages(String containerName, String filename)
        {
            for (int idx = 0; ; idx++)
                if (!await StorageFunctions.DeleteFile(containerName, $"{filename}|{idx:D02}.jpg"))
                    break;
        }
    }
}