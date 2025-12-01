namespace HeroServer
{
    public class RegisterHappeningRequest : RegisterPostRequest
    {
        public Happening Happening { get; set; }

        public RegisterHappeningRequest()
        {
        }

        public RegisterHappeningRequest(Happening happening)
        {
            Happening = happening;
        }
    }
}
