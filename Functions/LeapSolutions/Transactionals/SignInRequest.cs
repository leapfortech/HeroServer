using System;

namespace HeroServer
{
    public class SignInRequest
    {
        public UserCredential UserCredential { get; set; }

        public SignInRequest()
        {
        }

        public SignInRequest(UserCredential userCredential)
        {
            UserCredential = userCredential;
        }
    }
}
