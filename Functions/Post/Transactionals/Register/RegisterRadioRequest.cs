namespace HeroServer
{
    public class RegisterRadioRequest : RegisterPostRequest
    {
        public Radio Radio { get; set; }

        public RegisterRadioRequest()
        {
        }

        public RegisterRadioRequest(Radio radio)
        {
            Radio = radio;
        }
    }
}
