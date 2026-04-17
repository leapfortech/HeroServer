using System;
using System.Collections.Generic;

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

        public List<String> Images { get; set; }

        public RecipeFull()
        {
        }

        public RecipeFull(long id, long postId, long appUserId, String appUserAlias,
                          long postTypeId, long postCountryId, long postStateId,
                          String title, String titleImage, String summary, String description,
                          int imageCount, int likeCount, DateTime publicationDateTime,
                          int postStatusId,
                          ContactFull contactFull,
                          List<LinkFull> linkFulls,
                          List<CommentFull> commentFulls,
                          long recipeTypeId, String ingredients, String preparation,
                          int portions, int cookingTime, int status,
                          List<String> images)
            : base(postId, appUserId, appUserAlias, postTypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatusId,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            RecipeTypeId = recipeTypeId;
            Ingredients = ingredients;
            Preparation = preparation;
            Portions = portions;
            CookingTime = cookingTime;
            Status = status;
            Images = images;
        }
    }
}
