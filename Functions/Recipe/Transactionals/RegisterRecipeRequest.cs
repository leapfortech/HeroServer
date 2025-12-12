namespace HeroServer
{
    public class RegisterRecipeRequest : RegisterPostRequest
    {
        public Recipe Recipe { get; set; }

        public RegisterRecipeRequest()
        {
        }

        public RegisterRecipeRequest(Recipe recipe)
        {
            Recipe = recipe;
        }
    }
}
