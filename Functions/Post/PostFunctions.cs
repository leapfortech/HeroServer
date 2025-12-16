using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class PostFunctions
    {
        // GET
        public static async Task<List<Post>> GetAllByStatus(int status)
        {
            return await new PostDB().GetAllByStatus(status);
        }

        public static async Task<Post> GetById(long id)
        {
            return await new PostDB().GetById(id);
        }

        // REGISTER
        public static async Task<long> Register(RegisterPostRequest RegisterPostRequest)
        {
            RegisterPostRequest.Post.Status = 1;
            long postId = await new PostDB().Add(RegisterPostRequest.Post);

            await RegisterImages(postId, RegisterPostRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterShare(Share share)
        {
            return await new ShareDB().Add(share);
        }

        public static async Task<long> RegisterFavorite(Favorite favorite)
        {
            favorite.Status = 1;
            return await new FavoriteDB().Add(favorite);
        }

        public static async Task<long> RegisterComment(Comment comment)
        {
            comment.Status = 1;
            return await new CommentDB().Add(comment);
        }

        public static async Task<long> RegisterPostPlaint(PostPlaint postPlaint)
        {
            postPlaint.Status = 1;
            return await new PostPlaintDB().Add(postPlaint);
        }

        public static async Task<long> RegisterPostRead(PostRead postRead)
        {
            return await new PostReadDB().Add(postRead);
        }

        public static async Task<long> RegisterReaction(Reaction reaction)
        {
            reaction.Status = 1;
            return await new ReactionDB().Add(reaction);
        }

        public static async Task<long> RegisterLike(Like like)
        {
            like.Status = 1;
            return await new LikeDB().Add(like);
        }

        // IMAGES
        public static async Task<List<String>> GetImagesById(int id, bool first)
        {
            return await GetImages(id, await new PostDB().GetImageCount(id), first);
        }

        public static async Task<List<String>> GetImages(int id, int count, bool first)
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

        public static async Task DeleteSoftImages(String containerName, String filename)
        {
            for (int idx = 0; ; idx++)
                if (!await StorageFunctions.DeleteSoftFile(containerName, $"{filename}|{idx:D02}", "jpg"))
                    break;
        }

        public static async Task DeleteImages(String containerName, String filename)
        {
            for (int idx = 0; ; idx++)
                if (!await StorageFunctions.DeleteFile(containerName, $"{filename}|{idx:D02}.jpg"))
                    break;
        }

        // ADD
        public static async Task<long> Add(Post post)
        {
            return await new PostDB().Add(post);
        }

        // UPDATE
        public static async Task<bool> Update(Post post)
        {
            return await new PostDB().Update(post);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new PostDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new PostDB().DeleteById(id);
        }
    }
}