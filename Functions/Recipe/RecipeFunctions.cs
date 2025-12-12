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