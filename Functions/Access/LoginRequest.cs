using System;

namespace HeroServer
{
    public class LoginRequest
    {
        public String Email { get; set; }
        public String Version { get; set; }

        public LoginRequest()
        {
        }

        public LoginRequest(String email, String version)
        {
            Email = email;
            Version = version;
        }
    }
}
