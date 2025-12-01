namespace HeroServer
{
    public class RegisterTaleRequest : RegisterPostRequest
    {
        public Tale Tale { get; set; }

        public RegisterTaleRequest()
        {
        }

        public RegisterTaleRequest(Tale tale)
        {
            Tale = tale;
        }
    }
}
