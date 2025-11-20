using System;

namespace HeroServer
{
    public class Recipe
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long RecipeTypeId { get; set; }
        public String Ingredients { get; set; }
        public String Preparation { get; set; }
        public int Portions { get; set; }
        public int CookingTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Recipe() { }

        public Recipe(long id, long postId, long recipeTypeId, String ingredients, String preparation, int portions,
                      int cookingTime, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            RecipeTypeId = recipeTypeId;
            Ingredients = ingredients;
            Preparation = preparation;
            Portions = portions;
            CookingTime = cookingTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
