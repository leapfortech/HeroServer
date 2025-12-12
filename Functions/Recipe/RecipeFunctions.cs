using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public static async Task<RecipeFull> GetFullById(long id)
        {
            return await new RecipeDB().GetFullById(id);
        }

        public static async Task<RecipeFull> GetFullByPostId(long postId)
        {
            return await new RecipeDB().GetFullByPostId(postId);
        }

        public static async Task<List<RecipeFull>> GetFullsByStatus(int status)
        {
            RecipeDataFull recipeDataFull = await new RecipeDB().GetDataFullByStatus(status);

            return GetFulls(recipeDataFull);
        }

        public static List<RecipeFull> GetFulls(RecipeDataFull recipeDataFull)
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

                recipeFulls.Add(recipeFull);
            }

            return recipeFulls;
        }

        // REGISTER
        public static async Task<long> Register(Recipe recipe)
        {
            recipe.Status = 1;

            return await Add(recipe);
        }

        // ADD
        public static async Task<long> Add(Recipe recipe)
        {
            return await new RecipeDB().Add(recipe);
        }

        // UPDATE
        public static async Task<bool> Update(Recipe recipe)
        {
            return await new RecipeDB().Update(recipe);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new RecipeDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new RecipeDB().DeleteById(id);
        }
    }
}