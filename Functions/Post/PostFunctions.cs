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

        // GET FULL
        public static async Task<List<TaleFull>> GetTaleFullsByStatus(int status)
        {
            return await TaleFunctions.GetFullsByStatus(status);
        }

        public static async Task<List<RecipeFull>> GetRecipeFullsByStatus(int status)
        {
            return await RecipeFunctions.GetFullsByStatus(status);
        }

        public static async Task<List<TreatmentFull>> GetTreatmentFullsByStatus(int status)
        {
            return await TreatmentFunctions.GetFullsByStatus(status);
        }

        public static async Task<List<RadioFull>> GetRadioFullsByStatus(int status)
        {
            return await RadioFunctions.GetFullsByStatus(status);
        }

        public static async Task<List<ProductFull>> GetProductFullsByStatus(int status)
        {
            return await ProductFunctions.GetFullsByStatus(status);
        }

        public static async Task<List<HappeningFull>> GetHappeningFullsByStatus(int status)
        {
            return await HappeningFunctions.GetFullsByStatus(status);
        }

        public static async Task<List<NewsFull>> GetNewsFullsByStatus(int status)
        {
            return await NewsFunctions.GetFullsByStatus(status);
        }

        public static async Task<List<PuzzleFull>> GetPuzzleFullsByStatus(int status)
        {
            return await PuzzleFunctions.GetFullsByStatus(status);
        }

        // REGISTER
        public static async Task<long> RegisterTale(RegisterTaleRequest registerTaleRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerTaleRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerTaleRequest.Post);

                registerTaleRequest.Tale.PostId = postId;
                await TaleFunctions.Register(registerTaleRequest.Tale);

                if (registerTaleRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerTaleRequest.Contact);

                if (registerTaleRequest.Links != null && registerTaleRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerTaleRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerTaleRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterRecipe(RegisterRecipeRequest registerRecipeRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerRecipeRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerRecipeRequest.Post);

                registerRecipeRequest.Recipe.PostId = postId;
                await RecipeFunctions.Register(registerRecipeRequest.Recipe);

                if (registerRecipeRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerRecipeRequest.Contact);

                if (registerRecipeRequest.Links != null && registerRecipeRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerRecipeRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerRecipeRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterTreatment(RegisterTreatmentRequest registerTreatmentRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerTreatmentRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerTreatmentRequest.Post);

                registerTreatmentRequest.Treatment.PostId = postId;
                await TreatmentFunctions.Register(registerTreatmentRequest);

                if (registerTreatmentRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerTreatmentRequest.Contact);

                if (registerTreatmentRequest.Links != null && registerTreatmentRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerTreatmentRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerTreatmentRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterRadio(RegisterRadioRequest registerRadioRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerRadioRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerRadioRequest.Post);

                registerRadioRequest.Radio.PostId = postId;
                await RadioFunctions.Register(registerRadioRequest);

                if (registerRadioRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerRadioRequest.Contact);

                if (registerRadioRequest.Links != null && registerRadioRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerRadioRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerRadioRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterProduct(RegisterProductRequest registerProductRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerProductRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerProductRequest.Post);

                registerProductRequest.Product.PostId = postId;
                await ProductFunctions.Register(registerProductRequest);

                if (registerProductRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerProductRequest.Contact);

                if (registerProductRequest.Links != null && registerProductRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerProductRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerProductRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterHappening(RegisterHappeningRequest registerHappeningRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerHappeningRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerHappeningRequest.Post);

                registerHappeningRequest.Happening.PostId = postId;
                await HappeningFunctions.Register(registerHappeningRequest.Happening);

                if (registerHappeningRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerHappeningRequest.Contact);

                if (registerHappeningRequest.Links != null && registerHappeningRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerHappeningRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerHappeningRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterNews(RegisterNewsRequest registerNewsRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerNewsRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerNewsRequest.Post);

                registerNewsRequest.News.PostId = postId;
                await NewsFunctions.Register(registerNewsRequest.News);

                if (registerNewsRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerNewsRequest.Contact);

                if (registerNewsRequest.Links != null && registerNewsRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerNewsRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerNewsRequest.Images);

            return postId;
        }

        public static async Task<long> RegisterPuzzle(RegisterPuzzleRequest registerPuzzleRequest)
        {
            long postId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerPuzzleRequest.Post.Status = 1;

                postId = await new PostDB().Add(registerPuzzleRequest.Post);

                registerPuzzleRequest.Puzzle.PostId = postId;
                await PuzzleFunctions.Register(registerPuzzleRequest);

                if (registerPuzzleRequest.Contact != null)
                    await ContactFunctions.Register(postId, registerPuzzleRequest.Contact);

                if (registerPuzzleRequest.Links != null && registerPuzzleRequest.Links.Count > 0)
                    await LinkFunctions.Register(postId, registerPuzzleRequest.Links);

                scope.Complete();
            }

            await RegisterImages(postId, registerPuzzleRequest.Images);

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
        public static async Task RegisterImages(long postId, List<String> images)
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