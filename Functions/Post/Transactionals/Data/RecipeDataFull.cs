using System.Collections.Generic;

namespace HeroServer
{
    public class RecipeDataFull
    {
        public List<RecipeFull> RecipeFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public RecipeDataFull()
        {
        }

        public RecipeDataFull(List<RecipeFull> recipeFulls,
                            List<ContactFull> contactFulls,
                            List<LinkFull> linkFulls,
                            List<CommentFull> commentFulls)
        {
            RecipeFulls = recipeFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
