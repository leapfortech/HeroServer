namespace HeroServer
{
    public class RegisterNewsRequest : RegisterPostRequest
    {
        public News News { get; set; }

        public RegisterNewsRequest()
        {
        }

        public RegisterNewsRequest(News news)
        {
            News = news;
        }
    }
}
