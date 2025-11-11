using System;

namespace HeroServer
{
    public class SignInResponse
    {
        public String AccessToken { get; set; }
        public int Granted { get; set; }
        public String Message { get; set; }

        public SignInResponse()
        {
        }

        public SignInResponse(String accessToken, int granted, String message = null)
        {
            AccessToken = accessToken;
            Granted = granted;
            Message = message;
        }
    }
}
