using System;

namespace HeroServer
{
    public class RecipeFull : PostFull
    {
        public long Id { get; set; }
        public long RecipeTypeId { get; set; }
        public String Ingredients { get; set; }
        public String Preparation { get; set; }
        public int Portions { get; set; }
        public int CookingTime { get; set; }
        public int Status { get; set; }

        public RecipeFull()
        {
        }

        public RecipeFull(long id, long postId, long appUserId, String appUserAlias,
                          long postSubtypeId, long postCountryId, long postStateId,
                          String title, String summary, String description,
                          int imageCount, int likeCount, DateTime publicationDateTime,
                          int postStatusId, long recipeTypeId,
                          String ingredients, String preparation,
                          int portions, int cookingTime, int status)
            : base(postId, appUserId, appUserAlias, postSubtypeId,
                   postCountryId, postStateId, title, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatusId)
        {
            Id = id;
            RecipeTypeId = recipeTypeId;
            Ingredients = ingredients;
            Preparation = preparation;
            Portions = portions;
            CookingTime = cookingTime;
            Status = status;
        }
    }
}
