using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class RecipeFunctions
    {
        // GET
        public static async Task<List<Recipe>> GetAllByStatus(int status)
        {
            return await new RecipeDB().GetAllByStatus(status);
        }

        public static async Task<Recipe> GetById(long id)
        {
            return await new RecipeDB().GetById(id);
        }

        public static async Task<RecipeFull> GetFullById(long id, long likeAppUserId)
        {
            RecipeFull recipeFull = await new RecipeDB().GetFullById(id, likeAppUserId);

            if (recipeFull == null)
                return null;

            recipeFull.Images = await PostFunctions.GetImagesById(recipeFull.PostId, true);

            return recipeFull;
        }

        public static async Task<RecipeFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            RecipeFull recipeFull = await new RecipeDB().GetFullByPostId(postId, likeAppUserId);

            if (recipeFull == null)
                return null;

            recipeFull.Images = await PostFunctions.GetImagesById(recipeFull.PostId, true);

            return recipeFull;
        }

        public static async Task<List<RecipeFull>> GetFullsByStatus(int status)
        {
            RecipeDataFull recipeDataFull = await new RecipeDB().GetDataFullByStatus(status);

            return await GetFulls(recipeDataFull);
        }

        public static async Task<List<RecipeFull>> GetFulls(RecipeDataFull recipeDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in recipeDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in recipeDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in recipeDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // RecipeFull
            List<RecipeFull> recipeFulls = [];
            foreach (RecipeFull recipeFull in recipeDataFull.RecipeFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(recipeFull.PostId, out ContactFull contact))
                    contact = null;

                recipeFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(recipeFull.PostId, out List<LinkFull> links))
                    links = [];

                recipeFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(recipeFull.PostId, out List<CommentFull> comments))
                    comments = [];

                recipeFull.CommentFulls = comments;

                // Images
                recipeFull.Images = await PostFunctions.GetImagesById(recipeFull.PostId, true);

                recipeFulls.Add(recipeFull);
            }

            return recipeFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterRecipeRequest registerRecipeRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerRecipeRequest.Post.PostTypeId = (long)PostType.Recipe;
                registerRecipeRequest.Recipe.PostId = await PostFunctions.Register(registerRecipeRequest);

                registerRecipeRequest.Recipe.Status = 1;
                id = await Add(registerRecipeRequest.Recipe);

                scope.Complete();
            }

            return id;
        }

        // ADD
        public static async Task<long> Add(Recipe recipe)
        {
            return await new RecipeDB().Add(recipe);
        }

        // UPDATE
        public static async Task<bool> Update(RegisterRecipeRequest registerRecipeRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerRecipeRequest);

                // Update Recipe
                // Soft Delete
                await new RecipeDB().UpdateStatusByPostId(registerRecipeRequest.Post.Id, 1, 0);

                registerRecipeRequest.Recipe.PostId = registerRecipeRequest.Post.Id;
                registerRecipeRequest.Recipe.Status = 1;

                if (registerRecipeRequest.Recipe.Id == -1 || registerRecipeRequest.Recipe.Id == 0)
                {
                    await Add(registerRecipeRequest.Recipe);
                }
                else
                {
                    await Update(registerRecipeRequest.Recipe);
                    await UpdateStatus(registerRecipeRequest.Recipe.Id, 1);
                }

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Accept(long postId, long recipeId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 3);
                bool recipeOk = await UpdateStatus(recipeId, 3);

                if (!postOk || !recipeOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long recipeId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 0);
                bool recipeOk = await UpdateStatus(recipeId, 0);

                if (!postOk || !recipeOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Recipe recipe)
        {
            return await new RecipeDB().Update(recipe);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new RecipeDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            return await new RecipeDB().UpdateStatusByPostId(postId, curStatus, newStatus);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new RecipeDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            await new RecipeDB().DeleteByPostId(postId);
        }
    }
}